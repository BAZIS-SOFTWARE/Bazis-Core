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

        /// <inheritdoc/>
        public int AddVolume(int[] shellTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelGeoAddVolume(shellTags, (IntPtr)shellTags.Length, tag, ref ierr);
        }
        /// <inheritdoc/>
        public int AddSurfaceLoop(int[] surfaceTags, int tag = -1)
        {
            var ierr = 0;
            return gmshModelGeoAddSurfaceLoop(surfaceTags, (IntPtr)surfaceTags.Length, tag, ref ierr);
        }
        /// <inheritdoc/>
        public void Synchronize()
        {
            var ierr = 0;
            gmshModelGeoSynchronize(ref ierr);
        }
    }
}
