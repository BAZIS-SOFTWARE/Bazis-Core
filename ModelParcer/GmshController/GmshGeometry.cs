using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer.GmshController
{
    /// <summary>
    /// Класс с группой методов для работы с геометрией
    /// </summary>
    public static class GmshGeometry
    {
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
        /// Получает данные об идентификаторе модели
        /// </summary>
        /// <param name="tag">[In]Идентификатор</param>
        /// <param name="step">[In]Шаг >=0, указывает индекс в последовательности</param>
        /// <param name="dataType">[Out]Вектор строк типов данных вида {NodeData,ElementData,ElementNodeData}</param>
        /// <param name="tags">[Out]Вектор идентификаторов для соответсвующих типов данных</param>
        /// <param name="tags_n">[Out]Длина вектора идентификаторов</param>
        /// <param name="data">[Out]Массив векторов записей (точки, элементы...) соответствующие их идентификаторам</param>
        /// <param name="data_n">[Out]Вектор длин записей</param>
        /// <param name="data_nn">[Out]Размерность внуреннего массива</param>
        /// <param name="time">[Out]Опциональное ассоциированое с данными время Double</param>
        /// <param name="numComponents">[Out]Длина компонента в записи</param>
        /// <param name="ierr">[Out]Флаг ошибки</param>
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshViewGetModelData(int tag, int step, ref IntPtr dataType,
                                                       ref IntPtr tags, ref IntPtr tags_n,
                                                       ref IntPtr data, ref IntPtr data_n, ref IntPtr data_nn,
                                                       ref IntPtr time, ref int numComponents,
                                                       ref int ierr);
    }
}
