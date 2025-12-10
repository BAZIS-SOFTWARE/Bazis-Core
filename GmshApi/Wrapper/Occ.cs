using System;
using System.Drawing;
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

        private delegate int gmshModelOccAddRectangleFun(double x, double y, double z, double dx, double dy, int tag, double roundedRadius, ref int ierr);
        private gmshModelOccAddRectangleFun gmshModelOccAddRectangle = GetFunction<gmshModelOccAddRectangleFun>("gmshModelOccAddRectangle");

        private delegate int gmshModelOccAddBoxFun(double x, double y, double z, double dx, double dy, double dz, int tag, ref int ierr);
        private gmshModelOccAddBoxFun gmshModelOccAddBox = GetFunction<gmshModelOccAddBoxFun>("gmshModelOccAddBox");

        private delegate int gmshModelOccAddWedgeFun(double x, double y, double z, double dx, double dy, double dz, 
                                                      int tag, double ltx, double[] zAxis, IntPtr zAxis_n, ref int ierr);
        private gmshModelOccAddWedgeFun gmshModelOccAddWedge = GetFunction<gmshModelOccAddWedgeFun>("gmshModelOccAddWedge");

        private delegate void gmshModelOccFragmentFun(int[] objectDimTags, IntPtr objectDimTags_n,
                                                      int[] toolDimTags, IntPtr toolDimTags_n,
                                                      ref IntPtr outDimTags, ref IntPtr outDimTags_n,
                                                      ref IntPtr outDimTagsMap, ref IntPtr outDimTagsMap_n, ref IntPtr outDimTagsMap_nn,
                                                      int tag, int removeObject, int removeTool, ref int ierr);
        private gmshModelOccFragmentFun gmshModelOccFragment = GetFunction<gmshModelOccFragmentFun>("gmshModelOccFragment");

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
        /// <returns></returns>
        public int AddRectangle(double x, double y, double z, double dx, double dy, int tag = -1, double roundedRadius = 0)
        {
            var ierr = 0;
            return gmshModelOccAddRectangle(x, y, z, dx, dy, tag, roundedRadius, ref ierr);
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
