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
        public delegate void ModelRemoveEntitiesDelegate(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        public ModelRemoveEntitiesDelegate gmshModelRemoveEntities;
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

        /*/// <summary>
        /// Получает список всех идентификаторов для заданной размерности
        /// </summary>
        /// <param name="dimTags">[Out]Массив пар размерность - идентификатор</param>
        /// <param name="dimTags_n">[Out]Длина массива пар размерность - идентификатор</param>
        /// <param name="dim">[In]Размерность для получения идентификаторов, -1 для всех размерностей</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetEntities(ref IntPtr dimTags, ref IntPtr dimTags_n,
                                                       int dim, ref int ierr);
        /// <summary>
        /// Получает геометрию модели в виде вектора пар размерность : идентификатор
        /// </summary>
        /// <param name="dimTags">[Out]Вектор пар размерность : идентификатор</param>
        /// <param name="dimTags_n">[Out]Размерность вектора пар</param>
        /// <param name="dim">[In]Размерность для получения, -1 все размерности</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelOccGetEntities(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);*/





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
                gmshModelRemoveEntities = GetFunction<ModelRemoveEntitiesDelegate>("gmshModelRemoveEntities");
                gmshModelOccSynchronize = GetFunction<ModelOccSynchronizeDelegate>("gmshModelOccSynchronize");
                gmshModelGeoSynchronize = GetFunction<ModelGeoSynchronizeDelegate>("gmshModelGeoSynchronize");
                gmshModelOccImportShapes = GetFunction<ModelOccImportShapesDelegate>("gmshModelOccImportShapes");
                
                
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

        /*
        private ModelGetFileNameDelegate GetFileNameDelegate()
        {
            IntPtr fileName = IntPtr.Zero;
            int ierr = 0;
            var funPtr = GetFunction<NativeModelGetFileNameDelegate>("gmshModelGetFileName");
            funPtr(ref fileName, ref ierr);
            
            return null;
        }*/
    }
}
