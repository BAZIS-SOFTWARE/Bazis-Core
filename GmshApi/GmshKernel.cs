using System;
using System.Runtime.InteropServices;

namespace GmshApi.GmshKernel
{
    /// <summary>
    /// GmshKernel
    /// </summary>
    public static class GmshKernel
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
    }
}
