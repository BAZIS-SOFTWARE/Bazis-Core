using System;
using System.Runtime.InteropServices;
using static GmshApi.GmshKernel;


namespace GmshApi
{
    /// <inheritdoc/>
    internal static class Memory
    {
        private delegate IntPtr gmshMallocFun(IntPtr n);
        private static gmshMallocFun gmshMalloc = GetFunction<gmshMallocFun>("gmshMalloc");
        
        private delegate void gmshFreeFun(IntPtr p);
        private static gmshFreeFun gmshFree = GetFunction<gmshFreeFun>("gmshFree");

        /// <summary>
        /// Выделяет неуправляемую память
        /// </summary>
        /// <param name="n">[In]Количество байт</param>
        /// <returns>Указатель на неуправляемую память</returns>
        public static IntPtr Malloc(IntPtr n) => gmshMalloc(n);

        /// <summary>
        /// Очищает неуправляемую память
        /// </summary>
        /// <param name="p">[In]Указатель на блок памяти</param>
        public static void Free(IntPtr p) => gmshFree(p);

        /// <inheritdoc/>
        public static T[] GetObjectsData<T>(IntPtr src, IntPtr eCount) where T : struct
        {
            var count = eCount.ToInt32();
            T value = default;
            var size = Marshal.SizeOf(value) * count;
            var buffer = new byte[size];
            var dst = new T[count];
            if (src != IntPtr.Zero)//Защита от нулевого указателя
            {
                Marshal.Copy(src, buffer, 0, size);
                Buffer.BlockCopy(buffer, 0, dst, 0, size);
                Free(src);
            }
            return dst;
        }

        /// <inheritdoc/>
        public static T[][] GetArrayOfObjectsData<T>(IntPtr outArray, IntPtr outArrayLength, IntPtr inArray) where T : struct 
        {
            var outLength = outArrayLength.ToInt32();
            var tArray = new IntPtr[outLength];
            var result = new T[outLength][];
            var inLength = new IntPtr[outLength];
            if (outArray != IntPtr.Zero && inArray != IntPtr.Zero)//Защита от нулевого указателя
            {
                Marshal.Copy(outArray, tArray, 0, outLength);
                Marshal.Copy(inArray, inLength, 0, outLength);
                for (var i = 0; i < outLength; ++i)
                    result[i] = GetObjectsData<T>(tArray[i], inLength[i]);
                Free(outArray);
            }
            return result;
        }

        /// <inheritdoc/>
        public static string GetString(IntPtr ptr)
        {
            var result = string.Empty;
            if (ptr != IntPtr.Zero)//Защита от нулевого указателя
            {
                result = Marshal.PtrToStringAnsi(ptr);
                Free(ptr);
            }
            return result;
        }

        /// <inheritdoc/>
        public static string[] GetArrayOfString(IntPtr array, IntPtr length)
        {
            var arrayLength = length.ToInt32();
            var stringArray = new IntPtr[arrayLength];
            var result = new string[arrayLength];
            if (array != IntPtr.Zero)//Защита от нулевого указателя
            {
                Marshal.Copy(array, stringArray, 0, arrayLength);
                for (var i = 0; i < arrayLength; ++i)
                    result[i] = GetString(stringArray[i]);
                Free(array);
            }
            return result;
        }
    }
}
