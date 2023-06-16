using ModelParcer.GmshController;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ModelParcer.GmshUser
{
    /// <summary>
    /// Класс переводит неуправляемые данные в управляемые с освобождением неуправляемой памяти
    /// </summary>
    public static class GmshUserMemory
    {
        /// <summary>
        /// Извлекает данные из матрицы, находящейся в неуправляемой памяти
        /// </summary>
        /// <typeparam name="T">[In]T = valueType</typeparam>
        /// <param name="data">[Out]Матрица для заполнения</param>
        /// <param name="tagsData">[In]Вектор векторов идентификаторов</param>
        /// <param name="lengthPerDim">[In]Длина вектора идентификаторов</param>
        /// <param name="dimCount">[In]Размерность внешнего вектора</param>
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
        /// <summary>
        /// Извлекает данные из вектора, находящейся в неуправляемой памяти
        /// </summary>
        /// <typeparam name="T">[In]T = valueType</typeparam>
        /// <param name="src">[In]Вектор с данными</param>
        /// <param name="dst">[Out]Вектор для заполнения</param>
        /// <param name="eCount">[In]Длина вектора</param>
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
    }
}
