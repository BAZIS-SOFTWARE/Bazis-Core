using System;
using System.Runtime.InteropServices;

namespace GmshApi
{
    /// <summary>
    /// Класс с группой обобщенных методов для установки предварительных значений Gmsh Api
    /// </summary>
    public static class GmshGeneral
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

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshWrite(string fileName);
        /// <summary>
        /// Возращает сообщение об последней ошибки
        /// </summary>
        /// <param name="error">[Out]Строка с ошибкой</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshLoggerGetLastError(ref IntPtr error, ref int ierr);
    }
}
