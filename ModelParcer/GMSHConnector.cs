using Model;
using Model.Interfaces;
using PrFunctionLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer
{
    /// <summary>
    /// GMSHConnector
    /// </summary>
    public class GMSHConnector : IDisposable
    {
        private static bool isInit = false;
        private static int globalTags = 1;
        private int tag;
        private static Dictionary<string, int> dim = new Dictionary<string, int>()
                        { { "Узлы", 0 }, { "Элементы1D", 1 }, { "Линия", 1 },
                          { "Элементы2D", 2 }, { "Элементы3D", 3 }};
        /// <summary>
        /// Get/Set the deletion flag for the destination file
        /// </summary>
        public bool DeleteDstFile { get; set; }
        /// <summary>
        /// Get/Set ObjType
        /// </summary>
        public string ObjType { get; set; }

        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Get/Set mesh density
        /// </summary>
        public string MeshDensity 
        {
            get 
            {
                var error = 0;
                double min = 0, max = 0;
                gmshModelSetCurrent(tag.ToString(), ref error);
                gmshOptionGetNumber("Mesh.MeshSizeMin", ref min, ref error);
                gmshOptionGetNumber("Mesh.MeshSizeMax", ref max, ref error);
                return min.ToString() + " " + max.ToString();
            }
            set
            {
                var error = 0;
                gmshModelSetCurrent(tag.ToString(), ref error);
                var values = value.Split(' ');
                var min = Double.Parse(values[0]);
                var max = Double.Parse(values[1]);
                gmshOptionSetNumber("Mesh.MeshSizeMin", min, ref error);
                gmshOptionSetNumber("Mesh.MeshSizeMax", max, ref error);
            }
        }

        public GMSHConnector(string srcPath, string objType)
        {
            DeleteDstFile = false;
            ObjType = dim.ContainsKey(objType) ? objType : String.Empty;
            Init();
            if (File.Exists(srcPath))
            {
                var error = 0;
                gmshOpen(srcPath, ref error);
                srcPath = String.Empty;
                var fName = GetAssociatedFileName();
                //gmshModelSetFileName(srcPath, ref error);
                tag = globalTags++;
            }
        }

        public string GetAssociatedFileName()
        {
            var error = 0;
            //gmshModelSetCurrent(tag.ToString(), ref error);
            byte[][] filename = new byte[1][];
            gmshModelGetFileName(filename, ref error);
            return null;
        }

        /// <summary>
        /// Create ModelData from script file
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="objTypeCreate"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// 
        public ModelData CreateModelData()
        {
            if (isInit)
            {
                var fileName = GetAssociatedFileName();
                if(!String.IsNullOrEmpty(fileName) && !String.IsNullOrEmpty(ObjType))
                {
                    var file = Path.GetFileNameWithoutExtension(fileName);
                    var ext = Path.GetExtension(fileName);
                    var folder = Path.GetDirectoryName(fileName);
                    var dstFile = folder + @"\" + file + ".inp";
                    var error = 0;
                    gmshOpen(fileName, ref error);
                    gmshOptionSetNumber("Mesh.SaveAll", 1, ref error);
                    gmshOptionSetNumber("Mesh.SaveGroupsOfNodes", -1111, ref error);
                    gmshModelMeshGenerate(dim[ObjType], ref error);
                    gmshWrite(dstFile, ref error);
                    var data = new LoadModelFromGMSHTextFile();
                    data.LoadEvent += (ar1, ar2) => { };
                    var modelData = data.Load(dstFile);
                    return modelData;
                }
            }
            return null;
        }

        /*
        public static ModelData CreateFromScript(string srcPath, string objTypeCreate, bool delDst = true,
                                                     double min = 0.0, double max = 1e+22)
        {
            if (isInit && File.Exists(srcPath) && dim.ContainsKey(objTypeCreate))
            {
                var file = Path.GetFileNameWithoutExtension(srcPath);
                var ext = Path.GetExtension(srcPath);
                var folder = Path.GetDirectoryName(srcPath);
                var dstFile = folder + @"\" + file + ".inp";
                var error = 0;
                gmshOpen(srcPath, ref error);
                gmshOptionSetNumber("Mesh.SaveAll", 1, ref error);
                gmshOptionSetNumber("Mesh.SaveGroupsOfNodes", -1111, ref error);
                gmshOptionSetNumber("Mesh.MeshSizeMin", min, ref error);
                gmshOptionSetNumber("Mesh.MeshSizeMax", max, ref error);
                gmshModelMeshGenerate(dim[objTypeCreate], ref error);
                gmshWrite(dstFile, ref error);
                var data = new LoadModelFromGMSHTextFile();
                data.LoadEvent += (ar1, ar2) => { };
                var modelData = data.Load(dstFile);
                if (error == 0 && delDst)
                    File.Delete(dstFile);
                return modelData;
            }
            return null;
        }
        */

        /// <summary>
        /// Remove model from gmsh API
        /// </summary>
        public void Dispose()
        {
            if(isInit)
            {
                var error = 0;
                if (IsLoaded && DeleteDstFile)
                {
                    var fileName = GetAssociatedFileName();
                    if (!String.IsNullOrEmpty(fileName))
                        File.Delete(fileName);
                }
                gmshModelRemove(ref error);
            }
        }

        /// <summary>
        /// Free resources allocated by gmsh
        /// </summary>
        public static void Finalize()
        {
            if (isInit)
            {
                isInit = false;
                var error = 0;
                globalTags = 1;
                gmshClear(ref error);
                gmshFinalize(ref error);
            }
        }

        private static void Init()
        {
            if (!isInit)
            {
                int error = 0;
                var str = String.Empty;
                gmshInitialize(0, ref str, 0, 0, ref error);
                isInit = true;
            }
        }

        private void SetCurrentModel()
        {
            if (isInit)
            {
                /*var error = 0;
                gmshModelSetCurrent(tag.ToString(), ref error);
                var fileName = String.Empty;
                gmshModelGetFileName(out fileName, ref error);*/
            }
        }

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshInitialize(int argc, ref String argv, int readConfigFile,
                                                   int run, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshFinalize(ref int error);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshOpen(String src, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshMerge(String src, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshWrite(String src, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int gmshClear(ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshModelMeshGenerate(int dim, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshOptionSetNumber(String src, double value, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshOptionGetNumber(String src, ref double value, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshOptionSetString(String name, String value, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshModelAdd(String src, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshModelMeshOptimize(String src, int force, int niter, ref int dimTags,
                                                         UIntPtr dimTags_n, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshModelSetFileName(String fileName, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshModelGetFileName(byte[][] filename, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshModelSetCurrent(String src, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshModelRemove(ref int ierr);
    }
}
