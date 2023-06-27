using PrFunctionLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Model.GroupsData;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using Model.GmshController;
using System.Diagnostics.Tracing;
using Model.Interfaces;
using System.Runtime.CompilerServices;

namespace Model.GmshUser
{
    /// <summary>
    /// Извлекает данные о геометрии заданной модели
    /// </summary>
    public class GmshUserGeometryData : GmshUserModelData
    {
        private delegate void Derivative(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n,
                                         ref IntPtr derivatives, ref IntPtr derivatives_n, ref int ierr);
        private int geometryMode;
        private int pointsId;
        private int surfaceId;
        /// <summary>
        /// Задает шаг параметризации кривой
        /// </summary>
        public int CurveSteps { get; set; }
        /// <summary>
        /// Задает шаг параметризации поверхности U
        /// </summary>
        public int SurfaceStepsU { get; set; }
        /// <summary>
        /// Задает шаг параметризации поверхности U
        /// </summary>
        public int SurfaceStepsV { get; set; }
        /// <summary>
        /// Экземляр данных о геометрии заданной модели
        /// </summary>
        /// <param name="modelId"></param>
        public GmshUserGeometryData(long modelId) : base(modelId)
        {
            CurveSteps = 10;
            SurfaceStepsU = 10;
            SurfaceStepsV = 10;
            var ext = Path.GetExtension(GmshUserIO.GetFileName());
            geometryMode = ext.Contains("step") || ext.Contains("igs") || ext.Contains("brep") ? 1 : 0;
            //GetData();
        }
        /// <summary>
        /// Извлекает данные геометрии из модели
        /// </summary>
        public override void GetData() => ExtractData(GetEntities());

        private int[] GetEntities()
        {
            var error = 0;
            IntPtr dimTags, dimTags_n, outDimTags, outDimTags_n;
            dimTags = dimTags_n = outDimTags = outDimTags_n = IntPtr.Zero;
            if (geometryMode == 1)
                GmshGeometry.gmshModelOccGetEntities(ref dimTags, ref dimTags_n, -1, ref error);
            else
                GmshModel.gmshModelGetEntities(ref dimTags, ref dimTags_n, -1, ref error);
            int[] data;
            GmshUserMemory.GetTypesData(dimTags, out data, dimTags_n);
            return data;
        }

        private void ExtractData(int[] dimTags)
        {
            if (dimTags.Length == 0)
                return;
            var pair = new int[2];
            var groups = dimTags.Select((v, i) => pair[i & 1] = v)
                                .Where((v, i) => (i & 1) == 1)
                                .Select(v => new { D = pair[0], T = pair[1] })
                                .GroupBy(v => v.D)
                                .Select(v => v.ToArray())
                                .ToArray();
            var actions = new Action<int, int>[] { AddPoint, AddCurve, AddSurface, AddVolumes };
            var j = 0;
            foreach (var dimension in groups)
            {
                foreach (var values in dimension)
                    actions[j](values.D, values.T);
                ++j;
            }
        }

        private void AddPoint(int dim, int tag)
        {
            if(nodes == null)
                nodes = new List<Node>();
            var point = GetPoints(null, dim, tag);
            nodes.Add(new Node(tag, new Point3D((float)point[0], (float)point[1], (float)point[2])));
        }

        private int[] CreatePointTags(int count)
        {
            var tagsPoints = Enumerable.Range(pointsId, count).ToArray();
            pointsId += count;
            return tagsPoints;
        }

        private void AddCurve(int dim, int tag)
        {
            if (curves == null)
            {
                pointsId += base.nodes.Count + 1;
                curves = new List<Curve>();
            }
            var paramsCoord = GetParametrizationBounds(dim, tag, 1);
            var temp = CurveSteps;
            if (IsLine(paramsCoord[0], dim, tag))
                CurveSteps = 2;
            else
                paramsCoord[0] = SetCustomStepParametrization(paramsCoord[0], dim, tag, CurveSteps);
            var pointTags = CreatePointTags(CurveSteps);
            var nodes = GetNodes(paramsCoord[0], dim, tag, pointTags);
            curves.Add(new Curve(tag, nodes));
            CurveSteps = temp;
        }

