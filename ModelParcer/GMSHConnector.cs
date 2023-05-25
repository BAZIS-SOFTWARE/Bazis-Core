using Model;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer
{
    public static class GMSHConnector
    {
        private static bool isInit = false;

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshInitialize(int argc, ref String argv, 
                                                  int readConfigFile, int run, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshFinalize(ref int error);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshOpen([MarshalAs(UnmanagedType.LPStr)] String src, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshMerge([MarshalAs(UnmanagedType.LPStr)] String src, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshWrite([MarshalAs(UnmanagedType.LPStr)] String src, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int gmshClear(ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshModelMeshGenerate(int dim, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshOptionSetNumber([MarshalAs(UnmanagedType.LPStr)] String src,
                                                        double value, ref int ierr);

        public static void Init() 
        {
            if (!isInit)
            {
                int error = 0;
                var str = String.Empty;
                gmshInitialize(0, ref str, 0, 0, ref error);
                isInit = true;
            }
        }

        public static ModelData GMSHCreateFromScript(string srcPath, string dstExt, string objTypeCreate)
        {
            if (isInit)
            {
                var dim = new Dictionary<string, int>()
                        {
                            { "Элементы1D", 1 }, { "Линия", 1},
                            { "Элементы2D" , 2 }, { "Элементы3D", 3 }
                        };
                if (File.Exists(srcPath) && dim.ContainsKey(objTypeCreate))
                {
                    var error = 0;
                    gmshOpen(srcPath, ref error);
                    var file = Path.GetFileNameWithoutExtension(srcPath);
                    var folder = Path.GetDirectoryName(srcPath);
                    var dstFile = folder + @"\" + file + dstExt;
                    switch (dstExt)
                    {
                        default:
                            gmshOptionSetNumber("Mesh.SaveAll", 1, ref error);
                            gmshModelMeshGenerate(dim[objTypeCreate], ref error);
                            gmshOptionSetNumber("Mesh.SaveGroupsOfNodes", 2, ref error);
                            gmshWrite(dstFile, ref error);
                            var data = new LoadModelFromGMSHTextFile();
                            var modelData = data.Load(dstFile);
                            gmshClear(ref error);
                            return modelData;
                    }
                }
            }
            return null;
        }

        public static void Dispose()
        {
            if(isInit)
            {
                isInit = false;
                var error = 0;
                gmshFinalize(ref error);
            }
        }
    }
}
