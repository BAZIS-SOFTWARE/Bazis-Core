using System;
using System.Runtime.InteropServices;

namespace GmshApi
{
    /// <summary>
    /// Класс с группой методов для работы с сеткой
    /// </summary>
    public static class GmshMesh
    {
        /// <summary>
        /// Генерирует сетку в текущей размерности
        /// </summary>
        /// <param name="dim">[In]Размерность в которой нужно сгенерировать модель</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGenerate(int dim, ref int ierr);

        /// <summary>
        /// Переводит большую часть сетки в четырехугольники
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshRecombine(ref int ierr);

        /// <summary>
        /// Равномерно уплотняет сетку у текущей модели
        /// </summary>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshRefine(ref int ierr);

        /// <summary>
        /// Получает все элементы в текущей модели, их идентификаторы и идентификаторы точек внутри элемента
        /// </summary>
        /// <param name="elementTypes">[Out]Массив строк типов элементов</param>
        /// <param name="elementTypes_n">[Out]Длина массива строк типов элементов</param>
        /// <param name="elementTags">[Out]Матрица - массив идентификаторов для каждого типа элемента</param>
        /// <param name="elementTags_n">[Out]Массив - длина массива идентификаторов для типов элементов</param>
        /// <param name="elementTags_nn">[Out]Длина массива типов объектов, дублирует elementTypes_n</param>
        /// <param name="nodeTags">[Out]Матрица - массив идентификаторов точек для типов элементов</param>
        /// <param name="nodeTags_n">[Out]Массив - длина массива идентификаторов точек для типов объекта</param>
        /// <param name="nodeTags_nn">[Out]Длина массива типов объектов, дублирует elementTypes_n</param>
        /// <param name="dim">[In]Размерность для получения точек, или -1 для получения всех размерностей</param>
        /// <param name="tag">[In]Индекс элемента в пределах dim, -1 для получения всех элементов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGetElements(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                           ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                           ref IntPtr elementTags_nn, ref IntPtr nodeTags,
                                                           ref IntPtr nodeTags_n, ref IntPtr nodeTags_nn,
                                                           int dim, int tag, ref int ierr);
        
