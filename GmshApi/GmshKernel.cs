using System;
using System.Runtime.InteropServices;

namespace GmshApi
{
    /// <summary>
    /// GmshKernel
    /// </summary>
    public static class GmshKernel
    {
        private static IntPtr DLL { get; set; }
        /// <inheritdoc/>
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        /// <inheritdoc/>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        /// <inheritdoc/>
        public static T GetFunction<T>(string functionName)
        {
            var funPtr = GetProcAddress(DLL, functionName);
            return Marshal.GetDelegateForFunctionPointer<T>(funPtr);
        }
    }
}
