using System;
using System.Runtime.InteropServices;

namespace GmshApi
{
    /// <summary>
    /// GmshKernel
    /// </summary>
    public static class GmshKernel
    {
        /// Указатель библиотеки, возвращенный вызовом LoadLibrary
        public static IntPtr DLL { get; set; }
        /// <summary>
        /// Загружает библиотеку, указанную параметром dllToLoad
        /// </summary>
        /// <param name="dllToLoad">Путь до dll файла</param>
        /// <returns>Указатель на библиотеку</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        /// <summary>
        /// Возвращает указатель на процедуру procedureName, взятую из библиотеки на которую указывает hModule
        /// </summary>
        /// <param name="hModule">Указатель библиотеки, возвращенный вызовом LoadLibrary</param>
        /// <param name="procedureName">Название процедуры</param>
        /// <returns>Указатель на процедуру</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        /// <summary>
        /// Конвертирует неуправляемый указатель на процедуру в делегат C# для возможности вызова
        /// </summary>
        /// <typeparam name="T">Объявленный с# делегат</typeparam>
        /// <param name="functionName">Имя функции в библиотеке</param>
        /// <returns>Делегат C#</returns>
        public static T GetFunction<T>(string functionName)
        {
            var funPtr = GetProcAddress(DLL, functionName);
            return Marshal.GetDelegateForFunctionPointer<T>(funPtr);
        }
    }
}