        /// <summary>
        /// Получает свойство элемента
        /// </summary>
        /// <param name="elementType">[In]Тип элемента</param>
        /// <param name="elementName"></param>
        /// <param name="dim"></param>
        /// <param name="order"></param>
        /// <param name="numNodes"></param>
        /// <param name="localeNodeCoord"></param>
        /// <param name="localeNodeCoord_n"></param>
        /// <param name="numPrimaryNodes"></param>
        /// <param name="ierr"></param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGetElementProperties(int elementType, ref IntPtr elementName,
                                                                    ref int dim, ref int order, ref int numNodes,
                                                                    ref IntPtr localeNodeCoord,
                                                                    ref IntPtr localeNodeCoord_n, ref int numPrimaryNodes,
                                                                    ref int ierr);
        /// <summary>
        /// Извлекает все точки, связанные с текущей моделью, параметром dim,tag
        /// </summary>
        /// <param name="nodeTags">[Out]Массив идентификаторов точек</param>
        /// <param name="nodeTags_n">[Out]Длина массива идентификаторов точек</param>
        /// <param name="coord">[Out]Координаты точек x,y,z</param>
        /// <param name="coord_n">[Out]Количество координат точек</param>
        /// <param name="parametricCoord">[Out]Параметрические координаты u,u1,u2...</param>
        /// <param name="parametricCoord_n">[Out]Количество параметрических координат</param>
        /// <param name="dim">[In]Размерность для получения точек, или -1 для получения всех размерностей</param>
        /// <param name="tag">[In]Индекс элемента в пределах dim, -1 для получения всех элементов</param>
        /// <param name="includeBoundary">[In]Флаг получения ограничивающего параллелепипеда</param>
        /// <param name="returnParametricCoord">[In]Флаг получения координат</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGetNodes(ref IntPtr nodeTags, ref IntPtr nodeTags_n,
                                    ref IntPtr coord, ref IntPtr coord_n,
                                    ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                    int dim, int tag, int includeBoundary, int returnParametricCoord,
                                    ref int ierr);
        /// <summary>
        /// Задает количество точек на кривой
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="numNodes">[In]Число точек</param>
        /// <param name="meshType">[In]Алгоритм распределения точек</param>
        /// <param name="coef">[In]Коэффициент прогрессии</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshSetTransfiniteCurve(int tag, int numNodes, String meshType,
                                                                   double coef, ref int ierr);
        /// <summary>
        /// Находит элементы сетки по переданной координате x,y,z
        /// </summary>
        /// <param name="x">[In]Координата x</param>
        /// <param name="y">[In]Координата y</param>
        /// <param name="z">[In]Координата z</param>
        /// <param name="elementTags">[Out]Массив найденных элементов</param>
        /// <param name="elementTags_n">[Out]Длина массива</param>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="strict">[In]Строгий поиск?</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGetElementsByCoordinates(double x, double y, double z, ref IntPtr elementTags,
                                                                        ref IntPtr elementTags_n, int dim, 
                                                                        int strict, ref int ierr);

        /// <summary>
        /// Получает локальные координаты элемента с заданным идентификатором
        /// </summary>
        /// <param name="elementTag">[In]Идентификатор элемента</param>
        /// <param name="x">[In]Координата x</param>
        /// <param name="y">[In]Координата y</param>
        /// <param name="z">[In]Координата z</param>
        /// <param name="u">[Out]Параметрические координаты u</param>
        /// <param name="v">[Out]Параметрические координаты v</param>
        /// <param name="w">[Out]Параметрические координаты w</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGetLocalCoordinatesInElement(UIntPtr elementTag, double x, double y, double z,
                                                                            ref double u, ref double v, ref double w, 
                                                                            ref int ierr);
        /// <summary>
        /// Перестроить кеш точек
        /// </summary>
        /// <param name="onlyIfNecessary">[In]Флаг удаления</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshRebuildNodeCache(int onlyIfNecessary, ref int ierr);
        /// <summary>
        /// Перестроить кеш элементов
        /// </summary>
        /// <param name="onlyIfNecessary">[In]Флаг удаления</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshRebuildElementCache(int onlyIfNecessary, ref int ierr);

        /// <summary>
        /// Удаляет сетку свзанную с dim:tag всю вплоть до точек
        /// </summary>
        /// <param name="dimTags">[In]Пары dim:tag</param>
        /// <param name="dimTags_n">[In]Длина массива dim:tag</param>
        /// <param name="ierr">[Out]Флаг ошибок</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshClear(int[] dimTags, IntPtr dimTags_n, ref int ierr);
        /// <summary>
        /// Задать видимость элементов
        /// </summary>
        /// <param name="elementTags">[In]Массив элементов</param>
        /// <param name="elementTags_n">[In]Размер массива</param>
        /// <param name="value">[In]Значение</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshSetVisibility(IntPtr[] elementTags, IntPtr elementTags_n, 
                                                             int value, ref int ierr);

        /// <summary>
        /// Устанавливает алгоритм триангуляции для сетки
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="val">[In]Численное значение поля алгоритма</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// 
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshSetAlgorithm(int dim, int tag, int val, ref int ierr);
        /// <summary>
        /// Получить качество элемента
        /// </summary>
        /// <param name="elementTags">[In]Идентификаторы элементов</param>
        /// <param name="elementTags_n">[In]Количество идентификаторов</param>
        /// <param name="elementsQuality">[Out]Качество элементов</param>
        /// <param name="elementQuality_n">[Out]Количество элементов</param>
        /// <param name="qualityName">[In]Строка-запроса (minDetJac,maxDetJac,minSJ, minSICN, minSIGE, gamma,
        /// innerRadius, outerRadius, minIsotropy, angleShape, minEdge, maxEdge, volume) </param>
        /// <param name="task">[In]Task</param>
        /// <param name="numTasks">[In]NumTask</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGetElementQualities(IntPtr[] elementTags, IntPtr elementTags_n,
            ref IntPtr elementsQuality, ref IntPtr elementQuality_n,
            string qualityName,
            IntPtr task,
            IntPtr numTasks,
            ref int ierr);
        /// <summary>
        /// Добавить MeshField
        /// </summary>
        /// <param name="fieldType">[In]Тип поля</param>
        /// <param name="tag">[In]Идентификатор (может быть -1, выделится автоматически)</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        /// <returns></returns>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gmshModelMeshFieldAdd(String fieldType, int tag, ref int ierr);
        /// <summary>
        /// Удалить MeshField
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldRemove(int tag, ref int ierr);
        /// <summary>
        /// Возвращает список полей
        /// </summary>
        /// <param name="tags">[Out]Идентификаторы</param>
        /// <param name="tags_n">[Out]Количество идентификаторов</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldList(ref IntPtr tags, ref IntPtr tags_n, ref int ierr);
        /// <summary>
        /// Возвращает тип поля по заданному идентификатору
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="fieldType">[Out]Тип поля</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldGetType(int tag, ref IntPtr fieldType, ref int ierr);

        /// <summary>
        /// Задает значение опции поля
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Опция</param>
        /// <param name="value">[In]Значение</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldSetNumber(int tag, String option, double value, ref int ierr);
        /// <summary>
        /// Получает значение опции поля
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Опция</param>
        /// <param name="value">[In]Значение</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldGetNumber(int tag, String option, ref double value, ref int ierr);
        /// <summary>
        /// Задает строку значений для опции
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Опция</param>
        /// <param name="value">[In]Значения опции</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldSetString(int tag, String option, String value, ref int ierr);
        /// <summary>
        /// Возвращает строку значений для опции
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="value">[Out]Набор значений</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldGetString(int tag, String option, ref IntPtr value, ref int ierr);
        /// <summary>
        /// Задает список значений для опции 
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="values">[In]Массив значений</param>
        /// <param name="values_n">[In]количество значений</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldSetNumbers(int tag, String option, double[] values,
                                                               IntPtr values_n, ref int ierr);
        /// <summary>
        /// Возвращает список значений для опции 
        /// </summary>
        /// <param name="tag">[In]Идентификатор поля</param>
        /// <param name="option">[In]Имя опции</param>
        /// <param name="values">[Out]Массив значений типа double</param>
        /// <param name="values_n">[Out]Количество значений типа double</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldGetNumbers(int tag, String option, ref IntPtr values,
                                                               ref IntPtr values_n, ref int ierr);
        /// <summary>
        /// Установить поле как BackgroundMesh
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldSetAsBackgroundMesh(int tag, ref int ierr);
        /// <summary>
        /// Установить поле как BoundaryLayer
        /// </summary>
        /// <param name="tag">[In]Идентифкатор поля</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshFieldSetAsBoundaryLayer(int tag, ref int ierr);
    }
}