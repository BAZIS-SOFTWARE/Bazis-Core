using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshController
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
        public static extern void gmshOptionSetNumber(String src, double value, ref int ierr);
        /// <summary>
        /// Получает значение из поля категории опций
        /// </summary>
        /// <param name="src">[In]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[Out]Значение поля категории</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOptionGetNumber(String src, ref double value, ref int ierr);

        /// <summary>
        /// Устанавливает строку параметров для категории опций
        /// </summary>
        /// <param name="name">[Out]Строка в формате доступа к полю категории опций</param>
        /// <param name="value">[In]Значение поля категории</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOptionSetString(String name, String value, ref int ierr);
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
    }
}
