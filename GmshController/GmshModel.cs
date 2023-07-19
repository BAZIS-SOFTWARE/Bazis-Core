using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshController
{ 
    /// <summary>
    /// Класс с группой методов для работы с моделью
    /// </summary>
    public static class GmshModel
    {
        /// <summary>
        /// Добавляет новую модель на Gmsh сцену и делает ее текущей
        /// </summary>
        /// <param name="src">[In]Строка идентификатор для новой модели</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelAdd(String src, ref int ierr);

        /// <summary>
        /// Устанавливает текущую модель в Gmsh сцене
        /// </summary>
        /// <param name="name">[In]Строка существующий идентификатор для текущей модели</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelSetCurrent(String name, ref int ierr);
        /// <summary>
        /// Возвращает имя текущей модели
        /// </summary>
        /// <param name="name">[Out]Имя текущей модели</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetCurrent(ref IntPtr name, ref int ierr);
        /// <summary>
        /// Удаляет текущую модель из Gmsh сцены
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelRemove(ref int ierr);
        /// <summary>
        /// Удаляет все модели из Gmsh сцены
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshClear(ref int ierr);
        /// <summary>
        /// Возвращает весь список моделей в gmsh API
        /// </summary>
        /// <param name="names">[Out]Массив имен моделей</param>
        /// <param name="names_n">[Out]Длина массива</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelList(ref IntPtr names, ref IntPtr names_n, ref int ierr);
    }
}
