using System;
using System.Net.Http.Headers;
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

        private delegate int gmshModelOccAddCurveLoopFun(int[] curveTags, IntPtr curveTags_n, int tag, ref int ierr);
        private gmshModelOccAddCurveLoopFun gmshModelOccAddCurveLoop = GetFunction<gmshModelOccAddCurveLoopFun>("gmshModelOccAddCurveLoop");

        private delegate int gmshModelOccAddPlaneSurfaceFun(int[] wireTags, IntPtr wireTags_n, int tag, ref int ierr);
        private gmshModelOccAddPlaneSurfaceFun gmshModelOccAddPlaneSurface = GetFunction<gmshModelOccAddPlaneSurfaceFun>("gmshModelOccAddPlaneSurface");

        private delegate int gmshModelOccAddSurfaceLoopFun(int[] surfaceTags, IntPtr surfaceTags_n, int tag, int sewing, ref int ierr);
        private gmshModelOccAddSurfaceLoopFun gmshModelOccAddSurfaceLoop = GetFunction<gmshModelOccAddSurfaceLoopFun>("gmshModelOccAddSurfaceLoop");

        private delegate int gmshModelOccAddVolumeFun(int[] shellTags, IntPtr shellTags_n, int tag, ref int ierr);
        private gmshModelOccAddVolumeFun gmshModelOccAddVolume = GetFunction<gmshModelOccAddVolumeFun>("gmshModelOccAddVolume");

        private delegate void gmshModelOccFuseFun(int[] objectDimTags, IntPtr objectDimTags_n, int[] toolDimTags, IntPtr toolDimTags_n,
                                                  ref IntPtr outDimTags, ref IntPtr outDimTags_n,
                                                  ref IntPtr outDimTagsMap, ref IntPtr outDimTagsMap_n, ref IntPtr outDimTagsMap_nn,
                                                  int tag, int removeObject, int removeTool, ref int ierr);
        private gmshModelOccFuseFun gmshModelOccFuse = GetFunction<gmshModelOccFuseFun>("gmshModelOccFuse");

        private delegate void gmshModelOccRemoveFun(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        private gmshModelOccRemoveFun gmshModelOccRemove = GetFunction<gmshModelOccRemoveFun>("gmshModelOccRemove");

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
        /// <param name="x">Координата x</param>
        /// <param name="y">Координата y</param>
        /// <param name="z">Координата z</param>
        /// <param name="meshSize">Характеристическая длина элемента</param>
        /// <param name="tag">Желаемый идентификатор</param>
        /// <returns>Идентификатор геометрической точки</returns>
        public int AddPoint(double x, double y, double z, double meshSize = 0, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddPoint(x, y, z, meshSize, tag, ref ierr);
        }
        /// <summary>
        /// Добавляет геометрическую линию в ядро OCC
        /// </summary>
        /// <param name="startTag">Идентификатор начальной точки</param>
        /// <param name="endTag">Идентификатор конечной точки</param>
        /// <param name="tag">Желаемый идентификатор</param>
        /// <returns>Идентификатор линии</returns>
        public int AddLine(int startTag, int endTag, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddLine(startTag, endTag, tag, ref ierr);
        }
        /// <summary>
        /// Добавляет контур кривых в ядро OCC
        /// </summary>
        /// <param name="curveTags">Массив идентификаторов кривых (идентификатор может быть отрицательный, тогда кривая будет инвертированна)</param>
        /// <param name="tag">Желаемый идентификатор контура</param>
        /// <returns>Идентификатор контура</returns>
        public int AddCurveLoop(int[] curveTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddCurveLoop(curveTags, (IntPtr)curveTags.Length, tag, ref ierr);
        }
        /// <summary>
        /// Добавляет плоскую поверхность в ядро OCC
        /// </summary>
        /// <param name="wireTags">Массив контуров кривых (первый контур определяет границу, второй и последующий дыры в поверхности)</param>
        /// <param name="tag">Желаемый идентификатор поверхности</param>
        /// <returns>Идентификатор поверхности</returns>
        public int AddPlaneSurface(int[] wireTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddPlaneSurface(wireTags, (IntPtr)wireTags.Length, tag, ref ierr);
        }
        /// <summary>
        /// Добавляет замкнутую оболочку в ядро OCC
        /// </summary>
        /// <param name="surfaceTags">Массив идентификаторов поверхностей</param>
        /// <param name="tag">Желаемый идентификатор оболочки</param>
        /// <param name="sewing">Флаг разрешающий объединение геометрических одинаковых, но топологически разных поврехностей</param>
        /// <returns>Идентификатор замкнутой оболочки</returns>
        public int AddSurfaceLoop(int[] surfaceTags, int tag = -1, bool sewing = false)
        {
            var ierr = 0;
            return gmshModelOccAddSurfaceLoop(surfaceTags, (IntPtr)surfaceTags.Length, tag, Convert.ToInt32(sewing), ref ierr);
        }
        /// <summary>
        /// Добавляет объем в ядро OCC
        /// </summary>
        /// <param name="shellTags">Массив оболочек (первый идентификатор определяет внешние границы, второй и последующие петли поверхностей формирующие дыры)</param>
        /// <param name="tag">Желаемый идентификатор объема</param>
        /// <returns>Идентификатор объема</returns>
        public int AddVolume(int[] shellTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelOccAddVolume(shellTags, (IntPtr)shellTags.Length, tag, ref ierr);
        }
        /// <summary>
        /// Выполняет булеву операцию объединения
        /// </summary>
        /// <param name="objectDimTags">Массив пар dim:tag объекта 1</param>
        /// <param name="toolDimTags">Массив пар dim:tag объекта 2</param>
        /// <param name="tag">Идентификатор булевой операции(валидный только если результат операции одна единственная сущность) или -1</param>
        /// <param name="removeObject">Флаг удаление объекта 1</param>
        /// <param name="removeTool">Флаг удаления объекта 2</param>
        /// <returns>Кортеж(массив пар dim:tag изменившие свои идентификаторы, массив массивов dim:tag получивших новый mapping)</returns>
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
            var oDimTags = GetObjectsData<int>(outDimTags, outDimTags_n);
            var oMapDimTags = GetArrayOfObjectsData<int>(outDimTagsMap, outDimTagsMap_nn, outDimTagsMap_n);
            return Tuple.Create(oDimTags, oMapDimTags);
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
