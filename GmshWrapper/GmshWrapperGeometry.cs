using System;
using Model.GmshController;
using Model.GmshUser;
using System.Linq;
using System.Collections.Generic;
using Model.Interfaces;

namespace Model.GmshWrapper
{
    /// <summary>
    /// Оборачивает основные методы геометрии полученные из GmshController
    /// </summary>
    public static class GmshWrapperGeometry
    {
        /// <summary>
        /// Возвращает набор геометрических сущностей Gmsh
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <returns>массив сущностей</returns>
        public static int[] GetEntities(int dim)
        {
            var ierr = 0;
            IntPtr dimTags, dimTags_n;
            dimTags = dimTags_n = IntPtr.Zero;
            var extension = GmshWrapperIO.ModelGetFileNameExtension();
            if (extension.Contains("stp") || extension.Contains("step") ||
               extension.Contains("brep") || extension.Contains("igs"))
                GmshGeometry.gmshModelOccGetEntities(ref dimTags, ref dimTags_n, dim, ref ierr);
            else
                GmshGeometry.gmshModelGetEntities(ref dimTags, ref dimTags_n, dim, ref ierr);
            int[] tags;
            GmshWrapperMemory.GetTypesData<int>(dimTags, out tags, dimTags_n);
            return tags;
        }
        /// <summary>
        /// Задает имя сущности для текщих dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="name">[In]Имя сущности</param>
        public static void SetEntityName(int dim, int tag, string name)
        {
            var ierr = 0;
            GmshGeometry.gmshModelSetEntityName(dim, tag, name, ref ierr);
        }
        /// <summary>
        /// Возвращает имя сущности для заданной размерности : идентификатора
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <returns>Имя сущности или пустая строка если не найдена</returns>
        public static string GetEntityName(int dim, int tag)
        {
            var error = 0;
            IntPtr name = IntPtr.Zero;
            GmshGeometry.gmshModelGetEntityName(dim, tag, ref name, ref error);
            return GmshUserMemory.GetName(name);
        }
        /// <summary>
        /// Удаляет сущность по имени
        /// </summary>
        /// <param name="name">[In]Имя сущности</param>
        public static void RemoveEntityName(String name)
        {
            var ierr = 0;
            GmshGeometry.gmshModelRemoveEntityName(name, ref ierr);
        }
        /// <summary>
        /// Получает физическое имя для данной размерности : идентификатора
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <returns>Физическое имя</returns>
        public static string GetPhysicalName(int dim, int tag)
        {
            var error = 0;
            IntPtr name = IntPtr.Zero;
            GmshGeometry.gmshModelGetPhysicalName(dim, tag, ref name, ref error);
            return GmshUserMemory.GetName(name);
        }
        /// <summary>
        /// Возвращает максимальную размерность текущего геометрического объекта
        /// </summary>
        /// <returns>Размерность</returns>
        public static int GetDimension()
        {
            var ierr = 0;
            return GmshGeometry.gmshModelGetDimension(ref ierr);
        }
        /// <summary>
        /// Возвращает ограничивающий параллелепипед для указанного dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="min">[Out]Набор минимальных координат x,y,z</param>
        /// <param name="max">[Out]Набор максимальных координат x,y,z</param>
        public static void GetBoundingBox(int dim, int tag, out double[] min, out double[] max)
        {
            var ierr = 0;
            min = new double[3];
            max = new double[3];
            GmshGeometry.gmshModelGetBoundingBox(dim, tag, ref min[0], ref min[1], ref min[2],
                                                 ref max[0], ref max[1], ref max[2], ref ierr);
        }

