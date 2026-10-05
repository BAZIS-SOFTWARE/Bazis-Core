using System;
using System.Runtime.InteropServices;

namespace GmshApi
{
    /// <summary>
    /// GmshKernel
    /// </summary>
    public static class GmshKernel
    {
        private static bool IsWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// Указатель библиотеки, возвращенный вызовом LoadLibrary (Windows) или DlOpen (Linux)
        public static IntPtr DLL { get; set; }

        /// <summary>
        /// Имя файла нативной библиотеки gmsh по умолчанию для текущей ОС
        /// </summary>
        public static string LibraryFileName => IsWindows ? "gmsh.dll" : "libgmsh.so";

        /// <summary>
        /// Загружает библиотеку, указанную параметром dllToLoad (Windows)
        /// </summary>
        /// <param name="dllToLoad">Путь до dll файла</param>
        /// <returns>Указатель на библиотеку</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        /// <summary>
        /// Возвращает указатель на процедуру procedureName, взятую из библиотеки на которую указывает hModule (Windows)
        /// </summary>
        /// <param name="hModule">Указатель библиотеки, возвращенный вызовом LoadLibrary</param>
        /// <param name="procedureName">Название процедуры</param>
        /// <returns>Указатель на процедуру</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        /// <summary>
        /// Выгружает библиотеку, на которую указывает hModule (Windows)
        /// </summary>
        /// <param name="hModule">Указатель библиотеки, возвращенный вызовом LoadLibrary</param>
        /// <returns>Признак успешной выгрузки</returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// Загружает библиотеку, указанную параметром fileName (Linux)
        /// </summary>
        /// <param name="fileName">Путь до so файла</param>
        /// <param name="flags">Флаги загрузки, например RTLD_NOW | RTLD_GLOBAL</param>
        /// <returns>Указатель на библиотеку</returns>
        [DllImport("libdl.so.2", EntryPoint = "dlopen")]
        public static extern IntPtr DlOpen(string fileName, int flags);

        /// <summary>
        /// Возвращает указатель на символ symbol из библиотеки, на которую указывает handle (Linux)
        /// </summary>
        /// <param name="handle">Указатель библиотеки, возвращенный вызовом DlOpen</param>
        /// <param name="symbol">Название символа</param>
        /// <returns>Указатель на символ</returns>
        [DllImport("libdl.so.2", EntryPoint = "dlsym")]
        public static extern IntPtr DlSym(IntPtr handle, string symbol);

        /// <summary>
        /// Выгружает библиотеку, на которую указывает handle (Linux)
        /// </summary>
        /// <param name="handle">Указатель библиотеки, возвращенный вызовом DlOpen</param>
        /// <returns>0 при успехе, иначе код ошибки</returns>
        [DllImport("libdl.so.2", EntryPoint = "dlclose")]
        public static extern int DlClose(IntPtr handle);

        /// <summary>
        /// Возвращает текст последней ошибки, накопленной DlOpen/DlSym/DlClose (Linux)
        /// </summary>
        /// <returns>Указатель на строку с описанием ошибки</returns>
        [DllImport("libdl.so.2", EntryPoint = "dlerror")]
        public static extern IntPtr DlError();

        /// <summary>
        /// Загружает нативную библиотеку, выбирая способ загрузки в зависимости от ОС
        /// </summary>
        /// <param name="libraryPath">Путь до нативной библиотеки (gmsh.dll или libgmsh.so)</param>
        /// <returns>Указатель на библиотеку</returns>
        public static IntPtr Load(string libraryPath)
        {
            var handle = IsWindows
                ? LoadLibrary(libraryPath)
                : DlOpen(libraryPath, 0x00002 | 0x00100);

            if (handle == IntPtr.Zero)
                throw new DllNotFoundException($"Не удалось загрузить '{libraryPath}': {GetErrorMessage()}");

            DLL = handle;
            return handle;
        }

        /// <summary>
        /// Выгружает ранее загруженную библиотеку в зависимости от ОС
        /// </summary>
        public static void Unload()
        {
            if (DLL == IntPtr.Zero)
                return;

            if (IsWindows)
                FreeLibrary(DLL);
            else
                DlClose(DLL);

            DLL = IntPtr.Zero;
        }

        /// <summary>
        /// Конвертирует неуправляемый указатель на процедуру в делегат C# для возможности вызова
        /// </summary>
        /// <typeparam name="T">Объявленный с# делегат</typeparam>
        /// <param name="functionName">Имя функции в библиотеке</param>
        /// <returns>Делегат C#</returns>
        public static T GetFunction<T>(string functionName)
        {
            var funPtr = IsWindows
                ? GetProcAddress(DLL, functionName)
                : DlSym(DLL, functionName);

            if (funPtr == IntPtr.Zero)
                throw new EntryPointNotFoundException($"Символ '{functionName}' не найден: {GetErrorMessage()}");

            return Marshal.GetDelegateForFunctionPointer<T>(funPtr);
        }

        /// <summary>
        /// Возвращает описание последней ошибки в зависимости от ОС
        /// </summary>
        private static string GetErrorMessage()
        {
            if (IsWindows)
                return $"Win32 error {Marshal.GetLastWin32Error()}";

            var errPtr = DlError();
            return errPtr == IntPtr.Zero ? "unknown error" : Marshal.PtrToStringAnsi(errPtr);
        }
    }
}
