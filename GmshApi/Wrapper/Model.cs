using System;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Model
    {
        private delegate void gmshModelAddFun(string name, ref int ierr);
        private gmshModelAddFun gmshModelAdd = GetFunction<gmshModelAddFun>("gmshModelAdd");

        private delegate void gmshModelRemoveFun(ref int ierr);
        private gmshModelRemoveFun gmshModelRemove = GetFunction<gmshModelRemoveFun>("gmshModelRemove");

        private delegate void gmshModelListFun(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        private gmshModelListFun gmshModelList = GetFunction<gmshModelListFun>("gmshModelList");

        private delegate void gmshModelGetCurrentFun(ref IntPtr name, ref int ierr);
        private gmshModelGetCurrentFun gmshModelGetCurrent = GetFunction<gmshModelGetCurrentFun>("gmshModelGetCurrent");

        private delegate void gmshModelSetCurrentFun(string name, ref int ierr);
        private gmshModelSetCurrentFun gmshModelSetCurrent = GetFunction<gmshModelSetCurrentFun>("gmshModelSetCurrent");

        private delegate void gmshModelGetFileNameFun(ref IntPtr fileName, ref int ierr);
        private gmshModelGetFileNameFun gmshModelGetFileName = GetFunction<gmshModelGetFileNameFun>("gmshModelGetFileName");

        private delegate void gmshModelSetFileNameFun(string fileName, ref int ierr);
        private gmshModelSetFileNameFun gmshModelSetFileName = GetFunction<gmshModelSetFileNameFun>("gmshModelSetFileName");

        private delegate void gmshModelGetEntitiesFun(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        private gmshModelGetEntitiesFun gmshModelGetEntities = GetFunction<gmshModelGetEntitiesFun>("gmshModelGetEntities");

        private delegate void gmshModelSetEntityNameFun(int dim, int tag, string name, ref int ierr);
        private gmshModelSetEntityNameFun gmshModelSetEntityName = GetFunction<gmshModelSetEntityNameFun>("gmshModelSetEntityName");

        private delegate void gmshModelGetEntityNameFun(int dim, int tag, ref IntPtr name, ref int ierr);
        private gmshModelGetEntityNameFun gmshModelGetEntityName = GetFunction<gmshModelGetEntityNameFun>("gmshModelGetEntityName");

        private delegate void gmshModelRemoveEntityNameFun(string name, ref int ierr);
        private gmshModelRemoveEntityNameFun gmshModelRemoveEntityName = GetFunction<gmshModelRemoveEntityNameFun>("gmshModelRemoveEntityName");

        private delegate void gmshModelGetBoundaryFun(int[] dimTags, IntPtr dimTags_n, ref IntPtr outDimTags,
                                                      ref IntPtr outDimTags_n, int combined, int oriented,
                                                      int recursive, ref int ierr);
        private gmshModelGetBoundaryFun gmshModelGetBoundary = GetFunction<gmshModelGetBoundaryFun>("gmshModelGetBoundary");

        private delegate void gmshModelGetAdjacenciesFun(int dim, int tag, ref IntPtr upward, ref IntPtr upward_n,
                                                         ref IntPtr downward, ref IntPtr downward_n, ref int ierr);
        private gmshModelGetAdjacenciesFun gmshModelGetAdjacencies = GetFunction<gmshModelGetAdjacenciesFun>("gmshModelGetAdjacencies");

        private delegate void gmshModelGetBoundingBoxFun(int dim, int tag, ref double xmin,
                                                         ref double ymin, ref double zmin,
                                                         ref double xmax, ref double ymax,
                                                         ref double zmax, ref int ierr);
        private gmshModelGetBoundingBoxFun gmshModelGetBoundingBox = GetFunction<gmshModelGetBoundingBoxFun>("gmshModelGetBoundingBox");

        private delegate int gmshModelGetDimensionFun(ref int ierr);
        private gmshModelGetDimensionFun gmshModelGetDimension = GetFunction<gmshModelGetDimensionFun>("gmshModelGetDimension");

        private delegate int gmshModelAddDiscreteEntityFun(int dim, int tag, int[] boundary, IntPtr boundary_n, ref int ierr);
        private gmshModelAddDiscreteEntityFun gmshModelAddDiscreteEntity = GetFunction<gmshModelAddDiscreteEntityFun>("gmshModelAddDiscreteEntity");

        private delegate void gmshModelRemoveEntitiesFun(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        private gmshModelRemoveEntitiesFun gmshModelRemoveEntities = GetFunction<gmshModelRemoveEntitiesFun>("gmshModelRemoveEntities");

        private delegate void gmshModelGetTypeFun(int dim, int tag, ref IntPtr entityType, ref int ierr);
        private gmshModelGetTypeFun gmshModelGetType = GetFunction<gmshModelGetTypeFun>("gmshModelGetType");

        private delegate void gmshModelGetValueFun(int dim, int tag, double[] parametricCoord, IntPtr parameticCoord_n, 
                                                   ref IntPtr coord, ref IntPtr coord_n, ref int ierr);
        private gmshModelGetValueFun gmshModelGetValue = GetFunction<gmshModelGetValueFun>("gmshModelGetValue");

        private delegate void gmshModelGetDerivativeFun(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n, 
                                                        ref IntPtr derivatives, ref IntPtr derivatives_n, ref int ierr);
        private gmshModelGetDerivativeFun gmshModelGetDerivative = GetFunction<gmshModelGetDerivativeFun>("gmshModelGetDerivative");

        private delegate void gmshModelGetSecondDerivativeFun(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n, 
                                                              ref IntPtr derivatives, ref IntPtr derivatives_n, ref int ierr);
        private gmshModelGetSecondDerivativeFun gmshModelGetSecondDerivative = GetFunction<gmshModelGetSecondDerivativeFun>("gmshModelGetSecondDerivative");

        private delegate void gmshModelGetCurvatureFun(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n, 
                                                       ref IntPtr curvatures, ref IntPtr curvatures_n, ref int ierr);
        private gmshModelGetCurvatureFun gmshModelGetCurvature = GetFunction<gmshModelGetCurvatureFun>("gmshModelGetCurvature");

        private delegate void gmshModelGetParametrizationFun(int dim, int tag, double[] coord, IntPtr coord_n,
                                                             ref IntPtr parametricCoord, ref IntPtr parametricCoord_n, ref int ierr);
        private gmshModelGetParametrizationFun gmshModelGetParametrization = GetFunction<gmshModelGetParametrizationFun>("gmshModelGetParametrization");

        private delegate void gmshModelGetParametrizationBoundsFun(int dim, int tag, ref IntPtr min, ref IntPtr min_n, 
                                                                   ref IntPtr max, ref IntPtr max_n, ref int ierr);
        private gmshModelGetParametrizationBoundsFun gmshModelGetParametrizationBounds = GetFunction<gmshModelGetParametrizationBoundsFun>("gmshModelGetParametrizationBounds");

        private delegate int gmshModelIsInsideFun(int dim, int tag, double[] coord, IntPtr coord_n, int parametric, ref int ierr);
        private gmshModelIsInsideFun gmshModelIsInside = GetFunction<gmshModelIsInsideFun>("gmshModelIsInside");

        private delegate void gmshModelGetClosestPointFun(int dim, int tag, double[] coord, IntPtr coord_n, ref IntPtr closestCoord,
                                                          ref IntPtr closestCoord_n, ref IntPtr parametricCoord, ref IntPtr parametricCoord_n, ref int ierr);
        private gmshModelGetClosestPointFun gmshModelGetClosestPoint = GetFunction<gmshModelGetClosestPointFun>("gmshModelGetClosestPoint");

        private delegate void gmshModelReparametrizeOnSurfaceFun(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n, int surfaceTag,
                                                             ref IntPtr surfaceParametricCoord, ref IntPtr surfaceParametricCoord_n,
                                                             int which, ref int ierr);
        private gmshModelReparametrizeOnSurfaceFun gmshModelReparametrizeOnSurface = GetFunction<gmshModelReparametrizeOnSurfaceFun>("gmshModelReparametrizeOnSurface");

        private delegate void gmshModelSetCoordinatesFun(int tag, double x, double y, double z, ref int ierr);
        private gmshModelSetCoordinatesFun gmshModelSetCoordinates = GetFunction<gmshModelSetCoordinatesFun>("gmshModelSetCoordinates");

        private delegate void gmshModelSetAttributeFun(string name, string[] values, IntPtr values_n, ref int ierr);
        private gmshModelSetAttributeFun gmshModelSetAttribute = GetFunction<gmshModelSetAttributeFun>("gmshModelSetAttribute");
        
        private delegate void gmshModelGetAttributeFun(string name, ref IntPtr values, ref IntPtr values_n, ref int ierr);
        private gmshModelGetAttributeFun gmshModelGetAttribute = GetFunction<gmshModelGetAttributeFun>("gmshModelGetAttribute");

        private delegate void gmshModelGetAttributeNamesFun(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        private gmshModelGetAttributeNamesFun gmshModelGetAttributeNames = GetFunction<gmshModelGetAttributeNamesFun>("gmshModelGetAttributeNames");

        private delegate void gmshModelRemoveAttributeFun(string name, ref int ierr);
        private gmshModelRemoveAttributeFun gmshModelRemoveAttribute = GetFunction<gmshModelRemoveAttributeFun>("gmshModelRemoveAttribute");

        /// <inheritdoc/>
        public Mesh Mesh { get; private set; }
        /// <inheritdoc/>
        public Geo Geo { get; private set; }
        /// <inheritdoc/>
        public Occ Occ { get; private set; }

        /// <inheritdoc/>
        public Model()
        {
            Mesh = new Mesh();
            Geo = new Geo();
            Occ = new Occ();
        }
        /// <summary>
        /// Добавляет новую модель с заданным именем в список моделей и делает ее текущей
        /// </summary>
        /// <param name="name">[In]Имя новой модели</param>
        public void Add(string name)
        {
            var ierr = 0;
            gmshModelAdd(name, ref ierr);
        }
        /// <summary>
        /// Удаляет текущую модель из списка моделей
        /// </summary>
        public void Remove()
        {
            var ierr = 0;
            gmshModelRemove(ref ierr);
        }
        /// <summary>
        /// Возвращает список имен моделей
        /// </summary>
        /// <returns>Список имен моделей</returns>
        public string[] List()
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelList(ref intValue, ref intValue_n, ref ierr);
            return GetArrayOfString(intValue, intValue_n);
        }
        /// <summary>
        /// Возвращает имя текущей модели
        /// </summary>
        /// <returns>Имя текущей модели</returns>
        public string GetCurrent()
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelGetCurrent(ref intValue, ref ierr);
            return GetString(intValue);
        }
        /// <summary>
        /// Задает текущую модель с именем name
        /// </summary>
        /// <param name="name">Имя модели</param>
        public void SetCurrent(string name)
        {
            var ierr = 0;
            gmshModelSetCurrent(name, ref ierr);
        }
        /// <summary>
        /// Возвращает имя файла ассоциированного с текущей моделью
        /// </summary>
        /// <returns>Имя файла</returns>
        public string GetFileName()
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelGetFileName(ref intValue, ref ierr);
            return GetString(intValue);
        }
        /// <summary>
        /// Ассоциирует файл с текущей моделью
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public void SetFileName(string fileName)
        {
            var ierr = 0;
            gmshModelSetFileName(fileName, ref ierr);
        }
        /// <summary>
        /// Возвращает массив dim:tag(геометрические сущности) по указанному dim
        /// </summary>
        /// <param name="dim">-1 все геометрические сущности, 0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <returns>
        /// Массив пар 
        /// dim - размерность объекта : tag - номер объекта
        /// </returns>
        public int[] GetEntities(int dim = -1)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetEntities(ref intValue, ref intValue_n, dim, ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <summary>
        /// Задать для dim:tag имя сущности
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="name">Имя сущности</param>
        public void SetEntityName(int dim, int tag, string name)
        {
            var ierr = 0;
            gmshModelSetEntityName(dim, tag, name, ref ierr);
        }
        /// <summary>
        /// Возвращает имя сущности для указаных dim:tag
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <returns>Имя сущности</returns>
        public string GetEntityName(int dim, int tag)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelGetEntityName(dim, tag, ref intValue, ref ierr);
            return GetString(intValue);
        }
        /// <summary>
        /// Удалить имя сущности
        /// </summary>
        /// <param name="name"></param>
        public void RemoveEntityName(string name)
        {
            var ierr = 0;
            gmshModelRemoveEntityName(name, ref ierr);
        }
        /// <summary>
        /// Возвращает граничные сущности для массива пар dim:tags
        /// </summary>
        /// <param name="dimTags">Массив пар dim:tag</param>
        /// <param name="combined">Флаг, возвращать единным массивом или по отдельности(флаг false не поддерживается)</param>
        /// <param name="oriented">Флаг, учитывать ориентированность границ(знак перед числом означает что сущность задана в обратном направлении)</param>
        /// <param name="recursive">Флаг установленный в true спускается до уровня геометрических точек и возвращает только их</param>
        /// <returns>Массив границ</returns>
        public int[] GetBoundary(int[] dimTags, bool combined = true, bool oriented = true, bool recursive = false)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetBoundary(dimTags, (IntPtr)dimTags.Length, ref intValue, ref intValue_n,
                                 Convert.ToInt32(combined), Convert.ToInt32(oriented), Convert.ToInt32(recursive), ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <summary>
        /// Взять соседей для текущего dim tag
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <returns>(массив идентификаторов верхнего уровня соседей, массив идентификаторов нижнего уровня соседей)</returns>
        public Tuple<int[], int[]> GetAdjacencies(int dim, int tag)
        {
            var ierr = 0;
            var intValueUp = IntPtr.Zero;
            var intValueUp_n = IntPtr.Zero;
            var intValueDown = IntPtr.Zero;
            var intValueDown_n = IntPtr.Zero;
            gmshModelGetAdjacencies(dim, tag, ref intValueUp, ref intValueUp_n, ref intValueDown, ref intValueDown_n, ref ierr);
            var upAdjacencies = GetObjectsData<int>(intValueUp, intValueUp_n);
            var downAdjacencies = GetObjectsData<int>(intValueDown, intValueDown_n);
            return Tuple.Create(upAdjacencies, downAdjacencies);
        }

        /// <summary>
        /// Вернуть ограничивающий параллелепипед для текущих dim tag. Если dim = tag = -1 взять параллелепипед всей модели
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <returns>(minX,minY,minZ,maxX,maxY,maxZ)</returns>
        public Tuple<double, double, double, double, double, double> GetBoundingBox(int dim, int tag)
        {
            var ierr = 0;
            double xMin = 0.0, yMin = 0.0, zMin = 0.0;
            double xMax = 0.0, yMax = 0.0, zMax = 0.0;
            gmshModelGetBoundingBox(dim, tag, ref xMin, ref yMin, ref zMin, ref xMax, ref yMax, ref zMax, ref ierr);
            return Tuple.Create(xMin, yMin, zMin, xMax, yMax, zMax);
        }
        /// <summary>
        /// Возвращает максимальную размерность модели
        /// </summary>
        /// <returns>0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</returns>
        public int GetDimension()
        {
            var ierr = 0;
            return gmshModelGetDimension(ref ierr);
        }
        /// <summary>
        /// Создает дискретную сущность dim tag с границами boundary
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">-1 для автоматического присвоения или задать номер</param>
        /// <param name="boundary">Массив границ(опционально), позволяет построить топологию модели</param>
        public int AddDiscreteEntity(int dim, int tag = -1, int[] boundary = null)
        {
            var ierr = 0;
            boundary = boundary ?? new int[0];
            return gmshModelAddDiscreteEntity(dim, tag, boundary, (IntPtr)boundary.Length, ref ierr);
        }
        /// <summary>
        /// Удалить пары dim:tag заданные массивом. Если пара dim:tag опирается на соcедей верхнего уровня, то удаление невозможно.
        /// </summary>
        /// <param name="dimTags">Массив пар dim:tag</param>
        /// <param name="recursive">Флаг задает рекурсивное удаление всех сущностей вплоть до точек</param>
        public void RemoveEntities(int[] dimTags, bool recursive = false)
        {
            var ierr = 0;
            gmshModelRemoveEntities(dimTags, (IntPtr)dimTags.Length,Convert.ToInt32(recursive), ref ierr);
        }
        /// <summary>
        /// Возвращает тип геометрической сущности dim:tag
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <returns>Тип геометрической сущности</returns>
        public string GetType(int dim, int tag)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelGetType(dim, tag, ref intValue, ref ierr);
            return GetString(intValue);
        }
        /// <summary>
        /// Возвращает координаты точек заданные параметром для указанного идентификатора и размерности
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="parametricCoord">Массив параметрических координат (пустой массив для геометрических точек, t - для кривых и u,v - для поверхностей)</param>
        /// <returns>Массив локальных координат</returns>
        public double[] GetValue(int dim, int tag, double[] parametricCoord)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetValue(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length, ref intValue, ref intValue_n, ref ierr);
            return GetObjectsData<double>(intValue, intValue_n);
        }
        /// <summary>
        /// Возвращает массив частной производной
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="parametricCoord">Параметрические координаты (t - для кривых и u,v - для поверхностей)</param>
        /// <returns>Массив частной производной</returns>
        public double[] GetDerivative(int dim, int tag, double[] parametricCoord)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetDerivative(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length, ref intValue, ref intValue_n, ref ierr);
            return GetObjectsData<double>(intValue, intValue_n);
        }
        /// <summary>
        /// Возвращает массив частной второй производной
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="parametricCoord">Параметрические координаты (t - для кривых и u,v - для поверхностей)</param>
        /// <returns>Массив частной второй производной</returns>
        public double[] GetSecondDerivative(int dim, int tag, double[] parametricCoord)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetSecondDerivative(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length, ref intValue, ref intValue_n, ref ierr);
            return GetObjectsData<double>(intValue, intValue_n);
        }
        /// <summary>
        /// Возвращает кривизну для указанных dim:tag и заданных параметрических координат
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="parametricCoord">Параметрические координаты (t - для кривых и u,v - для поверхностей)</param>
        /// <returns>Массив кривизны</returns>
        public double[] GetCurvature(int dim, int tag, double[] parametricCoord)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetCurvature(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length, ref intValue, ref intValue_n, ref ierr);
            return GetObjectsData<double>(intValue, intValue_n);
        }
        /// <summary>
        /// Возвращает параметризацию сущности заданной парой dim:tag
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="coord">Массив локальных координат</param>
        /// <returns>Массив параметрических координат (t - для кривых и u,v - для поверхностей)</returns>
        public double[] GetParametrization(int dim, int tag, double[] coord)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetParametrization(dim, tag, coord, (IntPtr)coord.Length, ref intValue, ref intValue_n, ref ierr);
            return GetObjectsData<double>(intValue, intValue_n);
        }
        /// <summary>
        /// Возвращает минимальные и максимальные границы параметризации для текущих dim : tag
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <returns>(Массив  минимальных параметрических координат, массив максимальных параметрических координат)</returns>
        public Tuple<double[], double[]> GetParametrizationBounds(int dim, int tag)
        {
            var ierr = 0;
            var intValueMin = IntPtr.Zero;
            var intValueMin_n = IntPtr.Zero;
            var intValueMax = IntPtr.Zero;
            var intValueMax_n = IntPtr.Zero;
            gmshModelGetParametrizationBounds(dim, tag, ref intValueMin, ref intValueMin_n, ref intValueMax, ref intValueMax_n, ref ierr);
            var min = GetObjectsData<double>(intValueMin, intValueMin_n);
            var max = GetObjectsData<double>(intValueMax, intValueMax_n);
            return Tuple.Create(min, max);
        }
        /// <summary>
        /// Определяет, находится ли точка параметрическая или локальная внутри массива коодинат
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="coord">Масив координат</param>
        /// <param name="parametric">Флаг, является ли координата параметрической</param>
        /// <returns>Число точек лежащих внутри модели</returns>
        public int IsInside(int dim, int tag, double[] coord, bool parametric = false)
        {
            var ierr = 0;
            return gmshModelIsInside(dim, tag, coord, (IntPtr)coord.Length, Convert.ToInt32(parametric), ref ierr);
        }

        /// <summary>
        /// Ищет ближайшую координату к заданной coord, используется ортогональное проецирование на сущность dim:tag
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="coord">Координата точки</param>
        /// <returns>Кортеж(трехмерная координата ближайшая к coord, параметрическая координата локальной координаты)</returns>
        public Tuple<double[], double[]> GetClosestPoint(int dim, int tag, double[] coord)
        {
            var ierr = 0;

            IntPtr closestCoord = IntPtr.Zero;
            IntPtr closestCoord_n = IntPtr.Zero;
            IntPtr parametricCoord = IntPtr.Zero;
            IntPtr parametricCoord_n = IntPtr.Zero;

            gmshModelGetClosestPoint(dim, tag, coord, (IntPtr)coord.Length, ref closestCoord, ref closestCoord_n,
                                     ref parametricCoord, ref parametricCoord_n, ref ierr);

            var localCoords = GetObjectsData<double>(closestCoord, closestCoord_n);
            var parametricCoords = GetObjectsData<double>(parametricCoord, parametricCoord_n);

            return Tuple.Create(localCoords, parametricCoords);
        }

        /// <summary>
        /// Выполняет перепараметризацию сущностей dim : tag (точки и кривые) с исходной параметризацией для surfaceTag
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="parametricCoord">Параметрические координаты</param>
        /// <param name="surfaceTag">Идентификатор поверхности</param>
        /// <param name="which">Флаг, указывающий множественный результат в случае переодических поверхностей</param>
        /// <returns>Соответствующие параметрические координаты поверхностей</returns>
        public double[] ReparametrizeOnSurface(int dim, int tag, double[] parametricCoord, int surfaceTag, int which = 0)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelReparametrizeOnSurface(dim,tag, parametricCoord, (IntPtr)parametricCoord.Length, surfaceTag, ref intValue, ref intValue_n, which, ref ierr);
            return GetObjectsData<double> (intValue, intValue_n);
        }
        /// <summary>
        /// Создает геометрическую точку с идендификатором tag и локальными координатами x,y,z
        /// </summary>
        /// <param name="tag">Идентификатор</param>
        /// <param name="x">Локальная координата X</param>
        /// <param name="y">Локальная координата Y</param>
        /// <param name="z">Локальная координата Z</param>
        public void SetCoordinates(int tag, double x, double y, double z)
        {
            var ierr = 0;
            gmshModelSetCoordinates(tag, x, y, z, ref ierr);
        }
        /// <summary>
        /// Связать имя аттрибута со значениями
        /// </summary>
        /// <param name="name">Имя аттрибута</param>
        /// <param name="values">Массив значений</param>
        public void SetAttribute(string name, string[] values)
        {
            var ierr = 0;
            gmshModelSetAttribute(name, values, (IntPtr)values.Length, ref ierr);
        }
        /// <summary>
        /// Возвращает значения для заданного аттрибута
        /// </summary>
        /// <param name="name">Имя аттрибута</param>
        /// <returns>Массив значений</returns>
        public string[] GetAttribute(string name)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetAttribute(name, ref intValue, ref intValue_n, ref ierr);
            return GetArrayOfString(intValue, intValue_n);
        }
        /// <summary>
        /// Получить список имен аттрибутов
        /// </summary>
        /// <returns>Массив имен аттрибутов</returns>
        public string[] GetAttributeNames()
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetAttributeNames(ref intValue, ref intValue_n, ref ierr);
            return GetArrayOfString(intValue, intValue_n);
        }
        /// <summary>
        /// Удалить указанное имя аттрибута
        /// </summary>
        /// <param name="name">Имя аттрибута</param>
        public void RemoveAttribute(string name)
        {
            var ierr = 0;
            gmshModelRemoveAttribute(name, ref ierr);
        }
    }
}
