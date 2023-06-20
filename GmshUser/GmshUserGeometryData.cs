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
        private int totalPointsId;
        /// <summary>
        /// Экземляр данных о геометрии заданной модели
        /// </summary>
        /// <param name="modelId"></param>
        public GmshUserGeometryData(long modelId) : base(modelId)
        {
            var ext = Path.GetExtension(GmshUserIO.GetFileName());
            geometryMode = ext.Contains("step") || ext.Contains("igs") || ext.Contains("brep") ? 1 : 0;
            GetData();
        }
        /// <summary>
        /// Извлекает данные геометрии из модели
        /// </summary>
        public override void GetData()
        {
            var data = GetEntities();
            ExtractData(data);
        }

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
            var point = GetPoint(null, dim, tag);
            nodes.Add(new Node(tag, new Point3D((float)point[0], (float)point[1], (float)point[2])));
        }

        private void AddCurve(int dim, int tag)
        {
            if (curves == null)
                curves = new List<Curve>();
            var paramsCoord = GetParametrizationBounds(dim, tag);
            var lastCurvePoints = curves.Count != 0 ? curves[curves.Count - 1].NumberOfPoints : 0;
            totalPointsId += lastCurvePoints;
            var lastPointTag = base.nodes.Count + 1;
            var steps = 50;
            if (IsLine(paramsCoord, dim, tag))
                steps = 2;
            else
                paramsCoord = SetCustomStepParametrization(paramsCoord, dim, tag, steps);
            var tagsPoints = Enumerable.Range(lastPointTag + totalPointsId, steps).ToArray();
            var nodes = GetNodes(paramsCoord, dim, tag, tagsPoints);
            curves.Add(new Curve(tag, nodes));
        }

        private void AddSurface(int dim, int tag)
        {

        }

        private void AddVolumes(int dim, int tag)
        {

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

        private double[] GetParametrizationBounds(int dim, int tag)
        {
            var error = 0;
            IntPtr min, min_n, max, max_n;
            min = min_n = max = max_n = IntPtr.Zero;
            GmshModel.gmshModelGetParametrizationBounds(dim, tag, ref min, ref min_n, ref max, ref max_n, ref error);
            double[] minBounds, maxBounds;
            GmshUserMemory.GetTypesData(min, out minBounds, min_n);
            GmshUserMemory.GetTypesData(max, out maxBounds, max_n);
            return minBounds.Concat(maxBounds).ToArray();
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

        private double[] GetPoint(double[] parametric,int dim, int tag)
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
            var nodes = pointsTags.Select((v, i) => new { T = v, A = GetPoint(new double[] { parametric[i] }, dim, tag) })
                                  .Select(v => new Node(v.T, new Point3D((float)v.A[0], (float)v.A[1], (float)v.A[2])))
                                  .ToArray();
            return nodes;
        }
    }
}
