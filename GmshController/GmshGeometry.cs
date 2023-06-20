using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshController
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
    }
}
