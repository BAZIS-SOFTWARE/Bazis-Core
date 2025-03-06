using System;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Geo
    {
        private delegate int gmshModelGeoAddSurfaceLoopFun(int[] surfaceTags, IntPtr surfaceTags_n, int tag, ref int ierr);
        private gmshModelGeoAddSurfaceLoopFun gmshModelGeoAddSurfaceLoop = GetFunction<gmshModelGeoAddSurfaceLoopFun>("gmshModelGeoAddSurfaceLoop");
        
        private delegate int gmshModelGeoAddVolumeFun(int[] shellTags, IntPtr shellTags_n, int tag, ref int ierr);
        private gmshModelGeoAddVolumeFun gmshModelGeoAddVolume = GetFunction<gmshModelGeoAddVolumeFun>("gmshModelGeoAddVolume");

        private delegate void gmshModelGeoSynchronizeFun(ref int ierr);
        private gmshModelGeoSynchronizeFun gmshModelGeoSynchronize = GetFunction<gmshModelGeoSynchronizeFun>("gmshModelGeoSynchronize");

        /// <summary>
        /// Формирует объем из заданных замкнутых контуров, первый элемент массива замкнутая оболочка, следующие элементы
        /// указывают отверстия в геометрии
        /// </summary>
        /// <param name="shellTags">Массив идентификаторов замкнутых контуров</param>
        /// <param name="tag">Идентификатор объема или -1 для автоматического назначения</param>
        /// <returns>Идентификатор объема</returns>
        public int AddVolume(int[] shellTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelGeoAddVolume(shellTags, (IntPtr)shellTags.Length, tag, ref ierr);
        }
        /// <summary>
        /// Создает замкнутый контур из массива поверхностей
        /// </summary>
        /// <param name="surfaceTags">Массив поверхностей</param>
        /// <param name="tag">Идентификатор поверхности или -1 для автоматического назначения</param>
        /// <returns>Идентификатор замкнутого контура</returns>
        public int AddSurfaceLoop(int[] surfaceTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelGeoAddSurfaceLoop(surfaceTags, (IntPtr)surfaceTags.Length, tag, ref ierr);
        }
        /// <summary>
        /// Синхронизировать изменения с ядром OCC, например после манипуляции с геометрическими сущностями(добавление точек, булевы операции)
        /// </summary>
        public void Synchronize()
        {
            var ierr = 0;
            gmshModelGeoSynchronize(ref ierr);
        }
    }
}
