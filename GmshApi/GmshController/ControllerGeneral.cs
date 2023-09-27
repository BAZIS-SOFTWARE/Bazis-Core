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

namespace GmshApi.GmshController
{
    public class ControllerGeneral
    {
        private IntPtr DLL { get; set; }
/*INITIALIZATION*/
        public delegate void InitializeDelegate(int argc, ref string argv, int readConfigFile, int run, ref int ierr);
        public InitializeDelegate gmshInitialize;
        public delegate int IsInitializedDelegate(ref int ierr);
        public IsInitializedDelegate gmshIsInitialized;
        public delegate void FinalizeDelegate(ref int ierr);
        public FinalizeDelegate gmshFinalize;
/*MEMORY*/
        public delegate IntPtr MallocDelegate(IntPtr n);
        public MallocDelegate gmshMalloc;
        public delegate void FreeDelegate(IntPtr p);
        public FreeDelegate gmshFree;
/*GENERAL*/
        public delegate void OptionSetNumberDelegate(string name, double value, ref int ierr);
        public OptionSetNumberDelegate gmshOptionSetNumber;
        public delegate void OptionGetNumberDelegate(string name, ref double value, ref int ierr);
        public OptionGetNumberDelegate gmshOptionGetNumber;
        public delegate void OptionSetStringDelegate(string name, string value, ref int ierr);
        public OptionSetStringDelegate gmshOptionSetString;
        public delegate void OptionGetStringDelegate(string name, ref IntPtr value, ref int ierr);
        public OptionGetStringDelegate gmshOptionGetString;
/*PLUGIN*/
        public delegate void PluginSetNumberDelegate(string name, string option, double value, ref int ierr);
        public PluginSetNumberDelegate gmshPluginSetNumber;
        public delegate void PluginSetStringDelegate(string name, string option, string value, ref int ierr);
        public PluginSetStringDelegate gmshPluginSetString;
        public delegate int PluginRunDelegate(string name, ref int ierr);
        public PluginRunDelegate gmshPluginRun;
/*IO*/
        public delegate void OpenDelegate(string fileName, ref int ierr);
        public OpenDelegate gmshOpen;
        public delegate void MergeDelegate(string fileName, ref int ierr);
        public MergeDelegate gmshMerge;
        public delegate void WriteDelegate(string fileName, ref int ierr);
        public WriteDelegate gmshWrite;
/*MODEL*/
        public delegate void ModelSetFileNameDelegate(string fileName, ref int ierr);
        public ModelSetFileNameDelegate gmshModelSetFileName;
        public delegate string ModelGetFileNameDelegate(ref IntPtr fileName, ref int ierr);
        public ModelGetFileNameDelegate gmshModelGetFileName;
        public delegate void ModelAddDelegate(string name, ref int ierr);
        public ModelAddDelegate gmshModelAdd;
        public delegate void ModelRemoveDelegate(ref int ierr);
        public ModelRemoveDelegate gmshModelRemove;
        public delegate void ModelListDelegate(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        public ModelListDelegate gmshModelList;
        public delegate void ClearDelegate(ref int ierr);
        public ClearDelegate gmshClear;
        public delegate void ModelSetCurrentDelegate(string name, ref int ierr);
        public ModelSetCurrentDelegate gmshModelSetCurrent;
        public delegate void ModelGetCurrentDelegate(ref IntPtr name, ref int ierr);
        public ModelGetCurrentDelegate gmshModelGetCurrent;
        public delegate void ModelOccSynchronizeDelegate(ref int ierr);
        public ModelOccSynchronizeDelegate gmshModelOccSynchronize;
        public delegate void ModelGeoSynchronizeDelegate(ref int ierr);
        public ModelGeoSynchronizeDelegate gmshModelGeoSynchronize;
        public delegate void ModelOccImportShapesDelegate(string fileName, ref IntPtr outDimTags,
                                                          ref IntPtr outDimTags_n, int highestDimOnly,
                                                          string format, ref int ierr);
        public ModelOccImportShapesDelegate gmshModelOccImportShapes;
/*GEOMETRY*/
        public delegate void ModelGetEntitiesDelegate(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        public ModelGetEntitiesDelegate gmshModelGetEntities;
        public delegate void ModelOccGetEntitiesDelegate(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        public ModelOccGetEntitiesDelegate gmshModelOccGetEntities;
        public delegate void ModelRemoveEntitiesDelegate(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        public ModelRemoveEntitiesDelegate gmshModelRemoveEntities;
        public delegate void ModelOccRemoveDelegate(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        public ModelOccRemoveDelegate gmshModelOccRemove;
        public delegate void ModelSetEntityNameDelegate(int dim, int tag, string name, ref int ierr);
        public ModelSetEntityNameDelegate gmshModelSetEntityName;
        public delegate void ModelGetEntityNameDelegate(int dim, int tag, ref IntPtr name, ref int ierr);
        public ModelGetEntityNameDelegate gmshModelGetEntityName;
        public delegate void ModelRemoveEntityNameDelegate(string name, ref int ierr);
        public ModelRemoveEntityNameDelegate gmshModelRemoveEntityName;
        public delegate void ModelGetBoundingBoxDelegate(int dim, int tag, ref double xmin,
                                                          ref double ymin, ref double zmin,
                                                          ref double xmax, ref double ymax,
                                                          ref double zmax, ref int ierr);
        public ModelGetBoundingBoxDelegate gmshModelGetBoundingBox;
        public delegate void ModelGetAdjacenciesDelegate(int dim, int tag, ref IntPtr upward, ref IntPtr upward_n,
                                                         ref IntPtr downward, ref IntPtr downward_n, ref int ierr);
        public ModelGetAdjacenciesDelegate gmshModelGetAdjacencies;
        public delegate void ModelGetBoundaryDelegate(int[] dimTags, IntPtr dimTags_n, ref IntPtr outDimTags,
                                                       ref IntPtr outDimTags_n, int combined, int oriented,
                                                       int recursive, ref int ierr);
        public ModelGetBoundaryDelegate gmshModelGetBoundary;
        public delegate void ModelGetTypeDelegate(int dim, int tag, ref IntPtr entityType, ref int ierr);
        public ModelGetTypeDelegate gmshModelGetType;
        public delegate void ModelGetDimensionDelegate(ref int ierr);
        public ModelGetDimensionDelegate gmshModelGetDimension;
        public delegate void ModelIsInsideDelegate(int dim, int tag, double[] coord, IntPtr coord_n,
                                                   int parametric, ref int ierr);
        public ModelIsInsideDelegate gmshModelIsInside;
        public delegate void ModelSetVisibilityDelegate(int[] dimTags, IntPtr dimTags_n, int value,
                                                         int recursive, ref int ierr);
        public ModelSetVisibilityDelegate gmshModelSetVisibility;
        public delegate void ModelSetAttributeDelegate(string name, string[] values, IntPtr values_n, ref int ierr);
        public ModelSetAttributeDelegate gmshModelSetAttribute;
        public delegate void ModelGetAttributeDelegate(string name, ref IntPtr values, ref IntPtr values_n, ref int ierr);
        public ModelGetAttributeDelegate gmshModelGetAttribute;
        public delegate void ModelGetAttributeNamesDelegate(ref IntPtr names, ref IntPtr names_n, ref int ierr);
        public ModelGetAttributeNamesDelegate gmshModelGetAttributeNames;
        public delegate void ModelRemoveAttributeDelegate(string name, ref int ierr);
        public ModelRemoveAttributeDelegate gmshModelRemoveAttribute;
        public delegate void ModelGetPhysicalNameDelegate(int dim, int tag, ref IntPtr name, ref int ierr);
        public ModelGetPhysicalNameDelegate gmshModelGetPhysicalName;
        public delegate void ModelGetDerivativeDelegate(int dim, int tag, double[] parametricCoord, 
                                                        IntPtr parametricCoord_n, ref IntPtr derivatives, 
                                                        ref IntPtr derivatives_n, ref int ierr);
        public ModelGetDerivativeDelegate gmshModelGetDerivative;
        public delegate void ModelGetSecondDerivativeDelegate(int dim, int tag, double[] parametricCoord, 
                                                              IntPtr parametricCoord_n, ref IntPtr derivatives, 
                                                              ref IntPtr derivatives_n, ref int ierr);
        public ModelGetSecondDerivativeDelegate gmshModelGetSecondDerivative;
        public delegate void ModelGetCurvatureDelegate(int dim, int tag, double[] parametricCoord, 
                                                       IntPtr parametricCoord_n, ref IntPtr curvatures, 
                                                       ref IntPtr curvatures_n, ref int ierr);
        public ModelGetCurvatureDelegate gmshModelGetCurvature;
        public delegate void ModelGetValueDelegate(int dim, int tag, double[] parametricCoord,
                                                   IntPtr parameticCoord_n, ref IntPtr coord,
                                                   ref IntPtr coord_n, ref int ierr);
        public ModelGetValueDelegate gmshModelGetValue;
        public delegate void ModelGetParametrizationDelegate(int dim, int tag, double[] coord, IntPtr coord_n,
                                                                 ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                                 ref int ierr);
        public ModelGetParametrizationDelegate gmshModelGetParametrization;
        public delegate void ModelGetParametrizationBoundsDelegate(int dim, int tag, ref IntPtr min,
                                                                   ref IntPtr min_n, ref IntPtr max,
                                                                   ref IntPtr max_n, ref int ierr);
        public ModelGetParametrizationBoundsDelegate gmshModelGetParametrizationBounds;
        public delegate void ModelReparametrizeOnSurfaceDelegate(int dim, int tag, double[] parametricCoord,
                                                                 IntPtr parametricCoord_n, int surfaceTag,
                                                                 ref IntPtr surfaceParametricCoord,
                                                                 ref IntPtr surfaceParametricCoord_n,
                                                                 int which, ref int ierr);
        public ModelReparametrizeOnSurfaceDelegate gmshModelReparametrizeOnSurface;
        public delegate void ModelOccGetCurveLoopsDelegate(int surfaceTag, ref IntPtr curveLoopTags,
                                                            ref IntPtr curveLoopTags_n, ref IntPtr curveTags,
                                                            ref IntPtr curveTags_n, ref IntPtr curveTags_nn,
                                                            ref int ierr);
        public ModelOccGetCurveLoopsDelegate gmshModelOccGetCurveLoops;
        public delegate void ModelOccGetSurfaceLoopsDelegate(int volumeTag, ref IntPtr surfaceLoopTags,
                                                            ref IntPtr surfaceLoopTags_n, ref IntPtr surfaceTags,
                                                            ref IntPtr surfaceTags_n, ref IntPtr surfaceTags_nn,
                                                            ref int ierr);
        public ModelOccGetSurfaceLoopsDelegate gmshModelOccGetSurfaceLoops;

        public ControllerGeneral(string dllToLoad)
        {
                DLL = NativeMethods.LoadLibrary(dllToLoad);
            /*Initialization functions*/
                gmshInitialize = GetFunction<InitializeDelegate>("gmshInitialize");
                gmshIsInitialized = GetFunction<IsInitializedDelegate>("gmshIsInitialized");
                gmshFinalize = GetFunction<FinalizeDelegate>("gmshFinalize");
            /*Memory functions*/
                gmshMalloc = GetFunction<MallocDelegate>("gmshMalloc");
                gmshFree = GetFunction<FreeDelegate>("gmshFree");
            /*General functions*/
                gmshOptionSetNumber = GetFunction<OptionSetNumberDelegate>("gmshOptionSetNumber");
                gmshOptionGetNumber = GetFunction<OptionGetNumberDelegate>("gmshOptionGetNumber");
                gmshOptionSetString = GetFunction<OptionSetStringDelegate>("gmshOptionSetString");
                gmshOptionGetString = GetFunction<OptionGetStringDelegate>("gmshOptionGetString");
            /*Plugin functions*/
                gmshPluginSetNumber = GetFunction<PluginSetNumberDelegate>("gmshPluginSetNumber");
                gmshPluginSetString = GetFunction<PluginSetStringDelegate>("gmshPluginSetString");
                gmshPluginRun = GetFunction<PluginRunDelegate>("gmshPluginRun");
            /*IO functions*/
                gmshOpen = GetFunction<OpenDelegate>("gmshOpen");
                gmshMerge = GetFunction<MergeDelegate>("gmshMerge");
                gmshWrite = GetFunction<WriteDelegate>("gmshWrite");
            /*Model functions*/
                gmshModelSetFileName = GetFunction<ModelSetFileNameDelegate>("gmshModelSetFileName");
                gmshModelGetFileName = GetFunction<ModelGetFileNameDelegate>("gmshModelGetFileName");
                gmshModelAdd = GetFunction<ModelAddDelegate>("gmshModelAdd");
                gmshModelRemove = GetFunction<ModelRemoveDelegate>("gmshModelRemove");
                gmshModelList = GetFunction<ModelListDelegate>("gmshModelList");
                gmshClear = GetFunction<ClearDelegate>("gmshClear");
                gmshModelSetCurrent = GetFunction<ModelSetCurrentDelegate>("gmshModelSetCurrent");
                gmshModelGetCurrent = GetFunction<ModelGetCurrentDelegate>("gmshModelGetCurrent");
                gmshModelOccSynchronize = GetFunction<ModelOccSynchronizeDelegate>("gmshModelOccSynchronize");
                gmshModelGeoSynchronize = GetFunction<ModelGeoSynchronizeDelegate>("gmshModelGeoSynchronize");
                gmshModelOccImportShapes = GetFunction<ModelOccImportShapesDelegate>("gmshModelOccImportShapes");
            /*Geometry functions*/
                gmshModelGetEntities = GetFunction<ModelGetEntitiesDelegate>("gmshModelGetEntities");
                gmshModelOccGetEntities = GetFunction<ModelOccGetEntitiesDelegate>("gmshModelOccGetEntities");
                gmshModelRemoveEntities = GetFunction<ModelRemoveEntitiesDelegate>("gmshModelRemoveEntities");
                gmshModelOccRemove = GetFunction<ModelOccRemoveDelegate>("gmshModelOccRemove");
                gmshModelSetEntityName = GetFunction<ModelSetEntityNameDelegate>("gmshModelSetEntityName");
                gmshModelGetEntityName = GetFunction<ModelGetEntityNameDelegate>("gmshModelGetEntityName");
                gmshModelRemoveEntityName = GetFunction<ModelRemoveEntityNameDelegate>("gmshModelRemoveEntityName");
                gmshModelGetBoundingBox = GetFunction<ModelGetBoundingBoxDelegate>("gmshModelGetBoundingBox");
                gmshModelGetAdjacencies = GetFunction<ModelGetAdjacenciesDelegate>("gmshModelGetAdjacencies");
                gmshModelGetBoundary = GetFunction<ModelGetBoundaryDelegate>("gmshModelGetBoundary");
                gmshModelGetType = GetFunction<ModelGetTypeDelegate>("gmshModelGetType");
                gmshModelGetDimension = GetFunction<ModelGetDimensionDelegate>("gmshModelGetDimension");
                gmshModelIsInside = GetFunction<ModelIsInsideDelegate>("gmshModelIsInside");
                gmshModelSetVisibility = GetFunction<ModelSetVisibilityDelegate>("gmshModelSetVisibility");
                gmshModelSetAttribute = GetFunction<ModelSetAttributeDelegate>("gmshModelSetAttribute");
                gmshModelGetAttribute = GetFunction<ModelGetAttributeDelegate>("gmshModelGetAttribute");
                gmshModelGetAttributeNames = GetFunction<ModelGetAttributeNamesDelegate>("gmshModelGetAttributeNames");
                gmshModelRemoveAttribute = GetFunction<ModelRemoveAttributeDelegate>("gmshModelRemoveAttribute");
                gmshModelGetPhysicalName = GetFunction<ModelGetPhysicalNameDelegate>("gmshModelGetPhysicalName");
                gmshModelGetDerivative = GetFunction<ModelGetDerivativeDelegate>("gmshModelGetDerivative");
                gmshModelGetSecondDerivative = GetFunction<ModelGetSecondDerivativeDelegate>("gmshModelGetSecondDerivative");
                gmshModelGetCurvature = GetFunction<ModelGetCurvatureDelegate>("gmshModelGetCurvature");
                gmshModelGetValue = GetFunction<ModelGetValueDelegate>("gmshModelGetValue");
                gmshModelGetParametrization = GetFunction<ModelGetParametrizationDelegate>("gmshModelGetParametrization");
                gmshModelGetParametrizationBounds = GetFunction<ModelGetParametrizationBoundsDelegate>("gmshModelGetParametrizationBounds");
                gmshModelReparametrizeOnSurface = GetFunction<ModelReparametrizeOnSurfaceDelegate>("gmshModelReparametrizeOnSurface");
                gmshModelOccGetCurveLoops = GetFunction<ModelOccGetCurveLoopsDelegate>("gmshModelOccGetCurveLoops");
                gmshModelOccGetSurfaceLoops = GetFunction<ModelOccGetSurfaceLoopsDelegate>("gmshModelOccGetSurfaceLoops");
        }
        ~ControllerGeneral()
        {
            NativeMethods.FreeLibrary(DLL);
        }

        public T GetFunction<T>(string functionName)
        {
            var funPtr = NativeMethods.GetProcAddress(DLL, functionName);
            return Marshal.GetDelegateForFunctionPointer<T>(funPtr);
        }
    }
}
