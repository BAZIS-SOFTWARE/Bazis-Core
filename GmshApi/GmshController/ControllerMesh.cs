using Geometry;
using Model.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using GmshApi.GmshKernel;
using System.Runtime.InteropServices;

namespace GmshApi.GmshController
{
    /// <summary>
    /// Класс содержит информацию о типе извлечнного узла
    /// </summary>
    public class NodeInfo
    {
        /// <summary>Координаты</summary>
        public double[] coords;
        ///<summary>Параметрические координаты</summary>
        public double[] parametric;
        ///<summary>Размерность элемента</summary>
        public int dim;
        ///<summary>Идентификатор элемента</summary>
        public int tag;
    }
    /// <summary>
    /// Класс содержит информацию о типе извлечнного элемента
    /// </summary>
    public class ElementInfo
    {
        /// <summary>Имя элемента</summary>
        public string elementName;
        /// <summary>Размерность</summary>
        public int dim;
        /// <summary>Порядок</summary>
        public int order;
        /// <summary>Количество узлов</summary>
        public int numNodes;
        /// <summary>Массив координат</summary>
        public double[] coords;
        /// <summary>Количество координат</summary>
        public int coords_n;
        /// <summary>Главные узлы</summary>
        public int primNodes;
    }
    /// <summary>
    /// Оборачивает основные методы сетки полученные из GmshController
    /// </summary>
    public static class ControllerMesh
    {
        /// <summary>
        /// [In]Размерность в которой нужно сгенерировать модель
        /// </summary>
        /// <param name="dim"></param>
        public static void Generate(int dim)
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshGenerate(dim, ref ierr);
        }
        /// <summary>
        /// Переводит большую часть сетки в четырехугольники
        /// </summary>
        public static void Recombine()
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshRecombine(ref ierr);
        }
        /// <summary>
        /// Равномерно уплотняет сетку у текущей модели
        /// </summary>
        public static void Refine()
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshRefine(ref ierr);
        }
        /// <summary>
        /// Получает свойство элемента
        /// </summary>
        /// <param name="elementTypes">[In]Тип элемента</param>
        /// <returns>Массив ElementInfo</returns>
        public static ElementInfo[] GetElementProperties(int[] elementTypes)
        {
            var ierr = 0;
            IntPtr localeNodeCoord, localeNodeCoord_n, elementName;
            localeNodeCoord = localeNodeCoord_n = elementName = IntPtr.Zero;
            var dataInfo = new ElementInfo[elementTypes.Length];
            for (var i = 0; i < elementTypes.Length; ++i)
            {
                var info = new ElementInfo();
                KernelMesh.gmshModelMeshGetElementProperties(elementTypes[i], ref elementName, ref info.dim, ref info.order,
                                                           ref info.numNodes, ref localeNodeCoord, ref localeNodeCoord_n,
                                                           ref info.primNodes, ref ierr);
                info.elementName = ControllerMemory.GetName(elementName);
                ControllerMemory.GetObjectsData(localeNodeCoord, out info.coords, localeNodeCoord_n);
                info.coords_n = info.coords.Length;
                dataInfo[i] = info;
            }
            return dataInfo;
        }
        /// <summary>
        /// Получает все элементы в текущей модели, их идентификаторы и идентификаторы точек внутри элемента
        /// </summary>
        /// <param name="dim">[In]Размерность для получения точек, или -1 для получения всех размерностей</param>
        /// <param name="tag">[In]Индекс элемента в пределах dim, -1 для получения всех элементов</param>
        /// <param name="elemInfo">[Out]Массив ElementInfo для заполнения</param>
        /// <param name="elemsTags">[Out]Массив elementTags для заполнения</param>
        /// <param name="nodesTags">[Out]Массив elementTags для заполнения</param>
        /// <returns>true если элементы были найдены</returns>
        public static bool GetElementTags(int dim, int tag, out ElementInfo[] elemInfo,
                                       out long[][] elemsTags, out long[][] nodesTags)
        {
            var ierr = 0;
            IntPtr eTypes, eTypes_n, eTags, eTags_n, eTags_nn, nodeTags, nodeTags_n, nodeTags_nn;
            eTypes = eTypes_n = eTags = eTags_n = eTags_nn = nodeTags = nodeTags_n = nodeTags_nn = IntPtr.Zero;
            KernelMesh.gmshModelMeshGetElements(ref eTypes, ref eTypes_n,
                                              ref eTags, ref eTags_n, ref eTags_nn,
                                              ref nodeTags, ref nodeTags_n, ref nodeTags_nn, dim, tag, ref ierr);
            elemInfo = null;
            elemsTags = nodesTags = null;
            if (ierr == 0)
            {
                int[] elemTypes;
                ControllerMemory.GetObjectsData<int>(eTypes, out elemTypes, eTypes_n);
                elemInfo = GetElementProperties(elemTypes);

                int objsTypesCount;
                IntPtr[] tagsData, lengthData;
                long[][] eData;
                NewMethod(eTags, eTags_n, eTags_nn, out objsTypesCount, out tagsData, out lengthData, out eData);
                for (var i = 0; i < objsTypesCount; ++i)
                    ControllerMemory.GetObjectsData(tagsData[i], out eData[i], lengthData[i]);
                
                KernelGeneral.gmshFree(eTags);

                NewMethod(nodeTags, nodeTags_n, nodeTags_nn, out objsTypesCount, out tagsData, out lengthData, out eData);

                for (var i = 0; i < objsTypesCount; ++i)
                    ControllerMemory.GetObjectsData(tagsData[i], out eData[i], lengthData[i]);
                KernelGeneral.gmshFree(nodeTags);


                //ControllerMemory.GetElementData<long>(out elemsTags, eTags, eTags_n, eTags_nn);
                //ControllerMemory.GetElementData<long>(out nodesTags, nodeTags, nodeTags_n, nodeTags_nn);
            }
            return ierr == 0;
        }

        private static void NewMethod(IntPtr eTags, IntPtr eTags_n, IntPtr eTags_nn, out int objsTypesCount, out IntPtr[] tagsData, out IntPtr[] lengthData, out long[][] eData)
        {
            objsTypesCount = eTags_nn.ToInt32();
            eData = new long[objsTypesCount][];
            tagsData = new IntPtr[objsTypesCount];
            lengthData = new IntPtr[objsTypesCount];

            Marshal.Copy(eTags, tagsData, 0, objsTypesCount);
            Marshal.Copy(eTags_n, lengthData, 0, objsTypesCount);
            KernelGeneral.gmshFree(eTags_n);
        }

        /// <summary>
        /// Извлекает все точки, связанные с текущей моделью, параметром dim,tag
        /// </summary>
        /// <param name="dim">[In]Размерность для получения точек, или -1 для получения всех размерностей</param>
        /// <param name="tag">[In]Индекс элемента в пределах dim, -1 для получения всех элементов</param>
        /// <param name="nodeTags">[Out]Идентификаторы узлов</param>
        /// <param name="local">[Out]Координаты узлов</param>
        /// <param name="parametric">[Out]Параметрические координаты узлов</param>
        /// <param name="returnParametric">[In]Возвращать параметрические координаты?</param>
        /// <param name="includeBoundary">[In]Включать граничные узлы</param>
        /// <returns>true если узлы были найдены</returns>
        public static bool GetNodes(int dim, int tag, out long[] nodeTags, out double[] local,
                                    out double[] parametric, bool returnParametric = false,
                                    bool includeBoundary = true)
        {
            var ierr = 0;
            var retParam = Convert.ToInt32(returnParametric);
            var includeBound = Convert.ToInt32(includeBoundary);
            IntPtr nodeTagsInt, nodeTagsInt_n, coord, coord_n, parametricCoord, parametricCoord_n;
            nodeTagsInt = nodeTagsInt_n = coord = coord_n = parametricCoord = parametricCoord_n = IntPtr.Zero;
            KernelMesh.gmshModelMeshGetNodes(ref nodeTagsInt, ref nodeTagsInt_n, ref coord, ref coord_n,
                                          ref parametricCoord, ref parametricCoord_n, dim, tag,
                                          includeBound, retParam, ref ierr);
            nodeTags = null;
            local = null;
            parametric = null;
            if (ierr == 0)
            {
                ControllerMemory.GetObjectsData<long>(nodeTagsInt, out nodeTags, nodeTagsInt_n);
                ControllerMemory.GetObjectsData<double>(coord, out local, coord_n);
                if (returnParametric)
                    ControllerMemory.GetObjectsData<double>(parametricCoord, out parametric, parametricCoord_n);
            }
            return ierr == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="tag"></param>
        /// <param name="includeBoundary"></param>
        /// <returns></returns>
        public static IEnumerable<Node> GetNodes(int dim = -1, int tag = -1, bool includeBoundary = false)
        {
            long[] nodeTags;
            double[] local, parametric;
            if(!GetNodes(dim, tag, out nodeTags, out local, out parametric, false, includeBoundary))
                return new Node[0];
            var list = new List<Node>();
            var points = ControllerModel.ConvertLocalCoordsToPoints3D(local);
            for (var i = 0L; i < nodeTags.Length; ++i)
                list.Add(new Node((int)nodeTags[i], points[i]));
            return list;
        }
        /// <summary>
        /// Перестроить кэш точек
        /// </summary>
        public static void RebuildNodeCache()
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshRebuildNodeCache(1, ref ierr);
        }

        /// <summary>
        /// Перестроить кэш элементов
        /// </summary>
        public static void RebuildElementCache()
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshRebuildElementCache(1, ref ierr);
        }

        /// <summary>
        /// Находит элементы сетки по переданной координате x,y,z
        /// </summary>
        /// <param name="id">[In]Размерность</param>
        /// <param name="x">[In]Координата x</param>
        /// <param name="y">[In]Координата y</param>
        /// <param name="z">[In]Координата z</param>
        /// <param name="strict">[In]Выполнять строгий поиск?</param>
        public static int[] GetElementsByCoordinates(int dimension, double x, double y, double z, bool strict = false)
        {
            var ierr = 0;
            var strictInt = Convert.ToInt32(strict);
            IntPtr elementTags, elementTags_n;
            elementTags = elementTags_n = IntPtr.Zero;
            KernelMesh.gmshModelMeshGetElementsByCoordinates(x, y, z, ref elementTags, ref elementTags_n,
                                                           dimension, strictInt, ref ierr);
            int[] tags;
            ControllerMemory.GetObjectsData<int>(elementTags, out tags, elementTags_n);
            return tags;
        }
        /// <summary>
        /// Возвращает локальные координаты элемента с заданным идентификатором
        /// </summary>
        /// <param name="elementTag">[In]Идентификатор элемента</param>
        /// <param name="x">[In]Координата x</param>
        /// <param name="y">[In]Координата y</param>
        /// <param name="z">[In]Координата z</param>
        /// <returns>Массив локальных координат uvw</returns>
        public static double[] GetLocalCoordinatesInElement(uint elementTag, double x, double y, double z)
        {
            var ierr = 0;
            var elTag = new UIntPtr(elementTag);
            var result = new double[3] { 0, 0, 0 };
            KernelMesh.gmshModelMeshGetLocalCoordinatesInElement(elTag, x, y, z, ref result[0], ref result[1],
                                                               ref result[2], ref ierr);
            return result;
        }

        /// <summary>
        /// Возвращает набор элементов по указанным dim:tag
        /// </summary>
        /// <param name="dimension">[In]Размерность для получения элементов, или -1 для получения всех элементов</param>
        /// <param name="tag">[In]Идентифкатор элемента или -1 для получения всех элементов в пределах dim</param>
        /// <returns>Список элементов или пустой список если элементы не найдены</returns>
        public static List<ModelObject> GetElements(int dimension, int tag)
        {
            ElementInfo[] info;
            long[][] elemTags, nodeTags;
            long[] nodesT;
            double[] local, parametric;
            var list = new List<ModelObject>();
            if (GetElementTags(dimension, tag, out info, out elemTags, out nodeTags))
            {
                GetNodes(dimension, tag, out nodesT, out local, out parametric);
                var nodes = LocalCoordinatesToNodes(nodesT, local);
                for (int i = 0; i < info.Length; ++i)
                {
                    var elements = elemTags[i];
                    for (var j = 0; j < elements.Length; ++j)
                    {
                        var nodesCount = info[i].numNodes;
                        var nodesPerElem = new Node[nodesCount];
                        for (int k = 0; k < info[i].numNodes; ++k)
                            nodesPerElem[k] = nodes[nodeTags[i][j * nodesCount + k]];
                        list.Add(CreateModelObject(ref info[i].elementName, elements[j], nodesPerElem));
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// Указыввает алгоритм распределения точек на кривой
        /// </summary>
        /// <param name="tag">[In]Идентификатор кривой</param>
        /// <param name="numNodes">[In]Число точек</param>
        /// <param name="meshType">[In]Тип алгоритма</param>
        /// <param name="coef">[In]Коэффициент прогрессии</param>
        public static void SetTransfiniteCurve(int tag, int numNodes, String meshType, double coef)
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshSetTransfiniteCurve(tag, numNodes, meshType, coef, ref ierr);
        }
        /// <summary>
        /// Удаляет выбранные элементы по парам dim:tag
        /// </summary>
        /// <param name="dimTags"></param>
        /// <returns>Флаг успешности операции</returns>
        public static bool DeleteElements(int[] dimTags)
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshClear(dimTags, (IntPtr)dimTags.Length ,ref ierr);
            return ierr == 0;
        }

        /// <summary>
        /// Удаляет элементы сетки по указанным идентификаторам
        /// </summary>
        /// <param name="elementsTags">[In]Идентификаторы элементов</param>
        /// <returns></returns>
        public static bool DeleteMeshElements(int[] elementsTags)
        {
            var ierr = 0;
            var elements = new IntPtr[elementsTags.Length];
            for (var i = 0; i < elementsTags.Length; ++i)
                elements[i] = new IntPtr(elementsTags[i]);
            KernelMesh.gmshModelMeshSetVisibility(elements, (IntPtr)elements.Length, 0, ref ierr);
            if (ierr == 0)
            {
                KernelPlugin.gmshPluginSetNumber("Invisible", "DeleteElements", 1, ref ierr);
                KernelPlugin.gmshPluginRun("Invisible", ref ierr);
            }
            return ierr == 0;
        }
        /// <summary>
        /// Устанавливает алгоритм триангуляции сетки
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="value">[In]Численное значение поля алгоритма</param>
        /// <returns>Флаг ошибки</returns>
        public static bool MeshSetAlgorithm(int tag, int value)
        {
            var ierr = 0;
            KernelMesh.gmshModelMeshSetAlgorithm(2, tag, value, ref ierr);
            return ierr == 0;
        }

        private static IntPtr[] GetPointerToElementTags(long[][] elementTags)
        {
            var result = new List<IntPtr>();
            foreach (var tags in elementTags.Skip(2).Take(1))
            {
                foreach (var tag in tags)
                {
                    result.Add((IntPtr)tag);
                }
            }
            return result.ToArray();
        }

        public static double[] GetElementQualities(string qualityName)
        {
            int error = 0;
            GetElementTags(-1, -1, out ElementInfo[] elementInfo, out long[][] elementTags, out long[][] nodeTags);
            var elementsQuality_n = IntPtr.Zero; 
            var elementsQuality = IntPtr.Zero;
            var pointers = GetPointerToElementTags(elementTags);
            KernelMesh.gmshModelMeshGetElementQualities(
                pointers,
                (IntPtr)pointers.Length,
                ref elementsQuality,
                ref elementsQuality_n,
                qualityName, (IntPtr)0, (IntPtr)1, ref error);
            ControllerMemory.GetObjectsData(elementsQuality, out double[] result, elementsQuality_n);
            return result;
        }

        private static Dictionary<long, Node> LocalCoordinatesToNodes(long[] nodesTags, double[] coords)
        {
            var dict = new Dictionary<long, Node>();
            for (int i = 0; i < nodesTags.Length; ++i)
            {
                if (!dict.ContainsKey(nodesTags[i]))
                    dict.Add(nodesTags[i], new Node((int)nodesTags[i], new Point3D((float)coords[i * 3],
                                                (float)coords[i * 3 + 1], (float)coords[i * 3 + 2])));
            }
            return dict;
        }

        private static ModelObject CreateModelObject(ref string elementName, 
                                                  long elementId, Node[] nodes)
        {
            if (elementName.Contains("Point"))
                return new Node((int)elementId, nodes[0].Position);
            else if (elementName.Contains("Line"))
                return new Element1D((int)elementId, nodes, ObjKind.Beam);
            else if (elementName.Contains("Triangle"))
                return new Element2D((int)elementId, nodes, ObjKind.Triangle);
            else if (elementName.Contains("Quadrilateral"))
                return new Element2D((int)elementId, nodes, ObjKind.Quad);
            else if (elementName.Contains("Tetrahedron"))
                return new Element3D((int)elementId, nodes, ObjKind.Tetra);
            else if (elementName.Contains("Hexahedron"))
                return new Element3D((int)elementId, nodes, ObjKind.Hexa);
            else
                return new Element3D((int)elementId, nodes, ObjKind.Penta);
        }
    }
}
