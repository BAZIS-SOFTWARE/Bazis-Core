using System;
using System.ComponentModel;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Mesh
    {
        private delegate void gmshModelMeshGenerateFun(int dim, ref int ierr);
        private gmshModelMeshGenerateFun gmshModelMeshGenerate = GetFunction<gmshModelMeshGenerateFun>("gmshModelMeshGenerate");

        private delegate void gmshModelMeshRecombineFun(ref int ierr);
        private gmshModelMeshRecombineFun gmshModelMeshRecombine = GetFunction<gmshModelMeshRecombineFun>("gmshModelMeshRecombine");

        private delegate void gmshModelMeshRefineFun(ref int ierr);
        private gmshModelMeshRefineFun gmshModelMeshRefine = GetFunction<gmshModelMeshRefineFun>("gmshModelMeshRefine");

        private delegate void gmshModelMeshClearFun(int[] dimTags, IntPtr dimTags_n, ref int ierr);
        private gmshModelMeshClearFun gmshModelMeshClear = GetFunction<gmshModelMeshClearFun>("gmshModelMeshClear");

        private delegate void gmshModelMeshGetNodesFun(ref IntPtr nodeTags, ref IntPtr nodeTags_n,
                                                       ref IntPtr coord, ref IntPtr coord_n,
                                                       ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                       int dim, int tag, int includeBoundary,
                                                       int returnParametricCoord, ref int ierr);
        private gmshModelMeshGetNodesFun gmshModelMeshGetNodes = GetFunction<gmshModelMeshGetNodesFun>("gmshModelMeshGetNodes");

        private delegate void gmshModelMeshGetNodesByElementTypeFun(int elementType, ref IntPtr nodeTags,
                                                                    ref IntPtr nodeTags_n, ref IntPtr coord,
                                                                    ref IntPtr coord_n, ref IntPtr parametricCoord,
                                                                    ref IntPtr parametricCoord_n, int tag,
                                                                    int returnParametricCoord, ref int ierr);
        private gmshModelMeshGetNodesByElementTypeFun gmshModelMeshGetNodesByElementType = GetFunction<gmshModelMeshGetNodesByElementTypeFun>("gmshModelMeshGetNodesByElementType");

        private delegate void gmshModelMeshAddNodesFun(int dim, int tag, IntPtr[] nodeTags, IntPtr nodeTags_n,
                                                      double[] coord, IntPtr coord_n, double[] parametricCoord,
                                                      IntPtr parametricCoord_n, ref int ierr);
        private gmshModelMeshAddNodesFun gmshModelMeshAddNodes = GetFunction<gmshModelMeshAddNodesFun>("gmshModelMeshAddNodes");

        private delegate void gmshModelMeshReclassifyNodesFun(ref int ierr);
        private gmshModelMeshReclassifyNodesFun gmshModelMeshReclassifyNodes = GetFunction<gmshModelMeshReclassifyNodesFun>("gmshModelMeshReclassifyNodes");

        private delegate void gmshModelMeshGetElementsFun(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                          ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                          ref IntPtr elementTags_nn, ref IntPtr nodeTags,
                                                          ref IntPtr nodeTags_n, ref IntPtr nodeTags_nn,
                                                          int dim, int tag, ref int ierr);
        private gmshModelMeshGetElementsFun gmshModelMeshGetElements = GetFunction<gmshModelMeshGetElementsFun>("gmshModelMeshGetElements");

        private delegate void gmshModelMeshGetElementsByCoordinatesFun(double x, double y, double z,
                                                                       ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                                       int dim, int strict, ref int ierr);
        private gmshModelMeshGetElementsByCoordinatesFun gmshModelMeshGetElementsByCoordinates = GetFunction<gmshModelMeshGetElementsByCoordinatesFun>("gmshModelMeshGetElementsByCoordinates");

        private delegate void gmshModelMeshGetElementTypesFun(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                              int dim, int tag, ref int ierr);
        private gmshModelMeshGetElementTypesFun gmshModelMeshGetElementTypes = GetFunction<gmshModelMeshGetElementTypesFun>("gmshModelMeshGetElementTypes");

        private delegate void gmshModelMeshGetElementPropertiesFun(int elementType, ref IntPtr elementName, ref int dim,
                                                                   ref int order, ref int numNodes, ref IntPtr localeNodeCoord,
                                                                   ref IntPtr localeNodeCoord_n, ref int numPrimaryNodes,
                                                                   ref int ierr);
        private gmshModelMeshGetElementPropertiesFun gmshModelMeshGetElementProperties = GetFunction<gmshModelMeshGetElementPropertiesFun>("gmshModelMeshGetElementProperties");

        private delegate void gmshModelMeshGetElementQualitiesFun(IntPtr[] elementTags, IntPtr elementTags_n,
                                                                  ref IntPtr elementsQuality, ref IntPtr elementQuality_n,
                                                                  string qualityName, IntPtr task,
                                                                  IntPtr numTasks, ref int ierr);
        private gmshModelMeshGetElementQualitiesFun gmshModelMeshGetElementQualities = GetFunction<gmshModelMeshGetElementQualitiesFun>("gmshModelMeshGetElementQualities");

        private delegate void gmshModelMeshAddElementsByTypeFun(int tag, int elementType, IntPtr[] elementTags,
                                                            IntPtr elementTags_n, IntPtr[] nodeTags, IntPtr nodeTags_n,
                                                            ref int ierr);
        private gmshModelMeshAddElementsByTypeFun gmshModelMeshAddElementsByType = GetFunction<gmshModelMeshAddElementsByTypeFun>("gmshModelMeshAddElementsByType");

        private delegate void gmshModelMeshSetSizeFun(int[] dimTags, IntPtr dimTags_n, double size, ref int ierr);
        private gmshModelMeshSetSizeFun gmshModelMeshSetSize = GetFunction<gmshModelMeshSetSizeFun>("gmshModelMeshSetSize");

        private delegate void gmshModelMeshGetSizesFun(int[] dimTags, IntPtr dimTags_n, ref IntPtr sizes, ref IntPtr sizes_n, ref int ierr);
        private gmshModelMeshGetSizesFun gmshModelMeshGetSizes = GetFunction<gmshModelMeshGetSizesFun>("gmshModelMeshGetSizes");


        private delegate void gmshModelMeshSetTransfiniteCurveFun(int tag, int numNodes, string meshType, double coef, ref int ierr);
        private gmshModelMeshSetTransfiniteCurveFun gmshModelMeshSetTransfiniteCurve = GetFunction<gmshModelMeshSetTransfiniteCurveFun>("gmshModelMeshSetTransfiniteCurve");

        private delegate void gmshModelMeshSetTransfiniteSurfaceFun(int tag, string arrangement, int[] cornerTags, IntPtr cornerTags_n, ref int ierr);
        private gmshModelMeshSetTransfiniteSurfaceFun gmshModelMeshSetTransfiniteSurface = GetFunction<gmshModelMeshSetTransfiniteSurfaceFun>("gmshModelMeshSetTransfiniteSurface");

        private delegate void gmshModelMeshSetTransfiniteVolumeFun(int tag, int[] cornerTags, IntPtr cornerTags_n, ref int ierr);
        private gmshModelMeshSetTransfiniteVolumeFun gmshModelMeshSetTransfiniteVolume = GetFunction<gmshModelMeshSetTransfiniteVolumeFun>("gmshModelMeshSetTransfiniteVolume");

        private delegate void gmshModelMeshSetTransfiniteAutomaticFun(int[] dimTags, IntPtr dimTags_n, double cornerAngle, int recombine, ref int ierr);
        private gmshModelMeshSetTransfiniteAutomaticFun gmshModelMeshSetTransfiniteAutomatic = GetFunction<gmshModelMeshSetTransfiniteAutomaticFun>("gmshModelMeshSetTransfiniteAutomatic");

        private delegate void gmshModelMeshSetRecombineFun(int dim, int tag, double angle, ref int ierr);
        private gmshModelMeshSetRecombineFun gmshModelMeshSetRecombine = GetFunction<gmshModelMeshSetRecombineFun>("gmshModelMeshSetRecombine");

        private delegate void gmshModelMeshSetAlgorithmFun(int dim, int tag, int val, ref int ierr);
        private gmshModelMeshSetAlgorithmFun gmshModelMeshSetAlgorithm = GetFunction<gmshModelMeshSetAlgorithmFun>("gmshModelMeshSetAlgorithm");

        private delegate void gmshModelMeshRemoveConstraintsFun(int[] dimTags, IntPtr dimTags_n, ref int ierr);
        private gmshModelMeshRemoveConstraintsFun gmshModelMeshRemoveConstraints = GetFunction<gmshModelMeshRemoveConstraintsFun>("gmshModelMeshRemoveConstraints");

        private delegate void gmshModelMeshEmbedFun(int dim, int[] tags, IntPtr tags_n, int inDim, int inTag, ref int ierr);
        private gmshModelMeshEmbedFun gmshModelMeshEmbed = GetFunction<gmshModelMeshEmbedFun>("gmshModelMeshEmbed");

        private delegate void gmshModelMeshRemoveEmbeddedFun(int[] dimTags, IntPtr dimTags_n, int dim, ref int ierr);
        private gmshModelMeshRemoveEmbeddedFun gmshModelMeshRemoveEmbedded = GetFunction<gmshModelMeshRemoveEmbeddedFun>("gmshModelMeshRemoveEmbedded");

        private delegate void gmshModelMeshGetEmbeddedFun(int dim, int tag, ref IntPtr dimTags, ref IntPtr dimTags_n, ref int ierr);
        private gmshModelMeshGetEmbeddedFun gmshModelMeshGetEmbedded = GetFunction<gmshModelMeshGetEmbeddedFun>("gmshModelMeshGetEmbedded");

        private delegate void gmshModelMeshImportStlFun(ref int ierr);
        private gmshModelMeshImportStlFun gmshModelMeshImportStl = GetFunction<gmshModelMeshImportStlFun>("gmshModelMeshImportStl");

        private delegate void gmshModelMeshSetVisibilityFun(IntPtr[] elementTags, IntPtr elementTags_n, int value, ref int ierr);
        private gmshModelMeshSetVisibilityFun gmshModelMeshSetVisibility = GetFunction<gmshModelMeshSetVisibilityFun>("gmshModelMeshSetVisibility");

        private delegate void gmshModelMeshClassifySurfacesFun(double angle, int boundary, int forReparametrization, double curveAngle, int exportDiscrete, ref int ierr);
        private gmshModelMeshClassifySurfacesFun gmshModelMeshClassifySurfaces = GetFunction<gmshModelMeshClassifySurfacesFun>("gmshModelMeshClassifySurfaces");

        private delegate void gmshModelMeshCreateGeometryFun(int[] dimTags, IntPtr dimTags_n, ref int ierr);
        private gmshModelMeshCreateGeometryFun gmshModelMeshCreateGeometry = GetFunction<gmshModelMeshCreateGeometryFun>("gmshModelMeshCreateGeometry");

        /// <inheritdoc/>
        public Field Field { get; private set; }

        /// <inheritdoc/>
        public Mesh()
        {
            Field = new Field();
        }

        /// <summary>
        /// Генерирует сеточные элементы модели: 0 - узлы, 1 - элементы 1D, 2 - элементы 2D, 3 - элементы 3D
        /// </summary>
        /// <param name="dim">[In]Элементы которые нужно сгенерировать</param>
        public void Generate(int dim)
        {
            var ierr = 0;
            gmshModelMeshGenerate(dim, ref ierr);
        }
        /// <summary>
        /// Генирирует сетку преимущественно в четырехугольники
        /// </summary>
        public void Recombine()
        {
            var ierr = 0;
            gmshModelMeshRecombine(ref ierr);
        }
        /// <summary>
        /// Равномерно уплотнить сетку
        /// </summary>
        public void Refine()
        {
            var ierr = 0;
            gmshModelMeshRefine(ref ierr);
        }
        /// <summary>
        /// Удаляет сетку связанную с массивом пар dim:tag, включая точки. Если массив пустой удаляет сетку у всей модели
        /// </summary>
        /// <param name="dimTags">[In]Массив пары dim:tag для удаления с которыми ассоциирована сетка</param>
        public void Clear(int[] dimTags = null)
        {
            var ierr = 0;
            dimTags = dimTags ?? new int[0];
            gmshModelMeshClear(dimTags, (IntPtr)dimTags.Length, ref ierr);
        }
        /// <summary>
        /// Возвращает идентификаторы, координаты, параметрические координаты(опционально) узлов элементов. Если dim и tag равен -1 возвращает узлы для всей модели.
        /// </summary>
        /// <param name="dim">-1 все геометрические сущности, 0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">-1 - все идентификаторы, tag >= 0 - конкретный идентификатор</param>
        /// <param name="includeBoundary">Флаг, указывающий, включать ли узлы на границах модели</param>
        /// <param name="returnParametricCoord">Флаг, указывающий, возвращать ли параметрические координаты узлов</param>
        /// <returns>Идентификаторы, координаты, параметрические координаты</returns>
        public Tuple<IntPtr[], double[], double[]> GetNodes(int dim = -1, int tag = -1, bool includeBoundary = false, bool returnParametricCoord = true)
        {
            var ierr = 0;
            var nodeTags = IntPtr.Zero;
            var nodeTags_n = IntPtr.Zero;
            var coord = IntPtr.Zero;
            var coord_n = IntPtr.Zero;
            var parametricCoord = IntPtr.Zero;
            var parametricCoord_n = IntPtr.Zero;
            gmshModelMeshGetNodes(ref nodeTags, ref nodeTags_n, ref coord, ref coord_n, ref parametricCoord, ref parametricCoord_n,
                                  dim, tag, Convert.ToInt32(includeBoundary), Convert.ToInt32(returnParametricCoord), ref ierr);
            var tags = GetObjectsData<IntPtr>(nodeTags, nodeTags_n);
            var coords = GetObjectsData<double>(coord, coord_n);
            var parametric = GetObjectsData<double>(parametricCoord, parametricCoord_n);
            return Tuple.Create(tags, coords, parametric);
        }
        /// <summary>
        /// Возвращает идентификаторы, координаты, параметрические координаты(опционально) узлов по типу и идентификатору элемента.
        /// </summary>
        /// <param name="elementType">Тип элемента</param>
        /// <param name="tag">Идентификатор элемента</param>
        /// <param name="returnParametricCoord">Флаг, указывающий, возвращать ли параметрические координаты узлов</param>
        public Tuple<IntPtr[], double[], double[]> GetNodesByElementType(int elementType, int tag = -1, bool returnParametricCoord = true)
        {
            var ierr = 0;
            var nodeTags = IntPtr.Zero;
            var nodeTags_n = IntPtr.Zero;
            var coord = IntPtr.Zero;
            var coord_n = IntPtr.Zero;
            var parametricCoord = IntPtr.Zero;
            var parametricCoord_n = IntPtr.Zero;
            gmshModelMeshGetNodesByElementType(elementType, ref nodeTags, ref nodeTags_n, ref coord, ref coord_n, ref parametricCoord, ref parametricCoord_n,
                                                tag, Convert.ToInt32(returnParametricCoord), ref ierr);
            var tags = GetObjectsData<IntPtr>(nodeTags, nodeTags_n);
            var coords = GetObjectsData<double>(coord, coord_n);
            var parametric = GetObjectsData<double>(parametricCoord, parametricCoord_n);
            return Tuple.Create(tags, coords, parametric);
        }
        /// <summary>
        /// Добавляет узлы к указанной паре dim:tag предоставляя координаты и опционально идентификаторы точек и параметрические координаты
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор геометрической сущности</param>
        /// <param name="nodeTags">Массив идентификаторов точек (опционально), если массив пуст, идентифкаторы сгенеририруются автоматически</param>
        /// <param name="coord">Массив локальных координат</param>
        /// <param name="parametricCoord">[In]Массив параметрических координат (опционально), может быть пустым</param>
        public void AddNodes(int dim, int tag, IntPtr[] nodeTags, double[] coord, double[] parametricCoord = null)
        {
            var ierr = 0;
            parametricCoord = parametricCoord ?? new double[0];
            gmshModelMeshAddNodes(dim, tag, nodeTags, (IntPtr)nodeTags.Length, coord, (IntPtr)coord.Length,
                                  parametricCoord, (IntPtr)parametricCoord.Length, ref ierr);
        }
        /// <summary>
        /// Классифицировать узлы
        /// </summary>
        public void ReclassifyNodes()
        {
            var ierr = 0;
            gmshModelMeshReclassifyNodes(ref ierr);
        }
        /// <summary>
        /// Возвращает сеточные элементы по указанному dim:tag, если dim и tag = -1 вернуть элементы всей модели
        /// </summary>
        /// <param name="dim">-1 все геометрические сущности, 0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">-1 - все идентификаторы, tag >= 0 - конкретный идентификатор</param>
        /// <returns>Типы элементов, массив идентификаторов элементов по конкретному типу, массив идентификаторов узлов по конкретному типу</returns>
        public Tuple<int[], IntPtr[][], IntPtr[][]> GetElements(int dim = -1, int tag = -1)
        {
            var ierr = 0;
            var elemTypes = IntPtr.Zero;
            var elemTypes_n = IntPtr.Zero;
            var elemTags = IntPtr.Zero;
            var elemTags_n = IntPtr.Zero;
            var elemTags_nn = IntPtr.Zero;
            var nodeTags = IntPtr.Zero;
            var nodeTags_n = IntPtr.Zero;
            var nodeTags_nn = IntPtr.Zero;
            gmshModelMeshGetElements(ref elemTypes, ref elemTypes_n, ref elemTags,
                                     ref elemTags_n, ref elemTags_nn, ref nodeTags,
                                     ref nodeTags_n, ref nodeTags_nn, dim, tag, ref ierr);
            var eTypes = GetObjectsData<int>(elemTypes, elemTypes_n);
            var eTags = GetArrayOfObjectsData<IntPtr>(elemTags, elemTags_nn, elemTags_n);
            var nTags = GetArrayOfObjectsData<IntPtr>(nodeTags, nodeTags_nn, nodeTags_n);
            return Tuple.Create(eTypes, eTags, nTags);
        }
        /// <summary>
        /// Возвращает идентификаторы элементов по указанной координате
        /// </summary>
        /// <param name="x">Координата X</param>
        /// <param name="y">Координата Y</param>
        /// <param name="z">Координата Z</param>
        /// <param name="dim">-1 все геометрические сущности, 0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="strict">Флаг, указывающий, строгое соответствие координате</param>
        /// <returns>Идентифкаторы элементов</returns>
        public IntPtr[] GetElementsByCoordinates(double x, double y, double z, int dim = -1, bool strict = false)
        {
            var ierr = 0;
            var elemTags = IntPtr.Zero;
            var elemTags_n = IntPtr.Zero;
            gmshModelMeshGetElementsByCoordinates(x, y, z, ref elemTags, ref elemTags_n, dim, Convert.ToInt32(strict), ref ierr);
            return GetObjectsData<IntPtr>(elemTags, elemTags_n);
        }
        /// <summary>
        /// Возвращает типы элементов по указанному dim:tag
        /// </summary>
        /// <param name="dim">-1 все геометрические сущности, 0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">-1 - все идентификаторы, tag >= 0 - конкретный идентификатор</param>
        /// <returns>Типы элементов</returns>
        public int[] GetElementTypes(int dim = -1, int tag = -1)
        {
            var ierr = 0;
            var elemTypes = IntPtr.Zero;
            var elemTypes_n = IntPtr.Zero;
            gmshModelMeshGetElementTypes(ref elemTypes, ref elemTypes_n, dim, tag, ref ierr);
            return GetObjectsData<int>(elemTypes, elemTypes_n);
        }
        /// <summary>
        /// Возвращает свойство элемента для указанного типа элемента
        /// </summary>
        /// <param name="elementType">Тип элемента</param>
        /// <returns>Свойство элемента</returns>
        public ElementProperties GetElementProperties(int elementType)
        {
            var ierr = 0;
            var localeNodeCoord = IntPtr.Zero;
            var localeNodeCoord_n = IntPtr.Zero;
            var elementName = IntPtr.Zero;
            var elemProperties = new ElementProperties();
            gmshModelMeshGetElementProperties(elementType, ref elementName, ref elemProperties.dim,
                                              ref elemProperties.order, ref elemProperties.numNodes,
                                              ref localeNodeCoord, ref localeNodeCoord_n,
                                              ref elemProperties.numPrimaryNodes, ref ierr);
            elemProperties.elementName = GetString(elementName);
            elemProperties.coords = GetObjectsData<double>(localeNodeCoord, localeNodeCoord_n);
            elemProperties.coords_n = elemProperties.coords.Length;
            return elemProperties;
        }
        /// <summary>
        /// Получить качество элемента
        /// </summary>
        /// <param name="elementTags">Массив идентификаторов элементов</param>
        /// <param name="qualityName">Параметр запроса</param>
        /// <param name="task">Номер потока</param>
        /// <param name="numTasks">Число потоков</param>
        /// <returns>Массив качеств для элементов</returns>
        public double[] GetElementQualities(IntPtr[] elementTags, QualityName qualityName = QualityName.minSICN, int task = 0, int numTasks = 1)
        {
            var ierr = 0;
            var elemQuality = IntPtr.Zero;
            var elemQuality_n = IntPtr.Zero;
            gmshModelMeshGetElementQualities(elementTags, (IntPtr)elementTags.Length, ref elemQuality, ref elemQuality_n,
                                             qualityName.ToString(), (IntPtr)task, (IntPtr)numTasks, ref ierr);
            return GetObjectsData<double>(elemQuality, elemQuality_n);
        }
        /// <summary>
        /// Добавляет идентифкаторы элементов и их узлов согласно типу элемента и его геометрическому идентификатору
        /// </summary>
        /// <param name="tag">Геометрический идентификатор</param>
        /// <param name="elementType">Тип элемента</param>
        /// <param name="elementTags">Массив идентифкаторов элементов (опционально), может быть пустым</param>
        /// <param name="nodeTags">Массив перечисленных идентификаторов узлов элементов</param>
        public void AddElementsByType(int tag, int elementType, IntPtr[] elementTags, IntPtr[] nodeTags)
        {
            var ierr = 0;
            gmshModelMeshAddElementsByType(tag, elementType, elementTags, (IntPtr)elementTags.Length, nodeTags, (IntPtr)nodeTags.Length, ref ierr);
        }
        /// <summary>
        /// Задает плотность для геометрических точек (4 координата)
        /// </summary>
        /// <param name="dimTags">Масив пар dim:tags (поддерживается только для dim = 0)</param>
        /// <param name="size">Задаваемая плотность</param>
        public void SetSize(int[] dimTags, double size)
        {
            var ierr = 0;
            gmshModelMeshSetSize(dimTags, (IntPtr)dimTags.Length, size, ref ierr);
        }
        /// <summary>
        /// Возвращает плотности для указанных геометрических точек
        /// </summary>
        /// <param name="dimTags">Масив пар dim:tags (поддерживается только для dim = 0)</param>
        /// <returns>Массив плотностей для указанных геометрических точек</returns>
        public double[] GetSizes(int[] dimTags)
        {
            var ierr = 0;
            var sizes = IntPtr.Zero;
            var sizes_n = IntPtr.Zero;
            gmshModelMeshGetSizes(dimTags, (IntPtr)dimTags.Length, ref sizes, ref sizes_n, ref ierr);
            return GetObjectsData<double>(sizes, sizes_n);
        }
        /// <summary>
        /// Задать трансфиницию кривых (уплотнение точек вдоль кривой) по определенному закону
        /// </summary>
        /// <param name="tag">Идентификатор кривой</param>
        /// <param name="numNodes">Число точек</param>
        /// <param name="meshType">Закон распределения точек</param>
        /// <param name="coef">Коэффициент распределения</param>
        public void SetTransfiniteCurve(int tag, int numNodes, MeshType meshType = MeshType.Progression, double coef = 1)
        {
            var ierr = 0;
            gmshModelMeshSetTransfiniteCurve(tag, numNodes, meshType.ToString(), coef, ref ierr);
        }
        /// <summary>
        /// Назначить трансфиницию поверхности (равномерная сетка) с указанным направлением ребер
        /// </summary>
        /// <param name="tag">Идентифкатор поверхности</param>
        /// <param name="arrangement">Направление расположения ребер</param>
        /// <param name="cornerTags">Пустой массив для автоматического определения углов или указанные точки массива (допускается число точек 3 или 4)</param>
        public void SetTransfiniteSurface(int tag, Arrangement arrangement = Arrangement.Left, int[] cornerTags = null)
        {
            var ierr = 0;
            cornerTags = cornerTags ?? new int[0];
            gmshModelMeshSetTransfiniteSurface(tag, arrangement.ToString(), cornerTags, (IntPtr)cornerTags.Length, ref ierr);
        }
        /// <summary>
        /// Назначить трансфиницию объема (равномерный объем)
        /// </summary>
        /// <param name="tag">Идентифкатор объема</param>
        /// <param name="cornerTags">Пустой массив для автоматического определения углов или указанные точки массива (допускается число точек 6 или 8)</param>
        public void SetTransfiniteVolume(int tag, int[] cornerTags = null)
        {
            var ierr = 0;
            cornerTags = cornerTags ?? new int[0];
            gmshModelMeshSetTransfiniteVolume(tag, cornerTags, (IntPtr)cornerTags.Length, ref ierr);
        }
        /// <summary>
        /// Установить автоматическую трансфиницию для dim:tags
        /// </summary>
        /// <param name="dimTags">Массив dim:tag поверхностей</param>
        /// <param name="cornerAngle">Пороговый угол</param>
        /// <param name="recombine">Флаг, указывающий, нужна ли сетка из четырехугольников</param>
        public void SetTransfiniteAutomatic(int[] dimTags, double cornerAngle = 2.35, bool recombine = true)
        {
            var ierr = 0;
            gmshModelMeshSetTransfiniteAutomatic(dimTags, (IntPtr)dimTags.Length, cornerAngle, Convert.ToInt32(recombine), ref ierr);
        }
        /// <summary>
        /// Помечает указанный dim:tag флагом генерации преимущественно квадратных элементов вместо треугольных (работает только для dim = 2)
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="angle">Значение порогового угла для внутреннего алгоритма gmsh</param>
        public void SetRecombine(int dim, int tag, double angle = 45.0)
        {
            var ierr = 0;
            gmshModelMeshSetRecombine(dim, tag, angle, ref ierr);
        }
        /*gmshModelMeshSetRecombineFun(int dim, int tag, double angle, ref int ierr);*/
        /// <summary>
        /// Установить алгоритм генериации сетки для конкретного dim:tag (поддерживается только для dim = 2)
        /// </summary>
        /// <param name="dim">0 - геометрические точки, 1 - кривые, 2 - поверхности, 3 - объемы</param>
        /// <param name="tag">Идентификатор</param>
        /// <param name="val">Алгоритм</param>
        public void SetAlgorithm(int dim, int tag, MeshAlgorithm2D val = MeshAlgorithm2D.Automatic)
        {
            var ierr = 0;
            gmshModelMeshSetAlgorithm(dim, tag, (int)val, ref ierr);
        }
        /// <summary>
        /// Удалить все ограничители для заданных пар dim:tag (трансфиниция кривых, поверхностей, объемов, плотность в точках)
        /// </summary>
        /// <param name="dimTags">Массив пар dim:tag или пустой массив(снять все ограничения)</param>
        public void RemoveConstraints(int[] dimTags = null)
        {
            var ierr = 0;
            dimTags = dimTags ?? new int[0];
            gmshModelMeshRemoveConstraints(dimTags, (IntPtr)dimTags.Length, ref ierr);
        }
        /// <summary>
        /// Встраивает элементы размерности dim и его идентификаторы tags в размерность inDim и его идентификатор inTag,
        /// размерность dim должна быть строго меньше (0,1,2), чем inDim (2,3). Встроенные элементы не должны пересекать друг друга 
        /// или быть частью границы сущности inTag, сетка которой будет соответствовать сетке встроенных сущностей.
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="tags"></param>
        /// <param name="inDim"></param>
        /// <param name="inTag"></param>
        public void Embed(int dim, int[] tags, int inDim, int inTag)
        {
            var ierr = 0;
            gmshModelMeshEmbed(dim, tags, (IntPtr)tags.Length, inDim, inTag, ref ierr);
        }
        /// <summary>
        /// Удаляет встроенные элементы' из указанных сущностей dimTags, если параметр dim >=0 удаляет только указанный dim (т.е 0-только точки и т.д)
        /// </summary>
        /// <param name="dimTags">Массив dimTags, у которых нужно удалить встроенные элементы</param>
        /// <param name="dim">Конкретная размерность которая должна рассматриваться при удалении или -1 удаление всех</param>
        public void RemoveEmbedded(int[] dimTags, int dim = -1)
        {
            var ierr = 0;
            gmshModelMeshRemoveEmbedded(dimTags, (IntPtr)dimTags.Length, dim, ref ierr);
        }
        /// <summary>
        /// Возвращает dimTags встроенные в указанные dim:tag (если имеются)
        /// </summary>
        /// <param name="dim">Опрашиваемая размерность</param>
        /// <param name="tag">Опрашиваемый идентификатор</param>
        /// <returns>Массив dimTags встроенных в dim:tag или пустой массив, если нет встраивания</returns>
        public int[] GetEmbedded(int dim, int tag)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelMeshGetEmbedded(dim, tag, ref intValue, ref intValue_n, ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <summary>
        /// Генерирует сетку для геометрии (поддерживаются только форматы OCC)
        /// </summary>
        public void ImportStl()
        {
            var ierr = 0;
            gmshModelMeshImportStl(ref ierr);
        }
        /// <summary>
        /// Установить флаг видимости для массива идентификаторов элементов (например для последующего удаления)
        /// </summary>
        /// <param name="elementTags">Массив идентификаторов элементов</param>
        /// <param name="value">Флаг видимости</param>
        public void SetVisibility(IntPtr[] elementTags, int value)
        {
            var ierr = 0;
            gmshModelMeshSetVisibility(elementTags, (IntPtr)elementTags.Length, value, ref ierr);
        }
        /// <summary>
        /// Классифицирует (цветом) сеточные поверхности по углу и создает дискретные сущности(точки, кривые, поверхности)
        /// </summary>
        /// <param name="angle">Пороговый угол разбиения поверхностей (в радианах)</param>
        /// <param name="boundary">Создавать дискретные кривые на открытых поверхностях?</param>
        /// <param name="forReparametrization">Создавать кривые и поверхности, которые могут быть репараметризированы одним отображением?</param>
        /// <param name="curveAngle">Угол разбиения для кривых (в радианах)</param>
        /// <param name="exportDiscrete">Экспортировать дискретные сущности в ядро CAD</param>
        public void ClassifySurfaces(double angle, bool boundary = true, bool forReparametrization = false,
                                     double curveAngle = Math.PI, bool exportDiscrete = true)
        {
            var ierr = 0;
            gmshModelMeshClassifySurfaces(angle, Convert.ToInt32(boundary), Convert.ToInt32(forReparametrization), 
                                          curveAngle, Convert.ToInt32(exportDiscrete), ref ierr);
        }
        /// <summary>
        /// Создает геометрию по дискретным сущностям dim:tags, создает параметризацию для кривых и поверхностей
        /// </summary>
        /// <param name="dimTags">[In]Масив dim:tags</param>
        public void CreateGeometry(int[] dimTags = null)
        {
            var ierr = 0;
            dimTags = dimTags ?? new int[0];
            gmshModelMeshCreateGeometry(dimTags, (IntPtr)dimTags.Length, ref ierr);
        }
    }
}
