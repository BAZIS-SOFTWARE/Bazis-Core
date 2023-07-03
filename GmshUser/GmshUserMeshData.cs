using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PrFunctionLib;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Xml.Linq;
using System.Collections;
using System.IO;
using Model.GmshController;
using Model.Interfaces;

namespace Model.GmshUser
{
    /// <summary>
    /// Класс содержит результат извлечения типов элементов
    /// </summary>
    public class ElementInfo
    {
        /// <summary>
        /// Имя элемента
        /// </summary>
        public string elementName;
        /// <summary>
        /// Размерность
        /// </summary>
        public int dim;
        /// <summary>
        /// Порядок
        /// </summary>
        public int order;
        /// <summary>
        /// Количество узлов
        /// </summary>
        public int numNodes;
        /// <summary>
        /// Массив координат
        /// </summary>
        public double[] coords;
        /// <summary>
        /// Количество координат
        /// </summary>
        public int coords_n;
        /// <summary>
        /// Главные узлы
        /// </summary>
        public int primNodes;
    }
    /// <summary>
    /// Извлекает данные о сетке заданной модели
    /// </summary>
    public static class GmshUserMeshData
    {
        /// <summary>
        /// Извлекает элементы сетки из модели по интересующим нам размерности : идентификатору
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <param name="dim">[In]Размерность (-1 для взятия всех размерностей)</param>
        /// <param name="tag">[In]Идентификатор (-1 для взятия всех идентификаторов)</param>
        /// <param name="info">[In]Массив ElementsInfo для заполнения </param>
        /// <param name="elTags">[In]Матрица тегов элементов по типам элемента</param>
        /// <param name="ptsTags">[In]Матрица тегов точек по типам элемента</param>
        public static void GetMeshData(long modelId, int dim, int tag, out ElementInfo[] info, 
                                      out long[][] elTags, out long[][] ptsTags)
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            IntPtr eTypes, eTypes_n, eTags, eTags_n, eTags_nn, nodeTags, nodeTags_n, nodeTags_nn;
            eTypes = eTypes_n = eTags = eTags_n = eTags_nn = nodeTags = nodeTags_n = nodeTags_nn = IntPtr.Zero;
            GmshMesh.gmshModelMeshGetElements(ref eTypes, ref eTypes_n,
                                              ref eTags, ref eTags_n, ref eTags_nn,
                                              ref nodeTags, ref nodeTags_n, ref nodeTags_nn, dim, tag, ref error);
            GetDataInfo(eTypes, eTypes_n, out info);
            GmshUserMemory.GetElementData(out elTags, eTags, eTags_n, eTags_nn);
            GmshUserMemory.GetElementData(out ptsTags, nodeTags, nodeTags_n, nodeTags_nn);
        }
        /// <summary>
        /// Возвращает отфильтрованные элементы сетки полученные от GetMeshData
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <param name="elFilter">[In]Набор названий элементов для фильтрации</param>
        /// <param name="info">[In]Список для заполнения информации о элементе</param>
        /// <param name="elTags">[In]Идентификаторы элементов</param>
        /// <param name="ptsTags">[In]Идентификаторы точек</param>
        public static void FilterMeshData(long modelId, string[] elFilter, List<ElementInfo> info,
                                          List<long[]> elTags, List<long[]> ptsTags)
        {
            long[] tagPoints;
            double[] coords;
            GetPoints(modelId, out coords, out tagPoints);
            ElementInfo[] getInfo;
            long[][] getElements, getPoints;
            GetMeshData(modelId, -1, -1, out getInfo, out getElements, out getPoints);
            for(var i = 0; i < getInfo.Length; ++i)
            {
                var status = false;
                for (var j = 0; j < elFilter.Length; ++j)
                    if (getInfo[i].elementName.Contains(elFilter[j]))
                    {
                        status = true;
                        break;
                    }
                if(status)
                {
                    info.Add(getInfo[i]);
                    elTags.Add(getElements[i]);
                    ptsTags.Add(getPoints[i]);
                }
            }
            /*var filteredData = getInfo.Select((v, i) => new { V = v, I = i })
                   .Where(v => elFilter.All(b => !b.Contains(v.V.elementName)));
            foreach(var v in filteredData)
            {
                info.Add(v.V);
                elTags.Add(getElements[v.I]);
                ptsTags.Add(getPoints[v.I]);
            }*/
        }
        /// <summary>
        /// Извлекает локальные координаты точек и их уникальные идентификаторы
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <param name="points">[Out]Массив локальных координат для заполнения</param>
        /// <param name="tagPoints">[Out]Массив идентификаторов</param>
        public static void GetPoints(long modelId, out double[] points, out long[] tagPoints)
        {
            var error = 0;
            IntPtr nodeTags, nodeTags_n, coord, coord_n, paramCoord, paramCoord_n;
            nodeTags = nodeTags_n = coord = coord_n = paramCoord = paramCoord_n = IntPtr.Zero;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            GmshMesh.gmshModelMeshGetNodes(ref nodeTags, ref nodeTags_n, ref coord, ref coord_n,
                                           ref paramCoord, ref paramCoord_n, -1, -1, 0, 0, ref error);
            var nodes = new List<Node>(nodeTags_n.ToInt32());
            GmshUserMemory.GetTypesData(nodeTags, out tagPoints, nodeTags_n);
            GmshUserMemory.GetTypesData(coord, out points, coord_n);
        }
        /// <summary>
        /// Извлекает информацию о элементе
        /// </summary>
        /// <param name="eTypes">[In]Массив типов</param>
        /// <param name="eTypes_n">[In]Размер массива</param>
        /// <param name="dataInfo">[In]Массив ElementInfo для заполнения</param>
        /// <returns>Информацию о каждом элементе</returns>
        private static void GetDataInfo(IntPtr eTypes, IntPtr eTypes_n, out ElementInfo[] dataInfo)
        {
            var dataInfoCount = eTypes_n.ToInt32();
            dataInfo = new ElementInfo[dataInfoCount];
            var error = 0;
            IntPtr elName, localeNodeCoord, localeNodeCoord_n;
            elName = localeNodeCoord = localeNodeCoord_n = IntPtr.Zero;
            int[] eMeta;
            GmshUserMemory.GetTypesData(eTypes, out eMeta, eTypes_n);
            for (var i = 0; i < eMeta.Length; ++i)
            {
                var info = new ElementInfo();
                GmshMesh.gmshModelMeshGetElementProperties(eMeta[i], ref elName, ref info.dim, ref info.order,
                                                           ref info.numNodes, ref localeNodeCoord, ref localeNodeCoord_n,
                                                           ref info.primNodes, ref error);
                info.elementName = GmshUserMemory.GetName(elName);
                GmshUserMemory.GetTypesData(localeNodeCoord, out info.coords, localeNodeCoord_n);
                info.coords_n = info.coords.Length;
                dataInfo[i] = info;
            }
        }
        /// <summary>
        /// Возвращает отфильтрованные элементы сетки
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        /// <param name="filter">[In]Фильтр нужных элементов</param>
        /// <returns></returns>
        public static IEnumerable<Surface> GetElements(long modelId, params string[] filter)
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(modelId.ToString(), ref error);
            GmshMesh.gmshModelMeshGenerate(2, ref error);
            double[] points;
            long[] tagPoints;
            GetPoints(modelId, out points, out tagPoints);
            var points3D = GmshUserGeometryData.ConvertToPoint3D(points);
            var surfaces = new List<Surface>();
            var info = new List<ElementInfo>();
            var elTags = new List<long[]>();
            var ptsTags = new List<long[]>();
            GmshUserMeshData.FilterMeshData(modelId, filter, info, elTags, ptsTags);
            for (var i = 0; i < info.Count; ++i)
            {
                var eltsTags = elTags[i];
                var pointTags = ptsTags[i];
                for (var j = 0; j < eltsTags.Length; ++j)
                {
                    var numNodes = info[i].numNodes;
                    var nodes = new Node[numNodes];
                    for (var k = 0; k < numNodes; ++k)
                    {
                        var tag = pointTags[j * numNodes + k];
                        nodes[k] = new Node((int)tag, points3D[tag - 1]);
                    }
                    surfaces.Add(new Surface((int)eltsTags[j], nodes, ObjKind.Triangle));
                }
            }
            return surfaces;
        }


        /*
        private static void CreateElements(long[][] elements, long[][] points, ElementInfo[] info)
        {
            for (var i = 0; i < elements.GetLength(0); ++i)
            {
                var typePoints = types[i].Split(' ');
                switch (typePoints[0])
                {
                    case "Line":
                        //CreateCurves(elements[i], points[i], Int32.Parse(typePoints[1]));
                        break;
                    case "Triangle":
                        break;
                    case "Quadrilateral":
                        break;
                    case "Tetrahedron":
                        break;
                    default:
                        break;
                }
            }
        }*/
        /*
        private static void CreateCurves(long[] tagElements, long[] tagPoints, int stride)
        {
            var curves = new List<Curve>(tagElements.Length);

            for (var i = 0; i < tagElements.Length; ++i)
            {
                var pointsPerStride = new Node[stride];
                for (var j = 0; j < stride; ++j)
                {
                    //pointsPerStride[j] = nodes[(int)tagPoints[i * stride + j] - 1];
                }
                curves.Add(new Curve((int)tagElements[i], pointsPerStride));
            }
        }*/
    }
}
