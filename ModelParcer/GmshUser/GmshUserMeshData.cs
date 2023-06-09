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
using System.Xml.Linq;
using System.Collections;
using System.IO;

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
            GmshUserMemory.GetTypesData(eTypes, out eMeta, eTypes_n);
            //GetTypesData(out eMeta, eTypes, eTypes_n);
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
                GmshUserMemory.GetElementData(out elements, eTags, eTags_n, eTags_nn);
                GmshUserMemory.GetElementData(out points, nodeTags, nodeTags_n, nodeTags_nn);
                /*GetElementData(out elements, eTags, eTags_n, eTags_nn);
                GetElementData(out points, nodeTags, nodeTags_n, nodeTags_nn);*/
                CreateElements(elements, points, types);
            }
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
