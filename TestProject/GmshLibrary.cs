using System;
using System.IO;
using GmshApi;

namespace TestProject_test
{
    /// <summary>
    /// Определяет путь до нативной библиотеки gmsh для текущей операционной системы.
    /// </summary>
    internal static class GmshLibrary
    {
        /// <summary>
        /// Путь до нативной библиотеки gmsh в каталоге GmshApi репозитория.
        /// </summary>
        public static string FilePath => Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "GmshApi", GmshKernel.LibraryFileName));
    }
}