        private void AddSurface(int dim, int tag)
        {
            if (surfaces == null)
                surfaces = new List<Surface>();
            var paramsCoord = GetParametrizationBounds(dim, tag, 2);
            paramsCoord[0] = SetCustomStepParametrization(paramsCoord[0], dim, tag, SurfaceStepsU);//Массив полярных углов
            paramsCoord[1] = SetCustomStepParametrization(paramsCoord[1], dim, tag, SurfaceStepsV);//Массив радиус-векторов
            var surfaceParams = ParamsToSurfaces(paramsCoord[0], paramsCoord[1]);
            //Не работает корректно нужно переделать
            /*double[] surfaceParams;
            CombineSurfaceParametrization(out surfaceParams, paramsCoord[0], paramsCoord[1]);*/
            var pointTags = CreatePointTags(surfaceParams.Length / 2);
            var nodes = GetNodes(surfaceParams, dim, tag, pointTags);
            CreateSurfaces(nodes);
        }

        private double[] ParamsToSurfaces(double[] polars, double[] rVectors)
        {
            //Перевести в линейный массив пары polars[i], rVectors[i], пары должны быть упорядочены по полярному углу (поясню на доске)
            //Перевести упорядочные пары polars[i],rVector[i] в тройку пар для формирования треугольника параметров (поясню на доске)
            return null;
        }

        private void AddVolumes(int dim, int tag)
        {

        }

        private void CreateSurfaces(Node[] nodes)
        {
            var triangle = new Node[3];
            for(var i = 2; i < nodes.Length; i+=3)
            {
                triangle[0] = nodes[i - 2];
                triangle[1] = nodes[i - 1];
                triangle[2] = nodes[i];
                surfaces.Add(new Surface(surfaceId++, triangle ,ObjKind.GeometrySurface));
            }
        }

        private void CombineSurfaceParametrization(out double[] dst, double[] uParams, double[] vParams)
        {
            var triangles = (uParams.Length * 2 - 2) * (vParams.Length - 1);
            var coords = triangles * 3 * 2;
            dst = new double[coords];
            var total = 0;
            for (var v = 1; v < vParams.Length; ++v)
            {
                for (var u = 0; u < uParams.Length - 1; ++u, total+=12)
                {
                    dst[total] = uParams[u];
                    dst[total + 1] = vParams[v - 1];
                    dst[total + 2] = uParams[u + 1];
                    dst[total + 3] = vParams[v];
                    dst[total + 4] = uParams[u];
                    dst[total + 5] = vParams[v];
                    dst[total + 6] = uParams[u];
                    dst[total + 7] = vParams[v - 1];
                    dst[total + 8] = uParams[u + 1];
                    dst[total + 9] = vParams[v - 1];
                    dst[total + 10] = uParams[u + 1];
                    dst[total + 11] = vParams[v];
                }
            }
        }

        private int[] GetBoundary(int dim, int tag)
        {
            var error = 0;
            var dimTags = new int[] { dim, tag };
            IntPtr dimTags_n, outDimTags, outDimTags_n;
            dimTags_n = outDimTags = outDimTags_n = IntPtr.Zero;
            GmshModel.gmshModelGetBoundary(dimTags, (IntPtr)dimTags.Length, ref outDimTags, 
                                           ref outDimTags_n, 0, 0, 0, ref error);
            int[] outDTags;
            GmshUserMemory.GetTypesData(outDimTags, out outDTags, outDimTags_n);
            return outDTags.Where( (v, i) => (i & 1) == 1).ToArray();
        }

        private double[] SetCustomStepParametrization(double[] paramsCoord, int dim, int tag, int steps)
        {
            var step = (paramsCoord[1] - paramsCoord[0]) / (steps - 1);
            Array.Resize(ref paramsCoord, steps);
            for (var i = 0; i < steps; ++i)
                paramsCoord[i] = paramsCoord[0] + step * i;
            return paramsCoord;
        }

