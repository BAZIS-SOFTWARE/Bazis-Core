using Geometry;
using Model.Interfaces;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using PrFunctionLib;
using ModelParcer.GmshController;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace ModelParcer.GmshUser
{
    /// <summary>
    /// Извлекает данные о сетке заданной модели
    /// </summary>
    public class GmshUserMeshData : GmshUserModelData
    {
        protected List<Element3D> volumes;
        /// <summary>
        /// Экземляр данных о сетке заданной модели
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        public GmshUserMeshData(long modelId) : base(modelId) => GetElements();

        private string[] GetElementsTypes(IntPtr eTypes, IntPtr eTypes_n)
        {
            int[] eMeta;
            IntPtr elName, localeNodeCoord, localeNodeCoord_n;
            elName = localeNodeCoord = localeNodeCoord_n = IntPtr.Zero;
            int dim, order, numNodes, primaryNodes, error;
            dim = order = numNodes = primaryNodes = error = 0;
            GetTypesData(out eMeta, eTypes, eTypes_n);
            var result = new string[eMeta.Length];
            for (var i = 0; i < eMeta.Length; ++i)
            {
                GmshMesh.gmshModelMeshGetElementProperties(eMeta[i], ref elName, ref dim, ref order, ref numNodes,
                                                             ref localeNodeCoord, ref localeNodeCoord_n,
                                                             ref primaryNodes, ref error);
                var name = Marshal.PtrToStringAnsi(elName);
                GmshGeneral.gmshFree(elName);
                GmshGeneral.gmshFree(localeNodeCoord);
                result[i] = name;
            }
            return result;
        }

        /// <summary>
        /// Извлекает все элементы сетки из модели
        /// </summary>
        protected override void GetElements()
        {
            var error = 0;
            IntPtr eTypes, eTypes_n, eTags, eTags_n, eTags_nn, nodeTags, nodeTags_n, nodeTags_nn;
            eTypes = eTypes_n = eTags = eTags_n = eTags_nn = nodeTags = nodeTags_n = nodeTags_nn = IntPtr.Zero;
            GmshMesh.gmshModelMeshGetElements(ref eTypes, ref eTypes_n,
                                                ref eTags, ref eTags_n, ref eTags_nn,
                                                ref nodeTags, ref nodeTags_n, ref nodeTags_nn, -1, -1, ref error);
            var types = GetElementsTypes(eTypes, eTypes_n);
            if (types.Contains("Point"))
            {
                GetPoints();
                long[][] elements, points;
                GetElementData(out elements, eTags, eTags_n, eTags_nn);
                GetElementData(out points, nodeTags, nodeTags_n, nodeTags_nn);
                CreateElements(elements, points, types);
            }
        }

        private void GetPoints()
        {
            var error = 0;
            IntPtr nodeTags, nodeTags_n, coord, coord_n, paramCoord, paramCoord_n;
            nodeTags = nodeTags_n = coord = coord_n = paramCoord = paramCoord_n = IntPtr.Zero;
            GmshMesh.gmshModelMeshGetNodes(ref nodeTags, ref nodeTags_n, ref coord, ref coord_n,
                                             ref paramCoord, ref paramCoord_n, -1, -1, 0, 0, ref error);
            long[] tagPoints;
            double[] coords;
            var pointsCount = nodeTags_n.ToInt32();
            nodes = new List<Node>(pointsCount);
            GetTypesData(out tagPoints, nodeTags, nodeTags_n);
            GetTypesData(out coords, coord, coord_n);
            for (var i = 0; i < pointsCount; ++i)
            {
                var stride = i * 3;
                nodes.Add(new Node((int)tagPoints[i], new Point3D((float)coords[stride], (float)coords[stride + 1],
                                                                 (float)coords[stride + 2])));
            }
        }

        //TODO: Сделать GetTypedData универсальным!!!
        private void GetTypesData(out long[] data, IntPtr eTypes, IntPtr eCount)
        {
            var count = eCount.ToInt32();
            data = new long[count];
            Marshal.Copy(eTypes, data, 0, count);
            GmshGeneral.gmshFree(eTypes);
        }
        
        private void GetTypesData(out double[] data, IntPtr eTypes, IntPtr eCount)
        {
            var count = eCount.ToInt32();
            data = new double[count];
            Marshal.Copy(eTypes, data, 0, count);
            GmshGeneral.gmshFree(eTypes);
        }

        private void GetTypesData(out int[] data, IntPtr eTypes, IntPtr eCount)
        {
            var count = eCount.ToInt32();
            data = new int[count];
            Marshal.Copy(eTypes, data, 0, count);
            GmshGeneral.gmshFree(eTypes);
        }

        private void GetElementData(out long[][] data, IntPtr tagsData, IntPtr lengthPerDim, IntPtr dimCount)
        {
            var dCount = dimCount.ToInt32();
            data = new long[dCount][];
            var tagsDataPtr = new IntPtr[dCount];
            var lengthData = new IntPtr[dCount];
            Marshal.Copy(tagsData, tagsDataPtr, 0, dCount);
            Marshal.Copy(lengthPerDim, lengthData, 0, dCount);
            GmshGeneral.gmshFree(lengthPerDim);
            for (var i = 0; i < dCount; ++i)
            {
                var tags = lengthData[i].ToInt32();
                data[i] = new long[tags];
                Marshal.Copy(tagsDataPtr[i], data[i], 0, tags);
                GmshGeneral.gmshFree(tagsDataPtr[i]);
            }
            GmshGeneral.gmshFree(tagsData);
        }

        private void CreateElements(long[][] elements, long[][] points, string[] types)
        {
            for (var i = 0; i < elements.GetLength(0); ++i)
            {
                var typePoints = types[i].Split(' ');
                switch (typePoints[0])
                {
                    case "Line":
                        CreateCurves(elements[i], points[i], Int32.Parse(typePoints[1]));
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
        }

        private void CreateCurves(long[] tagElements, long[] tagPoints, int stride)
        {
            curves = new List<Curve>(tagElements.Length);
            var pointsPerStride = new Node[stride];
            for (var i = 0; i < tagElements.Length; ++i)
            {
                for (var j = 0; j < stride; ++j)
                    pointsPerStride[j] = nodes[(int)tagPoints[i * stride + j] - 1];
                curves.Add(new Curve((int)tagElements[i], pointsPerStride));
            }
        }
    }
}
