using System;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Mesh
    {
        private delegate void gmshModelMeshGenerateFun(int dim, ref int ierr);
        private gmshModelMeshGenerateFun gmshModelMeshGenerate = GetFunction<gmshModelMeshGenerateFun>("gmshModelMeshGenerate");

        private delegate void gmshModelMeshRecombineFun(ref int ierr);
        private gmshModelMeshRecombineFun gmshModelMeshRecombine = GetFunction<gmshModelMeshRecombineFun>("gmshModelMeshRecombine");

        private delegate void gmshModelMeshRefineFun(ref int ierr);
        private gmshModelMeshRefineFun gmshModelMeshRefine = GetFunction<gmshModelMeshRefineFun>("gmshModelMeshRefine");

        private delegate void gmshModelMeshClearFun(int[] dimTags, IntPtr dimTags_n, ref int ierr);
        private gmshModelMeshClearFun gmshModelMeshClear = GetFunction<gmshModelMeshClearFun>("gmshModelMeshClear");

        private delegate void gmshModelMeshGetNodesFun(ref IntPtr nodeTags, ref IntPtr nodeTags_n,
                                                       ref IntPtr coord, ref IntPtr coord_n,
                                                       ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                       int dim, int tag, int includeBoundary,
                                                       int returnParametricCoord, ref int ierr);
        private gmshModelMeshGetNodesFun gmshModelMeshGetNodes = GetFunction<gmshModelMeshGetNodesFun>("gmshModelMeshGetNodes");

        private delegate void gmshModelMeshGetNodesByElementTypeFun(int elementType, ref IntPtr nodeTags,
                                                                    ref IntPtr nodeTags_n, ref IntPtr coord,
                                                                    ref IntPtr coord_n, ref IntPtr parametricCoord,
                                                                    ref IntPtr parametricCoord_n, int tag,
                                                                    int returnParametricCoord, ref int ierr);
        private gmshModelMeshGetNodesByElementTypeFun gmshModelMeshGetNodesByElementType = GetFunction<gmshModelMeshGetNodesByElementTypeFun>("gmshModelMeshGetNodesByElementType");

        private delegate void gmshModelMeshAddNodesFun(int dim, int tag, IntPtr[] nodeTags, IntPtr nodeTags_n,
                                                      double[] coord, IntPtr coord_n, double[] parametricCoord,
                                                      IntPtr parametricCoord_n, ref int ierr);
        private gmshModelMeshAddNodesFun gmshModelMeshAddNodes = GetFunction<gmshModelMeshAddNodesFun>("gmshModelMeshAddNodes");

        private delegate void gmshModelMeshReclassifyNodesFun(ref int ierr);
        private gmshModelMeshReclassifyNodesFun gmshModelMeshReclassifyNodes = GetFunction<gmshModelMeshReclassifyNodesFun>("gmshModelMeshReclassifyNodes");

        private delegate void gmshModelMeshGetElementsFun(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                          ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                          ref IntPtr elementTags_nn, ref IntPtr nodeTags,
                                                          ref IntPtr nodeTags_n, ref IntPtr nodeTags_nn,
                                                          int dim, int tag, ref int ierr);
        private gmshModelMeshGetElementsFun gmshModelMeshGetElements = GetFunction<gmshModelMeshGetElementsFun>("gmshModelMeshGetElements");

        private delegate void gmshModelMeshGetElementsByCoordinatesFun(double x, double y, double z,
                                                                       ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                                       int dim, int strict, ref int ierr);
        private gmshModelMeshGetElementsByCoordinatesFun gmshModelMeshGetElementsByCoordinates = GetFunction<gmshModelMeshGetElementsByCoordinatesFun>("gmshModelMeshGetElementsByCoordinates");

        private delegate void gmshModelMeshGetElementTypesFun(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                              int dim, int tag, ref int ierr);
        private gmshModelMeshGetElementTypesFun gmshModelMeshGetElementTypes = GetFunction<gmshModelMeshGetElementTypesFun>("gmshModelMeshGetElementTypes");

        private delegate void gmshModelMeshGetElementPropertiesFun(int elementType, ref IntPtr elementName, ref int dim,
                                                                   ref int order, ref int numNodes, ref IntPtr localeNodeCoord,
                                                                   ref IntPtr localeNodeCoord_n, ref int numPrimaryNodes,
                                                                   ref int ierr);
        private gmshModelMeshGetElementPropertiesFun gmshModelMeshGetElementProperties = GetFunction<gmshModelMeshGetElementPropertiesFun>("gmshModelMeshGetElementProperties");

        private delegate void gmshModelMeshGetElementQualitiesFun(IntPtr[] elementTags, IntPtr elementTags_n,
                                                                  ref IntPtr elementsQuality, ref IntPtr elementQuality_n,
                                                                  string qualityName, IntPtr task,
                                                                  IntPtr numTasks, ref int ierr);
        private gmshModelMeshGetElementQualitiesFun gmshModelMeshGetElementQualities = GetFunction<gmshModelMeshGetElementQualitiesFun>("gmshModelMeshGetElementQualities");

        private delegate void gmshModelMeshAddElementsByTypeFun(int tag, int elementType, IntPtr[] elementTags,
                                                            IntPtr elementTags_n, IntPtr[] nodeTags, IntPtr nodeTags_n,
                                                            ref int ierr);
        private gmshModelMeshAddElementsByTypeFun gmshModelMeshAddElementsByType = GetFunction<gmshModelMeshAddElementsByTypeFun>("gmshModelMeshAddElementsByType");

        private delegate void gmshModelMeshSetSizeFun(int[] dimTags, IntPtr dimTags_n, double size, ref int ierr);
        private gmshModelMeshSetSizeFun gmshModelMeshSetSize = GetFunction<gmshModelMeshSetSizeFun>("gmshModelMeshSetSize");

        private delegate void gmshModelMeshGetSizesFun(int[] dimTags, IntPtr dimTags_n, ref IntPtr sizes, ref IntPtr sizes_n, ref int ierr);
        private gmshModelMeshGetSizesFun gmshModelMeshGetSizes = GetFunction<gmshModelMeshGetSizesFun>("gmshModelMeshGetSizes");


        private delegate void gmshModelMeshSetTransfiniteCurveFun(int tag, int numNodes, string meshType, double coef, ref int ierr);
        private gmshModelMeshSetTransfiniteCurveFun gmshModelMeshSetTransfiniteCurve = GetFunction<gmshModelMeshSetTransfiniteCurveFun>("gmshModelMeshSetTransfiniteCurve");

        private delegate void gmshModelMeshSetTransfiniteSurfaceFun(int tag, string arrangement, int[] cornerTags, IntPtr cornerTags_n, ref int ierr);
        private gmshModelMeshSetTransfiniteSurfaceFun gmshModelMeshSetTransfiniteSurface = GetFunction<gmshModelMeshSetTransfiniteSurfaceFun>("gmshModelMeshSetTransfiniteSurface");

        private delegate void gmshModelMeshSetTransfiniteVolumeFun(int tag, int[] cornerTags, IntPtr cornerTags_n, ref int ierr);
        private gmshModelMeshSetTransfiniteVolumeFun gmshModelMeshSetTransfiniteVolume = GetFunction<gmshModelMeshSetTransfiniteVolumeFun>("gmshModelMeshSetTransfiniteVolume");

        private delegate void gmshModelMeshSetTransfiniteAutomaticFun(int[] dimTags, IntPtr dimTags_n, double cornerAngle, int recombine, ref int ierr);
        private gmshModelMeshSetTransfiniteAutomaticFun gmshModelMeshSetTransfiniteAutomatic = GetFunction<gmshModelMeshSetTransfiniteAutomaticFun>("gmshModelMeshSetTransfiniteAutomatic");

        private delegate void gmshModelMeshSetAlgorithmFun(int dim, int tag, int val, ref int ierr);
        private gmshModelMeshSetAlgorithmFun gmshModelMeshSetAlgorithm = GetFunction<gmshModelMeshSetAlgorithmFun>("gmshModelMeshSetAlgorithm");

        private delegate void gmshModelMeshRemoveConstraintsFun(int[] dimTags, IntPtr dimTags_n, ref int ierr);
        private gmshModelMeshRemoveConstraintsFun gmshModelMeshRemoveConstraints = GetFunction<gmshModelMeshRemoveConstraintsFun>("gmshModelMeshRemoveConstraints");

        private delegate void gmshModelMeshImportStlFun(ref int ierr);
        private gmshModelMeshImportStlFun gmshModelMeshImportStl = GetFunction<gmshModelMeshImportStlFun>("gmshModelMeshImportStl");

        private delegate void gmshModelMeshSetVisibilityFun(IntPtr[] elementTags, IntPtr elementTags_n, int value, ref int ierr);
        private gmshModelMeshSetVisibilityFun gmshModelMeshSetVisibility = GetFunction<gmshModelMeshSetVisibilityFun>("gmshModelMeshSetVisibility");

        private delegate void gmshModelMeshClassifySurfacesFun(double angle, int boundary, int forReparametrization, double curveAngle, int exportDiscrete, ref int ierr);
        private gmshModelMeshClassifySurfacesFun gmshModelMeshClassifySurfaces = GetFunction<gmshModelMeshClassifySurfacesFun>("gmshModelMeshClassifySurfaces");

        private delegate void gmshModelMeshCreateGeometryFun(int[] dimTags, IntPtr dimTags_n, ref int ierr);
        private gmshModelMeshCreateGeometryFun gmshModelMeshCreateGeometry = GetFunction<gmshModelMeshCreateGeometryFun>("gmshModelMeshCreateGeometry");

        /// <inheritdoc/>
        public Field Field { get; private set; }

        /// <inheritdoc/>
        public Mesh()
        {
            Field = new Field();
        }

        /// <inheritdoc/>
        public void Generate(int dim)
        {
            var ierr = 0;
            gmshModelMeshGenerate(dim, ref ierr);
        }
        /// <inheritdoc/>
        public void Recombine()
        {
            var ierr = 0;
            gmshModelMeshRecombine(ref ierr);
        }
        /// <inheritdoc/>
        public void Refine()
        {
            var ierr = 0;
            gmshModelMeshRefine(ref ierr);
        }
        /// <inheritdoc/>
        public void Clear(int[] dimTags)
        {
            var ierr = 0;
            gmshModelMeshClear(dimTags, (IntPtr)dimTags.Length, ref ierr);
        }
        /// <inheritdoc/>
        public Tuple<IntPtr[], double[], double[]> GetNodes(int dim = -1, int tag = -1, bool includeBoundary = false, bool returnParametricCoord = true)
        {
            var ierr = 0;
            var nodeTags = IntPtr.Zero;
            var nodeTags_n = IntPtr.Zero;
            var coord = IntPtr.Zero;
            var coord_n = IntPtr.Zero;
            var parametricCoord = IntPtr.Zero;
            var parametricCoord_n = IntPtr.Zero;
            gmshModelMeshGetNodes(ref nodeTags, ref nodeTags_n, ref coord, ref coord_n, ref parametricCoord, ref parametricCoord_n,
                                  dim, tag, Convert.ToInt32(includeBoundary), Convert.ToInt32(returnParametricCoord), ref ierr);
            var tags = GetObjectsData<IntPtr>(nodeTags, nodeTags_n);
            var coords = GetObjectsData<double>(coord, coord_n);
            var parametric = GetObjectsData<double>(parametricCoord, parametricCoord_n);
            return Tuple.Create(tags, coords, parametric);
        }
        /// <inheritdoc/>
        public Tuple<IntPtr[], double[], double[]> GetNodesByElementType(int elementType, int tag = -1, bool returnParametricCoord = true)
        {
            var ierr = 0;
            var nodeTags = IntPtr.Zero;
            var nodeTags_n = IntPtr.Zero;
            var coord = IntPtr.Zero;
            var coord_n = IntPtr.Zero;
            var parametricCoord = IntPtr.Zero;
            var parametricCoord_n = IntPtr.Zero;
            gmshModelMeshGetNodesByElementType(elementType, ref nodeTags, ref nodeTags_n, ref coord, ref coord_n, ref parametricCoord, ref parametricCoord_n,
                                                tag, Convert.ToInt32(returnParametricCoord), ref ierr);
            var tags = GetObjectsData<IntPtr>(nodeTags, nodeTags_n);
            var coords = GetObjectsData<double>(coord, coord_n);
            var parametric = GetObjectsData<double>(parametricCoord, parametricCoord_n);
            return Tuple.Create(tags, coords, parametric);
        }
        /// <inheritdoc/>
        public void AddNodes(int dim, int tag, IntPtr[] nodeTags, double[] coord, double[] parametricCoord = null)
        {
            var ierr = 0;
            parametricCoord = parametricCoord ?? new double[0];
            gmshModelMeshAddNodes(dim, tag, nodeTags, (IntPtr)nodeTags.Length, coord, (IntPtr)coord.Length,
                                  parametricCoord, (IntPtr)parametricCoord.Length, ref ierr);
        }
        /// <inheritdoc/>
        public void ReclassifyNodes()
        {
            var ierr = 0;
            gmshModelMeshReclassifyNodes(ref ierr);
        }
        /// <inheritdoc/>
        public Tuple<int[], IntPtr[][], IntPtr[][]> GetElements(int dim = -1, int tag = -1)
        {
            var ierr = 0;
            var elemTypes = IntPtr.Zero;
            var elemTypes_n = IntPtr.Zero;
            var elemTags = IntPtr.Zero;
            var elemTags_n = IntPtr.Zero;
            var elemTags_nn = IntPtr.Zero;
            var nodeTags = IntPtr.Zero;
            var nodeTags_n = IntPtr.Zero;
            var nodeTags_nn = IntPtr.Zero;
            gmshModelMeshGetElements(ref elemTypes, ref elemTypes_n, ref elemTags,
                                     ref elemTags_n, ref elemTags_nn, ref nodeTags,
                                     ref nodeTags_n, ref nodeTags_nn, dim, tag, ref ierr);
            var eTypes = GetObjectsData<int>(elemTypes, elemTypes_n);
            var eTags = GetArrayOfObjectsData<IntPtr>(elemTags, elemTags_nn, elemTags_n);
            var nTags = GetArrayOfObjectsData<IntPtr>(nodeTags, nodeTags_nn, nodeTags_n);
            return Tuple.Create(eTypes, eTags, nTags);
        }
        /// <inheritdoc/>
        public IntPtr[] GetElementsByCoordinates(double x, double y, double z, int dim = -1, bool strict = false)
        {
            var ierr = 0;
            var elemTags = IntPtr.Zero;
            var elemTags_n = IntPtr.Zero;
            gmshModelMeshGetElementsByCoordinates(x, y, z, ref elemTags, ref elemTags_n, dim, Convert.ToInt32(strict), ref ierr);
            return GetObjectsData<IntPtr>(elemTags, elemTags_n);
        }
        /// <inheritdoc/>
        public int[] GetElementTypes(int dim = -1, int tag = -1)
        {
            var ierr = 0;
            var elemTypes = IntPtr.Zero;
            var elemTypes_n = IntPtr.Zero;
            gmshModelMeshGetElementTypes(ref elemTypes, ref elemTypes_n, dim, tag, ref ierr);
            return GetObjectsData<int>(elemTypes, elemTypes_n);
        }
        /// <inheritdoc/>
        public ElementProperties GetElementProperties(int elementType)
        {
            var ierr = 0;
            var localeNodeCoord = IntPtr.Zero;
            var localeNodeCoord_n = IntPtr.Zero;
            var elementName = IntPtr.Zero;
            var elemProperties = new ElementProperties();
            gmshModelMeshGetElementProperties(elementType, ref elementName, ref elemProperties.dim,
                                              ref elemProperties.order, ref elemProperties.numNodes,
                                              ref localeNodeCoord, ref localeNodeCoord_n,
                                              ref elemProperties.numPrimaryNodes, ref ierr);
            elemProperties.elementName = GetString(elementName);
            elemProperties.coords = GetObjectsData<double>(localeNodeCoord, localeNodeCoord_n);
            elemProperties.coords_n = elemProperties.coords.Length;
            return elemProperties;
        }
        /// <inheritdoc/>
        public double[] GetElementQualities(IntPtr[] elementTags, string qualityName = "minSICN", int task = 0, int numTasks = 1)
        {
            var ierr = 0;
            var elemQuality = IntPtr.Zero;
            var elemQuality_n = IntPtr.Zero;
            gmshModelMeshGetElementQualities(elementTags, (IntPtr)elementTags.Length, ref elemQuality, ref elemQuality_n,
                                             qualityName, (IntPtr)task, (IntPtr)numTasks, ref ierr);
            return GetObjectsData<double>(elemQuality, elemQuality_n);
        }
        /// <inheritdoc/>
        public void AddElementsByType(int tag, int elementType, IntPtr[] elementTags, IntPtr[] nodeTags)
        {
            var ierr = 0;
            gmshModelMeshAddElementsByType(tag, elementType, elementTags, (IntPtr)elementTags.Length, nodeTags, (IntPtr)nodeTags.Length, ref ierr);
        }
        /// <inheritdoc/>
        public void SetSize(int[] dimTags, double size)
        {
            var ierr = 0;
            gmshModelMeshSetSize(dimTags, (IntPtr)dimTags.Length, size, ref ierr);
        }
        /// <inheritdoc/>
        public double[] GetSizes(int[] dimTags)
        {
            var ierr = 0;
            var sizes = IntPtr.Zero;
            var sizes_n = IntPtr.Zero;
            gmshModelMeshGetSizes(dimTags, (IntPtr)dimTags.Length, ref sizes, ref sizes_n, ref ierr);
            return GetObjectsData<double>(sizes, sizes_n);
        }
        /// <inheritdoc/>
        public void SetTransfiniteCurve(int tag, int numNodes, MeshType meshType = MeshType.Progression, double coef = 1)
        {
            var ierr = 0;
            gmshModelMeshSetTransfiniteCurve(tag, numNodes, meshType.ToString(), coef, ref ierr);
        }
        /// <inheritdoc/>
        public void SetTransfiniteSurface(int tag, Arrangement arrangement = Arrangement.Left, int[] cornerTags = null)
        {
            var ierr = 0;
            cornerTags = cornerTags ?? new int[0];
            gmshModelMeshSetTransfiniteSurface(tag, arrangement.ToString(), cornerTags, (IntPtr)cornerTags.Length, ref ierr);
        }
        /// <inheritdoc/>
        public void SetTransfiniteVolume(int tag, int[] cornerTags = null)
        {
            var ierr = 0;
            cornerTags = cornerTags ?? new int[0];
            gmshModelMeshSetTransfiniteVolume(tag, cornerTags, (IntPtr)cornerTags.Length, ref ierr);
        }
        /// <inheritdoc/>
        public void SetTransfiniteAutomatic(int[] dimTags, double cornerAngle = 2.35, bool recombine = true)
        {
            var ierr = 0;
            gmshModelMeshSetTransfiniteAutomatic(dimTags, (IntPtr)dimTags.Length, cornerAngle, Convert.ToInt32(recombine), ref ierr);
        }
        /// <inheritdoc/>
        public void SetAlgorithm(int dim, int tag, MeshAlgorithm val = MeshAlgorithm.Automatic)
        {
            var ierr = 0;
            gmshModelMeshSetAlgorithm(dim, tag, (int)val, ref ierr);
        }
        /// <inheritdoc/>
        public void RemoveConstraints(int[] dimTags = null)
        {
            var ierr = 0;
            dimTags = dimTags ?? new int[0];
            gmshModelMeshRemoveConstraints(dimTags, (IntPtr)dimTags.Length, ref ierr);
        }
        /// <inheritdoc/>
        public void ImportStl()
        {
            var ierr = 0;
            gmshModelMeshImportStl(ref ierr);
        }
        /// <inheritdoc/>
        public void SetVisibility(IntPtr[] elementTags, int value)
        {
            var ierr = 0;
            gmshModelMeshSetVisibility(elementTags, (IntPtr)elementTags.Length, value, ref ierr);
        }
        /// <inheritdoc/>
        public void ClassifySurfaces(double angle, bool boundary = true, bool forReparametrization = false,
                                     double curveAngle = Math.PI, bool exportDiscrete = true)
        {
            var ierr = 0;
            gmshModelMeshClassifySurfaces(angle, Convert.ToInt32(boundary), Convert.ToInt32(forReparametrization), 
                                          curveAngle, Convert.ToInt32(exportDiscrete), ref ierr);
        }
        /// <inheritdoc/>
        public void CreateGeometry(int[] dimTags = null)
        {
            var ierr = 0;
            dimTags = dimTags ?? new int[0];
            gmshModelMeshCreateGeometry(dimTags, (IntPtr)dimTags.Length, ref ierr);
        }
    }
}