        private double[][] GetParametrizationBounds(int dim, int tag, int paramsCount)
        {
            var error = 0;
            IntPtr min, min_n, max, max_n;
            min = min_n = max = max_n = IntPtr.Zero;
            GmshModel.gmshModelGetParametrizationBounds(dim, tag, ref min, ref min_n, ref max, ref max_n, ref error);
            double[] minBounds, maxBounds;
            GmshUserMemory.GetTypesData(min, out minBounds, min_n);
            GmshUserMemory.GetTypesData(max, out maxBounds, max_n);
            var result = new double[paramsCount][];
            for(var i = 0; i < paramsCount; ++i)
            {
                result[i] = new double[2];
                result[i][0] = minBounds[i];
                result[i][1] = maxBounds[i];
            }
            return result;
        }

        private double[] GetDerivative(double[] parametrs, string derNum, int dim, int tag)
        {
            Derivative derFun = GmshGeometry.gmshModelGetDerivative; 
            if (derNum == "second") 
                derFun = GmshGeometry.gmshModelGetSecondDerivative;
            var error = 0;
            var derivaties = IntPtr.Zero;
            var derivaties_n = IntPtr.Zero;
            derFun(dim, tag, parametrs, (IntPtr)parametrs.Length, ref derivaties, ref derivaties_n, ref error);
            double[] derivative;
            GmshUserMemory.GetTypesData(derivaties, out derivative, derivaties_n);
            return derivative;
        }

        private double[] GetCurvatures(double[] parametrs, int dim, int tag)
        {
            var error = 0;
            var curvatures = IntPtr.Zero;
            var curvatures_n = IntPtr.Zero;
            GmshGeometry.gmshModelGetCurvature(dim, tag, parametrs, (IntPtr)parametrs.Length,
                                               ref curvatures, ref curvatures_n, ref error);
            double[] curvature;
            GmshUserMemory.GetTypesData(curvatures, out curvature, curvatures_n);
            return curvature;
        }

        private double[] GetPoints(double[] parametric,int dim, int tag)
        {
            var parametricLength = parametric == null ? 0 : parametric.Length;
            var error = 0;
            var coords = IntPtr.Zero;
            var coords_n = IntPtr.Zero;
            GmshModel.gmshModelGetValue(dim, tag, parametric, (IntPtr)parametricLength, ref coords, ref coords_n, ref error);
            double[] points;
            GmshUserMemory.GetTypesData(coords, out points, coords_n);
            return points;
        }

        private void GetAdjacencies(out int[] down, out int[] up, int dim, int tag) 
        {
            var error = 0;
            IntPtr downward, downward_n, upward, upward_n;
            downward = downward_n = upward = upward_n = IntPtr.Zero;
            GmshModel.gmshModelGetAdjacencies(dim, tag, ref upward, ref upward_n,
                                              ref downward, ref downward_n, ref error);
            GmshUserMemory.GetTypesData(upward, out up, upward_n);
            GmshUserMemory.GetTypesData(downward, out down, downward_n);
        }

        private bool IsLine(double[] paramsBound,int dim, int tag)
        {
            var modParams = new double[] { paramsBound[0] + 0.01, paramsBound[1] - 0.01};
            var derivatives = GetDerivative(paramsBound, "first", dim, tag);
            var derCount = derivatives.Length / 3;
            for (var i = 1; i < derCount; ++i)
                for(var j = 0; j < 3; ++j)
                    if (Math.Abs(derivatives[i * 3 + j] - derivatives[(i - 1) * 3 + j]) > 1E-8)
                        return false;
            return true;
        }

        private Node[] GetNodes(double[] parametric, int dim, int tag ,int[] pointsTags)
        {
            var points = GetPoints(parametric, dim, tag);
            var nodes = pointsTags.Select((v, i) => new { T = v, X = (float)points[i * 3],Y = (float)points[i * 3 + 1], 
                                          Z = (float)points[i * 3 + 2] })
                                  .Select(v => new Node(v.T, new Point3D(v.X, v.Y, v.Z)))
                                  .ToArray();
            return nodes;
        }
    }
}
