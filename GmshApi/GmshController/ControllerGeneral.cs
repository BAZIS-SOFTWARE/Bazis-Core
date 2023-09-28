using GmshApi.GmshKernel;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Linq;

namespace GmshApi.GmshController
{
    public class ControllerGeneral
    {
        public IntPtr DLL { get; private set; }
        #region "Initialization Delegates"
        public delegate void gmshInitialize(int argc, ref string argv, int readConfigFile, int run, ref int ierr);
        public delegate int gmshIsInitialized(ref int ierr);
        public delegate void gmshFinalize(ref int ierr);
        #endregion
        #region "Memory Delegates"
        private delegate IntPtr gmshMalloc(IntPtr n);
        private delegate void gmshFree(IntPtr p);
        #endregion
        #region "General Delegates"
        public delegate void gmshOptionSetNumber(string name, double value, ref int ierr);
        public delegate void gmshOptionGetNumber(string name, ref double value, ref int ierr);
        public delegate void gmshOptionSetString(string name, string value, ref int ierr);
        private delegate void gmshOptionGetString(string name, ref IntPtr value, ref int ierr);
        #endregion
        #region "Plugin Delegates"
        public delegate void gmshPluginSetNumber(string name, string option, double value, ref int ierr);
        public delegate void gmshPluginSetString(string name, string option, string value, ref int ierr);
        public delegate int gmshPluginRun(string name, ref int ierr);
        #endregion
        #region "IO Delegates"
        public delegate void gmshOpen(string fileName, ref int ierr);
        public delegate void gmshMerge(string fileName, ref int ierr);
        public delegate void gmshWrite(string fileName, ref int ierr);
        #endregion
        #region "Model Delegates"
        public delegate void gmshModelSetFileName(string fileName, ref int ierr);
        private delegate string gmshModelGetFileName(ref IntPtr fileName, ref int ierr);
        public delegate void gmshModelAdd(string name, ref int ierr);
        public delegate void gmshModelRemove(ref int ierr);
        private delegate void gmshModelList(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        public delegate void gmshClear(ref int ierr);
        public delegate void gmshModelSetCurrent(string name, ref int ierr);
        private delegate void gmshModelGetCurrent(ref IntPtr name, ref int ierr);
        public delegate void gmshModelOccSynchronize(ref int ierr);
        public delegate void gmshModelGeoSynchronize(ref int ierr);
        private delegate void gmshModelOccImportShapes(string fileName, ref IntPtr outDimTags,
                                                          ref IntPtr outDimTags_n, int highestDimOnly,
                                                          string format, ref int ierr);
        #endregion
        #region "Geometry Delegates"
        private delegate void gmshModelGetEntities(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        private delegate void gmshModelOccGetEntities(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        public delegate void gmshModelRemoveEntities(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        public delegate void gmshModelOccRemove(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        public delegate void gmshModelSetEntityName(int dim, int tag, string name, ref int ierr);
        private delegate void gmshModelGetEntityName(int dim, int tag, ref IntPtr name, ref int ierr);
        public delegate void gmshModelRemoveEntityNamee(string name, ref int ierr);
        public delegate void gmshModelGetBoundingBox(int dim, int tag, ref double xmin,
                                                          ref double ymin, ref double zmin,
                                                          ref double xmax, ref double ymax,
                                                          ref double zmax, ref int ierr);
        private delegate void gmshModelGetAdjacencies(int dim, int tag, ref IntPtr upward, ref IntPtr upward_n,
                                                         ref IntPtr downward, ref IntPtr downward_n, ref int ierr);
        private delegate void gmshModelGetBoundary(int[] dimTags, IntPtr dimTags_n, ref IntPtr outDimTags,
                                                       ref IntPtr outDimTags_n, int combined, int oriented,
                                                       int recursive, ref int ierr);
        private delegate void gmshModelGetType(int dim, int tag, ref IntPtr entityType, ref int ierr);
        public delegate void gmshModelGetDimension(ref int ierr);
        public delegate void gmshModelIsInside(int dim, int tag, double[] coord, IntPtr coord_n,
                                                   int parametric, ref int ierr);
        public delegate void gmshModelSetVisibility(int[] dimTags, IntPtr dimTags_n, int value,
                                                         int recursive, ref int ierr);
        public delegate void gmshModelSetAttribute(string name, string[] values, IntPtr values_n, ref int ierr);
        public delegate void gmshModelGetAttribute(string name, ref IntPtr values, ref IntPtr values_n, ref int ierr);
        private delegate void gmshModelGetAttributeNames(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        public delegate void gmshModelRemoveAttribute(string name, ref int ierr);
        private delegate void gmshModelGetPhysicalName(int dim, int tag, ref IntPtr name, ref int ierr);
        private delegate void gmshModelGetDerivative(int dim, int tag, double[] parametricCoord,
                                                        IntPtr parametricCoord_n, ref IntPtr derivatives,
                                                        ref IntPtr derivatives_n, ref int ierr);
        private delegate void gmshModelGetSecondDerivative(int dim, int tag, double[] parametricCoord,
                                                              IntPtr parametricCoord_n, ref IntPtr derivatives,
                                                              ref IntPtr derivatives_n, ref int ierr);
        private delegate void gmshModelGetCurvature(int dim, int tag, double[] parametricCoord,
                                                       IntPtr parametricCoord_n, ref IntPtr curvatures,
                                                       ref IntPtr curvatures_n, ref int ierr);
        private delegate void gmshModelGetValue(int dim, int tag, double[] parametricCoord,
                                                   IntPtr parameticCoord_n, ref IntPtr coord,
                                                   ref IntPtr coord_n, ref int ierr);
        private delegate void gmshModelGetParametrization(int dim, int tag, double[] coord, IntPtr coord_n,
                                                                 ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                                 ref int ierr);
        private delegate void gmshModelGetParametrizationBounds(int dim, int tag, ref IntPtr min,
                                                                   ref IntPtr min_n, ref IntPtr max,
                                                                   ref IntPtr max_n, ref int ierr);
        private delegate void gmshModelReparametrizeOnSurface(int dim, int tag, double[] parametricCoord,
                                                                 IntPtr parametricCoord_n, int surfaceTag,
                                                                 ref IntPtr surfaceParametricCoord,
                                                                 ref IntPtr surfaceParametricCoord_n,
                                                                 int which, ref int ierr);
        private delegate void gmshModelOccGetCurveLoops(int surfaceTag, ref IntPtr curveLoopTags,
                                                            ref IntPtr curveLoopTags_n, ref IntPtr curveTags,
                                                            ref IntPtr curveTags_n, ref IntPtr curveTags_nn,
                                                            ref int ierr);
        private delegate void gmshModelOccGetSurfaceLoops(int volumeTag, ref IntPtr surfaceLoopTags,
                                                            ref IntPtr surfaceLoopTags_n, ref IntPtr surfaceTags,
                                                            ref IntPtr surfaceTags_n, ref IntPtr surfaceTags_nn,
                                                            ref int ierr);
        #endregion
        #region "Mesh Delegates"
        public delegate void gmshModelMeshGenerateDelegate(int dim, ref int ierr);
        public delegate void gmshModelMeshRecombineDelegate(ref int ierr);
        public delegate void gmshModelMeshRefineDelegate(ref int ierr);
        public delegate void gmshModelMeshGetElementsDelegate(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                  ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                  ref IntPtr elementTags_nn, ref IntPtr nodeTags,
                                                  ref IntPtr nodeTags_n, ref IntPtr nodeTags_nn,
                                                  int dim, int tag, ref int ierr);
        public delegate void gmshModelMeshGetElementTypesDelegate(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                              int dim, int tag, ref int ierr);
        public delegate void gmshModelMeshGetElementPropertiesDelegate(int elementType, ref IntPtr elementName, ref int dim,
                                                           ref int order, ref int numNodes, ref IntPtr localeNodeCoord,
                                                           ref IntPtr localeNodeCoord_n, ref int numPrimaryNodes,
                                                           ref int ierr);
        public delegate void gmshModelMeshGetNodesDelegate(ref IntPtr nodeTags, ref IntPtr nodeTags_n,
                                                       ref IntPtr coord, ref IntPtr coord_n,
                                                       ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                       int dim, int tag, int includeBoundary,
                                                       int returnParametricCoord, ref int ierr);
        public delegate void gmshModelMeshGetNodesByElementTypeDelegate(int elementType, ref IntPtr nodeTags,
                                                                    ref IntPtr nodeTags_n, ref IntPtr coord,
                                                                    ref IntPtr coord_n, ref IntPtr parametricCoord,
                                                                    ref IntPtr parametricCoord_n, int tag,
                                                                    int returnParametricCoord, ref int ierr);
        public delegate void gmshModelMeshSetTransfiniteCurveDelegate(int tag, int numNodes, string meshType,
                                                                  double coef, ref int ierr);
        public delegate void gmshModelMeshGetElementsByCoordinatesDelegate(double x, double y, double z,
                                                                       ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                                       int dim, int strict, ref int ierr);
        public delegate void gmshModelMeshGetLocalCoordinatesInElementDelegate(UIntPtr elementTag, double x, double y, double z,
                                                                           ref double u, ref double v, ref double w,
                                                                           ref int ierr);
        public delegate void gmshModelMeshRebuildNodeCacheDelegate(int onlyIfNecessary, ref int ierr);
        public delegate void gmshModelMeshRebuildElementCacheDelegate(int onlyIfNecessary, ref int ierr);
        public delegate void gmshModelMeshClearDelegate(int[] dimTags, IntPtr dimTags_n, ref int ierr);
        public delegate void gmshModelMeshSetVisibilityDelegate(IntPtr[] elementTags, IntPtr elementTags_n,
                                                            int value, ref int ierr);
        public delegate void gmshModelMeshSetAlgorithmDelegate(int dim, int tag, int val, ref int ierr);
        public delegate void gmshModelMeshGetElementQualitiesDelegate(IntPtr[] elementTags, IntPtr elementTags_n,
                                                                  ref IntPtr elementsQuality, ref IntPtr elementQuality_n,
                                                                  string qualityName, IntPtr task,
                                                                  IntPtr numTasks, ref int ierr);
        #endregion
        #region "Mesh Field Delegates"
        public delegate int gmshModelMeshFieldAdd(string fieldType, int tag, ref int ierr);
        public delegate void gmshModelMeshFieldRemove(int tag, ref int ierr);
        public delegate void gmshModelMeshFieldList(ref IntPtr tags, ref IntPtr tags_n, ref int ierr);
        public delegate void gmshModelMeshFieldGetType(int tag, ref IntPtr fieldType, ref int ierr);
        public delegate void gmshModelMeshFieldSetNumber(int tag, string option, double value, ref int ierr);
        public delegate void gmshModelMeshFieldGetNumber(int tag, string option, ref double value, ref int ierr);
        public delegate void gmshModelMeshFieldSetString(int tag, string option, string value, ref int ierr);
        public delegate void gmshModelMeshFieldGetString(int tag, string option, ref IntPtr value, ref int ierr);
        public delegate void gmshModelMeshFieldSetNumbers(int tag, string option, double[] values,
                                                              IntPtr values_n, ref int ierr);
        public delegate void gmshModelMeshFieldGetNumbers(int tag, string option, ref IntPtr values,
                                                              ref IntPtr values_n, ref int ierr);
        public delegate void gmshModelMeshFieldSetAsBackgroundMesh(int tag, ref int ierr);
        public delegate void gmshModelMeshFieldSetAsBoundaryLayer(int tag, ref int ierr);
        #endregion
        public ControllerGeneral(string dllToLoad)
        {
            DLL = NativeMethods.LoadLibrary(dllToLoad);
            var argv = string.Empty;
            var ierr = 0;
            GetFunction<gmshInitialize>("gmshInitialize")(0, ref argv, 0, 0, ref ierr);
        }

        ~ControllerGeneral()
        {
            var ierr = 0;
            GetFunction<gmshFinalize>("gmshFinalize")(ref ierr);
            NativeMethods.FreeLibrary(DLL);
        }

        public T GetFunction<T>(string functionName)
        {
            var funPtr = NativeMethods.GetProcAddress(DLL, functionName);
            return Marshal.GetDelegateForFunctionPointer<T>(funPtr);
        }

        public bool OptionGetString(string name, out string value)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            value = null;
            GetFunction<gmshOptionGetString>("gmshOptionGetString")(name, ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
                value = GetString(unmanagedString);
            return status;
        }

        public bool ModelGetFileName(out string fileName)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            fileName = null;
            GetFunction<gmshModelGetFileName>("gmshModelGetFileName")(ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
                fileName = GetString(unmanagedString);
            return status;
        }

        public bool ModelList(out string[] modelList)
        {
            IntPtr names = IntPtr.Zero;
            IntPtr names_n = IntPtr.Zero;
            var ierr = 0;
            modelList = null;
            GetFunction<gmshModelList>("gmshModelList")(ref names, ref names_n, ref ierr);
            var status = ierr == 0;
            if (status)
                modelList = GetArrayOfString(names, names_n);
            return status;
        }

        public bool ModelGetCurrent(out string current)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            current = null;
            GetFunction<gmshModelGetCurrent>("gmshOptionGetString")(ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
                current = GetString(unmanagedString);
            return status;
        }

        public bool ModelGetEntities(out int[] dimTags, int dim)
        {
            var ierr = 0;
            IntPtr dTags, dimTags_n;
            dTags = dimTags_n = IntPtr.Zero;
            dimTags = null;
            var extension = ControllerIO.ModelGetFileNameExtension();
            if (extension.Contains("stp") || extension.Contains("step") ||
                extension.Contains("brep") || extension.Contains("igs"))
                GetFunction<gmshModelOccGetEntities>("gmshModelOccGetEntities")(ref dTags, ref dimTags_n, dim, ref ierr);
            else
                GetFunction<gmshModelGetEntities>("gmshModelGetEntities")(ref dTags, ref dimTags_n, dim, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(dTags, out dimTags, dimTags_n);
            return status;
        }

        public bool ModelGetEntityName(int dim, int tag, out string name)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            name = null;
            GetFunction<gmshModelGetEntityName>("gmshModelGetEntityName")(dim, tag, ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
                name = GetString(unmanagedString);
            return status;
        }

        public bool ModelGetAdjacencies(int dim, int tag, out int[] upward, out int[] downward)
        {
            var ierr = 0;
            IntPtr upw, upw_n, downw, downw_n;
            upw = upw_n = downw = downw_n = IntPtr.Zero;
            upward = null; 
            downward = null;
            var delegateCall = GetFunction<gmshModelGetAdjacencies>("gmshModelGetAdjacencies");
            delegateCall(dim, tag, ref upw, ref upw_n, ref downw, ref downw_n, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(downw, out downward, downw_n);
                GetObjectsData(upw, out upward, upw_n);
            }
            return status;
        }

        public bool ModelGetAttribute(string name, out string[] attributes)
        {
            var ierr = 0;
            IntPtr values = IntPtr.Zero, values_n = IntPtr.Zero;
            attributes = null;
            var delegateCall = GetFunction<gmshModelGetAttribute>("gmshModelGetAttribute");
            delegateCall(name, ref values, ref values_n, ref ierr);
            var status = ierr == 0;
            if (status)
                attributes = GetArrayOfString(values, values_n);
            return status;
        }

        public bool ModelGetAttributeNames(out string[] names)
        {
            var ierr = 0;
            IntPtr nms = IntPtr.Zero, nms_n = IntPtr.Zero;
            names = null;
            var delegateCall = GetFunction<gmshModelGetAttributeNames>("gmshModelGetAttributeNames");
            delegateCall(ref nms, ref nms_n, ref ierr);
            var status = ierr == 0;
            if (status)
                names = GetArrayOfString(nms, nms_n);
            return status;
        }

        public bool ModelGetDerivative(int dim, int tag, double[] parametricCoord, out double[] derivative)
        {
            var ierr = 0;
            var der = IntPtr.Zero;
            var der_n = IntPtr.Zero;
            derivative = null;
            var delegateCall = GetFunction<gmshModelGetDerivative>("gmshModelGetDerivative");
            delegateCall(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length, ref der, ref der_n, ref ierr);
            var status = ierr == 0; 
            if(status)
                GetObjectsData(der, out derivative, der_n);
            return status;
        }

        public bool ModelGetSecondDerivative(int dim, int tag, double[] parametricCoord, out double[] derivative)
        {
            var ierr = 0;
            var der = IntPtr.Zero;
            var der_n = IntPtr.Zero;
            derivative = null;
            var delegateCall = GetFunction<gmshModelGetSecondDerivative>("gmshModelGetSecondDerivative");
            delegateCall(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length, ref der, ref der_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(der, out derivative, der_n);
            return status;
        }

        public bool ModelGetValue(int dim, int tag, double[] parametricCoord, out double[] coord)
        {
            var ierr = 0;
            IntPtr locCoord, locCoord_n;
            locCoord = locCoord_n = IntPtr.Zero;
            coord = null;
            var delegateCall = GetFunction<gmshModelGetValue>("gmshModelGetValue");
            delegateCall(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length, ref locCoord, ref locCoord_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(locCoord, out coord, locCoord_n);
            return status;
        }

        public bool ModelGetParametrization(int dim, int tag, double[] coord, out double[] parametricCoord)
        {
            var ierr = 0;
            IntPtr pCoord, pCoord_n;
            pCoord = pCoord_n = IntPtr.Zero;
            parametricCoord = null;
            var delegateCall = GetFunction<gmshModelGetParametrization>("gmshModelGetParametrization");
            delegateCall(dim, tag, coord, (IntPtr)coord.Length, ref pCoord, ref pCoord_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(pCoord, out parametricCoord, pCoord_n);
            return status;
        }

        public bool ModelGetParametrizationBounds(int dim, int tag, out double[] min, out double[] max)
        {
            var ierr = 0;
            IntPtr minBounds, minBounds_n;
            IntPtr maxBounds, maxBounds_n;
            minBounds = minBounds_n = maxBounds = maxBounds_n = IntPtr.Zero;
            min = null;
            max = null;
            var delegateCall = GetFunction<gmshModelGetParametrizationBounds>("gmshModelGetParametrizationBounds");
            delegateCall(dim, tag, ref minBounds, ref minBounds_n, ref maxBounds, ref maxBounds_n, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(minBounds, out min, minBounds_n);
                GetObjectsData(maxBounds, out max, maxBounds_n);
            }
            return status;
        }

        public bool ModelMeshFieldGetType(int tag, out string fieldType)
        {
            IntPtr fType = IntPtr.Zero;
            var ierr = 0;
            fieldType = null;
            GetFunction<gmshModelMeshFieldGetType>("gmshModelMeshFieldGetType")(tag, ref fType, ref ierr);
            var status = ierr == 0;
            if (status)
                fieldType = GetString(fType);
            return status;
        }

        private void GetObjectsData<T>(IntPtr src, out T[] dst, IntPtr eCount)
        {
            var count = eCount.ToInt32();
            T value = default;
            var size = Marshal.SizeOf(value) * count;
            var buffer = new byte[size];
            dst = new T[count];
            Marshal.Copy(src, buffer, 0, size);
            Buffer.BlockCopy(buffer, 0, dst, 0, size);
            GetFunction<gmshFree>("gmshFree")(src);
        }

        private string GetString(IntPtr name)
        {
            var result = Marshal.PtrToStringAnsi(name);
            GetFunction<gmshFree>("gmshFree")(name);
            return result;
        }

        private string[] GetArrayOfString(IntPtr array, IntPtr length)
        {
            var arrayLength = length.ToInt32();
            var stringArray = new IntPtr[arrayLength];
            var result = new string[arrayLength];
            Marshal.Copy(array, stringArray, 0, arrayLength);
            for (var i = 0; i < arrayLength; ++i)
            {
                result[i] = Marshal.PtrToStringAnsi(stringArray[i]);
                GetFunction<gmshFree>("gmshFree")(stringArray[i]);
            }
            GetFunction<gmshFree>("gmshFree")(array);
            return result;
        }
    }
}
