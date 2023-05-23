using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer
{
    public class GMSHConnector : IDisposable
    {
        private bool isInit = false;

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshInitialize(int argc, ref String argv, 
                                                  int readConfigFile, int run, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int gmshIsInitialized(ref int error);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshFinalize(ref int error);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshOpen([MarshalAs(UnmanagedType.LPStr)] String src, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshWrite([MarshalAs(UnmanagedType.LPStr)] String src, ref int ierr);

        public GMSHConnector() 
        {
            int error = 0;
            var status = gmshIsInitialized(ref error);
            if (status == 0)
            {
                var str = String.Empty;
                gmshInitialize(0,ref str, 0, 0, ref error);
                isInit = true;
            }
        }

        public void GMSHExtractModelFromScript(string srcFile, string dstFile,
                                               string srcFolder = @"..\..\Examples\GMSHConnector\", 
                                               string dstFolder = @"..\..\Examples\GMSHConnector\")
        {
            var fullSrc = srcFolder + srcFile;
            if(File.Exists(fullSrc) && Directory.Exists(dstFolder))
            {
                var error = 0;
                var fullDst = dstFolder + dstFile;
                gmshOpen(fullSrc + " -save_all", ref error);
                if(error == 0)
                    gmshWrite(fullDst, ref error);
            }
        }


        public void Dispose()
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
