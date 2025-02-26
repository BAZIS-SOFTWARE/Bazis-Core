using MathNet.Numerics.LinearAlgebra.Complex;
using System;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Occ
    {
        private delegate void gmshModelOccRemoveFun(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        private gmshModelOccRemoveFun gmshModelOccRemove = GetFunction<gmshModelOccRemoveFun>("gmshModelOccRemove");

        private delegate void gmshModelOccImportShapesFun(string fileName, ref IntPtr outDimTags,ref IntPtr outDimTags_n, 
                                                          int highestDimOnly, string format, ref int ierr);
        private gmshModelOccImportShapesFun gmshModelOccImportShapes = GetFunction<gmshModelOccImportShapesFun>("gmshModelOccImportShapes");

        private delegate void gmshModelOccGetEntitiesFun(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        private gmshModelOccGetEntitiesFun gmshModelOccGetEntities = GetFunction<gmshModelOccGetEntitiesFun>("gmshModelOccGetEntities");

        private delegate void gmshModelOccGetCurveLoopsFun(int surfaceTag, ref IntPtr curveLoopTags,
                                                           ref IntPtr curveLoopTags_n, ref IntPtr curveTags,
                                                           ref IntPtr curveTags_n, ref IntPtr curveTags_nn,
                                                           ref int ierr);
        private gmshModelOccGetCurveLoopsFun gmshModelOccGetCurveLoops = GetFunction<gmshModelOccGetCurveLoopsFun>("gmshModelOccGetCurveLoops");

        private delegate void gmshModelOccGetSurfaceLoopsFun(int volumeTag, ref IntPtr surfaceLoopTags,
                                                             ref IntPtr surfaceLoopTags_n, ref IntPtr surfaceTags,
                                                             ref IntPtr surfaceTags_n, ref IntPtr surfaceTags_nn,
                                                             ref int ierr);
        private gmshModelOccGetSurfaceLoopsFun gmshModelOccGetSurfaceLoops = GetFunction<gmshModelOccGetSurfaceLoopsFun>("gmshModelOccGetSurfaceLoops");

        private delegate void gmshModelOccGetMassFun(int dim, int tag, ref double mass, ref int ierr);
        private gmshModelOccGetMassFun gmshModelOccGetMass = GetFunction<gmshModelOccGetMassFun>("gmshModelOccGetMass");

        private delegate void gmshModelOccGetCenterOfMassFun(int dim, int tag, ref double x, ref double y, ref double z, ref int ierr);
        private gmshModelOccGetCenterOfMassFun gmshModelOccGetCenterOfMass = GetFunction<gmshModelOccGetCenterOfMassFun>("gmshModelOccGetCenterOfMass");

        private delegate void gmshModelOccSynchronizeFun(ref int ierr);
        private gmshModelOccSynchronizeFun gmshModelOccSynchronize = GetFunction<gmshModelOccSynchronizeFun>("gmshModelOccSynchronize");

        /// <inheritdoc/>
        public void Remove(int[] dimTags, bool recursive = false)
        {
            var ierr = 0;
            gmshModelOccRemove(dimTags, (IntPtr)dimTags.Length, Convert.ToInt32(recursive), ref ierr);
        }
        /// <inheritdoc/>
        public int[] ImportShapes(string fileName, bool highestDimOnly = true, string format = "")
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelOccImportShapes(fileName, ref intValue, ref intValue_n, Convert.ToInt32(highestDimOnly), format, ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <inheritdoc/>
        public int[] GetEntities(int dim = -1)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelOccGetEntities(ref intValue, ref intValue_n, dim, ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <inheritdoc/>
        public Tuple<int[], int[][]> GetCurveLoops(int surfaceTag)
        {
            var ierr = 0;
            var curveLoopTags = IntPtr.Zero;
            var curveLoopTags_n = IntPtr.Zero;
            var curveTags = IntPtr.Zero;
            var curveTags_n = IntPtr.Zero;
            var curveTags_nn = IntPtr.Zero;
            gmshModelOccGetSurfaceLoops(surfaceTag, ref curveLoopTags, ref curveLoopTags_n, ref curveTags, ref curveTags_n, ref curveTags_nn, ref ierr);
            var loopTags = GetObjectsData<int>(curveLoopTags, curveLoopTags_n);
            var curTags = GetArrayOfObjectsData<int>(curveTags, curveTags_nn, curveTags_n);
            return Tuple.Create(loopTags, curTags);
        }
        /// <inheritdoc/>
        public Tuple<int[], int[][]> GetSurfaceLoops(int volumeTag)
        {
            var ierr = 0;
            var surfaceLoopTags = IntPtr.Zero;
            var surfaceLoopTags_n = IntPtr.Zero;
            var surfaceTags = IntPtr.Zero;
            var surfaceTags_n = IntPtr.Zero;
            var surfaceTags_nn = IntPtr.Zero;
            gmshModelOccGetSurfaceLoops(volumeTag, ref surfaceLoopTags, ref surfaceLoopTags_n, ref surfaceTags, ref surfaceTags_n, ref surfaceTags_nn, ref ierr);
            var loopTags = GetObjectsData<int>(surfaceLoopTags, surfaceLoopTags_n);
            var surfTags = GetArrayOfObjectsData<int>(surfaceTags, surfaceTags_nn, surfaceTags_n);
            return Tuple.Create(loopTags, surfTags);
        }
        /// <inheritdoc/>
        public double GetMass(int dim, int tag)
        {
            var ierr = 0;
            var mass = 0.0;
            gmshModelOccGetMass(dim, tag, ref mass, ref ierr);
            return mass;
        }
        /// <inheritdoc/>
        public Tuple<double, double, double> GetCenterOfMass(int dim, int tag)
        {
            var ierr = 0;
            var x = 0.0;
            var y = 0.0;
            var z = 0.0;
            gmshModelOccGetCenterOfMass(dim, tag, ref x, ref y, ref z, ref ierr);
            return Tuple.Create(x,y,z);
        }
        /// <inheritdoc/>
        public void Synchronize()
        {
            var ierr = 0;
            gmshModelOccSynchronize(ref ierr);
        }
    }
}
