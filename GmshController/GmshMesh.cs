using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshController
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
    }
}
