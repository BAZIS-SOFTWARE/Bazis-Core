using System;
using System.Runtime.InteropServices;

namespace GmshApi.GmshKernel
{
    /// <summary>
    /// Класс с группой обобщенных методов для установки предварительных значений Gmsh Api
    /// </summary>
    public static class KernelGeneral
    {
        /// <summary>
        /// Задает значение поля категории опций
        /// </summary>
        /// <param name="src">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[In]Значение поля категории</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOptionSetNumber(string src, double value, ref int ierr);
        /// <summary>
        /// Получает значение из поля категории опций
        /// </summary>
        /// <param name="src">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[Out]Значение поля категории</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOptionGetNumber(string src, ref double value, ref int ierr);

        /// <summary>
        /// Устанавливает строку параметров для категории опций
        /// </summary>
        /// <param name="name">[Out]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[In]Значение поля категории</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOptionSetString(string name, String value, ref int ierr);
        /// <summary>
        /// Выделяет неуправляемую память
        /// </summary>
        /// <param name="n">[In]Количество байт</param>
        /// <returns>Указатель на неуправляемую память</returns>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gmshMalloc(IntPtr n);
        /// <summary>
        /// Очищает неуправляемую память
        /// </summary>
        /// <param name="ptr">[In]Указатель на блок памяти</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshFree(IntPtr ptr);
        /// <summary>
        /// Возращает сообщение об последней ошибки
        /// </summary>
        /// <param name="error">[Out]Строка с ошибкой</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshLoggerGetLastError(ref IntPtr error, ref int ierr);
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
