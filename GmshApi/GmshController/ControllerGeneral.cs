using GmshApi.GmshKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Geometry;
using Model;
using Model.Interfaces;
using Functions.Extensions;
using System.ComponentModel;

namespace GmshApi.GmshController
{
    /// <summary>
    /// Класс содержит информацию о типе извлечнного элемента
    /// </summary>
    public class ElementProperties
    {
        /// <summary>Имя элемента</summary>
        public string elementName;
        /// <summary>Размерность</summary>
        public int dim;
        /// <summary>Порядок</summary>
        public int order;
        /// <summary>Количество узлов</summary>
        public int numNodes;
        /// <summary>Массив координат</summary>
        public double[] coords;
        /// <summary>Количество координат</summary>
        public int coords_n;
        /// <summary>Главные узлы</summary>
        public int numPrimaryNodes;
    }

    public class ControllerGeneral
    {
        public IntPtr DLL { get; private set; }
        #region "Initialization Delegates"
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshInitializeFun(int argc, ref string argv, int readConfigFile, int run, ref int ierr);
            public gmshInitializeFun gmshInitialize;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int gmshIsInitializedFun(ref int ierr);
            public gmshIsInitializedFun gmshIsInitialized;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshFinalizeFun(ref int ierr);
            public gmshFinalizeFun gmshFinalize;
        #endregion
        #region "Memory Delegates"
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr gmshMallocFun(IntPtr n);
            private gmshMallocFun gmshMalloc;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshFreeFun(IntPtr p);
            private gmshFreeFun gmshFree;
        #endregion
        #region "General Delegates"
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshOptionSetNumberFun(string name, double value, ref int ierr);
            public gmshOptionSetNumberFun gmshOptionSetNumber;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshOptionGetNumberFun(string name, ref double value, ref int ierr);
            public gmshOptionGetNumberFun gmshOptionGetNumber;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshOptionSetStringFun(string name, string value, ref int ierr);
            public gmshOptionSetStringFun gmshOptionSetString;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshOptionGetStringFun(string name, ref IntPtr value, ref int ierr);
            private gmshOptionGetStringFun gmshOptionGetString;
        #endregion
        #region "Plugin Delegates"
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshPluginSetNumberFun(string name, string option, double value, ref int ierr);
            public gmshPluginSetNumberFun gmshPluginSetNumber;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshPluginSetStringFun(string name, string option, string value, ref int ierr);
            public gmshPluginSetStringFun gmshPluginSetString;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int gmshPluginRunFun(string name, ref int ierr);
            public gmshPluginRunFun gmshPluginRun;
        #endregion
        #region "IO Delegates"
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshOpenFun(string fileName, ref int ierr);
            public gmshOpenFun gmshOpen;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshMergeFun(string fileName, ref int ierr);
            public gmshMergeFun gmshMerge;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshWriteFun(string fileName, ref int ierr);
            public gmshWriteFun gmshWrite;
        #endregion
        #region "Model Delegates"
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelSetFileNameFun(string fileName, ref int ierr);
            public gmshModelSetFileNameFun gmshModelSetFileName;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelAddFun(string name, ref int ierr);
            public gmshModelAddFun gmshModelAdd;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelRemoveFun(ref int ierr);
            public gmshModelRemoveFun gmshModelRemove;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshClearFun(ref int ierr);
            public gmshClearFun gmshClear;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelSetCurrentFun(string name, ref int ierr);
            public gmshModelSetCurrentFun gmshModelSetCurrent;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelOccSynchronizeFun(ref int ierr);
            public gmshModelOccSynchronizeFun gmshModelOccSynchronize;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelGeoSynchronizeFun(ref int ierr);
            public gmshModelGeoSynchronizeFun gmshModelGeoSynchronize;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetFileNameFun(ref IntPtr fileName, ref int ierr);
            private gmshModelGetFileNameFun gmshModelGetFileName;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelListFun(ref IntPtr names, ref IntPtr names_n, ref int ierr);
            private gmshModelListFun gmshModelList;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetCurrentFun(ref IntPtr name, ref int ierr);
            private gmshModelGetCurrentFun gmshModelGetCurrent;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelOccImportShapesFun(string fileName, ref IntPtr outDimTags,
                                                          ref IntPtr outDimTags_n, int highestDimOnly,
                                                          string format, ref int ierr);
            private gmshModelOccImportShapesFun gmshModelOccImportShapes;
        #endregion
        #region "Geometry Delegates"
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetEntitiesFun(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
            private gmshModelGetEntitiesFun gmshModelGetEntities;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelOccGetEntitiesFun(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
            private gmshModelOccGetEntitiesFun gmshModelOccGetEntities;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelRemoveEntitiesFun(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
            public gmshModelRemoveEntitiesFun gmshModelRemoveEntities;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelOccRemoveFun(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
            public gmshModelOccRemoveFun gmshModelOccRemove;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelSetEntityNameFun(int dim, int tag, string name, ref int ierr);
            public gmshModelSetEntityNameFun gmshModelSetEntityName;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetEntityNameFun(int dim, int tag, ref IntPtr name, ref int ierr);
            private gmshModelGetEntityNameFun gmshModelGetEntityName;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelRemoveEntityNameFun(string name, ref int ierr);
            public gmshModelRemoveEntityNameFun gmshModelRemoveEntityName;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelGetBoundingBoxFun(int dim, int tag, ref double xmin,
                                                          ref double ymin, ref double zmin,
                                                          ref double xmax, ref double ymax,
                                                          ref double zmax, ref int ierr);
            public gmshModelGetBoundingBoxFun gmshModelGetBoundingBox;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetAdjacenciesFun(int dim, int tag, ref IntPtr upward, ref IntPtr upward_n,
                                                             ref IntPtr downward, ref IntPtr downward_n, ref int ierr);
            private gmshModelGetAdjacenciesFun gmshModelGetAdjacencies;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetBoundaryFun(int[] dimTags, IntPtr dimTags_n, ref IntPtr outDimTags,
                                                       ref IntPtr outDimTags_n, int combined, int oriented,
                                                       int recursive, ref int ierr);
            private gmshModelGetBoundaryFun gmshModelGetBoundary;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetTypeFun(int dim, int tag, ref IntPtr entityType, ref int ierr);
            private gmshModelGetTypeFun gmshModelGetType;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelGetDimensionFun(ref int ierr);
            public gmshModelGetDimensionFun gmshModelGetDimension;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelIsInsideFun(int dim, int tag, double[] coord, IntPtr coord_n,
                                                   int parametric, ref int ierr);
            public gmshModelIsInsideFun gmshModelIsInside;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelSetVisibilityFun(int[] dimTags, IntPtr dimTags_n, int value,
                                                        int recursive, ref int ierr);
            public gmshModelSetVisibilityFun gmshModelSetVisibility;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelSetAttributeFun(string name, string[] values, IntPtr values_n, ref int ierr);
            public gmshModelSetAttributeFun gmshModelSetAttribute;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelGetAttributeFun(string name, ref IntPtr values, ref IntPtr values_n, ref int ierr);
            public gmshModelGetAttributeFun gmshModelGetAttribute;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetAttributeNamesFun(ref IntPtr names, ref IntPtr names_n, ref int ierr);
            private gmshModelGetAttributeNamesFun gmshModelGetAttributeNames;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelRemoveAttributeFun(string name, ref int ierr);
            public gmshModelRemoveAttributeFun gmshModelRemoveAttribute;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetPhysicalNameFun(int dim, int tag, ref IntPtr name, ref int ierr);
            private gmshModelGetPhysicalNameFun gmshModelGetPhysicalName;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetDerivativeFun(int dim, int tag, double[] parametricCoord,
                                                        IntPtr parametricCoord_n, ref IntPtr derivatives,
                                                        ref IntPtr derivatives_n, ref int ierr);
            private gmshModelGetDerivativeFun gmshModelGetDerivative;


            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetSecondDerivativeFun(int dim, int tag, double[] parametricCoord,
                                                              IntPtr parametricCoord_n, ref IntPtr derivatives,
                                                              ref IntPtr derivatives_n, ref int ierr);
            private gmshModelGetSecondDerivativeFun gmshModelGetSecondDerivative;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetCurvatureFun(int dim, int tag, double[] parametricCoord,
                                                       IntPtr parametricCoord_n, ref IntPtr curvatures,
                                                       ref IntPtr curvatures_n, ref int ierr);
            private gmshModelGetCurvatureFun gmshModelGetCurvature;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetValueFun(int dim, int tag, double[] parametricCoord,
                                                   IntPtr parameticCoord_n, ref IntPtr coord,
                                                   ref IntPtr coord_n, ref int ierr);
            private gmshModelGetValueFun gmshModelGetValue;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetParametrizationFun(int dim, int tag, double[] coord, IntPtr coord_n,
                                                              ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                              ref int ierr);
            private gmshModelGetParametrizationFun gmshModelGetParametrization;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelGetParametrizationBoundsFun(int dim, int tag, ref IntPtr min,
                                                                   ref IntPtr min_n, ref IntPtr max,
                                                                   ref IntPtr max_n, ref int ierr);
            private gmshModelGetParametrizationBoundsFun gmshModelGetParametrizationBounds;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelReparametrizeOnSurfaceFun(int dim, int tag, double[] parametricCoord,
                                                                 IntPtr parametricCoord_n, int surfaceTag,
                                                                 ref IntPtr surfaceParametricCoord,
                                                                 ref IntPtr surfaceParametricCoord_n,
                                                                 int which, ref int ierr);
            private gmshModelReparametrizeOnSurfaceFun gmshModelReparametrizeOnSurface;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelOccGetCurveLoopsFun(int surfaceTag, ref IntPtr curveLoopTags,
                                                            ref IntPtr curveLoopTags_n, ref IntPtr curveTags,
                                                            ref IntPtr curveTags_n, ref IntPtr curveTags_nn,
                                                            ref int ierr);
            private gmshModelOccGetCurveLoopsFun gmshModelOccGetCurveLoops;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelOccGetSurfaceLoopsFun(int volumeTag, ref IntPtr surfaceLoopTags,
                                                            ref IntPtr surfaceLoopTags_n, ref IntPtr surfaceTags,
                                                            ref IntPtr surfaceTags_n, ref IntPtr surfaceTags_nn,
                                                            ref int ierr);
            private gmshModelOccGetSurfaceLoopsFun gmshModelOccGetSurfaceLoops;
        #endregion
        #region "Mesh Delegates"
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshGenerateFun(int dim, ref int ierr);
            public gmshModelMeshGenerateFun gmshModelMeshGenerate;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshRecombineFun(ref int ierr);
            public gmshModelMeshRecombineFun gmshModelMeshRecombine;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshRefineFun(ref int ierr);
            public gmshModelMeshRefineFun gmshModelMeshRefine;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshGetElementsFun(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                      ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                      ref IntPtr elementTags_nn, ref IntPtr nodeTags,
                                                      ref IntPtr nodeTags_n, ref IntPtr nodeTags_nn,
                                                      int dim, int tag, ref int ierr);
            private gmshModelMeshGetElementsFun gmshModelMeshGetElements;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshGetElementTypesFun(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                              int dim, int tag, ref int ierr);
            private gmshModelMeshGetElementTypesFun gmshModelMeshGetElementTypes;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshGetElementPropertiesFun(int elementType, ref IntPtr elementName, ref int dim,
                                                               ref int order, ref int numNodes, ref IntPtr localeNodeCoord,
                                                               ref IntPtr localeNodeCoord_n, ref int numPrimaryNodes,
                                                               ref int ierr);
            private gmshModelMeshGetElementPropertiesFun gmshModelMeshGetElementProperties;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshGetNodesFun(ref IntPtr nodeTags, ref IntPtr nodeTags_n,
                                                           ref IntPtr coord, ref IntPtr coord_n,
                                                           ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                           int dim, int tag, int includeBoundary,
                                                           int returnParametricCoord, ref int ierr);
            private gmshModelMeshGetNodesFun gmshModelMeshGetNodes;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshGetNodesByElementTypeFun(int elementType, ref IntPtr nodeTags,
                                                                        ref IntPtr nodeTags_n, ref IntPtr coord,
                                                                        ref IntPtr coord_n, ref IntPtr parametricCoord,
                                                                        ref IntPtr parametricCoord_n, int tag,
                                                                        int returnParametricCoord, ref int ierr);
            private gmshModelMeshGetNodesByElementTypeFun gmshModelMeshGetNodesByElementType;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshSetTransfiniteCurveFun(int tag, int numNodes, string meshType,
                                                                      double coef, ref int ierr);
            public gmshModelMeshSetTransfiniteCurveFun gmshModelMeshSetTransfiniteCurve;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshGetElementsByCoordinatesFun(double x, double y, double z,
                                                                           ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                                           int dim, int strict, ref int ierr);
            private gmshModelMeshGetElementsByCoordinatesFun gmshModelMeshGetElementsByCoordinates;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshGetLocalCoordinatesInElementFun(UIntPtr elementTag, double x, double y, double z,
                                                                               ref double u, ref double v, ref double w,
                                                                               ref int ierr);
            public gmshModelMeshGetLocalCoordinatesInElementFun gmshModelMeshGetLocalCoordinatesInElement;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshRebuildNodeCacheFun(int onlyIfNecessary, ref int ierr);
            public gmshModelMeshRebuildNodeCacheFun gmshModelMeshRebuildNodeCache;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshRebuildElementCacheFun(int onlyIfNecessary, ref int ierr);
            public gmshModelMeshRebuildElementCacheFun gmshModelMeshRebuildElementCache;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshClearFun(int[] dimTags, IntPtr dimTags_n, ref int ierr);
            public gmshModelMeshClearFun gmshModelMeshClear;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshSetVisibilityFun(IntPtr[] elementTags, IntPtr elementTags_n,
                                                                int value, ref int ierr);
            public gmshModelMeshSetVisibilityFun gmshModelMeshSetVisibility;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshSetAlgorithmFun(int dim, int tag, int val, ref int ierr);
            public gmshModelMeshSetAlgorithmFun gmshModelMeshSetAlgorithm;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshGetElementQualitiesFun(IntPtr[] elementTags, IntPtr elementTags_n,
                                                                      ref IntPtr elementsQuality, ref IntPtr elementQuality_n,
                                                                      string qualityName, IntPtr task,
                                                                      IntPtr numTasks, ref int ierr);
            private gmshModelMeshGetElementQualitiesFun gmshModelMeshGetElementQualities;
        #endregion
        #region "Mesh Field Delegates"
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate int gmshModelMeshFieldAddFun(string fieldType, int tag, ref int ierr);
            public gmshModelMeshFieldAddFun gmshModelMeshFieldAdd;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshFieldRemoveFun(int tag, ref int ierr);
            public gmshModelMeshFieldRemoveFun gmshModelMeshFieldRemove;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshFieldListFun(ref IntPtr tags, ref IntPtr tags_n, ref int ierr);
            private gmshModelMeshFieldListFun gmshModelMeshFieldList;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshFieldGetTypeFun(int tag, ref IntPtr fieldType, ref int ierr);
            private gmshModelMeshFieldGetTypeFun gmshModelMeshFieldGetType;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshFieldSetNumberFun(int tag, string option, double value, ref int ierr);
            public gmshModelMeshFieldSetNumberFun gmshModelMeshFieldSetNumber;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshFieldGetNumberFun(int tag, string option, ref double value, ref int ierr);
            public gmshModelMeshFieldGetNumberFun gmshModelMeshFieldGetNumber;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshFieldSetStringFun(int tag, string option, string value, ref int ierr);
            public gmshModelMeshFieldSetStringFun gmshModelMeshFieldSetString;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshFieldGetStringFun(int tag, string option, ref IntPtr value, ref int ierr);
            private gmshModelMeshFieldGetStringFun gmshModelMeshFieldGetString;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshFieldSetNumbersFun(int tag, string option, double[] values,
                                                                  IntPtr values_n, ref int ierr);
            public gmshModelMeshFieldSetNumbersFun gmshModelMeshFieldSetNumbers;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate void gmshModelMeshFieldGetNumbersFun(int tag, string option, ref IntPtr values,
                                                                  ref IntPtr values_n, ref int ierr);
            private gmshModelMeshFieldGetNumbersFun gmshModelMeshFieldGetNumbers;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshFieldSetAsBackgroundMeshFun(int tag, ref int ierr);
            public gmshModelMeshFieldSetAsBackgroundMeshFun gmshModelMeshFieldSetAsBackgroundMesh;

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            public delegate void gmshModelMeshFieldSetAsBoundaryLayerFun(int tag, ref int ierr);
            public gmshModelMeshFieldSetAsBoundaryLayerFun gmshModelMeshFieldSetAsBoundaryLayer;
        #endregion
        public ControllerGeneral(string dllToLoad)
        {
            DLL = NativeMethods.LoadLibrary(dllToLoad);
            #region "Get Initialization delegates"
                gmshInitialize = GetFunction<gmshInitializeFun>("gmshInitialize");
                gmshIsInitialized = GetFunction<gmshIsInitializedFun>("gmshIsInitialized");
                gmshFinalize = GetFunction<gmshFinalizeFun>("gmshFinalize");
            #endregion
            #region "Get Memory delegates"
                gmshMalloc = GetFunction<gmshMallocFun>("gmshMalloc");
                gmshFree = GetFunction<gmshFreeFun>("gmshFree");
            #endregion
            #region "Get General delegates"
                gmshOptionSetNumber = GetFunction<gmshOptionSetNumberFun>("gmshOptionSetNumber");
                gmshOptionGetNumber = GetFunction<gmshOptionGetNumberFun>("gmshOptionGetNumber");
                gmshOptionSetString = GetFunction<gmshOptionSetStringFun>("gmshOptionSetString");
                //Private members
                gmshOptionGetString = GetFunction<gmshOptionGetStringFun>("gmshOptionGetString");
            #endregion
            #region "Get Plugin delegates"
                gmshPluginSetNumber = GetFunction<gmshPluginSetNumberFun>("gmshPluginSetNumber");
                gmshPluginSetString = GetFunction<gmshPluginSetStringFun>("gmshPluginSetString");
                gmshPluginRun = GetFunction<gmshPluginRunFun>("gmshPluginRun");
            #endregion
            #region "Get IO delegates"
                gmshOpen = GetFunction<gmshOpenFun>("gmshOpen");
                gmshMerge = GetFunction<gmshMergeFun>("gmshMerge");
                gmshWrite = GetFunction<gmshWriteFun>("gmshWrite");
            #endregion
            #region "Get Model delegates"
                gmshModelSetFileName = GetFunction<gmshModelSetFileNameFun>("gmshModelSetFileName");
                gmshModelAdd = GetFunction<gmshModelAddFun>("gmshModelAdd");
                gmshModelRemove = GetFunction<gmshModelRemoveFun>("gmshModelRemove");
                gmshClear = GetFunction<gmshClearFun>("gmshClear");
                gmshModelSetCurrent = GetFunction<gmshModelSetCurrentFun>("gmshModelSetCurrent");
                gmshModelOccSynchronize = GetFunction<gmshModelOccSynchronizeFun>("gmshModelOccSynchronize");
                gmshModelGeoSynchronize = GetFunction<gmshModelGeoSynchronizeFun>("gmshModelGeoSynchronize");
                //Private members
                gmshModelGetFileName = GetFunction<gmshModelGetFileNameFun>("gmshModelGetFileName");
                gmshModelList = GetFunction<gmshModelListFun>("gmshModelList");
                gmshModelGetCurrent = GetFunction<gmshModelGetCurrentFun>("gmshModelGetCurrent");
                gmshModelOccImportShapes = GetFunction<gmshModelOccImportShapesFun>("gmshModelOccImportShapes");
            #endregion
            #region "Get Geometry delegates"
                gmshModelGetEntities = GetFunction<gmshModelGetEntitiesFun>("gmshModelGetEntities");
                gmshModelOccGetEntities = GetFunction<gmshModelOccGetEntitiesFun>("gmshModelOccGetEntities");
                gmshModelRemoveEntities = GetFunction<gmshModelRemoveEntitiesFun>("gmshModelRemoveEntities");
                gmshModelOccRemove = GetFunction<gmshModelOccRemoveFun>("gmshModelOccRemove");
                gmshModelSetEntityName = GetFunction<gmshModelSetEntityNameFun>("gmshModelSetEntityName");
                gmshModelGetEntityName = GetFunction<gmshModelGetEntityNameFun>("gmshModelGetEntityName");
                gmshModelRemoveEntityName = GetFunction<gmshModelRemoveEntityNameFun>("gmshModelRemoveEntityName");
                gmshModelGetBoundingBox = GetFunction<gmshModelGetBoundingBoxFun>("gmshModelGetBoundingBox");
                gmshModelGetAdjacencies = GetFunction<gmshModelGetAdjacenciesFun>("gmshModelGetAdjacencies");
                gmshModelGetBoundary = GetFunction<gmshModelGetBoundaryFun>("gmshModelGetBoundary");
                gmshModelGetType = GetFunction<gmshModelGetTypeFun>("gmshModelGetType");
                gmshModelGetDimension = GetFunction<gmshModelGetDimensionFun>("gmshModelGetDimension");
                gmshModelIsInside = GetFunction<gmshModelIsInsideFun>("gmshModelIsInside");
                gmshModelSetVisibility = GetFunction<gmshModelSetVisibilityFun>("gmshModelSetVisibility");
                gmshModelSetAttribute = GetFunction<gmshModelSetAttributeFun>("gmshModelSetAttribute");
                gmshModelGetAttribute = GetFunction<gmshModelGetAttributeFun>("gmshModelGetAttribute");
                gmshModelGetAttributeNames = GetFunction<gmshModelGetAttributeNamesFun>("gmshModelGetAttributeNames");
                gmshModelRemoveAttribute = GetFunction<gmshModelRemoveAttributeFun>("gmshModelRemoveAttribute");
                gmshModelGetPhysicalName = GetFunction<gmshModelGetPhysicalNameFun>("gmshModelGetPhysicalName");
                gmshModelGetDerivative = GetFunction<gmshModelGetDerivativeFun>("gmshModelGetDerivative");
                gmshModelGetSecondDerivative = GetFunction<gmshModelGetSecondDerivativeFun>("gmshModelGetSecondDerivative");
                gmshModelGetCurvature = GetFunction<gmshModelGetCurvatureFun>("gmshModelGetCurvature");
                gmshModelGetValue = GetFunction<gmshModelGetValueFun>("gmshModelGetValue");
                gmshModelGetParametrization = GetFunction<gmshModelGetParametrizationFun>("gmshModelGetParametrization");
                gmshModelGetParametrizationBounds = GetFunction<gmshModelGetParametrizationBoundsFun>("gmshModelGetParametrizationBounds");
                gmshModelReparametrizeOnSurface = GetFunction<gmshModelReparametrizeOnSurfaceFun>("gmshModelReparametrizeOnSurface");
                gmshModelOccGetCurveLoops = GetFunction<gmshModelOccGetCurveLoopsFun>("gmshModelOccGetCurveLoops");
                gmshModelOccGetSurfaceLoops = GetFunction<gmshModelOccGetSurfaceLoopsFun>("gmshModelOccGetSurfaceLoops");
            #endregion
            #region "Get Mesh delegates"
                gmshModelMeshGenerate = GetFunction<gmshModelMeshGenerateFun>("gmshModelOccGetSurfaceLoops");
                gmshModelMeshRecombine = GetFunction<gmshModelMeshRecombineFun>("gmshModelMeshRecombine");
                gmshModelMeshRefine = GetFunction<gmshModelMeshRefineFun>("gmshModelMeshRefine");
                gmshModelMeshGetElements = GetFunction<gmshModelMeshGetElementsFun>("gmshModelMeshGetElements");
                gmshModelMeshGetElementTypes = GetFunction<gmshModelMeshGetElementTypesFun>("gmshModelMeshGetElementTypes");
                gmshModelMeshGetElementProperties = GetFunction<gmshModelMeshGetElementPropertiesFun>("gmshModelMeshGetElementProperties");
                gmshModelMeshGetNodes = GetFunction<gmshModelMeshGetNodesFun>("gmshModelMeshGetNodes");
                gmshModelMeshGetNodesByElementType = GetFunction<gmshModelMeshGetNodesByElementTypeFun>("gmshModelMeshGetNodesByElementType");
                gmshModelMeshSetTransfiniteCurve = GetFunction<gmshModelMeshSetTransfiniteCurveFun>("gmshModelMeshSetTransfiniteCurve");
                gmshModelMeshGetElementsByCoordinates = GetFunction<gmshModelMeshGetElementsByCoordinatesFun>("gmshModelMeshGetElementsByCoordinates");
                gmshModelMeshGetLocalCoordinatesInElement = GetFunction<gmshModelMeshGetLocalCoordinatesInElementFun>("gmshModelMeshGetLocalCoordinatesInElement");
                gmshModelMeshRebuildNodeCache = GetFunction<gmshModelMeshRebuildNodeCacheFun>("gmshModelMeshRebuildNodeCache");
                gmshModelMeshRebuildElementCache = GetFunction<gmshModelMeshRebuildElementCacheFun>("gmshModelMeshRebuildElementCache");
                gmshModelMeshClear = GetFunction<gmshModelMeshClearFun>("gmshModelMeshClear");
                gmshModelMeshSetVisibility = GetFunction<gmshModelMeshSetVisibilityFun>("gmshModelMeshSetVisibility");
                gmshModelMeshSetAlgorithm = GetFunction<gmshModelMeshSetAlgorithmFun>("gmshModelMeshSetAlgorithm");
                gmshModelMeshGetElementQualities = GetFunction<gmshModelMeshGetElementQualitiesFun>("gmshModelMeshGetElementQualities");
            #endregion
            #region "Get Mesh Field delegates"
                gmshModelMeshFieldAdd = GetFunction<gmshModelMeshFieldAddFun>("gmshModelMeshFieldAdd");
                gmshModelMeshFieldRemove = GetFunction<gmshModelMeshFieldRemoveFun>("gmshModelMeshFieldRemove");
                gmshModelMeshFieldList = GetFunction<gmshModelMeshFieldListFun>("gmshModelMeshFieldList");
                gmshModelMeshFieldGetType = GetFunction<gmshModelMeshFieldGetTypeFun>("gmshModelMeshFieldGetType");
                gmshModelMeshFieldSetNumber = GetFunction<gmshModelMeshFieldSetNumberFun>("gmshModelMeshFieldSetNumber");
                gmshModelMeshFieldGetNumber = GetFunction<gmshModelMeshFieldGetNumberFun>("gmshModelMeshFieldGetNumber");
                gmshModelMeshFieldGetString = GetFunction<gmshModelMeshFieldGetStringFun>("gmshModelMeshFieldGetString");
                gmshModelMeshFieldSetString = GetFunction<gmshModelMeshFieldSetStringFun>("gmshModelMeshFieldSetString");
                gmshModelMeshFieldSetNumbers = GetFunction<gmshModelMeshFieldSetNumbersFun>("gmshModelMeshFieldSetNumbers");
                gmshModelMeshFieldGetNumbers = GetFunction<gmshModelMeshFieldGetNumbersFun>("gmshModelMeshFieldGetNumbers");
                gmshModelMeshFieldSetAsBackgroundMesh = GetFunction<gmshModelMeshFieldSetAsBackgroundMeshFun>("gmshModelMeshFieldSetAsBackgroundMesh");
                gmshModelMeshFieldSetAsBoundaryLayer = GetFunction<gmshModelMeshFieldSetAsBoundaryLayerFun>("gmshModelMeshFieldSetAsBoundaryLayer");
            #endregion
            var argv = string.Empty;
            var ierr = 0;
            gmshInitialize(0, ref argv, 0, 0, ref ierr);
        }

        ~ControllerGeneral()
        {
            var ierr = 0;
            if(gmshIsInitialized(ref ierr) == 1)
                gmshFinalize(ref ierr);
            NativeMethods.FreeLibrary(DLL);
        }

        public T GetFunction<T>(string functionName)
        {
            var funPtr = NativeMethods.GetProcAddress(DLL, functionName);
            return Marshal.GetDelegateForFunctionPointer<T>(funPtr);
        }
        #region "General managed methods"
        public bool OptionGetString(string name, out string value)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            value = null;
            gmshOptionGetString(name, ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
                value = GetString(unmanagedString);
            return status;
        }
        #endregion
        #region "Model managed methods"
        public bool ModelGetFileName(out string fileName)
        {
            IntPtr unmanagedString = IntPtr.Zero;
            var ierr = 0;
            fileName = null;
            gmshModelGetFileName(ref unmanagedString, ref ierr);
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
            gmshModelList(ref names, ref names_n, ref ierr);
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
            gmshModelGetCurrent(ref unmanagedString, ref ierr);
            var status = ierr == 0;
            if (status)
                current = GetString(unmanagedString);
            return status;
        }
        #endregion
        #region "Geometry managed methods"
        public bool ModelGetEntities(out int[] dimTags, int dim)
        {
            var ierr = 0;
            IntPtr dTags, dimTags_n;
            dTags = dimTags_n = IntPtr.Zero;
            dimTags = null;
            var extension = ControllerIO.ModelGetFileNameExtension();
            if (extension.Contains("stp") || extension.Contains("step") ||
                extension.Contains("brep") || extension.Contains("igs"))
                gmshModelOccGetEntities(ref dTags, ref dimTags_n, dim, ref ierr);
            else
                gmshModelGetEntities(ref dTags, ref dimTags_n, dim, ref ierr);
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
            gmshModelGetEntityName(dim, tag, ref unmanagedString, ref ierr);
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
            gmshModelGetAdjacencies(dim, tag, ref upw, ref upw_n, ref downw, ref downw_n, ref ierr);
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
            gmshModelGetAttribute(name, ref values, ref values_n, ref ierr);
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
            gmshModelGetAttributeNames(ref nms, ref nms_n, ref ierr);
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
            gmshModelGetDerivative(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length,
                                   ref der, ref der_n, ref ierr);
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
            gmshModelGetSecondDerivative(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length,
                                         ref der, ref der_n, ref ierr);
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
            gmshModelGetValue(dim, tag, parametricCoord, (IntPtr)parametricCoord.Length,
                              ref locCoord, ref locCoord_n, ref ierr);
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
            gmshModelGetParametrization(dim, tag, coord, (IntPtr)coord.Length, ref pCoord, ref pCoord_n, ref ierr);
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
            gmshModelGetParametrizationBounds(dim, tag, ref minBounds, ref minBounds_n, 
                                              ref maxBounds, ref maxBounds_n, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(minBounds, out min, minBounds_n);
                GetObjectsData(maxBounds, out max, maxBounds_n);
            }
            return status;
        }
        #endregion
        #region "Mesh managed methods"
        public bool ModelMeshGetElements(int dim, int tag, out int[] elementTypes, 
                                         out long[][] elementTags, out long[][] nodeTags)
        {
            IntPtr elemTypes, elemTypes_n, elemTags, elemTags_n, elemTags_nn;
            IntPtr nodTags, nodTags_n, nodTags_nn;
            elemTypes = elemTypes_n = elemTags = elemTags_n = elemTags_nn = IntPtr.Zero;
            nodTags = nodTags_n = nodTags_nn = IntPtr.Zero;
            var ierr = 0;
            elementTypes = null;
            elementTags = nodeTags = null;
            gmshModelMeshGetElements(ref elemTypes, ref elemTypes_n, ref elemTags,
                                     ref elemTags_n, ref elemTags_nn, ref nodTags,
                                     ref nodTags_n, ref nodTags_nn, dim, tag, ref ierr);
            var status = ierr == 0;
            if(status)
            {
                GetObjectsData(elemTypes, out elementTypes, elemTypes_n);
                FillTwoDimArray(elemTags, elemTags_n, elemTags_nn, out elementTags);
                FillTwoDimArray(nodTags, nodTags_n, nodTags_nn, out nodeTags);
            }
            return status;
        }

        public bool ModelMeshGetElementTypes(int dim, int tag, out int[] elementTypes)
        {
            IntPtr elemTypes = IntPtr.Zero;
            IntPtr elemTypes_n = IntPtr.Zero;
            var ierr = 0;
            elementTypes = null;
            gmshModelMeshGetElementTypes(ref elemTypes, ref elemTypes_n, dim, tag, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(elemTypes, out elementTypes, elemTypes_n);
            return status;
        }

        public bool ModelMeshGetElementProperties(int[] elementTypes, out ElementProperties[] elemProperties)
        {
            IntPtr localeNodeCoord, localeNodeCoord_n, elementName;
            localeNodeCoord = localeNodeCoord_n = elementName = IntPtr.Zero;
            var ierr = 0;
            var status = true;
            elemProperties = new ElementProperties[elementTypes.Length];
            for (var i = 0; i < elementTypes.Length; ++i)
            {
                elemProperties[i] = new ElementProperties();
                gmshModelMeshGetElementProperties(elementTypes[i], ref elementName, ref elemProperties[i].dim, 
                                                  ref elemProperties[i].order, ref elemProperties[i].order, 
                                                  ref localeNodeCoord, ref localeNodeCoord_n, 
                                                  ref elemProperties[i].numPrimaryNodes, ref ierr);
                status = ierr == 0;
                if (!status)
                {
                    elemProperties = null;
                    break;
                }
                elemProperties[i].elementName = GetString(elementName);
                GetObjectsData(localeNodeCoord, out elemProperties[i].coords, localeNodeCoord_n);
                elemProperties[i].coords_n = elemProperties[i].coords.Length;
            }
            return status;
        }

        public bool ModelMeshGetNodes(int dim, int tag, bool includeBoundary, bool returnParametric,
                                      out long[] nodeTags, out double[] coords, out double[] parametricCoord)
        {
            IntPtr nodeTagsInt, nodeTagsInt_n, coord, coord_n, paramCoord, paramCoord_n;
            nodeTagsInt = nodeTagsInt_n = coord = coord_n = paramCoord = paramCoord_n = IntPtr.Zero;
            var ierr = 0;
            coords = parametricCoord = null;
            nodeTags = null;
            var includeBound = Convert.ToInt32(includeBoundary);
            var retParam = Convert.ToInt32(returnParametric);
            gmshModelMeshGetNodes(ref nodeTagsInt, ref nodeTagsInt_n, ref coord, ref coord_n, 
                                  ref paramCoord, ref paramCoord_n, dim, tag, includeBound, retParam, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(nodeTagsInt, out nodeTags, nodeTagsInt_n);
                GetObjectsData(coord, out coords, coord_n);
                if (returnParametric)
                    GetObjectsData(paramCoord, out parametricCoord, paramCoord_n);
            }
            return status;
        }

        public bool ModelMeshGetNodesByElementType(int elementType, int tag, bool returnParametric,
                                                   out long[] nodeTags, out double[] coords, out double[] parametricCoord)
        {
            IntPtr nodeTagsInt, nodeTagsInt_n, coord, coord_n, paramCoord, paramCoord_n;
            nodeTagsInt = nodeTagsInt_n = coord = coord_n = paramCoord = paramCoord_n = IntPtr.Zero;
            var ierr = 0;
            coords = parametricCoord = null;
            nodeTags = null;
            var retParam = Convert.ToInt32(returnParametric);
            gmshModelMeshGetNodesByElementType(elementType, ref nodeTagsInt, ref nodeTagsInt_n, ref coord, ref coord_n, 
                                               ref paramCoord, ref paramCoord_n, tag, retParam, ref ierr);
            var status = ierr == 0;
            if (status)
            {
                GetObjectsData(nodeTagsInt, out nodeTags, nodeTagsInt_n);
                GetObjectsData(coord, out coords, coord_n);
                if (returnParametric)
                    GetObjectsData(paramCoord, out parametricCoord, paramCoord_n);
            }
            return ierr == 0;
        }

        public bool ModelMeshGetElementsByCoordinates(double x, double y, double z, int dim, 
                                                      bool strict, out long[] elementTags)
        {
            IntPtr elemTags, elemTags_n;
            elemTags = elemTags_n = IntPtr.Zero;
            var ierr = 0;
            elementTags = null;
            var strictInt = Convert.ToInt32(strict);
            gmshModelMeshGetElementsByCoordinates(x, y, z, ref elemTags, ref elemTags_n, dim, strictInt, ref ierr);
            var status = ierr == 0;
            if(status)
                GetObjectsData(elemTags, out elementTags, elemTags_n);
            return status;
        }

        public bool ModelMeshGetElementQualities(long[] elementTags, string qualityName, 
                                                 long task, long numTasks, out double[] elementsQuality)
        {
            var eTags = elementTags.Select(x => (IntPtr)x).ToArray();
            var elemQuality = IntPtr.Zero;
            var elemQuality_n = IntPtr.Zero;
            var ierr = 0;
            elementsQuality = null;
            gmshModelMeshGetElementQualities(eTags, (IntPtr)elementTags.Length, ref elemQuality, ref elemQuality_n, 
                                             qualityName, (IntPtr)task, (IntPtr)numTasks, ref ierr);
            var status = ierr == 0;
            if(status)
                GetObjectsData(elemQuality, out elementsQuality, elemQuality_n);
            return status;
        }
        #endregion
        #region "Mesh Field managed methods"
        public bool ModelMeshFieldList(out int[] tags)
        {
            IntPtr tagsInt = IntPtr.Zero;
            IntPtr tagsInt_n = IntPtr.Zero;
            var ierr = 0;
            tags = null;
            gmshModelMeshFieldList(ref tagsInt, ref tagsInt_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(tagsInt, out tags, tagsInt_n);
            return status;
        }

        public bool ModelMeshFieldGetType(int tag, out string fieldType)
        {
            IntPtr fType = IntPtr.Zero;
            var ierr = 0;
            fieldType = null;
            gmshModelMeshFieldGetType(tag, ref fType, ref ierr);
            var status = ierr == 0;
            if (status)
                fieldType = GetString(fType);
            return status;
        }

        public bool ModelMeshFieldGetString(int tag, string option, out string value)
        {
            IntPtr valueInt = IntPtr.Zero;
            var ierr = 0;
            value = null;
            gmshModelMeshFieldGetString(tag, option, ref valueInt, ref ierr);
            var status = ierr == 0;
            if (status)
                value = GetString(valueInt);
            return status;
        }

        public bool MeshFieldGetNumbers(int tag, string option, out double[] values)
        {
            IntPtr valuesInt = IntPtr.Zero;
            IntPtr valuesInt_n = IntPtr.Zero;
            var ierr = 0;
            values = null;
            gmshModelMeshFieldGetNumbers(tag, option, ref valuesInt, ref valuesInt_n, ref ierr);
            var status = ierr == 0;
            if (status)
                GetObjectsData(valuesInt, out values, valuesInt_n);
            return status;
        }
        #endregion
        #region "Utility methods"
        public List<ModelObject> GetGeometryEntities(ObjKind kind, ref bool status)
        {
            int[] tags;
            int[] dimTags;
            var entities = new List<ModelObject>();
            status = false;
            switch (kind)
            {
                case ObjKind.Point:
                    if(ModelGetEntities(out dimTags,0))
                    {
                        tags = dimTags.Where((v, i) => (i & 1) == 1).ToArray();
                        entities = CreateControlPoints(tags);
                        status = true;
                    }
                    break;
                case ObjKind.Curve:
                case ObjKind.Beam:
                    if (ModelGetEntities(out dimTags, 1))
                    {
                        tags = dimTags.Where((v, i) => (i & 1) == 1).ToArray();
                        entities = CreateLines(tags, kind, ref status);
                        status = true;
                    }
                    break;
            }
            return entities;
        }

        private List<ModelObject> CreateLines(int[] tags, ObjKind kind, ref bool status)
        {
            var lines = new List<ModelObject>();
            double[] minBounds;
            double[] maxBounds;
            status = true;
            for (var i = 0; i < tags.Length; ++i)
            {
                if(!ModelGetParametrizationBounds(1, tags[i], out minBounds, out maxBounds))
                {
                    status = false;
                    lines.Clear();
                    break;
                }
                var newPB = minBounds.Concat(maxBounds).ToArray();
                if (!IsLine(1, tags[i], newPB))
                {
                    var ierr = 0;
                    var curvePoints = 0.0;
                    gmshOptionGetNumber("Geometry.NumSubEdges", ref curvePoints, ref ierr);
                    newPB = SetCustomStepParametrization(newPB, (int)curvePoints);
                }
                double[] coords;
                ModelGetValue(1, tags[i], newPB, out coords);
                var points = ConvertLocalCoordsToPoints3D(coords);
                var nodes = new List<Node>();
                var num = 0;
                foreach (var pair in points.Bigrams(false))
                {
                    var line = new Node[] { new Node(num++, pair.Item1), new Node(num++, pair.Item2) };
                    nodes.AddRange(line);
                }
                if (kind == ObjKind.Curve)
                    lines.Add(new Curve(tags[i], nodes.ToArray()));
                else
                    lines.Add(new Element1D(tags[i], nodes.ToArray(), kind));
            }
            return lines;
        }

        private double[] SetCustomStepParametrization(double[] paramsCoord, int steps)
        {
            var step = (paramsCoord[1] - paramsCoord[0]) / (steps - 1);
            Array.Resize(ref paramsCoord, steps);
            for (var i = 0; i < steps; ++i)
                paramsCoord[i] = paramsCoord[0] + step * i;
            return paramsCoord;
        }

        private bool IsLine(int dim, int tag, double[] paramsBound)
        {
            double[] derivative;
            ModelGetDerivative(dim, tag, paramsBound, out derivative);
            var derCount = derivative.Length / 3;
            for (var i = 1; i < derCount; ++i)
                for (var j = 0; j < 3; ++j)
                    if (Math.Abs(derivative[i * 3 + j] - derivative[(i - 1) * 3 + j]) > 1E-8)
                        return false;
            return true;
        }

        private List<ModelObject> CreateControlPoints(int[] tags)
        {
            var nodes = new List<ModelObject>();
            double[] coords;
            for (var i = 0; i < tags.Length; ++i)
            {
                if (!ModelGetValue(0, tags[i], new double[] { }, out coords))
                {
                    nodes.Clear();
                    break;
                }
                var points = ConvertLocalCoordsToPoints3D(coords);
                for (var j = 0; j < points.Length; ++j)
                    nodes.Add(new Node(tags[i], points[j]));
            }
            return nodes;
        }

        private Point3D[] ConvertLocalCoordsToPoints3D(double[] coords)
        {
            var points = new Point3D[coords.Length / 3];
            for (int i = 2, j = 0; i < coords.Length; i += 3, ++j)
            {
                var point = new Point3D((float)coords[i - 2], (float)coords[i - 1], (float)coords[i]);
                points[j] = point;
            }
            return points;
        }

        public List<ModelObject> GetElements(int dim, int tag, ref bool status)
        {
            ElementProperties[] properties;
            int[] elementTypes;
            long[][] elemTags, nodeTags;
            long[] nodesT;
            double[] coords, parametric;
            var list = new List<ModelObject>();
            status = false;
            if (ModelMeshGetElements(dim, tag, out elementTypes, out elemTags, out nodeTags))
            {
                if (ModelMeshGetElementProperties(elementTypes, out properties))
                {
                    for (int i = 0; i < properties.Length; ++i)
                    {
                        var elements = elemTags[i];
                        ModelMeshGetNodesByElementType(elementTypes[i], -1, false, out nodesT, out coords, out parametric);
                        for (var j = 0; j < elements.Length; ++j)
                        {
                            var nodesCount = properties[i].numNodes;
                            var nodesPerElem = new Node[nodesCount];
                            for (int k = 0; k < nodesCount; ++k)
                            {
                                var coordStride = j * nodesCount * 3 + k * 3;
                                var tagStride = j * nodesCount + k;
                                var x = (float)coords[coordStride + 0];
                                var y = (float)coords[coordStride + 1];
                                var z = (float)coords[coordStride + 2];
                                var point = new Point3D(x, y, z);
                                nodesPerElem[k] = new Node((int)nodeTags[i][tagStride], point);
                            }
                            list.Add(CreateModelObject(ref properties[i].elementName, elements[j], nodesPerElem));
                        }
                    }
                    status = true;
                }
            }
            return list;
        }

        private ModelObject CreateModelObject(ref string elementName,
                                                  long elementId, Node[] nodes)
        {
            if (elementName.Contains("Point"))
                return new Node((int)elementId, nodes[0].Position);
            else if (elementName.Contains("Line"))
                return new Element1D((int)elementId, nodes, ObjKind.Beam);
            else if (elementName.Contains("Triangle"))
                return new Element2D((int)elementId, nodes, ObjKind.Triangle);
            else if (elementName.Contains("Quadrilateral"))
                return new Element2D((int)elementId, nodes, ObjKind.Quad);
            else if (elementName.Contains("Tetrahedron"))
                return new Element3D((int)elementId, nodes, ObjKind.Tetra);
            else if (elementName.Contains("Hexahedron"))
                return new Element3D((int)elementId, nodes, ObjKind.Hexa);
            else
                return new Element3D((int)elementId, nodes, ObjKind.Penta);
        }
        #endregion
        #region "Memory managed methods"
        private void FillTwoDimArray(IntPtr eTags, IntPtr eTags_n, IntPtr eTags_nn, out long[][] eData)
        {
            var objsTypesCount = eTags_nn.ToInt32();
            IntPtr[] tagsData, lengthData;
            NewTwoDimArray(eTags, eTags_n, eTags_nn, out tagsData, out lengthData, out eData);
            for (var i = 0; i < objsTypesCount; ++i)
                ControllerMemory.GetObjectsData(tagsData[i], out eData[i], lengthData[i]);
            KernelGeneral.gmshFree(eTags);
        }

        private void NewTwoDimArray(IntPtr eTags, IntPtr eTags_n, IntPtr eTags_nn, 
                                    out IntPtr[] tagsData, out IntPtr[] lengthData, out long[][] eData)
        {
            var objectsCount = eTags_nn.ToInt32();
            eData = new long[objectsCount][];
            tagsData = new IntPtr[objectsCount];
            lengthData = new IntPtr[objectsCount];

            Marshal.Copy(eTags, tagsData, 0, objectsCount);
            Marshal.Copy(eTags_n, lengthData, 0, objectsCount);
            KernelGeneral.gmshFree(eTags_n);
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
            gmshFree(src);
        }

        private string GetString(IntPtr name)
        {
            var result = Marshal.PtrToStringAnsi(name);
            gmshFree(name);
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
                gmshFree(stringArray[i]);
            }
            gmshFree(array);
            return result;
        }
        #endregion
    }
}
