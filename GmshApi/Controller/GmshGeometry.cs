using System;
using System.Runtime.InteropServices;

namespace GmshApi.Controller
{
    /// <summary>
    /// Класс с группой методов для работы с геометрией
    /// </summary>
    public static class GmshGeometry 
    {
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
        /// Получает геометрию модели в виде вектора пар размерность : идентификатор
        /// </summary>
        /// <param name="dimTags">[Out]Вектор пар размерность : идентификатор</param>
        /// <param name="dimTags_n">[Out]Размерность вектора пар</param>
        /// <param name="dim">[In]Размерность для получения, -1 все размерности</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelOccGetEntities(ref IntPtr dimTags, ref IntPtr dimTags_n, int dim, ref int ierr);
        /// <summary>
        /// Задает имя сущности для текщих dim:tag
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="name">[In]Имя сущности</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelSetEntityName(int dim, int tag, String name, ref int ierr);
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
        /// Удаляет текущую сущность
        /// </summary>
        /// <param name="name">[In]Имя сущности</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelRemoveEntityName(String name, ref int ierr);
        /// <summary>
        /// Возвращает ограничивающий параллелепипед
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="xmin">[Out]Минимальное значение X</param>
        /// <param name="ymin">[Out]Минимальное значение Y</param>
        /// <param name="zmin">[Out]Минимальное значение Z</param>
        /// <param name="xmax">[Out]Максимальное значение X</param>
        /// <param name="ymax">[Out]Максимальное значение Y</param>
        /// <param name="zmax">[Out]Максимальное значение Z</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetBoundingBox(int dim, int tag, ref double xmin,
                                                          ref double ymin, ref double zmin,
                                                          ref double xmax, ref double ymax,
                                                          ref double zmax, ref int ierr);
        /// <summary>
        /// Получает петлю кривых для заданного идентификатора поверхности
        /// </summary>
        /// <param name="surfaceTag">[In]Идентификатор поверхности</param>
        /// <param name="curveLoopTags">[Out]Вектор идентификаторов петлей кривых</param>
        /// <param name="curveLoopTags_n">[Out]Вектор длин идентификаторов петлей кривых</param>
        /// <param name="curveTags">[Out]Вектор наборов идентификаторов кривых</param>
        /// <param name="curveTags_n">[Out]Вектор длин наборов идентификаторов кривых</param>
        /// <param name="curveTags_nn">[Out]Длина внешнего вектора наборов идентификаторов кривых</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelOccGetCurveLoops(int surfaceTag, ref IntPtr curveLoopTags, 
                                                            ref IntPtr curveLoopTags_n, ref IntPtr curveTags,
                                                            ref IntPtr curveTags_n, ref IntPtr curveTags_nn,
                                                            ref int ierr);
        /// <summary>
        /// Получает петлю поверхностей для заданного идентификатора объема
        /// </summary>
        /// <param name="volumeTag">[In]Идентификатор объема</param>
        /// <param name="surfaceLoopTags">[Out]Вектор идентификаторов петлей поверхностей</param>
        /// <param name="surfaceLoopTags_n">[Out]Вектор длин идентификаторов петлей поверхностей</param>
        /// <param name="surfaceTags">[Out]Вектор наборов идентификаторов поверхностей</param>
        /// <param name="surfaceTags_n">[Out]Вектор длин наборов идентификаторов поверхностей</param>
        /// <param name="surfaceTags_nn">[Out]Длина внешнего вектора наборов идентификаторов поверхностей</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelOccGetSurfaceLoops(int volumeTag, ref IntPtr surfaceLoopTags,
                                                            ref IntPtr surfaceLoopTags_n, ref IntPtr surfaceTags,
                                                            ref IntPtr surfaceTags_n, ref IntPtr surfaceTags_nn,
                                                            ref int ierr);
        /// <summary>
        /// Получает радиусы кривизны для заданных параметрических координат
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Вектор параметрических координат</param>
        /// <param name="parametricCoord_n">[In]Доина вектора параметрических координат</param>
        /// <param name="curvatures">[Out]Векторы радиусов кривизны</param>
        /// <param name="curvatures_n">[Out]Длина вектора кривизны</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetCurvature(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n,
                                                        ref IntPtr curvatures, ref IntPtr curvatures_n, ref int ierr);

        /// <summary>
        /// Получает производную по указанной размерности и идентификатору и заданному набору параметров
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Вектор параметрических координат</param>
        /// <param name="parametricCoord_n">[In]Длина вектора параметрических координат</param>
        /// <param name="derivatives">[Out]Вектор производных</param>
        /// <param name="derivatives_n">[Out]Длина вектора производных</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetDerivative(int dim, int tag, double[] parametricCoord, IntPtr parametricCoord_n,
                                                         ref IntPtr derivatives, ref IntPtr derivatives_n, ref int ierr);

        /// <summary>
        /// Получает вторую производную по указанной размерности и идентификатору и заданному набору параметров
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Вектор параметрических координат</param>
        /// <param name="parametricCoord_n">[In]Длина вектора параметрических координат</param>
        /// <param name="derivatives">[Out]Вектор вторых производных</param>
        /// <param name="derivatives_n">[Out]Длина вектора вторых производных</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetSecondDerivative(int dim, int tag, double[] parametricCoord,
                                                               IntPtr parametricCoord_n, ref IntPtr derivatives,
                                                               ref IntPtr derivatives_n, ref int ierr);
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
        /// Перепараметризирует пару размерность : идентификатор в параметризацию поверхностей
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="parametricCoord">[In]Вектор параметров</param>
        /// <param name="parametricCoord_n">[In]Длина вектора параметров</param>
        /// <param name="surfaceTag">[In]Идентификатор поверхности</param>
        /// <param name="surfaceParametricCoord">[Out]Вектор репараметризированных параметров</param>
        /// <param name="surfaceParametricCoord_n">[Out]Длина вектора</param>
        /// <param name="which">[In]Флаг совпадений параметрических поверхностей</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelReparametrizeOnSurface(int dim, int tag, double[] parametricCoord,
                                                                  IntPtr parametricCoord_n, int surfaceTag,
                                                                  ref IntPtr surfaceParametricCoord,
                                                                  ref IntPtr surfaceParametricCoord_n,
                                                                  int which, ref int ierr);
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
        /// Полчучает тип сущности по указанной паре размерность : идентификатор
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="entityType">[Out]Строка типа</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetType(int dim, int tag, ref IntPtr entityType, ref int ierr);
        /// <summary>
        /// Возвращает размерность текущего геометрического объекта
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Размерность</returns>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gmshModelGetDimension(ref int ierr);
        /// <summary>
        /// Определяет находится ли точка параметрическая или локальная внутри массива
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="coord">[In]Масив координат</param>
        /// <param name="coord_n">[In]Количество координат</param>
        /// <param name="parametric">[In]Флаг являяется ли координата параметрической</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns>Число координат лежащих внутри модели</returns>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gmshModelIsInside(int dim, int tag, double[] coord, IntPtr coord_n, 
                                                   int parametric, ref int ierr);
        /// <summary>
        /// Удаляет OCC сущности(.step,.brep,.igs)
        /// </summary>
        /// <param name="dimTags">[In]Массив пар dim:tag</param>
        /// <param name="dimTags_n">[In]Длина массива</param>
        /// <param name="recursive">[In]Рекурсивно улалять граничные элементы</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelOccRemove(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);
        /// <summary>
        /// Удаляет Built-in сущности 
        /// </summary>
        /// <param name="dimTags">[In]Массив пар dim:tag</param>
        /// <param name="dimTags_n">[In]Длина массива</param>
        /// <param name="recursive">[In]Рекурсивно улалять граничные элементы</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelRemoveEntities(int[] dimTags, IntPtr dimTags_n, int recursive, ref int ierr);

        /// <summary>
        /// Синхронизировать - вызывается для правильного отображения сущностей OCC
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelOccSynchronize(ref int ierr);
        /// <summary>
        /// Синхронизировать - вызывается для правильного отображения сущностей
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGeoSynchronize(ref int ierr);
        /// <summary>
        /// Устанавливает видимость для указанных пар dim:tags геометрических сущностей
        /// </summary>
        /// <param name="dimTags">[In]Массив пар dim:tag</param>
        /// <param name="dimTags_n">[In]Длина массива</param>
        /// <param name="value">[In]Значение видимости</param>
        /// <param name="recursive">[In]Распространить видимость рекурсивно на граничные элементы</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelSetVisibility(int[] dimTags, IntPtr dimTags_n, int value,
                                                         int recursive, ref int ierr);
    }
}
