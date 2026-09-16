using GmshApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Возвращает параметры разбиения кривой.
        /// </summary>
        /// <param name="curveNumber">Номер кривой.</param>
        /// <returns>Параметры разбиения кривой.</returns>
        public CurveMeshingSettings GetCurveMeshingSettings(int curveNumber)
        {
            EnsureGeometryInitialized();

            var attributes = GmshController.Gmsh.Model.GetAttribute($"transfinite curve {curveNumber}");
            if (attributes.Length == 0)
                return new CurveMeshingSettings(false, 0, MeshType.Progression, 1);

            if (attributes.Length != 3)
                throw new InvalidOperationException("Curve meshing settings are invalid.");

            var nodesCount = ParseInt(attributes[0], "Curve nodes count");
            var meshType = ParseEnum<MeshType>(attributes[1], "Curve mesh type");
            var coefficient = ParseDouble(attributes[2], "Curve mesh coefficient");
            return new CurveMeshingSettings(true, nodesCount, meshType, coefficient);
        }

        /// <summary>
        /// Устанавливает параметры разбиения кривой.
        /// </summary>
        /// <param name="curveNumber">Номер кривой.</param>
        /// <param name="settings">Параметры разбиения кривой.</param>
        public void SetCurveMeshingSettings(int curveNumber, CurveMeshingSettings settings)
        {
            EnsureGeometryInitialized();

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (!settings.IsTransfinite)
            {
                GmshController.Gmsh.Model.Mesh.RemoveConstraints([1, curveNumber]);
                GmshController.Gmsh.Model.RemoveAttribute($"transfinite curve {curveNumber}");
                return;
            }

            GmshController.SetTransfiniteCurve(curveNumber, settings.NodesCount, settings.MeshType, settings.Coefficient);
        }

        /// <summary>
        /// Возвращает параметры разбиения поверхности.
        /// </summary>
        /// <param name="surfaceNumber">Номер поверхности.</param>
        /// <returns>Параметры разбиения поверхности.</returns>
        public SurfaceMeshingSettings GetSurfaceMeshingSettings(int surfaceNumber)
        {
            EnsureGeometryInitialized();

            var attributes = GmshController.GetTransfiniteSurface(surfaceNumber);
            var isTransfinite = attributes.Length != 0;
            var cornerPointNumbers = Array.Empty<int>();
            var arrangement = Arrangement.Left;

            if (isTransfinite)
            {
                if (attributes.Length != 2)
                    throw new InvalidOperationException("Surface meshing settings are invalid.");

                cornerPointNumbers = ParseNumbers(attributes[0], "Surface corner point numbers");
                arrangement = ParseEnum<Arrangement>(attributes[1], "Surface arrangement");
            }

            var embeddedCurveNumbers = GetEmbeddedCurveNumbers(surfaceNumber);
            var boundaryPointNumbers = GetSurfaceBoundaryPointNumbers(surfaceNumber);
            var isRecombined = GmshController.GetRecombineSurface(surfaceNumber);
            return new SurfaceMeshingSettings(isTransfinite, cornerPointNumbers, arrangement, isRecombined, embeddedCurveNumbers, boundaryPointNumbers);
        }

        /// <summary>
        /// Устанавливает параметры разбиения поверхности.
        /// </summary>
        /// <param name="surfaceNumber">Номер поверхности.</param>
        /// <param name="settings">Параметры разбиения поверхности.</param>
        public void SetSurfaceMeshingSettings(int surfaceNumber, SurfaceMeshingSettings settings)
        {
            EnsureGeometryInitialized();

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            SetEmbeddedGeometryEntities(2, surfaceNumber, 1, settings.EmbeddedCurveNumbers);

            if (!settings.IsRecombined)
                ClearSurfaceRecombineSettings(surfaceNumber);

            if (settings.IsTransfinite)
            {
                var cornerPointNumbers = settings.CornerPointNumbers.ToArray();
                if (cornerPointNumbers.Length == 0)
                    cornerPointNumbers = GetSurfaceBoundaryPointNumbers(surfaceNumber);

                GmshController.SetTransfiniteSurface(surfaceNumber, settings.Arrangement, cornerPointNumbers);
            }
            else
                ClearSurfaceTransfiniteSettings(surfaceNumber);

            if (settings.IsRecombined)
                GmshController.SetRecombineSurface(surfaceNumber);
        }

        /// <summary>
        /// Возвращает параметры разбиения объёма.
        /// </summary>
        /// <param name="volumeNumber">Номер объёма.</param>
        /// <returns>Параметры разбиения объёма.</returns>
        public VolumeMeshingSettings GetVolumeMeshingSettings(int volumeNumber)
        {
            EnsureGeometryInitialized();

            var attributes = GmshController.GetTransfiniteVolume(volumeNumber);
            if (attributes.Length == 0)
                return new VolumeMeshingSettings(VolumeMeshingType.Undefined);

            if (attributes[0] == "регулярная")
                return new VolumeMeshingSettings(VolumeMeshingType.Regular);

            if (attributes[0] != "градиентная" || attributes.Length != 5)
                throw new InvalidOperationException("Volume meshing settings are invalid.");

            var power = ParseDouble(attributes[1], "Volume gradient power");
            var distanceMaximum = ParseDouble(attributes[2], "Volume gradient distance maximum");
            var surfaceElementSize = ParseDouble(attributes[3], "Volume surface element size");
            var coreElementSize = ParseDouble(attributes[4], "Volume core element size");
            var gradientSettings = new GradientVolumeMeshingSettings(power, distanceMaximum, surfaceElementSize, coreElementSize);
            return new VolumeMeshingSettings(VolumeMeshingType.Gradient, gradientSettings);
        }

        /// <summary>
        /// Устанавливает параметры разбиения объёма.
        /// </summary>
        /// <param name="volumeNumber">Номер объёма.</param>
        /// <param name="settings">Параметры разбиения объёма.</param>
        public void SetVolumeMeshingSettings(int volumeNumber, VolumeMeshingSettings settings)
        {
            EnsureGeometryInitialized();

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (settings.Type == VolumeMeshingType.Undefined)
            {
                ClearVolumeMeshingSettings(volumeNumber);
                return;
            }

            if (settings.Type == VolumeMeshingType.Regular)
            {
                GmshController.SetTransfiniteVolume(volumeNumber);
                return;
            }

            if (settings.GradientSettings == null)
                throw new ArgumentException("Gradient meshing settings are not specified.", nameof(settings));

            var gradientSettings = settings.GradientSettings;
            GmshController.SetGradientVolume(volumeNumber, gradientSettings.Power, gradientSettings.DistanceMaximum, gradientSettings.SurfaceElementSize, gradientSettings.CoreElementSize);
        }

        /// <summary>
        /// Возвращает размер элемента сетки в геометрической точке.
        /// </summary>
        /// <param name="pointNumber">Номер геометрической точки.</param>
        /// <returns>Размер элемента сетки.</returns>
        public double GetMeshPointSize(int pointNumber)
        {
            EnsureGeometryInitialized();
            return GmshController.GetSize(pointNumber);
        }

        /// <summary>
        /// Устанавливает размер элемента сетки в геометрической точке.
        /// </summary>
        /// <param name="pointNumber">Номер геометрической точки.</param>
        /// <param name="size">Размер элемента сетки.</param>
        public void SetMeshPointSize(int pointNumber, double size)
        {
            EnsureGeometryInitialized();
            GmshController.SetSize(pointNumber, size);
        }

        /// <summary>
        /// Устанавливает встроенные геометрические сущности.
        /// </summary>
        /// <param name="hostDimension">Размерность принимающей сущности.</param>
        /// <param name="hostNumber">Номер принимающей сущности.</param>
        /// <param name="embeddedDimension">Размерность встраиваемых сущностей.</param>
        /// <param name="embeddedNumbers">Номера встраиваемых сущностей.</param>
        public void SetEmbeddedGeometryEntities(int hostDimension, int hostNumber, int embeddedDimension, IEnumerable<int> embeddedNumbers)
        {
            EnsureGeometryInitialized();

            if (embeddedNumbers == null)
                throw new ArgumentNullException(nameof(embeddedNumbers));

            ValidateGeometryDimension(hostDimension, nameof(hostDimension));
            ValidateGeometryDimension(embeddedDimension, nameof(embeddedDimension));

            var existingEntities = GmshController.Gmsh.Model.Mesh.GetEmbedded(hostDimension, hostNumber);
            if (existingEntities.Length != 0)
                GmshController.Gmsh.Model.Mesh.RemoveEmbedded([hostDimension, hostNumber]);

            var entityNumbers = embeddedNumbers.ToArray();
            if (entityNumbers.Length != 0)
                GmshController.Gmsh.Model.Mesh.Embed(embeddedDimension, entityNumbers, hostDimension, hostNumber);
        }

        /// <summary>
        /// Проверяет готовность геометрического контроллера.
        /// </summary>
        private void EnsureGeometryInitialized()
        {
            if (!IsGeometryInitialized)
                throw new InvalidOperationException("Geometry controller is not initialized.");
        }

        /// <summary>
        /// Возвращает номера встроенных в поверхность кривых.
        /// </summary>
        /// <param name="surfaceNumber">Номер поверхности.</param>
        /// <returns>Номера встроенных кривых.</returns>
        private int[] GetEmbeddedCurveNumbers(int surfaceNumber)
        {
            var dimTags = GmshController.Gmsh.Model.Mesh.GetEmbedded(2, surfaceNumber);
            var curveNumbers = new List<int>();
            for (var i = 0; i < dimTags.Length; i += 2)
                if (dimTags[i] == 1)
                    curveNumbers.Add(dimTags[i + 1]);

            return curveNumbers.ToArray();
        }

        /// <summary>
        /// Возвращает номера граничных точек поверхности.
        /// </summary>
        /// <param name="surfaceNumber">Номер поверхности.</param>
        /// <returns>Номера граничных точек поверхности.</returns>
        private int[] GetSurfaceBoundaryPointNumbers(int surfaceNumber)
        {
            var curveNumbers = GmshController.Gmsh.Model.GetAdjacencies(2, surfaceNumber).Item2;
            var pointNumbers = new HashSet<int>();
            foreach (var curveNumber in curveNumbers)
            {
                var points = GmshController.Gmsh.Model.GetAdjacencies(1, curveNumber).Item2;
                foreach (var pointNumber in points)
                    pointNumbers.Add(pointNumber);
            }

            return pointNumbers.ToArray();
        }

        /// <summary>
        /// Удаляет настройки рекомбинации поверхности.
        /// </summary>
        /// <param name="surfaceNumber">Номер поверхности.</param>
        private void ClearSurfaceRecombineSettings(int surfaceNumber)
        {
            GmshController.Gmsh.Model.Mesh.RemoveConstraints([2, surfaceNumber]);
            GmshController.Gmsh.Model.RemoveAttribute($"recombine surface {surfaceNumber}");
        }

        /// <summary>
        /// Удаляет трансфинитные настройки поверхности.
        /// </summary>
        /// <param name="surfaceNumber">Номер поверхности.</param>
        private void ClearSurfaceTransfiniteSettings(int surfaceNumber)
        {
            GmshController.Gmsh.Model.Mesh.RemoveConstraints([2, surfaceNumber]);
            GmshController.Gmsh.Model.RemoveAttribute($"transfinite surface {surfaceNumber}");
        }

        /// <summary>
        /// Удаляет настройки разбиения объёма.
        /// </summary>
        /// <param name="volumeNumber">Номер объёма.</param>
        private void ClearVolumeMeshingSettings(int volumeNumber)
        {
            GmshController.Gmsh.Model.Mesh.RemoveConstraints([3, volumeNumber]);
            GmshController.Gmsh.Model.RemoveAttribute($"transfinite vol {volumeNumber}");
            GmshController.Gmsh.Model.Mesh.Field.Remove(volumeNumber);

            var points = GmshController.Gmsh.Model.GetEntities(0);
            GmshController.Gmsh.Model.Mesh.RemoveConstraints(points);
            GmshController.Gmsh.Option.SetNumber("Mesh.MeshSizeExtendFromBoundary", 1);
        }

        /// <summary>
        /// Преобразует строку в целое число.
        /// </summary>
        /// <param name="value">Строковое значение.</param>
        /// <param name="parameterName">Имя параметра.</param>
        /// <returns>Целое число.</returns>
        private int ParseInt(string value, string parameterName)
        {
            if (int.TryParse(value, out var result))
                return result;

            throw new InvalidOperationException($"{parameterName} is invalid.");
        }

        /// <summary>
        /// Преобразует строку в вещественное число.
        /// </summary>
        /// <param name="value">Строковое значение.</param>
        /// <param name="parameterName">Имя параметра.</param>
        /// <returns>Вещественное число.</returns>
        private double ParseDouble(string value, string parameterName)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out var result))
                return result;

            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return result;

            throw new InvalidOperationException($"{parameterName} is invalid.");
        }

        /// <summary>
        /// Преобразует строку в перечисление.
        /// </summary>
        /// <typeparam name="T">Тип перечисления.</typeparam>
        /// <param name="value">Строковое значение.</param>
        /// <param name="parameterName">Имя параметра.</param>
        /// <returns>Значение перечисления.</returns>
        private T ParseEnum<T>(string value, string parameterName) where T : struct, Enum
        {
            if (Enum.TryParse(value, out T result))
                return result;

            throw new InvalidOperationException($"{parameterName} is invalid.");
        }

        /// <summary>
        /// Преобразует строку с номерами объектов в массив чисел.
        /// </summary>
        /// <param name="value">Строка с номерами объектов.</param>
        /// <param name="parameterName">Имя параметра.</param>
        /// <returns>Массив номеров объектов.</returns>
        private int[] ParseNumbers(string value, string parameterName)
        {
            var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var numbers = new List<int>();
            foreach (var item in values)
                numbers.Add(ParseInt(item.Trim(), parameterName));

            return numbers.ToArray();
        }

        /// <summary>
        /// Проверяет размерность геометрической сущности.
        /// </summary>
        /// <param name="dimension">Размерность геометрической сущности.</param>
        /// <param name="parameterName">Имя параметра.</param>
        private void ValidateGeometryDimension(int dimension, string parameterName)
        {
            if (dimension < 0 || dimension > 3)
                throw new ArgumentOutOfRangeException(parameterName, "Geometry dimension must be from 0 to 3.");
        }
    }
}