        /// <summary>
        /// Возвращает матрицу смежности для текущей пары dim : tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <returns>[i][]Нижние элементы,[i + 1][]Верхние элементы</returns>
        public static int[][] GetAdjacencies(int dim, int tag)
        {
            var ierr = 0;
            IntPtr upward, upward_n, downward, downward_n;
            upward = upward_n = downward = downward_n = IntPtr.Zero;
            GmshGeometry.gmshModelGetAdjacencies(dim, tag, ref upward, ref upward_n,
                                              ref downward, ref downward_n, ref ierr);
            var result = new int[2][];
            if (ierr == 0)
            {
                int[] outDownward, outUpward;
                GmshUserMemory.GetTypesData(downward, out outDownward, downward_n);
                GmshUserMemory.GetTypesData(upward, out outUpward, upward_n);
                result[0] = outDownward;
                result[1] = outUpward;
            }
            return result;
        }
        /// <summary>
        /// Возвращает частную производную по параметрическим координатам
        /// </summary>
        /// <param name ="dim" >[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametrs">[In]Параметрические координаты</param>
        /// <param name="number">[In]Номер производной</param>
        /// <returns></returns>
        public static double[] GetDerivative(int dim, int tag, double[] parametrs, int number = 1)
        {
            var ierr= 0;
            var derivaties = IntPtr.Zero;
            var derivaties_n = IntPtr.Zero;
            if(number == 1)
                GmshGeometry.gmshModelGetDerivative(dim, tag, parametrs, (IntPtr)parametrs.Length,
                                                          ref derivaties, ref derivaties_n, ref ierr);
            else
                GmshGeometry.gmshModelGetSecondDerivative(dim, tag, parametrs, (IntPtr)parametrs.Length,
                                                          ref derivaties, ref derivaties_n, ref ierr);
            double[] derivative;
            GmshUserMemory.GetTypesData(derivaties, out derivative, derivaties_n);
            return derivative;
        }
        /// <summary>
        /// Получает координаты точек заданные параметром для указанного идентификатора и размерности
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametric">[In]Параметрические координаты могут быть пустые</param>
        /// <returns>Локальные координаты</returns>
        public static double[] GetLocalCoordByParametric(int dim, int tag, double[] parametric)
        {
            var ierr = 0;
            var local = new double[0];
            IntPtr coord, coord_n;
            coord = coord_n = IntPtr.Zero;
            GmshGeometry.gmshModelGetValue(dim, tag, parametric, (IntPtr)parametric.Length, ref coord, ref coord_n, ref ierr);
            if (ierr == 0)
                GmshWrapperMemory.GetTypesData(coord, out local, coord_n);
            return local;
        }
        /// <summary>
        /// Получает координаты точек заданные параметром для указанного идентификатора и размерности
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="local">[In]Локальные координаты</param>
        /// <returns>Параметрические координаты</returns>
        public static double[] GetParametricCoordByLocal(int dim, int tag, double[] local)
        {
            var ierr = 0;
            var parametric = new double[0];
            IntPtr parametricCoord, parametricCoord_n;
            parametricCoord = parametricCoord_n = IntPtr.Zero;
            GmshGeometry.gmshModelGetValue(dim, tag, local, (IntPtr)local.Length,
                                           ref parametricCoord, ref parametricCoord_n, ref ierr);
            if (ierr == 0)
                GmshWrapperMemory.GetTypesData(parametricCoord, out parametric, parametricCoord_n);
            return parametric;
        }
        /// <summary>
        /// Возвращает границы параметризации array[0] минимальные границы array[1] максимальные, длина внутренного массива
        /// зависит от dim (для 1 - 1 параметр, для 2 - 2 параметра)
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <returns>Возвращает границы параметризации</returns>
        public static double[][] GetParametrizationBounds(int dim, int tag)
        {
            var ierr = 0;
            IntPtr min, min_n, max, max_n;
            min = min_n = max = max_n = IntPtr.Zero;
            var paramsOutLength = dim == 1 ? 1 : 2;
            var result = new double[2][];
            for(var i = 0; i < 2; ++i)
                result[i] = new double[0];
            GmshGeometry.gmshModelGetParametrizationBounds(dim, tag, ref min, ref min_n,
                                                           ref max, ref max_n, ref ierr);
            if (ierr == 0)
            {
                GmshUserMemory.GetTypesData(min, out result[0], min_n);
                GmshUserMemory.GetTypesData(max, out result[1], max_n);
            }
            return result;
        }
        /// <summary>
        /// Возвращает число координат заданных массивом и dim:tag, оказавшихся внутри модели
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="coords">[In]Массив координат</param>
        /// <param name="isParametric">[In]Флаг является ли координаты параметрические</param>
        /// <returns>Число координат</returns>
        public static int IsInside(int dim, int tag, double[] coords, bool isParametric = true)
        {
            var ierr = 0;
            var paramFlag = Convert.ToInt32(isParametric);
            var count = 0;
            count = GmshGeometry.gmshModelIsInside(dim, tag, coords, (IntPtr)coords.Length, paramFlag, ref ierr);
            return count;
        }
        /// <summary>
        /// Лежит ли параметрическая координата внутри массива
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="coords">[In]Массив координат</param>
        /// <returns>Число координат</returns>
        public static bool IsParametricInside(int dim, int tag, double coords)
        {
            var ierr = 0;
            var count = GmshGeometry.gmshModelIsInside(dim, tag, new double[] { coords }, (IntPtr)1, 1, ref ierr);
            return count != 0;
        }
        /// <summary>
        /// Задать произвольный шаг параметризации для кривой
        /// </summary>
        /// <param name="paramsCoord">[In]Параметрические координаты</param>
        /// <param name="steps">[In]Число точек</param>
        /// <returns></returns>
        public static double[] SetCustomStepParametrization(double[] paramsCoord, int steps)
        {
            var step = (paramsCoord[1] - paramsCoord[0]) / (steps - 1);
            Array.Resize(ref paramsCoord, steps);
            for (var i = 0; i < steps; ++i)
                paramsCoord[i] = paramsCoord[0] + step * i;
            return paramsCoord;
        }

