using System;
using static GmshApi.GmshController.GmshController;
using static GmshApi.GmshKernel;
using static GmshApi.GmshMemory;

namespace GmshApi.Wrapper
{
    /// <inheritdoc/>
    public class Model
    {
        private delegate void gmshModelAddFun(string name, ref int ierr);
        private gmshModelAddFun gmshModelAdd = GetFunction<gmshModelAddFun>("gmshModelAdd");

        private delegate void gmshModelRemoveFun(ref int ierr);
        private gmshModelRemoveFun gmshModelRemove = GetFunction<gmshModelRemoveFun>("gmshModelRemove");

        private delegate void gmshModelListFun(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        private gmshModelListFun gmshModelList = GetFunction<gmshModelListFun>("gmshModelList");

        private delegate void gmshModelGetCurrentFun(ref IntPtr name, ref int ierr);
        private gmshModelGetCurrentFun gmshModelGetCurrent = GetFunction<gmshModelGetCurrentFun>("gmshModelGetCurrent");

        private delegate void gmshModelSetCurrentFun(string name, ref int ierr);
        private gmshModelSetCurrentFun gmshModelSetCurrent = GetFunction<gmshModelSetCurrentFun>("gmshModelSetCurrent");

        private delegate void gmshModelGetFileNameFun(ref IntPtr fileName, ref int ierr);
        private gmshModelGetFileNameFun gmshModelGetFileName = GetFunction<gmshModelGetFileNameFun>("gmshModelGetFileName");

        private delegate void gmshModelSetFileNameFun(string fileName, ref int ierr);
        private gmshModelSetFileNameFun gmshModelSetFileName = GetFunction<gmshModelSetFileNameFun>("gmshModelSetFileName");

        private delegate void gmshModelGetEntitiesFun(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        private gmshModelGetEntitiesFun gmshModelGetEntities = GetFunction<gmshModelGetEntitiesFun>("gmshModelGetEntities");

        private delegate void gmshModelSetEntityNameFun(int dim, int tag, string name, ref int ierr);
        private gmshModelSetEntityNameFun gmshModelSetEntityName = GetFunction<gmshModelSetEntityNameFun>("gmshModelSetEntityName");

        private delegate void gmshModelGetEntityNameFun(int dim, int tag, ref IntPtr name, ref int ierr);
        private gmshModelGetEntityNameFun gmshModelGetEntityName = GetFunction<gmshModelGetEntityNameFun>("gmshModelGetEntityName");

        private delegate void gmshModelRemoveEntityNameFun(string name, ref int ierr);
        private gmshModelRemoveEntityNameFun gmshModelRemoveEntityName = GetFunction<gmshModelRemoveEntityNameFun>("gmshModelRemoveEntityName");

        private delegate void gmshModelGetBoundaryFun(int[] dimTags, IntPtr dimTags_n, ref IntPtr outDimTags,
                                                      ref IntPtr outDimTags_n, int combined, int oriented,
                                                      int recursive, ref int ierr);
        private gmshModelGetBoundaryFun gmshModelGetBoundary = GetFunction<gmshModelGetBoundaryFun>("gmshModelGetBoundary");

        private delegate void gmshModelGetAdjacenciesFun(int dim, int tag, ref IntPtr upward, ref IntPtr upward_n,
                                                         ref IntPtr downward, ref IntPtr downward_n, ref int ierr);
        private gmshModelGetAdjacenciesFun gmshModelGetAdjacencies = GetFunction<gmshModelGetAdjacenciesFun>("gmshModelGetAdjacencies");

        private delegate void gmshModelGetBoundingBoxFun(int dim, int tag, ref double xmin,
                                                         ref double ymin, ref double zmin,
                                                         ref double xmax, ref double ymax,
                                                         ref double zmax, ref int ierr);
        private gmshModelGetBoundingBoxFun gmshModelGetBoundingBox = GetFunction<gmshModelGetBoundingBoxFun>("gmshModelGetBoundingBox");

        private delegate int gmshModelGetDimensionFun(ref int ierr);
        private gmshModelGetDimensionFun gmshModelGetDimension = GetFunction<gmshModelGetDimensionFun>("gmshModelGetDimension");

        private delegate int gmshModelAddDiscreteEntityFun(int dim, int tag, int[] boundary, IntPtr boundary_n, ref int ierr);
        private gmshModelAddDiscreteEntityFun gmshModelAddDiscreteEntity = GetFunction<gmshModelAddDiscreteEntityFun>("gmshModelAddDiscreteEntity");

        private delegate void gmshModelRemoveEntitiesFun(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        private gmshModelRemoveEntitiesFun gmshModelRemoveEntities = GetFunction<gmshModelRemoveEntitiesFun>("gmshModelRemoveEntities");

        private delegate void gmshModelGetTypeFun(int dim, int tag, ref IntPtr entityType, ref int ierr);
        private gmshModelGetTypeFun gmshModelGetType = GetFunction<gmshModelGetTypeFun>("gmshModelGetType");

        private delegate void gmshModelGetValueFun(int dim, int tag, double[] parametricCoord, IntPtr parameticCoord_n, 
                                                   ref IntPtr coord, ref IntPtr coord_n, ref int ierr);
        private gmshModelGetValueFun gmshModelGetValue = GetFunction<gmshModelGetValueFun>("gmshModelGetValue");

        private delegate void gmshModelGetDerivativeFun(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n, 
                                                        ref IntPtr derivatives, ref IntPtr derivatives_n, ref int ierr);
        private gmshModelGetDerivativeFun gmshModelGetDerivative = GetFunction<gmshModelGetDerivativeFun>("gmshModelGetDerivative");

        private delegate void gmshModelGetSecondDerivativeFun(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n, 
                                                              ref IntPtr derivatives, ref IntPtr derivatives_n, ref int ierr);
        private gmshModelGetSecondDerivativeFun gmshModelGetSecondDerivative = GetFunction<gmshModelGetSecondDerivativeFun>("gmshModelGetSecondDerivative");

        private delegate void gmshModelGetCurvatureFun(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n, 
                                                       ref IntPtr curvatures, ref IntPtr curvatures_n, ref int ierr);
        private gmshModelGetCurvatureFun gmshModelGetCurvature = GetFunction<gmshModelGetCurvatureFun>("gmshModelGetCurvature");

        private delegate void gmshModelGetParametrizationFun(int dim, int tag, double[] coord, IntPtr coord_n,
                                                             ref IntPtr parametricCoord, ref IntPtr parametricCoord_n, ref int ierr);
        private gmshModelGetParametrizationFun gmshModelGetParametrization = GetFunction<gmshModelGetParametrizationFun>("gmshModelGetParametrization");

        private delegate void gmshModelGetParametrizationBoundsFun(int dim, int tag, ref IntPtr min, ref IntPtr min_n, 
                                                                   ref IntPtr max, ref IntPtr max_n, ref int ierr);
        private gmshModelGetParametrizationBoundsFun gmshModelGetParametrizationBounds = GetFunction<gmshModelGetParametrizationBoundsFun>("gmshModelGetParametrizationBounds");

        private delegate int gmshModelIsInsideFun(int dim, int tag, double[] coord, IntPtr coord_n, int parametric, ref int ierr);
        private gmshModelIsInsideFun gmshModelIsInside = GetFunction<gmshModelIsInsideFun>("gmshModelIsInside");

        private delegate void gmshModelReparametrizeOnSurfaceFun(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n, int surfaceTag,
                                                             ref IntPtr surfaceParametricCoord, ref IntPtr surfaceParametricCoord_n,
                                                             int which, ref int ierr);
        private gmshModelReparametrizeOnSurfaceFun gmshModelReparametrizeOnSurface = GetFunction<gmshModelReparametrizeOnSurfaceFun>("gmshModelReparametrizeOnSurface");

        private delegate void gmshModelSetCoordinatesFun(int tag, double x, double y, double z, ref int ierr);
        private gmshModelSetCoordinatesFun gmshModelSetCoordinates = GetFunction<gmshModelSetCoordinatesFun>("gmshModelSetCoordinates");

        private delegate void gmshModelSetAttributeFun(string name, string[] values, IntPtr values_n, ref int ierr);
        private gmshModelSetAttributeFun gmshModelSetAttribute = GetFunction<gmshModelSetAttributeFun>("gmshModelSetAttribute");
        
        private delegate void gmshModelGetAttributeFun(string name, ref IntPtr values, ref IntPtr values_n, ref int ierr);
        private gmshModelGetAttributeFun gmshModelGetAttribute = GetFunction<gmshModelGetAttributeFun>("gmshModelGetAttribute");

        private delegate void gmshModelGetAttributeNamesFun(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        private gmshModelGetAttributeNamesFun gmshModelGetAttributeNames = GetFunction<gmshModelGetAttributeNamesFun>("gmshModelGetAttributeNames");

        private delegate void gmshModelRemoveAttributeFun(string name, ref int ierr);
        private gmshModelRemoveAttributeFun gmshModelRemoveAttribute = GetFunction<gmshModelRemoveAttributeFun>("gmshModelRemoveAttribute");

        /// <inheritdoc/>
        public void Add(string name)
        {
            var ierr = 0;
            gmshModelAdd(name, ref ierr);
        }
        /// <inheritdoc/>
        public void Remove()
        {
            var ierr = 0;
            gmshModelRemove(ref ierr);
        }
        /// <inheritdoc/>
        public string[] List()
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelList(ref intValue, ref intValue_n, ref ierr);
            return GetArrayOfString(intValue, intValue_n);
        }
        /// <inheritdoc/>
        public string GetCurrent()
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelGetCurrent(ref intValue, ref ierr);
            return GetString(intValue);
        }
        /// <inheritdoc/>
        public void SetCurrent(string name)
        {
            var ierr = 0;
            gmshModelSetCurrent(name, ref ierr);
        }
        /// <inheritdoc/>
        public string GetFileName()
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelGetFileName(ref intValue, ref ierr);
            return GetString(intValue);
        }
        /// <inheritdoc/>
        public void SetFileName(string fileName)
        {
            var ierr = 0;
            gmshModelSetFileName(fileName, ref ierr);
        }
        /// <inheritdoc/>
        public int[] GetEntities(int dim = -1)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetEntities(ref intValue, ref intValue_n, dim, ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <inheritdoc/>
        public void SetEntityName(int dim, int tag, string name)
        {
            var ierr = 0;
            gmshModelSetEntityName(dim, tag, name, ref ierr);
        }
        /// <inheritdoc/>
        public string GetEntityName(int dim, int tag)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelGetEntityName(dim, tag, ref intValue, ref ierr);
            return GetString(intValue);
        }
        /// <inheritdoc/>
        public void RemoveEntityName(string name)
        {
            var ierr = 0;
            gmshModelRemoveEntityName(name, ref ierr);
        }
        /// <inheritdoc/>
        public int[] GetBoundary(int[] dimTags, bool combined = true, bool oriented = true, bool recursive = false)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelGetBoundary(dimTags, (IntPtr)dimTags.Length, ref intValue, ref intValue_n,
                                 Convert.ToInt32(combined), Convert.ToInt32(oriented), Convert.ToInt32(recursive), ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <inheritdoc/>
        public Tuple<int[], int[]> GetAdjacencies(int dim, int tag)
        {
            var ierr = 0;
            var intValueUp = IntPtr.Zero;
            var intValueUp_n = IntPtr.Zero;
            var intValueDown = IntPtr.Zero;
            var intValueDown_n = IntPtr.Zero;
            gmshModelGetAdjacencies(dim, tag, ref intValueUp, ref intValueUp_n, ref intValueDown, ref intValueDown_n, ref ierr);
            var upAdjacencies = GetObjectsData<int>(intValueUp, intValueUp_n);
            var downAdjacencies = GetObjectsData<int>(intValueDown, intValueDown_n);
            return Tuple.Create(upAdjacencies, downAdjacencies);
        }

        /// <inheritdoc/>
        public Tuple<double, double, double, double, double, double> GetBoundingBox(int dim, int tag)
        {
            var ierr = 0;
            double xMin = 0.0, yMin = 0.0, zMin = 0.0;
            double xMax = 0.0, yMax = 0.0, zMax = 0.0;
            gmshModelGetBoundingBox(dim, tag, ref xMin, ref yMin, ref zMin, ref xMax, ref yMax, ref zMax, ref ierr);
            return Tuple.Create(xMin, yMin, zMin, xMax, yMax, zMax);
        }
        /// <inheritdoc/>
        public int GetDimension()
        {
            var ierr = 0;
            return gmshModelGetDimension(ref ierr);
        }
        /// <inheritdoc/>
        public int AddDiscreteEntity(int dim, int tag = -1, int[] boundary = null)
        {
            var ierr = 0;
            boundary = boundary ?? new int[0];
            return gmshModelAddDiscreteEntity(dim, tag, boundary, (IntPtr)boundary.Length, ref ierr);
        }
        /// <inheritdoc/>
        public void RemoveEntities(int[] dimTags, bool recursive = false)
        {
            var ierr = 0;
            gmshModelRemoveEntities(dimTags, (IntPtr)dimTags.Length,Convert.ToInt32(recursive), ref ierr);
        }
    }
}
