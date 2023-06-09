using ModelParcer.GmshController;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer.GmshUser
{
    public static class GmshUserMemory
    {
        public static void GetElementData<T>(out T[][] data, IntPtr tagsData, IntPtr lengthPerDim, IntPtr dimCount)
        {
            var dCount = dimCount.ToInt32();
            data = new T[dCount][];
            var tagsDataPtr = new IntPtr[dCount];
            var lengthData = new IntPtr[dCount];
            Marshal.Copy(tagsData, tagsDataPtr, 0, dCount);
            Marshal.Copy(lengthPerDim, lengthData, 0, dCount);
            GmshGeneral.gmshFree(lengthPerDim);
            for (var i = 0; i < dCount; ++i)
                GetTypesData(tagsDataPtr[i], out data[i], lengthData[i]);
            GmshGeneral.gmshFree(tagsData);
        }

        public static void GetTypesData<T>(IntPtr src, out T[] dst, IntPtr eCount)
        {
            var count = eCount.ToInt32();
            T value = default;
            var size = Marshal.SizeOf(value) * count;
            var buffer = new byte[size];
            dst = new T[count];
            Marshal.Copy(src, buffer, 0, size);
            Buffer.BlockCopy(buffer, 0, dst, 0, size);
            GmshGeneral.gmshFree(src);
        }
    }
}
