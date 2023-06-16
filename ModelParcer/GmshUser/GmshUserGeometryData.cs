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
        private int geometryMode;
        /// <summary>
        /// Экземляр данных о геометрии заданной модели
        /// </summary>
        /// <param name="modelId"></param>
        public GmshUserGeometryData(long modelId) : base(modelId)
        {
            var ext = Path.GetExtension(GmshUserIO.GetFileName());
            geometryMode = ext.Contains("stp") || ext.Contains("igs") ? 1 : 0;
            GetElements();
        }
        /// <summary>
        /// Извлекает данные геометрии из модели
        /// </summary>
        protected override void GetElements()
        {
            var error = 0;
            IntPtr dimTags, dimTags_n, outDimTags, outDimTags_n;
            dimTags = dimTags_n = outDimTags = outDimTags_n = IntPtr.Zero;
            if (geometryMode == 1)
                GmshGeometry.gmshModelOccGetEntities(ref dimTags, ref dimTags_n, -1, ref error);
            else
                GmshModel.gmshModelGetEntities(ref dimTags, ref dimTags_n, -1, ref error);
            int[] data;
            GmshUserMemory.GetTypesData(dimTags,out data, dimTags_n);
            GetByGroup(data);
        }

        private void GetByGroup(int[] dimTags)
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
                GetPoints(v.D, v.T);
            foreach(var v in groups[1])
                GetCurves(v.D, v.T);
        }

        private void GetPoints(int dim, int tag)
        {
            var error = 0;
            IntPtr coords = IntPtr.Zero;
            IntPtr coords_n = IntPtr.Zero;
            if(nodes == null)
                nodes = new List<DoublePoint3D>();
            GmshModel.gmshModelGetValue(dim, tag, IntPtr.Zero, IntPtr.Zero,
                                        ref coords, ref coords_n, ref error);
            double[] points;
            GmshUserMemory.GetTypesData(coords, out points, coords_n);
            nodes.Add(new DoublePoint3D(tag, points[0], points[1], points[2]));
        }

        /// <summary>
        /// Записывает минимальные\максимальные параметры для пары размерность : идентификатор
        /// </summary>
        /// <param name="parametrs">[Out]Параметры для пары</param>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        public void GetParametrizationBounds(out double[] parametrs, int dim, int tag)
        {
            var error = 0;
            IntPtr min, min_n, max, max_n;
            min = min_n = max = max_n = IntPtr.Zero;
            GmshModel.gmshModelGetParametrizationBounds(dim, tag, ref min, ref min_n, ref max, ref max_n, ref error);
            double[] minBounds, maxBounds;
            GmshUserMemory.GetTypesData(min, out minBounds, min_n);
            GmshUserMemory.GetTypesData(max, out maxBounds, max_n);
            parametrs = minBounds.Concat(maxBounds).ToArray();
        }



        private void GetCurves(int dim, int tag)
        {
            var error = 0;
            IntPtr min, min_n, max, max_n;
            min = min_n = max = max_n = IntPtr.Zero;
            GmshModel.gmshModelGetParametrizationBounds(dim, tag, ref min, ref min_n, ref max, ref max_n, ref error);
            double[] minBounds, maxBounds;
            GmshUserMemory.GetTypesData(min, out minBounds, min_n);
            GmshUserMemory.GetTypesData(max, out maxBounds, max_n);


            var derivaties = IntPtr.Zero;
            var derivaties_n = IntPtr.Zero;
            var minMax = new double[2] { minBounds[0], maxBounds[0] };
            GmshGeometry.gmshModelGetDerivative(dim, tag, minMax, (IntPtr)minMax.Length, 
                                                ref derivaties, ref derivaties_n, ref error);
            double[] derivative;
            GmshUserMemory.GetTypesData(derivaties, out derivative, derivaties_n);
            GmshGeometry.gmshModelGetSecondDerivative(dim, tag, minMax, (IntPtr)minMax.Length,
                                                ref derivaties, ref derivaties_n, ref error);
            double[] secDerivative;
            GmshUserMemory.GetTypesData(derivaties, out secDerivative, derivaties_n);

            var curvatures = IntPtr.Zero;
            var curvatures_n = IntPtr.Zero;
            GmshGeometry.gmshModelGetCurvature(dim, tag, minMax, (IntPtr)minMax.Length,
                                               ref curvatures, ref curvatures_n, ref error);
            double[] curvature;
            GmshUserMemory.GetTypesData(curvatures, out curvature, curvatures_n);
        }
    }
}
