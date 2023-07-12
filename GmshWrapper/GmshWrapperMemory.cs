using System;
using System.Runtime.InteropServices;
using Model.GmshController;

namespace Model.GmshWrapper
{
    /// <summary>
    /// Предоставляет основные методы для перевода неуправляемой памяти в управляемую
    /// </summary>
    public static class GmshWrapperMemory
    {
        /// <summary>
        /// Извлекает данные из матрицы, находящейся в неуправляемой памяти
        /// </summary>
        /// <typeparam name="T">[In]T = valueType</typeparam>
        /// <param name="tagsData">[In]Вектор векторов идентификаторов</param>
        /// <param name="lengthPerDim">[In]Длина вектора идентификаторов</param>
        /// <param name="dimCount">[In]Размерность внешнего вектора</param>
        /// <returns>Управляемая массив T[][]</returns>
        public static T[][] GetElementData<T>(IntPtr tagsData, IntPtr lengthPerDim, IntPtr dimCount)
        {
            var dCount = dimCount.ToInt32();
            var data = new T[dCount][];
            var tagsDataPtr = new IntPtr[dCount];
            var lengthData = new IntPtr[dCount];
            Marshal.Copy(tagsData, tagsDataPtr, 0, dCount);
            Marshal.Copy(lengthPerDim, lengthData, 0, dCount);
            GmshGeneral.gmshFree(lengthPerDim);
            for (var i = 0; i < dCount; ++i)
                data[i] = GetTypesData<T>(tagsDataPtr[i], lengthData[i]);
            GmshGeneral.gmshFree(tagsData);
            return data;
        }
        /// <summary>
        /// Извлекает данные из вектора, находящейся в неуправляемой памяти
        /// </summary>
        /// <typeparam name="T">[In]T = valueType</typeparam>
        /// <param name="src">[In]Вектор с данными</param>
        /// <param name="eCount">[In]Длина вектора</param>
        /// <returns>Управляемый массив T</returns>
        public static T[] GetTypesData<T>(IntPtr src, IntPtr eCount)
        {
            var count = eCount.ToInt32();
            T value = default;
            var size = Marshal.SizeOf(value) * count;
            var buffer = new byte[size];
            var dst = new T[count];
            Marshal.Copy(src, buffer, 0, size);
            Buffer.BlockCopy(buffer, 0, dst, 0, size);
            GmshGeneral.gmshFree(src);
            return dst;
        }
        /// <summary>
        /// Извлекает строку из неуправляемой памяти
        /// </summary>
        /// <param name="name">[In]Строка</param>
        /// <returns>Строка в управляемой памяти</returns>
        public static string GetName(IntPtr name)
        {
            var result = Marshal.PtrToStringAnsi(name);
            GmshGeneral.gmshFree(name);
            return result;
        }
        /// <summary>
        /// Извлекает массив строк из неуправляемой памяти в управляемую
        /// </summary>
        /// <param name="array">[In]Массив строк</param>
        /// <param name="length">[In]Длина массива</param>
        /// <returns>Массив строк в управляемой памяти</returns>
        public static string[] GetArrayOfNames(IntPtr array, IntPtr length)
        {
            var arrayLength = length.ToInt32();
            var stringArray = new IntPtr[arrayLength];
            var result = new string[arrayLength];
            Marshal.Copy(array, stringArray, 0, arrayLength);
            for (var i = 0; i < arrayLength; ++i)
                result[i] = Marshal.PtrToStringAnsi(stringArray[i]);
            return result;
        }
    }
}
