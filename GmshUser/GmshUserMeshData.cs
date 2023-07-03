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
    /// Извлекает данные о сетке заданной модели
    /// </summary>
    public class GmshUserMeshData : GmshUserModelData
    {
        protected List<Element3D> volumes;
        /// <summary>
        /// Экземляр данных о сетке заданной модели
        /// </summary>
        /// <param name="modelId">[In]Идентификатор модели</param>
        public GmshUserMeshData(long modelId) : base(modelId) => GetData();
        /// <summary>
        /// Извлекает все элементы сетки из модели
        /// </summary>
        public override void GetData()
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
                GmshUserMemory.GetElementData(out elements, eTags, eTags_n, eTags_nn);
                GmshUserMemory.GetElementData(out points, nodeTags, nodeTags_n, nodeTags_nn);
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
            GmshUserMemory.GetTypesData(nodeTags, out tagPoints, nodeTags_n);
            GmshUserMemory.GetTypesData(coord, out coords, coord_n);
            for (var i = 0; i < pointsCount; ++i)
            {
                var stride = i * 3;
                nodes.Add(new Node((int)tagPoints[i], new Point3D((float)coords[stride],
                                   (float)coords[stride + 1], (float)coords[stride + 2])));
            }
        }

        private string[] GetElementsTypes(IntPtr eTypes, IntPtr eTypes_n)
        {
            int[] eMeta;
            IntPtr elName, localeNodeCoord, localeNodeCoord_n;
            elName = localeNodeCoord = localeNodeCoord_n = IntPtr.Zero;
            int dim, order, numNodes, primaryNodes, error;
            dim = order = numNodes = primaryNodes = error = 0;
            GmshUserMemory.GetTypesData(eTypes, out eMeta, eTypes_n);
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

        private void CreateElements(long[][] elements, long[][] points, string[] types)
        {
            for (var i = 0; i < elements.GetLength(0); ++i)
            {
                var typePoints = types[i].Split(' ');
                switch (typePoints[0])
                {
                    case "Line":
                        CreateCurves(elements[i], points[i], int.Parse(typePoints[1]));
                        break;
                    case "Triangle":
                        CreatePlaneFigure(3, ObjKind.Triangle, elements[i], points[i]);
                        break;
                    case "Quadrilateral":
                        CreatePlaneFigure(4, ObjKind.Quad, elements[i], points[i]);
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
            if (curves == null)
                curves = new List<Curve>();
            for (var i = 0; i < tagElements.Length; ++i)
            {
                var pointsPerStride = new Node[stride];
                for (var j = 0; j < stride; ++j)
                    pointsPerStride[j] = nodes[(int)tagPoints[i * stride + j] - 1];
                curves.Add(new Curve((int)tagElements[i], pointsPerStride));
            }
        }

        private void CreatePlaneFigure(int nodeNumber, ObjKind objKind, long[] tagElements, long[] tagPoints)
        {
            var index = 0;
            for (int i = 0; i < tagElements.Length; i++)
            {
                var surfaceNodes = new List<Node>();
                for (int j = 0; j < nodeNumber; j++)
                {
                    surfaceNodes.Add(nodes[(int)tagPoints[nodeNumber * i + j] - 1]);
                }
                var surface = new Surface(index++, surfaceNodes.ToArray(), objKind);
                surfaces.Add(surface);
            }
        }
    }
}