using System;
using System.Drawing;
using System.Net.Http.Headers;
using System.Reflection;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Occ
    {
        private delegate int gmshModelOccAddPointFun(double x, double y, double z, double meshSize, int tag, ref int ierr);
        private gmshModelOccAddPointFun gmshModelOccAddPoint = GetFunction<gmshModelOccAddPointFun>("gmshModelOccAddPoint");

        private delegate int gmshModelOccAddLineFun(int startTag, int endTag, int tag, ref int ierr);
        private gmshModelOccAddLineFun gmshModelOccAddLine = GetFunction<gmshModelOccAddLineFun>("gmshModelOccAddLine");

        private delegate int gmshModelOccAddCircleArcFun(int startTag, int centerTag, int endTag, int tag, ref int ierr);
        private gmshModelOccAddCircleArcFun gmshModelOccAddCircleArc = GetFunction<gmshModelOccAddCircleArcFun>("gmshModelOccAddCircleArc");

        private delegate int gmshModelOccAddCircleFun(double x, double y, double z, double r, int tag, double angle1, double angle2, 
                                                      double[] zAxis, IntPtr zAxis_n, double[] xAxis, IntPtr xAxis_n, ref int ierr);
        private gmshModelOccAddCircleFun gmshModelOccAddCircle = GetFunction<gmshModelOccAddCircleFun>("gmshModelOccAddCircle");

        private delegate int gmshModelOccAddBezierFun(int[] pointTags, IntPtr pointTags_n, int tag, ref int ierr);
        private gmshModelOccAddBezierFun gmshModelOccAddBezier = GetFunction<gmshModelOccAddBezierFun>("gmshModelOccAddBezier");

        private delegate int gmshModelOccAddWireFun(int[] curveTags, IntPtr curveTags_n, int tag, int checkClosed, ref int ierr);
        private gmshModelOccAddWireFun gmshModelOccAddWire = GetFunction<gmshModelOccAddWireFun>("gmshModelOccAddWire");

        private delegate int gmshModelOccAddCurveLoopFun(int[] curveTags, IntPtr curveTags_n, int tag, ref int ierr);
        private gmshModelOccAddCurveLoopFun gmshModelOccAddCurveLoop = GetFunction<gmshModelOccAddCurveLoopFun>("gmshModelOccAddCurveLoop");

        private delegate int gmshModelOccAddRectangleFun(double x, double y, double z, double dx, double dy, int tag, double roundedRadius, ref int ierr);
        private gmshModelOccAddRectangleFun gmshModelOccAddRectangle = GetFunction<gmshModelOccAddRectangleFun>("gmshModelOccAddRectangle");

        private delegate int gmshModelOccAddPlaneSurfaceFun(int[] wireTags, IntPtr wireTags_n, int tag, ref int ierr);
        private gmshModelOccAddPlaneSurfaceFun gmshModelOccAddPlaneSurface = GetFunction<gmshModelOccAddPlaneSurfaceFun>("gmshModelOccAddPlaneSurface");

        private delegate int gmshModelOccAddSurfaceFillingFun(int wireTag, int tag, int[] pointTags, IntPtr pointTags_n, int degree, int numPointsOnCurves,
                                                              int numIter, int anisotropic, double tol2d, double tol3d, double tolAng, double tolCurv,
                                                              int maxDegree, int maxSegments, ref int ierr);
        private gmshModelOccAddSurfaceFillingFun gmshModelOccAddSurfaceFilling = GetFunction<gmshModelOccAddSurfaceFillingFun>("gmshModelOccAddSurfaceFilling");

        private delegate int gmshModelOccAddBoxFun(double x, double y, double z, double dx, double dy, double dz, int tag, ref int ierr);
        private gmshModelOccAddBoxFun gmshModelOccAddBox = GetFunction<gmshModelOccAddBoxFun>("gmshModelOccAddBox");

        private delegate int gmshModelOccAddWedgeFun(double x, double y, double z, double dx, double dy, double dz, 
                                                      int tag, double ltx, double[] zAxis, IntPtr zAxis_n, ref int ierr);
        private gmshModelOccAddWedgeFun gmshModelOccAddWedge = GetFunction<gmshModelOccAddWedgeFun>("gmshModelOccAddWedge");

        private delegate void gmshModelOccAddThruSectionsFun(int[] wireTags, IntPtr wireTags_n, ref IntPtr outDimTags, ref IntPtr outDimTags_n,
                                                             int tag, int makeSolid, int makeRuled, int maxDegree, string continuity, string parametrization,
                                                             int smoothing, ref int ierr);
        private gmshModelOccAddThruSectionsFun gmshModelOccAddThruSections = GetFunction<gmshModelOccAddThruSectionsFun>("gmshModelOccAddThruSections");

        private delegate void gmshModelOccAddPipeFun(int[] dimTags, IntPtr dimTags_n, int wireTag,
                                                    ref IntPtr outDimTags, ref IntPtr outDimTags_n, string trihedron, ref int ierr);
        private gmshModelOccAddPipeFun gmshModelOccAddPipe = GetFunction<gmshModelOccAddPipeFun>("gmshModelOccAddPipe");

        private delegate void gmshModelOccFuseFun(int[] objectDimTags, IntPtr objectDimTags_n,
                                                      int[] toolDimTags, IntPtr toolDimTags_n,
                                                      ref IntPtr outDimTags, ref IntPtr outDimTags_n,
                                                      ref IntPtr outDimTagsMap, ref IntPtr outDimTagsMap_n, ref IntPtr outDimTagsMap_nn,
                                                      int tag, int removeObject, int removeTool, ref int ierr);
        private gmshModelOccFuseFun gmshModelOccFuse = GetFunction<gmshModelOccFuseFun>("gmshModelOccFuse");

        private delegate void gmshModelOccIntersectFun(int[] objectDimTags, IntPtr objectDimTags_n,
                                                      int[] toolDimTags, IntPtr toolDimTags_n,
                                                      ref IntPtr outDimTags, ref IntPtr outDimTags_n,
                                                      ref IntPtr outDimTagsMap, ref IntPtr outDimTagsMap_n, ref IntPtr outDimTagsMap_nn,
                                                      int tag, int removeObject, int removeTool, ref int ierr);
        private gmshModelOccIntersectFun gmshModelOccIntersect = GetFunction<gmshModelOccIntersectFun>("gmshModelOccIntersect");

        private delegate void gmshModelOccFragmentFun(int[] objectDimTags, IntPtr objectDimTags_n,
                                                      int[] toolDimTags, IntPtr toolDimTags_n,
                                                      ref IntPtr outDimTags, ref IntPtr outDimTags_n,
                                                      ref IntPtr outDimTagsMap, ref IntPtr outDimTagsMap_n, ref IntPtr outDimTagsMap_nn,
                                                      int tag, int removeObject, int removeTool, ref int ierr);
        private gmshModelOccFragmentFun gmshModelOccFragment = GetFunction<gmshModelOccFragmentFun>("gmshModelOccFragment");

        private delegate void gmshModelOccRemoveFun(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        private gmshModelOccRemoveFun gmshModelOccRemove = GetFunction<gmshModelOccRemoveFun>("gmshModelOccRemove");

        private delegate void gmshModelOccRemoveAllDuplicatesFun(ref int ierr);
        private gmshModelOccRemoveAllDuplicatesFun gmshModelOccRemoveAllDuplicates = GetFunction<gmshModelOccRemoveAllDuplicatesFun>("gmshModelOccRemoveAllDuplicates");

        private delegate void gmshModelOccImportShapesFun(string fileName, ref IntPtr outDimTags,ref IntPtr outDimTags_n, 
                                                          int highestDimOnly, string format, ref int ierr);
        private gmshModelOccImportShapesFun gmshModelOccImportShapes = GetFunction<gmshModelOccImportShapesFun>("gmshModelOccImportShapes");

        private delegate void gmshModelOccGetEntitiesFun(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        private gmshModelOccGetEntitiesFun gmshModelOccGetEntities = GetFunction<gmshModelOccGetEntitiesFun>("gmshModelOccGetEntities");

        private delegate void gmshModelOccGetCurveLoopsFun(int surfaceTag, ref IntPtr curveLoopTags,
                                                           ref IntPtr curveLoopTags_n, ref IntPtr curveTags,
                                                           ref IntPtr curveTags_n, ref IntPtr curveTags_nn,
                                                           ref int ierr);
        private gmshModelOccGetCurveLoopsFun gmshModelOccGetCurveLoops = GetFunction<gmshModelOccGetCurveLoopsFun>("gmshModelOccGetCurveLoops");

        private delegate void gmshModelOccGetSurfaceLoopsFun(int volumeTag, ref IntPtr surfaceLoopTags,
                                                             ref IntPtr surfaceLoopTags_n, ref IntPtr surfaceTags,
                                                             ref IntPtr surfaceTags_n, ref IntPtr surfaceTags_nn,
                                                             ref int ierr);
        private gmshModelOccGetSurfaceLoopsFun gmshModelOccGetSurfaceLoops = GetFunction<gmshModelOccGetSurfaceLoopsFun>("gmshModelOccGetSurfaceLoops");

        private delegate void gmshModelOccGetMassFun(int dim, int tag, ref double mass, ref int ierr);
        private gmshModelOccGetMassFun gmshModelOccGetMass = GetFunction<gmshModelOccGetMassFun>("gmshModelOccGetMass");

        private delegate void gmshModelOccGetCenterOfMassFun(int dim, int tag, ref double x, ref double y, ref double z, ref int ierr);
        private gmshModelOccGetCenterOfMassFun gmshModelOccGetCenterOfMass = GetFunction<gmshModelOccGetCenterOfMassFun>("gmshModelOccGetCenterOfMass");

        private delegate void gmshModelOccSynchronizeFun(ref int ierr);
        private gmshModelOccSynchronizeFun gmshModelOccSynchronize = GetFunction<gmshModelOccSynchronizeFun>("gmshModelOccSynchronize");

        /// <summary>
        /// Добавляет геометрическую точку в ядро OCC
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="z">Координата Z</param>
        /// <param name="meshSize">Размер в точке для уплотнения сетки</param>
        /// <param name="tag">Желаемый идентификатор или -1 для автоназначения</param>
        /// <returns>Идентификатор геометрической точки</returns>
        public int AddPoint(double x, double y, double z, double meshSize = 0, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddPoint(x, y, z, meshSize, tag, ref ierr);
        }

        /// <summary>
        /// Добавляет геометрическую линию в ядро OCC
        /// </summary>
        /// <param name="startTag">Идентификатор точки начала</param>
        /// <param name="endTag">Идентификатор точки конца</param>
        /// <param name="tag">Желаемый идентификатор или -1 для автоназначения</param>
        /// <returns>Идентификатор геометрической линии</returns>
        public int AddLine(int startTag, int endTag, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddLine(startTag, endTag, tag, ref ierr);
        }

        /// <summary>
        /// Создает дугу окружности с центром centerTag, началом startTag и концом endTag в ядре OCC
        /// </summary>
        /// <param name="startTag">Идентификатор начальной точки</param>
        /// <param name="centerTag">Идентификатор центра точки</param>
        /// <param name="endTag">Идентификатор конечной точки</param>
        /// <param name="tag">Желаемый идентификатор или -1 для автоназначения</param>
        /// <returns>Идентификатор дуги</returns>
        public int AddCircleArc(int startTag, int centerTag, int endTag, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddCircleArc(startTag, centerTag, endTag, tag, ref ierr);
        }

        /// <summary>
        /// Добавляет окружность в ядро OCC с центром x,y,z и радиусом r
        /// </summary>
        /// <param name="x">Координата x центра окружности</param>
        /// <param name="y">Координата y центра окружности</param>
        /// <param name="z">Координата z центра окружности</param>
        /// <param name="r">Радиус окружности</param>
        /// <param name="tag">Желаемый идентификатор или -1 для автоназначения</param>
        /// <param name="angle1">Начальный угол (для генерации полуокружности)</param>
        /// <param name="angle2">Конечный угол (для генерации полуокружности)</param>
        /// <param name="zAxis">Желаемая орта оси Z(x,y,z) или null(если не требуется)</param>
        /// <param name="xAxis">Желаемая орта оси X(x,y,z) или null(если не требуется)</param>
        /// <returns>Идентификатор окружности</returns>
        public int AddCircle(double x, double y, double z, double r, int tag = -1, double angle1 = 0, double angle2 = 2 * Math.PI,
                             double[] zAxis = null, double[] xAxis = null)
        {
            var ierr = 0;
            zAxis = zAxis ?? new double[0];
            xAxis = xAxis ?? new double[0];
            return gmshModelOccAddCircle(x, y, z, r, tag, angle1, angle2, zAxis, (IntPtr)zAxis.Length, xAxis, (IntPtr)xAxis.Length, ref ierr);
        }

        /// <summary>
        /// Создает кривую безье по указанным pointTags в ядре OCC
        /// </summary>
        /// <param name="pointTags">Массив идентификаторов точек</param>
        /// <param name="tag">Желаемый идентификатор или -1 для автоназначения</param>
        /// <returns>Идентификатор кривой</returns>
        public int AddBezier(int[] pointTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddBezier(pointTags, (IntPtr)pointTags.Length, tag, ref ierr);
        }

        /// <summary>
        /// Добавляет в ядро OCC контур(замкнутый или открытый), контур может содержать геометрически одинаковые точки кривых, но топологически разные кривые
        /// </summary>
        /// <param name="curveTags">Массив идентификаторов кривых, которые формируют контур</param>
        /// <param name="tag">Желаемый идентификатор или -1 для автоназначения</param>
        /// <param name="checkClosed">Проверять замкнутость контура перед созданием?</param>
        /// <returns>Идентификатор контура</returns>
        public int AddWire(int[] curveTags, int tag = -1, bool checkClosed = false)
        {
            var ierr = 0;
            return gmshModelOccAddWire(curveTags, (IntPtr)curveTags.Length, tag, Convert.ToInt32(checkClosed), ref ierr);
        }

        /// <summary>
        ///  Добавляет в ядро OCC контур(замкнутый), контур может содержать геометрически одинаковые точки кривых, но топологически разные кривые, идентификаторы
        ///  со знаком минус рассматриваются рассматривается как инвертированное отображение кривой, где начальная точка кривой - рассматривается как конечная,
        ///  а конечная - рассматривается как начальная
        /// </summary>
        /// <param name="curveTags">Массив идентификаторов кривых, которые формируют контур</param>
        /// <param name="tag">Желаемый идентификатор или -1 для автоназначения</param>
        /// <returns>Идентификатор контура</returns>
        public int AddCurveLoop(int[] curveTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddCurveLoop(curveTags, (IntPtr)curveTags.Length, tag, ref ierr);
        }

        /// <summary>
        /// Добавляет прямоугольник в OCC ядро с левым нижним углом (x,y,z) и верхним правым углом(x + dx, y + dy, z),
        /// если tag положительный, назначает указанный идентификатор поверхности, иначе идентификатор генерируется автоматически,
        /// roundedRadius определяет радиус скругления углов
        /// </summary>
        /// <param name="x">Координата X левого нижнего угла</param>
        /// <param name="y">Координата Y левого нижнего угла</param>
        /// <param name="z">Координата Z левого нижнего угла</param>
        /// <param name="dx">Смещение X правого верхнего угла</param>
        /// <param name="dy">Смещение Y правого верхнего угла</param>
        /// <param name="tag">Желаемый идентификатор (положительный) или -1 (будет сгенерирован автоматически)</param>
        /// <param name="roundedRadius">Радиус скругления</param>
        /// <returns>Идентификатор контура</returns>
        public int AddRectangle(double x, double y, double z, double dx, double dy, int tag = -1, double roundedRadius = 0)
        {
            var ierr = 0;
            return gmshModelOccAddRectangle(x, y, z, dx, dy, tag, roundedRadius, ref ierr);
        }
        /// <summary>
        /// Строит плоскую поверхность по выбранному контуру
        /// </summary>
        /// <param name="wireTags">Массив контуров, первый элемент указывает на внешний контур, второй на контур отверстия</param>
        /// <param name="tag">Желаемый идентификатор, или -1 для автоназначения</param>
        /// <returns>Возврщает идентификатор плоской поверхности</returns>
        public int AddPlaneSurface(int[] wireTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddPlaneSurface(wireTags, (IntPtr)wireTags.Length, tag, ref ierr);
        }
        /// <summary>
        /// Создает криволинейную поверхность по указанному контуру и параметрам в ядре OCC
        /// </summary>
        /// <param name="wireTag">Идентификатор контура</param>
        /// <param name="tag">Желаемый идентификатор, или -1 для автоназначения</param>
        /// <param name="pointTags">Идентификаторы точек, через которые должна пройти поверхность</param>
        /// <param name="degree">Минимальная степень, которую необходимо вычислить для формирования деформации поверхности</param>
        /// <param name="numPointsOnCurves">Число точек на кривой</param>
        /// <param name="numIter">Максимальное количество итераций для процесса оптимизации</param>
        /// <param name="anisotropic">Внутренний флаг для повышения производительности</param>
        /// <param name="tol2d">Допуски для ограничителей в параметрической плоскости</param>
        /// <param name="tol3d">Максимальная допускаемая длина между поддержками поверхности и ограничителями</param>
        /// <param name="tolAng">Максимально допустимый угол между нормалью поверхности и ограничителями</param>
        /// <param name="tolCurv">Максимально допустимая разница кривизны между поверхностью и ограничителем</param>
        /// <param name="maxDegree">Наивысшая степень, который может иметь многочлен, определяющий заполнение поверхности</param>
        /// <param name="maxSegments">Наибольшее количество сегментов, которую может иметь поверхность заполнения</param>
        /// <returns>Возврщает идентификатор криволинейной поверхности</returns>
        public int AddSurfaceFilling(int wireTag, int tag = -1, int[] pointTags = null, int degree = 3, int numPointsOnCurves = 15,
                                     int numIter = 2, bool anisotropic = false, double tol2d = 0.00001, double tol3d = 0.0001, double tolAng = 0.01,
                                     double tolCurv = 0.1, int maxDegree = 8, int maxSegments = 9)
        {
            var ierr = 0;
            pointTags = pointTags ?? new int[0];
            return gmshModelOccAddSurfaceFilling(wireTag, tag, pointTags, (IntPtr)pointTags.Length, degree, numPointsOnCurves, numIter,
                                                Convert.ToInt32(anisotropic), tol2d, tol3d, tolAng, tolCurv, maxDegree, maxSegments, ref ierr);
        }

        /// <summary>
        /// Создает параллелепипед в геометрическом ядре OCC
        /// </summary>
        /// <param name="x">Координата x начала координат угла основания параллелепипеда</param>
        /// <param name="y">Координата y начала координат угла основания параллелепипеда</param>
        /// <param name="z">Координата z начала координат угла основания параллелепипеда</param>
        /// <param name="dx">Размерность вдоль оси x</param>
        /// <param name="dy">Размерность вдоль оси y</param>
        /// <param name="dz">Размерность вдоль оси z</param>
        /// <param name="tag">Желаемый идентификатор объема</param>
        /// <returns>Идентификатор объема</returns>
        public int AddBox(double x, double y, double z, double dx, double dy, double dz, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddBox(x, y, z, dx, dy, dz, tag, ref ierr);
        }
        /// <summary>
        /// Создает призму в геометрическом ядре OCC
        /// </summary>
        /// <param name="x">Координата x начала координат правильного угла основания призмы</param>
        /// <param name="y">Координата y начала координат правильного угла основания призмы</param>
        /// <param name="z">Координата z начала координат правильного угла основания призмы</param>
        /// <param name="dx">Размерность вдоль оси x</param>
        /// <param name="dy">Размерность вдоль оси y</param>
        /// <param name="dz">Размерность вдоль оси z</param>
        /// <param name="tag">Желаемый идентификатор объема</param>
        /// <param name="ltx">Верхнее расширение вдоль оси x</param>
        /// <param name="zAxis">Вектор оси z(опционально)</param>
        /// <returns>Идентификатор объема</returns>
        public int AddWedge(double x, double y, double z, double dx, double dy, double dz, 
                            int tag = -1, double ltx = 0, double[] zAxis = null)
        {
            var ierr = 0;
            zAxis = zAxis ?? new double[0];
            return gmshModelOccAddWedge(x, y, z, dx, dy, dz, tag, ltx, zAxis, (IntPtr)zAxis.Length, ref ierr);
        }
        /// <summary>
        /// Добавляет объем или поверхность по указанным сечениям wireTags в пространстве в ядро OCC
        /// </summary>
        /// <param name="wireTags">Массив контуров сечений</param>
        /// <param name="tag">Желаемый идентификатор объема или поверхности</param>
        /// <param name="makeSolid">Превратить в объем если true или поверхность</param>
        /// <param name="makeRuled">Линейчатая поверхность если true</param>
        /// <param name="maxDegree">Если положительна - назначить степень как максимальную для результирующей поверхности</param>
        /// <param name="continuity">Опциональный параметр - возможные значения "C0", "G1", "C1", "G2", "C2", "C3","CN" или "" если не требуется</param>
        /// <param name="parametrization">Опциональный параметр - возможные значения "ChordLength", "Centripetal", "IsoParametric" или "" если не требуется</param>
        /// <param name="smoothing">Опциональный параметр гладкости</param>
        /// <returns>Возвращает созданные результирующие сущности в формате размерность:идентификатор</returns>
        public int[] AddThruSections(int[] wireTags, int tag = -1, bool makeSolid = true, bool makeRuled = false, int maxDegree = -1, 
                                    string continuity = "", string parametrization = "", bool smoothing = false)
        {
            var ierr = 0;
            var outDimTags = IntPtr.Zero;
            var outDimTags_n = IntPtr.Zero;
            gmshModelOccAddThruSections(wireTags, (IntPtr)wireTags.Length, ref outDimTags, ref outDimTags_n, tag, Convert.ToInt32(makeSolid),
                                        Convert.ToInt32(makeRuled), maxDegree, continuity, parametrization, Convert.ToInt32(smoothing), ref ierr);
            return GetObjectsData<int>(outDimTags, outDimTags_n);
        }

        /// <summary>
        /// Экструдирует dimTags вдоль кривой wireTag в ядре OCC
        /// </summary>
        /// <param name="dimTags">Массив сущностей для экструзии</param>
        /// <param name="wireTag">Кривая вдоль которой осуществляеется экструзия</param>
        /// <param name="trihedron">Параметры создания</param>
        /// <returns>Новые образованные геометрические сущности</returns>
        public int[] AddPipe(int[] dimTags, int wireTag, string trihedron = "")
        {
            var ierr = 0;
            var outDimTags = IntPtr.Zero;
            var outDimTags_n = IntPtr.Zero;
            gmshModelOccAddPipe(dimTags, (IntPtr)dimTags.Length, wireTag, ref outDimTags, ref outDimTags_n, trihedron, ref ierr);
            var resDimTags = GetObjectsData<int>(outDimTags, outDimTags_n);
            return resDimTags;
        }

        /// <summary>
        /// Вычисляет булеву операцию объединения множеств objectDimTags и toolDimTags,
        /// если операция выполняется для разных размерностей, нижняя размерность автоматически поглащается верхней,
        /// возвращает результирующие сущности dimTags и ассоциированных с ней карты относительно старой нумерации (отслеживание где произошли изменения)
        /// </summary>
        /// <param name="objectDimTags">Множество 1 для взятия объединения</param>
        /// <param name="toolDimTags">Множество 2 для взятия объединения</param>
        /// <param name="tag">Желаемый идентификатор, валиден если применяется операция относительно одной сущности, или -1 для автоназначения</param>
        /// <param name="removeObject">Удалить исходное множество 1 после выполнении операции</param>
        /// <param name="removeTool">Удалить исходное множество 2 после выполнении операции</param>
        /// <returns>Результирующее множестов dim:tag и карта ассоциаций со старыми множествами</returns>
        public Tuple<int[], int[][]> Fuse(int[] objectDimTags, int[] toolDimTags, int tag = -1, bool removeObject = true, bool removeTool = true)
        {
            var ierr = 0;
            var outDimTags = IntPtr.Zero;
            var outDimTags_n = IntPtr.Zero;
            var outDimTagsMap = IntPtr.Zero;
            var outDimTagsMap_n = IntPtr.Zero;
            var outDimTagsMap_nn = IntPtr.Zero;
            gmshModelOccFuse(objectDimTags, (IntPtr)objectDimTags.Length, toolDimTags, (IntPtr)toolDimTags.Length,
                                 ref outDimTags, ref outDimTags_n, ref outDimTagsMap, ref outDimTagsMap_n, ref outDimTagsMap_nn,
                                 tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref ierr);
            var dimTags = GetObjectsData<int>(outDimTags, outDimTags_n);
            var dimTagsMap = GetArrayOfObjectsData<int>(outDimTagsMap, outDimTagsMap_nn, outDimTagsMap_n);
            return Tuple.Create(dimTags, dimTagsMap);
        }

        /// <summary>
        /// Вычисляет булеву операцию пересечения множеств (общач часть) objectDimTags и toolDimTags,
        /// если операция выполняется для разных размерностей, нижняя размерность автоматически поглащается верхней,
        /// возвращает результирующие сущности dimTags и ассоциированных с ней карты относительно старой нумерации (отслеживание где произошли изменения)
        /// </summary>
        /// <param name="objectDimTags">Множество 1 для взятия пересечения</param>
        /// <param name="toolDimTags">Множество 2 для взятия пересечения</param>
        /// <param name="tag">Желаемый идентификатор, валиден если применяется операция относительно одной сущности, или -1 для автоназначения</param>
        /// <param name="removeObject">Удалить исходное множество 1 после выполнении операции</param>
        /// <param name="removeTool">Удалить исходное множество 2 после выполнении операции</param>
        /// <returns>Результирующее множестов dim:tag и карта ассоциаций со старыми множествами</returns>
        public Tuple<int[], int[][]> Intersect(int[] objectDimTags, int[] toolDimTags, int tag = -1, bool removeObject = true, bool removeTool = true)
        {
            var ierr = 0;
            var outDimTags = IntPtr.Zero;
            var outDimTags_n = IntPtr.Zero;
            var outDimTagsMap = IntPtr.Zero;
            var outDimTagsMap_n = IntPtr.Zero;
            var outDimTagsMap_nn = IntPtr.Zero;
            gmshModelOccIntersect(objectDimTags, (IntPtr)objectDimTags.Length, toolDimTags, (IntPtr)toolDimTags.Length,
                                 ref outDimTags, ref outDimTags_n, ref outDimTagsMap, ref outDimTagsMap_n, ref outDimTagsMap_nn,
                                 tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref ierr);
            var dimTags = GetObjectsData<int>(outDimTags, outDimTags_n);
            var dimTagsMap = GetArrayOfObjectsData<int>(outDimTagsMap, outDimTagsMap_nn, outDimTagsMap_n);
            return Tuple.Create(dimTags, dimTagsMap);
        }

        /// <summary>
        /// Вычисляет булеву операцию взятие фрагментов в результате пересечения множеств objectDimTags и toolDimTags,
        /// если операция выполняется для разных размерностей, нижняя размерность автоматически поглащается верхней,
        /// возвращает результирующие сущности dimTags и ассоциированных с ней карты относительно старой нумерации (отслеживание где произошли изменения)
        /// </summary>
        /// <param name="objectDimTags">Множество 1 для взятия пересечения</param>
        /// <param name="toolDimTags">Множество 2 для взятия пересечения</param>
        /// <param name="tag">Желаемый идентификатор, валиден если применяется операция относительно одной сущности, или -1 для автоназначения</param>
        /// <param name="removeObject">Удалить исходное множество 1 после выполнении операции</param>
        /// <param name="removeTool">Удалить исходное множество 2 после выполнении операции</param>
        /// <returns>Результирующее множестов dim:tag и карта ассоциаций со старыми множествами</returns>
        public Tuple<int[],int[][]> Fragment(int[] objectDimTags, int[] toolDimTags, int tag = -1, bool removeObject = true, bool removeTool = true)
        {
            var ierr = 0;
            var outDimTags = IntPtr.Zero;
            var outDimTags_n = IntPtr.Zero;
            var outDimTagsMap = IntPtr.Zero;
            var outDimTagsMap_n = IntPtr.Zero;
            var outDimTagsMap_nn = IntPtr.Zero;
            gmshModelOccFragment(objectDimTags, (IntPtr)objectDimTags.Length, toolDimTags, (IntPtr)toolDimTags.Length,
                                 ref outDimTags, ref outDimTags_n, ref outDimTagsMap, ref outDimTagsMap_n, ref outDimTagsMap_nn,
                                 tag, Convert.ToInt32(removeObject), Convert.ToInt32(removeTool), ref ierr);
            var dimTags = GetObjectsData<int>(outDimTags, outDimTags_n);
            var dimTagsMap = GetArrayOfObjectsData<int>(outDimTagsMap, outDimTagsMap_nn, outDimTagsMap_n);
            return Tuple.Create(dimTags, dimTagsMap);
        }

        /// <summary>
        /// Удалить пары dim:tag заданные массивом. Если пара dim:tag опирается на соcедей верхнего уровня, то удаление невозможно.
        /// </summary>
        /// <param name="dimTags">Массив пар dim:tag</param>
        /// <param name="recursive">Флаг задает рекурсивное удаление всех сущностей вплоть до точек</param>
        public void Remove(int[] dimTags, bool recursive = false)
        {
            var ierr = 0;
            gmshModelOccRemove(dimTags, (IntPtr)dimTags.Length, Convert.ToInt32(recursive), ref ierr);
        }
        /// <summary>
        /// Удаляет все дублирующие сущности с одинаковым геометрическим расположением после булевых операций
        /// </summary>
        public void RemoveAllDuplicates()
        {
            var ierr = 0;
            gmshModelOccRemoveAllDuplicates(ref ierr);
        }

        /// <summary>
        /// Импортирует из заданного файла массив сущностей, если задан флаг highestDimOnly импортирует только сущности верхнего уровня
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="highestDimOnly">Указанный флаг импортирует только сущности верхнего уровня</param>
        /// <param name="format">Принудидетельно указать формат файла (brep, step, iges)</param>
        /// <returns>Массив пар dim:tag</returns>
        public int[] ImportShapes(string fileName, bool highestDimOnly = true, string format = "")
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelOccImportShapes(fileName, ref intValue, ref intValue_n, Convert.ToInt32(highestDimOnly), format, ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <summary>
        /// Возвращает массив dim:tag(геометрические сущности) по указанному dim
        /// </summary>
        /// <param name="dim">-1 все геометрические сущности, 0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <returns>Массив пар dim:tag</returns>
        public int[] GetEntities(int dim = -1)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelOccGetEntities(ref intValue, ref intValue_n, dim, ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <summary>
        /// Возвращает петлю кривых заданных для поверхности surfaceTag
        /// </summary>
        /// <param name="surfaceTag">Идентификатор поверхности</param>
        /// <returns>(Массив идентификаторов петли кривых, перечисление идентификаторов кривых для каждой петли)</returns>
        public Tuple<int[], int[][]> GetCurveLoops(int surfaceTag)
        {
            var ierr = 0;
            var curveLoopTags = IntPtr.Zero;
            var curveLoopTags_n = IntPtr.Zero;
            var curveTags = IntPtr.Zero;
            var curveTags_n = IntPtr.Zero;
            var curveTags_nn = IntPtr.Zero;
            gmshModelOccGetSurfaceLoops(surfaceTag, ref curveLoopTags, ref curveLoopTags_n, ref curveTags, ref curveTags_n, ref curveTags_nn, ref ierr);
            var loopTags = GetObjectsData<int>(curveLoopTags, curveLoopTags_n);
            var curTags = GetArrayOfObjectsData<int>(curveTags, curveTags_nn, curveTags_n);
            return Tuple.Create(loopTags, curTags);
        }
        /// <summary>
        /// Возвращает замкнутые контуры поверхностей для заданного объема volumeTag
        /// </summary>
        /// <param name="volumeTag">Идентификатор объема</param>
        /// <returns>(Массив идентификаторов замкнутых контуров поверхностей, перечисление идентификаторов поверхностей для каждого контура)</returns>
        public Tuple<int[], int[][]> GetSurfaceLoops(int volumeTag)
        {
            var ierr = 0;
            var surfaceLoopTags = IntPtr.Zero;
            var surfaceLoopTags_n = IntPtr.Zero;
            var surfaceTags = IntPtr.Zero;
            var surfaceTags_n = IntPtr.Zero;
            var surfaceTags_nn = IntPtr.Zero;
            gmshModelOccGetSurfaceLoops(volumeTag, ref surfaceLoopTags, ref surfaceLoopTags_n, ref surfaceTags, ref surfaceTags_n, ref surfaceTags_nn, ref ierr);
            var loopTags = GetObjectsData<int>(surfaceLoopTags, surfaceLoopTags_n);
            var surfTags = GetArrayOfObjectsData<int>(surfaceTags, surfaceTags_nn, surfaceTags_n);
            return Tuple.Create(loopTags, surfTags);
        }
        /// <summary>
        /// Взять массу? у dim:tag (для кривых длина кривой)
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <returns>Масса</returns>
        public double GetMass(int dim, int tag)
        {
            var ierr = 0;
            var mass = 0.0;
            gmshModelOccGetMass(dim, tag, ref mass, ref ierr);
            return mass;
        }
        /// <summary>
        /// Возвращает центр тяжести для указанных dim : tag
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <returns>(Координаты центра тяжести x,y,z)</returns>
        public Tuple<double, double, double> GetCenterOfMass(int dim, int tag)
        {
            var ierr = 0;
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;
            gmshModelOccGetCenterOfMass(dim, tag, ref x, ref y, ref z, ref ierr);
            return Tuple.Create(x,y,z);
        }
        /// <summary>
        /// Синхронизировать изменения с ядром OCC, например после манипуляции с геометрическими сущностями(добавление точек, булевы операции)
        /// </summary>
        public void Synchronize()
        {
            var ierr = 0;
            gmshModelOccSynchronize(ref ierr);
        }
    }
}