        /// <summary>
        /// Является ли кривая линией
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="paramsBound">[In]Параметрические координаты</param>
        /// <returns>Является ли кривая линией</returns>
        public static bool IsLine(int dim, int tag, double[] paramsBound)
        {
            var modParams = new double[] { paramsBound[0] + 0.01, paramsBound[1] - 0.01 };
            var derivatives = GetDerivative(dim, tag, paramsBound);
            var derCount = derivatives.Length / 3;
            for (var i = 1; i < derCount; ++i)
                for (var j = 0; j < 3; ++j)
                    if (Math.Abs(derivatives[i * 3 + j] - derivatives[(i - 1) * 3 + j]) > 1E-8)
                        return false;
            return true;
        }
        /// <summary>
        /// Удаляет сущности указанные параметром dim:tag
        /// </summary>
        /// <param name="dimTags">[In]Пары dim:tag</param>
        /// <param name="recursive">[In]Флаг рекурсивного удаления</param>
        /// <returns>Флаг успеха</returns>
        public static bool RemoveEntities(int[] dimTags, bool recursive = true)
        {
            var ierr = 0;
            var recFlag = Convert.ToInt32(recursive);
            var extension = GmshWrapperIO.ModelGetFileNameExtension();
            if (extension.Contains("stp") || extension.Contains("step") ||
                extension.Contains("brep") || extension.Contains("igs"))
            {
                GmshGeometry.gmshModelOccRemove(dimTags, (IntPtr)dimTags.Length, recFlag, ref ierr);
                GmshGeometry.gmshModelOccSynchronize(ref ierr);
            }
            else
            {
                GmshGeometry.gmshModelRemoveEntities(dimTags, (IntPtr)dimTags.Length, recFlag, ref ierr);
                GmshGeometry.gmshModelGeoSynchronize(ref ierr);
            }
            return ierr == 0;
        }


        /// <summary>
        /// Получает сущности конкретного ModelObject
        /// </summary>
        /// <typeparam name="T">[In]Тип ModelObject</typeparam>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="fillFun">[In]Метод для заполнения списка</param>
        /// <returns></returns>
        public static IEnumerable<T> GetEntities<T>(int dim, Action<List<T>, int, Node[]> fillFun) where T : ModelObject
        {
            var tags = GetEntities(dim).Where((v, i) => (i & 1) == 1).ToArray();
            var entities = new List<T>();
            if (dim == 0)
                CreateControlPoints(dim, tags, entities, fillFun);
            if (dim == 1)
                CreateLines(dim, tags, entities, fillFun);
            return entities;
        }
        /// <summary>
        /// Создает контрольную точку заполняя список
        /// </summary>
        /// <param name="list">[In]Список</param>
        /// <param name="id">[In]Идентификатор сущности</param>
        /// <param name="nodes">[In]Узлы для сущности</param>
        public static void CreateControlPoint(List<Node> list, int id, Node[] nodes) => list.AddRange(nodes);
        /// <summary>
        /// Создает кривую заполняя список
        /// </summary>
        /// <param name="list">[In]Список</param>
        /// <param name="id">[In]Идентификатор сущности</param>
        /// <param name="nodes">[In]Узлы для сущности</param>
        public static void CreateCurve(List<Curve> list, int id, Node[] nodes) => list.Add(new Curve(id, nodes));
        /// <summary>
        /// Создает Element1D заполняя список
        /// </summary>
        /// <param name="list">[In]Список</param>
        /// <param name="id">[In]Идентификатор сущности</param>
        /// <param name="nodes">[In]Узлы для сущности</param>
        public static void CreateElement1D(List<Element1D> list, int id, Node[] nodes) => list.Add(new Element1D(id, nodes, ObjKind.Beam));

        private static void CreateLines<T>(int dim, int[] tags, List<T> lines, 
                                                  Action<List<T>, int, Node[]> fillFun) where T : ModelObject
        {
            for (var i = 0; i < tags.Length; ++i)
            {
                var paramsBounds = GetParametrizationBounds(1, tags[i]);
                var newPB = paramsBounds[0].Concat(paramsBounds[1]).ToArray();
                if (!IsLine(1, tags[i], newPB))
                {
                    var curvePoints = Convert.ToInt32(GmshWrapperGeneral.OptionGetNumber("Geometry.NumSubEdges"));
                    newPB = SetCustomStepParametrization(newPB, curvePoints);
                }
                var locals = GetLocalCoordByParametric(1, tags[i], newPB);
                var points = GmshWrapperModel.ConvertLocalCoordsToPoints3D(locals);
                var nodes = new List<Node>();
                var num = 0;
                foreach (var pair in points.Bigrams())
                {
                    var line = new Node[] { new Node(num++, pair.Item1), new Node(num++, pair.Item2) };
                    nodes.AddRange(line);
                }
                fillFun(lines, tags[i], nodes.ToArray());
            }
        }

        private static void CreateControlPoints<T>(int dim, int[] tags, List<T> list,
                                                  Action<List<T>, int, Node[]> fillFun) where T : ModelObject
        {
            for (var i = 0; i < tags.Length; ++i)
            {
                var coords = GetLocalCoordByParametric(dim, tags[i], new double[] { });
                var points = GmshWrapperModel.ConvertLocalCoordsToPoints3D(coords);
                for(var j = 0; j < points.Length; ++j)
                {
                    var nodes = new Node[] { new Node(tags[i], points[j]) };
                    fillFun(list, 0, nodes);
                }
            }
        }
    }
}
