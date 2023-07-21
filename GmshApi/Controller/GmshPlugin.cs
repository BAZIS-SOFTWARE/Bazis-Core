using System;
using System.Runtime.InteropServices;

namespace GmshApi
{
    public static class GmshPlugin
    {
        /// <summary>
        /// Установить значение через плагин
        /// </summary>
        /// <param name="name">[In]Имя плагина</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="value">[In]Значение в опции</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshPluginSetNumber(String name, String option, double value, ref int ierr);
        /// <summary>
        /// Установить набор значений через плагин
        /// </summary>
        /// <param name="name">[In]Имя плагина</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="value">[In]Значения в опции</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshPluginSetString(String name, String option, String value, ref int ierr);

        /// <summary>
        /// Запускает плагин с имененм name и возвращает View если имеется
        /// </summary>
        /// <param name="name">[In]Имя плагина</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns></returns>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gmshPluginRun(String name, ref int ierr);
    }
}
