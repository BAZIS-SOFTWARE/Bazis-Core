using Functions.Extensions;
using GmshApi.GmshKernel;
using Model;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GmshApi.GmshController
{
    /// <summary>
    /// Оборачивает основные методы геометрии полученные из GmshController
    /// </summary>
    public static class ControllerGeometry
    {
        /// <summary>
        /// Возвращает набор геометрических сущностей Gmsh
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <returns>массив сущностей</returns>
        public static int[] GetEntitiesTags(int dim)
        {
            var ierr = 0;
            IntPtr dimTags, dimTags_n;
            dimTags = dimTags_n = IntPtr.Zero;
            var extension = ControllerIO.ModelGetFileNameExtension();
            if (extension.Contains("stp") || extension.Contains("step") ||
               extension.Contains("brep") || extension.Contains("igs"))
                KernelGeometry.gmshModelOccGetEntities(ref dimTags, ref dimTags_n, dim, ref ierr);
            else
                KernelGeometry.gmshModelGetEntities(ref dimTags, ref dimTags_n, dim, ref ierr);
            int[] tags;
            ControllerMemory.GetObjectsData<int>(dimTags, out tags, dimTags_n);
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
            KernelGeometry.gmshModelSetEntityName(dim, tag, name, ref ierr);
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
            KernelGeometry.gmshModelGetEntityName(dim, tag, ref name, ref error);
            return ControllerMemory.GetName(name);
        }
        /// <summary>
        /// Удаляет сущность по имени
        /// </summary>
        /// <param name="name">[In]Имя сущности</param>
        public static void RemoveEntityName(string name)
        {
            var ierr = 0;
            KernelGeometry.gmshModelRemoveEntityName(name, ref ierr);
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
            KernelGeometry.gmshModelGetPhysicalName(dim, tag, ref name, ref error);
            return ControllerMemory.GetName(name);
        }
        /// <summary>
        /// Возвращает максимальную размерность текущего геометрического объекта
        /// </summary>
        /// <returns>Размерность</returns>
        public static int GetDimension()
        {
            var ierr = 0;
            return KernelGeometry.gmshModelGetDimension(ref ierr);
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
            KernelGeometry.gmshModelGetBoundingBox(dim, tag, ref min[0], ref min[1], ref min[2],
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
            KernelGeometry.gmshModelGetAdjacencies(dim, tag, ref upward, ref upward_n,
                                              ref downward, ref downward_n, ref ierr);
            var result = new int[2][];
            if (ierr == 0)
            {
                int[] outDownward, outUpward;
                ControllerMemory.GetObjectsData(downward, out outDownward, downward_n);
                ControllerMemory.GetObjectsData(upward, out outUpward, upward_n);
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
                KernelGeometry.gmshModelGetDerivative(dim, tag, parametrs, (IntPtr)parametrs.Length,
                                                          ref derivaties, ref derivaties_n, ref ierr);
            else
                KernelGeometry.gmshModelGetSecondDerivative(dim, tag, parametrs, (IntPtr)parametrs.Length,
                                                          ref derivaties, ref derivaties_n, ref ierr);
            double[] derivative;
            ControllerMemory.GetObjectsData(derivaties, out derivative, derivaties_n);
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
            KernelGeometry.gmshModelGetValue(dim, tag, parametric, (IntPtr)parametric.Length, ref coord, ref coord_n, ref ierr);
            if (ierr == 0)
                ControllerMemory.GetObjectsData(coord, out local, coord_n);
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
            KernelGeometry.gmshModelGetValue(dim, tag, local, (IntPtr)local.Length,
                                           ref parametricCoord, ref parametricCoord_n, ref ierr);
            if (ierr == 0)
                ControllerMemory.GetObjectsData(parametricCoord, out parametric, parametricCoord_n);
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
            KernelGeometry.gmshModelGetParametrizationBounds(dim, tag, ref min, ref min_n,
                                                           ref max, ref max_n, ref ierr);
            if (ierr == 0)
            {
                ControllerMemory.GetObjectsData(min, out result[0], min_n);
                ControllerMemory.GetObjectsData(max, out result[1], max_n);
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
            count = KernelGeometry.gmshModelIsInside(dim, tag, coords, (IntPtr)coords.Length, paramFlag, ref ierr);
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
            var count = KernelGeometry.gmshModelIsInside(dim, tag, new double[] { coords }, (IntPtr)1, 1, ref ierr);
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
        public static void RemoveEntities(int[] dimTags, bool recursive = true)
        {
            var ierr = 0;
            var recFlag = Convert.ToInt32(recursive);
            var extension = ControllerIO.ModelGetFileNameExtension();
            if (extension.Contains("stp") || extension.Contains("step") ||
                extension.Contains("brep") || extension.Contains("igs"))
                KernelGeometry.gmshModelOccRemove(dimTags, (IntPtr)dimTags.Length, recFlag, ref ierr);
            else
                KernelGeometry.gmshModelRemoveEntities(dimTags, (IntPtr)dimTags.Length, recFlag, ref ierr);
            //GmshWrapperModel.Synchronize();
        }
        /// <summary>
        /// Удаление верхних сущностей рекурсивно
        /// </summary>
        /// <param name="dimTags">[In]Пары dim:tag</param>
        /// <param name="recursive">[In]Флаг рекурсии</param>
        public static void RemoveEntitiesEx(int[] dimTags, bool recursive = true)
        {
            for (var i = 1; i < dimTags.Length; i += 2)
            {
                GetUpAdjacenciesRecursive( dimTags[i - 1], dimTags[i]);
                RemoveEntities(new int[] { dimTags[i - 1], dimTags[i] }, recursive);
            }
        }
        /// <summary>
        /// Удаляет верхние граничные зависимости от текущей пары dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        public static void GetUpAdjacenciesRecursive(int dim, int tag)
        {
            var tags = GetAdjacencies(dim, tag)[1];
            for (var i = 0; i < tags.Length; ++i)
                GetUpAdjacenciesRecursive(dim + 1, tags[i]);
            RemoveEntities(new int[] { dim, tag }, false);
        }
        /// <summary>
        /// Установить аттрибуту значения
        /// </summary>
        /// <param name="name">[In]Имя аттрибута</param>
        /// <param name="values">[In]Значения</param>
        /// <param name="values_n">[In]Количество</param>
        public static void SetAttribute(string name, string[] values, int values_n)
        {
            var ierr = 0;
            KernelGeometry.gmshModelSetAttribute(name, values, (IntPtr)values_n,ref ierr);
        }
        /// <summary>
        /// Получает аттрибуты по имени
        /// </summary>
        /// <param name="name">[In]Имя аттрибута</param>
        /// <returns>Значения аттрибутов</returns>
        public static string[] GetAttribute(string name)
        {
            var ierr = 0;
            IntPtr values = IntPtr.Zero, values_n = IntPtr.Zero;    
            KernelGeometry.gmshModelGetAttribute(name, ref values, ref values_n ,ref ierr);
            return ControllerMemory.GetArrayOfNames(values, values_n);
        }
        /// <summary>
        /// Возвращает список всех аттрибутов
        /// </summary>
        /// <returns>Список аттрибутов</returns>
        public static string[] GetAttributeNames()
        {
            var ierr = 0;
            IntPtr names = IntPtr.Zero, names_n = IntPtr.Zero;
            KernelGeometry.gmshModelGetAttributeNames(ref names, ref names_n, ref ierr);
            return ControllerMemory.GetArrayOfNames(names, names_n);
        }
        /// <summary>
        /// Удалить аттрибут с именем name
        /// </summary>
        /// <param name="name">[In]Имя аттрибута</param>
        public static void RemoveAttribute(string name)
        {
            var ierr = 0;
            KernelGeometry.gmshModelRemoveAttribute(name, ref ierr);
        }
        /// <summary>
        /// Получает сущности конкретного ModelObject
        /// </summary>
        /// <param name="dimension">[In]Размерность</param>
        /// <param name="kind">[In]Тип объекта</param>
        /// <returns></returns>
        public static List<ModelObject> GetEntities(ObjKind kind)
        {
            int[] tags;
            var entities = new List<ModelObject>();

            switch(kind)
            {
                case ObjKind.Point:
                    tags = GetEntitiesTags(0).Where((v, i) => (i & 1) == 1).ToArray();
                    entities = CreateControlPoints(tags);
                    break;
                case ObjKind.Curve:
                case ObjKind.Beam:
                    tags = GetEntitiesTags(1).Where((v, i) => (i & 1) == 1).ToArray();
                    entities = CreateLines(tags, kind);
                    break;

            }
 
            return entities;
        }
        /*/// <summary>
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
        */
        private static List<ModelObject> CreateLines(int[] tags, ObjKind kind)
        {
            var lines = new List<ModelObject>();
            for (var i = 0; i < tags.Length; ++i)
            {
                var paramsBounds = GetParametrizationBounds(1, tags[i]);
                var newPB = paramsBounds[0].Concat(paramsBounds[1]).ToArray();
                if (!IsLine(1, tags[i], newPB))
                {
                    var curvePoints = Convert.ToInt32(ControllerGeneral2.OptionGetNumber("Geometry.NumSubEdges"));
                    newPB = SetCustomStepParametrization(newPB, curvePoints);
                }
                var locals = GetLocalCoordByParametric(1, tags[i], newPB);
                var points = ControllerModel.ConvertLocalCoordsToPoints3D(locals);
                var nodes = new List<Node>();
                var num = 0;
                foreach (var pair in points.Bigrams(false))
                {
                    var line = new Node[] { new Node(num++, pair.Item1), new Node(num++, pair.Item2) };
                    nodes.AddRange(line);
                }
                if(kind == ObjKind.Curve)
                    lines.Add(new Curve(tags[i], nodes.ToArray()));
                else
                    lines.Add(new Element1D(tags[i], nodes.ToArray(), kind));
            }
            return lines;
        }

        private static List<ModelObject> CreateControlPoints(int[] tags)
        {
            var nodes = new List<ModelObject>();
            for (var i = 0; i < tags.Length; ++i)
            {
                var coords = GetLocalCoordByParametric(0, tags[i], new double[] { });
                var points = ControllerModel.ConvertLocalCoordsToPoints3D(coords);
                for(var j = 0; j < points.Length; ++j)
                    nodes.Add(new Node(tags[i], points[j]));
            }
            return nodes;
        }
    }
}