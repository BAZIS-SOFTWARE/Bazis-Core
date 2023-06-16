using Model;
using ModelParcer.GmshController;
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

namespace ModelParcer.GmshUser
{
    /// <summary>
    /// Извлекает данные о геометрии заданной модели
    /// </summary>
    public class GmshUserGeometryData : GmshUserModelData
    {
        private delegate void Derivative(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n,
                                         ref IntPtr derivatives, ref IntPtr derivatives_n, ref int ierr);
        private int geometryMode;
        /// <summary>
        /// Экземляр данных о геометрии заданной модели
        /// </summary>
        /// <param name="modelId"></param>
        public GmshUserGeometryData(long modelId) : base(modelId)
        {
            var ext = Path.GetExtension(GmshUserIO.GetFileName());
            geometryMode = ext.Contains("stp") || ext.Contains("igs") ? 1 : 0;
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
            foreach (var v in groups[0])
                AddPoint(v.D, v.T);
            foreach (var v in groups[1])
                AddCurve(v.D, v.T);
            foreach (var v in groups[2])
                AddSurface(v.D, v.T);
        }

        private void AddPoint(int dim, int tag)
        {
            if(nodes == null)
                nodes = new List<Node>();
            var point = GetPoint(dim, tag);
            nodes.Add(new Node(tag, new Point3D((float)point[0], (float)point[1], (float)point[2])));
        }

        private void AddCurve(int dim, int tag)
        {
            if (curves == null)
                curves = new List<Curve>();
            if (IsLine(dim, tag))
            {
                var pointsPair = GetBoundary(dim, tag);
                var tags = ExtractTagsFromPair(pointsPair);
                curves.Add(new Curve(tag, GetNodes(dim, tags)));
                return;
            }
            //TODO: Исследовать кривую
        }

        private void AddSurface(int dim, int tag)
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
            if (parametrs == null)
                parametrs = GetParametrizationBounds(dim, tag);
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

        private double[] GetPoint(int dim, int tag)
        {
            var error = 0;
            var coords = IntPtr.Zero;
            var coords_n = IntPtr.Zero;
            GmshModel.gmshModelGetValue(dim, tag, null, (IntPtr)0,ref coords, ref coords_n, ref error);
            double[] points;
            GmshUserMemory.GetTypesData(coords, out points, coords_n);
            return points;
        }

        private bool IsLine(int dim, int tag)
        {
            var derivative = GetDerivative(null, "second", dim, tag);
            return derivative.All(v => v == 0);
        }

        private int[] ExtractTagsFromPair(int[] pointsPair) => pointsPair.Where((v, i) => (i & 1) == 1).ToArray();

        private Node[] GetNodes(int dim, int[] tags)
        {
            return tags.Select(v => new { T = v, A = GetPoint(dim, v) })
                       .Select(v => new Node(v.T, new Point3D((float)v.A[0], (float)v.A[1], (float)v.A[2])))
                       .ToArray();
        }
    }
}
