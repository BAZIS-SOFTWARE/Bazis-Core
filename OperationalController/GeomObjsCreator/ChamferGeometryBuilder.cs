using Geometry;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController.GeomObjsCreator
{
    /// <summary>
    /// Строит фаску на кривой объёмной OCC-геометрии.
    /// </summary>
    public class ChamferGeometryBuilder
    {
        private readonly IGmshController controller;

        /// <summary>
        /// Создаёт построитель фаски.
        /// </summary>
        /// <param name="controller">Загруженный контроллер Gmsh.</param>
        public ChamferGeometryBuilder(IGmshController controller)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        /// <summary>
        /// Строит фаски на нескольких кривых за одну OCC-операцию.
        /// </summary>
        /// <param name="curveTags">Теги кривых.</param>
        /// <param name="firstLength">Размер фаски на первой соседней поверхности.</param>
        /// <param name="secondLengthOrAngle">Размер на второй поверхности либо угол фаски в градусах.</param>
        /// <param name="isAngle"><c>true</c>, если <paramref name="secondLengthOrAngle"/> задаёт угол; иначе задаёт второй размер.</param>
        /// <param name="reflected"><c>true</c>, чтобы использовать вторую соседнюю поверхность как опорную.</param>
        /// <returns>Массив пар размерность-тег результирующих сущностей.</returns>
        public int[] Build(IEnumerable<int> curveTags, double firstLength, double secondLengthOrAngle, bool isAngle, bool reflected = false)
        {
            ValidateLength(firstLength, nameof(firstLength));
            var validatedCurveTags = ValidateCurveTags(curveTags);
            var contexts = validatedCurveTags.Select(GetChamferContext).ToArray();
            var secondLengths = GetSecondLengths(
                validatedCurveTags,
                contexts,
                firstLength,
                secondLengthOrAngle,
                isAngle);

            var surfaceIndex = reflected ? 1 : 0;
            
            var referenceSurfaces = contexts.Select(context => context.Surfaces[surfaceIndex]).ToArray();
            var volumeTags = contexts.SelectMany(context => context.Volumes).Distinct().ToArray();
            var distances = Enumerable.Repeat(firstLength, validatedCurveTags.Length).Concat(secondLengths).ToArray();

            return BuildChamfer(validatedCurveTags, referenceSurfaces, volumeTags, distances);
        }

        /// <summary>
        /// Получает контекст, необходимый для построения фаски для указанной кривой.
        /// Метод возвращает теги двух соседних поверхностей, которым принадлежит кривая, и теги общих объёмов этих поверхностей.
        /// </summary>
        private (int[] Surfaces, int[] Volumes) GetChamferContext(int curveTag)
        {
            if (controller.Gmsh == null)
                throw new InvalidOperationException("Контроллер Gmsh не загружен.");

            if (curveTag <= 0)
                throw new ArgumentOutOfRangeException(nameof(curveTag), "Тег кривой должен быть положительным.");

            if (!EntityExists(1, curveTag))
                throw new ArgumentException($"Кривая с тегом {curveTag} не найдена.", nameof(curveTag));

            var surfaces = GetSurfaceTags(curveTag);
            var volumes = GetCommonVolumeTags(surfaces);
            return (surfaces, volumes);
        }

        private double CalculateSecondLength(int curveTag, int[] surfaces, int volumeTag, double firstLength, double angleInDegrees, string parameterName)
        {
            ValidateAngle(angleInDegrees, parameterName);

            var surfaceAngle = GetAngleBetweenSurfaces(curveTag, surfaces, volumeTag);
            var chamferAngle = angleInDegrees * Math.PI / 180.0;

            if (surfaceAngle + chamferAngle >= Math.PI)
                throw new ArgumentOutOfRangeException(
                    parameterName,
                    $"Для угла между поверхностями {surfaceAngle * 180.0 / Math.PI:F3}° угол фаски должен быть меньше {180.0 - surfaceAngle * 180.0 / Math.PI:F3}°.");

            var secondLength = firstLength * Math.Sin(chamferAngle) / Math.Sin(surfaceAngle + chamferAngle);
            return ValidateAndReturnLength(secondLength, parameterName);
        }

        private double[] GetSecondLengths(int[] curveTags,(int[] Surfaces, int[] Volumes)[] contexts, double firstLength, double secondLengthOrAngle, bool isAngle)
        {
            if (!isAngle)
            {
                var secondLength = ValidateAndReturnLength(secondLengthOrAngle, nameof(secondLengthOrAngle));
                return Enumerable.Repeat(secondLength, curveTags.Length).ToArray();
            }

            return curveTags
                .Select((curveTag, index) => CalculateSecondLength(curveTag,contexts[index].Surfaces, contexts[index].Volumes[0],  firstLength,secondLengthOrAngle,  nameof(secondLengthOrAngle)))
                .ToArray();
        }

        private int[] BuildChamfer(int[] curveTags, int[] surfaceTags, int[] volumeTags, double[] distances)
        {
            var numbers = controller.Gmsh.Model.Occ.AddChamfer(volumeTags, curveTags, surfaceTags, distances);

            if (numbers == null || numbers.Length == 0)
                throw new InvalidOperationException("Не удалось построить фаску.");

            controller.Gmsh.Model.Occ.Synchronize();
            return numbers;
        }

        private int[] ValidateCurveTags(IEnumerable<int> curveTags)
        {
            if (curveTags == null)
                throw new ArgumentNullException(nameof(curveTags));

            var tags = curveTags.ToArray();
            if (tags.Length == 0)
                throw new ArgumentException("Необходимо указать хотя бы одну кривую.", nameof(curveTags));

            if (tags.Distinct().Count() != tags.Length)
                throw new ArgumentException("Список кривых не должен содержать повторяющиеся теги.", nameof(curveTags));

            return tags;
        }

        private double GetAngleBetweenSurfaces(int curveTag, int[] surfaceTags, int volumeTag)
        {
            var (minimum, maximum) = controller.Gmsh.Model.GetParametrizationBounds(1, curveTag);
            if (minimum.Length == 0 || maximum.Length == 0)
                throw new InvalidOperationException("Не удалось получить параметризацию кривой.");

            var middleParameter = (minimum[0] + maximum[0]) / 2.0;
            var point = controller.Gmsh.Model.GetValue(1, curveTag, new[] { middleParameter });
            if (point.Length != 3)
                throw new InvalidOperationException("Не удалось получить точку на кривой.");

            var firstNormal = GetOutwardNormal(surfaceTags[0], volumeTag, point);
            var secondNormal = GetOutwardNormal(surfaceTags[1], volumeTag, point);
            var dotProduct = firstNormal.Zip(secondNormal, (first, second) => first * second).Sum();
            var normalsAngle = Math.Acos(Math.Clamp(dotProduct, -1.0, 1.0));
            var surfaceAngle = Math.PI - normalsAngle;

            if (surfaceAngle <= 0 || surfaceAngle >= Math.PI)
                throw new InvalidOperationException("Не удалось определить корректный угол между смежными поверхностями.");

            return surfaceAngle;
        }

        private double[] GetOutwardNormal(int surfaceTag, int volumeTag, double[] point)
        {
            var parameters = controller.Gmsh.Model.GetParametrization(2, surfaceTag, point);
            var normal = controller.Gmsh.Model.GetNormal(surfaceTag, parameters);
            if (normal.Length != 3)
                throw new InvalidOperationException($"Не удалось получить нормаль поверхности {surfaceTag}.");

            var length = Math.Sqrt(normal.Sum(component => component * component));
            if (!double.IsFinite(length) || length == 0)
                throw new InvalidOperationException($"Поверхность {surfaceTag} имеет некорректную нормаль.");

            var orientation = GetSurfaceOrientation(volumeTag, surfaceTag);
            return normal.Select(component => orientation * component / length).ToArray();
        }

        private int GetSurfaceOrientation(int volumeTag, int surfaceTag)
        {
            var boundary = controller.Gmsh.Model.GetBoundary(new[] { 3, volumeTag }, oriented: true);
            for (var index = 1; index < boundary.Length; index += 2)
            {
                if (Math.Abs(boundary[index]) == surfaceTag)
                    return Math.Sign(boundary[index]);
            }

            throw new InvalidOperationException($"Поверхность {surfaceTag} не найдена на границе объёма {volumeTag}.");
        }

        private int[] GetSurfaceTags(int curveTag)
        {
            var (upper, _) = controller.Gmsh.Model.GetAdjacencies(1, curveTag);

            if (upper.Length != 2)
                throw new InvalidOperationException(
                    $"Для построения фаски кривая должна принадлежать двум поверхностям. Найдено: {upper.Length}.");

            return upper;
        }

        private int[] GetCommonVolumeTags(int[] surfaceTags)
        {
            var (firstVolumes, _) = controller.Gmsh.Model.GetAdjacencies(2, surfaceTags[0]);
            var (secondVolumes, _) = controller.Gmsh.Model.GetAdjacencies(2, surfaceTags[1]);
            var commonVolumes = firstVolumes.Intersect(secondVolumes).Distinct().ToArray();

            if (commonVolumes.Length != 1)
                throw new InvalidOperationException(
                    $"Для построения фаски соседние поверхности должны принадлежать одному общему объёму. Найдено: {commonVolumes.Length}.");

            return commonVolumes;
        }

        private bool EntityExists(int dimension, int tag)
        {
            var entities = controller.Gmsh.Model.GetEntities(dimension);
            for (var index = 1; index < entities.Length; index += 2)
            {
                if (entities[index] == tag)
                    return true;
            }

            return false;
        }

        private void ValidateLength(double length, string parameterName)
        {
            if (double.IsNaN(length) || double.IsInfinity(length) || length <= 0)
                throw new ArgumentOutOfRangeException(parameterName, "Длина фаски должна быть конечным положительным числом.");
        }

        private double ValidateAndReturnLength(double length, string parameterName)
        {
            ValidateLength(length, parameterName);
            return length;
        }

        private void ValidateAngle(double angleInDegrees, string parameterName)
        {
            if (!double.IsFinite(angleInDegrees) || angleInDegrees <= 0 || angleInDegrees >= 180)
                throw new ArgumentOutOfRangeException(parameterName, "Угол фаски должен находиться в диапазоне от 0° до 180°.");
        }
    }
}
