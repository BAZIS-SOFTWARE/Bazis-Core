using System;
using System.Runtime.InteropServices;

namespace GmshApi.Controller
{
    /// <summary>
    /// Класс с группой методов для работы с подключением и завершением работы gmsh api
    /// </summary>
    public static class GmshConnection
    {
        /// <summary>
        /// Возвращает флаг инициализировано ли API
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Инициализировано ли API</returns>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gmshIsInitialized(ref int ierr);
        /// <summary>
        /// Инициализирует Gmsh Api
        /// </summary>
        /// <param name="argc">[In]Количество строковых аргументов командной строки</param>
        /// <param name="argv">[In]Массив строк аргуметов командной строки</param>
        /// <param name="readConfigFile">[In]Флаг - читать конфигурационный файл gmsh</param>
        /// <param name="run">[In]Флаг запуска приложения в командной строке</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshInitialize(int argc, ref string argv, int readConfigFile,
                                                  int run, ref int ierr);
        /// <summary>
        /// Завершает работу Gmsh Api
        /// </summary>
        /// <param name="error">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshFinalize(ref int ierr);
    }
}
