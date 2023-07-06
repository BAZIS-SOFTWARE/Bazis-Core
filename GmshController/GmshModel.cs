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
        /// <param name="src">[In]Строка существующий идентификатор для текущей модели</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelSetCurrent(String src, ref int ierr);

        /// <summary>
        /// Удаляет текущую модель из Gmsh сцены
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelRemove(ref int ierr);

        /// <summary>
        /// Получает список всех идентификаторов для заданной размерности
        /// </summary>
        /// <param name="dimTags">[Out]Массив пар размерность - идентификатор</param>
        /// <param name="dimTags_n">[Out]Длина массива пар размерность - идентификатор</param>
        /// <param name="dim">[In]Размерность для получения идентификаторов, -1 для всех размерностей</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetEntities(ref IntPtr dimTags, ref IntPtr dimTags_n,
                                                       int dim, ref int ierr);
        /// <summary>
        /// Получает координаты точек заданные параметром для указанного идентификатора и размерности
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Массив параметрических координат double</param>
        /// <param name="parameticCoord_n">[In]Длина массива</param>
        /// <param name="coord">[Out]Массив координаты точек</param>
        /// <param name="coord_n">[Out]Длина массива координат точек</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetValue(int dim, int tag, double[] parametricCoord,
                                                    IntPtr parameticCoord_n, ref IntPtr coord, 
                                                    ref IntPtr coord_n, ref int ierr);

        /// <summary>
        /// Получает граничные элементы для заданной пары размерность : идентификатор
        /// </summary>
        /// <param name="dimTags">[In]Пара размерность : идентификатор</param>
        /// <param name="dimTags_n">[In]Количество пар</param>
        /// <param name="outDimTags">[Out]Граничные элементы внутри пар</param>
        /// <param name="outDimTags_n">[Out]Количество элементов</param>
        /// <param name="combined">[In]Флаг объединения резултатов для пар</param>
        /// <param name="oriented">[In]Флаг наличия знака, который умножается на идентификатор</param>
        /// <param name="recursive">[In]Флаг рекурсии - получить все до точек</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetBoundary(int[] dimTags, IntPtr dimTags_n, ref IntPtr outDimTags,
                                                       ref IntPtr outDimTags_n, int combined, int oriented,
                                                       int recursive, ref int ierr);
        /// <summary>
        /// Получает физическое имя для заданной размерности : идентификатора
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="name">[Out]Строка имя</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetPhysicalName(int dim, int tag, ref IntPtr name, ref int ierr);

        /// <summary>
        /// Получает имя сущности с указанной размерностью и идентификатором
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="name">[In]Имя =</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetEntityName(int dim, int tag, ref IntPtr name, ref int ierr);
        /// <summary>
        /// Получает параметризацию сущности заданной парой размерность : идентификатор
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="coord">[In]Вектор координат</param>
        /// <param name="coord_n">[In]Размерность вектора координат</param>
        /// <param name="parametricCoord">[Out]Вектор параметрических координат</param>
        /// <param name="parametricCoord_n">[Out]Длина вектора параметрические координат</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetParametrization(int dim, int tag, double[] coord, IntPtr coord_n,
                                                              ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                                              ref int ierr);
        /// <summary>
        /// Получает минимальные максимальные границы параметризации
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="min">[Out]Вектор минимальных границ</param>
        /// <param name="min_n">[Out]Длина вектора минимальных границ</param>
        /// <param name="max">[Out]Вектор максимальных границ</param>
        /// <param name="max_n">[Out]Длина вектора максимальных границ</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetParametrizationBounds(int dim, int tag, ref IntPtr min,
                                                                    ref IntPtr min_n, ref IntPtr max,
                                                                    ref IntPtr max_n, ref int ierr);
        /// <summary>
        /// Получает соседей верхнего\нижнего уровня для текущей пары размерность : идентификатор
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="upward">[Out]Идентификаторы верхнего уровня</param>
        /// <param name="upward_n">[Out]Длина вектора идентификаторов</param>
        /// <param name="downward">[Out]Идентификаторы нижнего уровня</param>
        /// <param name="downward_n">[Out]Длина вектора идентификаторов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetAdjacencies(int dim, int tag, ref IntPtr upward, ref IntPtr upward_n,
                                                          ref IntPtr downward, ref IntPtr downward_n, ref int ierr);
        /// <summary>
        /// Полчучает тип сущности по указанной паре размерность : идентификатор
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="entityType">[Out]Строка типа</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetType(int dim, int tag, ref IntPtr entityType, ref int ierr);

        /// <summary>
        /// Удаляет все модели из Gmsh сцены
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns></returns>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gmshClear(ref int ierr);
    }
}
