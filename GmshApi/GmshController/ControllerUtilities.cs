using GmshApi.GmshKernel;
using System;
using System.Runtime.InteropServices;

namespace GmshApi.GmshController
{
    public static class ControllerUtilities
    {
        public static void GetObjectsData<T>(IntPtr src, out T[] dst, IntPtr eCount)
        {
            var count = eCount.ToInt32();
            T value = default;
            var size = Marshal.SizeOf(value) * count;
            var buffer = new byte[size];
            dst = new T[count];
            Marshal.Copy(src, buffer, 0, size);
            Buffer.BlockCopy(buffer, 0, dst, 0, size);
            KernelGeneral.gmshFree(src);
        }
        /// <summary>
        /// Извлекает строку из неуправляемой памяти
        /// </summary>
        /// <param name="name">[In]Строка</param>
        /// <returns>Строка в управляемой памяти</returns>
        public static string GetString(IntPtr name)
        {
            var result = Marshal.PtrToStringAnsi(name);
            KernelGeneral.gmshFree(name);
            return result;
        }
        /// <summary>
        /// Извлекает массив строк из неуправляемой памяти в управляемую
        /// </summary>
        /// <param name="array">[In]Массив строк</param>
        /// <param name="length">[In]Длина массива</param>
        /// <returns>Массив строк в управляемой памяти</returns>
        public static string[] GetArrayOfString(IntPtr array, IntPtr length)
        {
            var arrayLength = length.ToInt32();
            var stringArray = new IntPtr[arrayLength];
            var result = new string[arrayLength];
            Marshal.Copy(array, stringArray, 0, arrayLength);
            for (var i = 0; i < arrayLength; ++i)
            {
                result[i] = Marshal.PtrToStringAnsi(stringArray[i]);
                KernelGeneral.gmshFree(stringArray[i]);
            }
            KernelGeneral.gmshFree(array);
            return result;
        }
    }
}
