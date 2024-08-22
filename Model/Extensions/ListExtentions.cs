using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extensions
{
    /// <summary>
    /// ModelExtentions
    /// </summary>
    public static class ListExtentions
    {
        /// <summary>
        /// SwapList
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectArray"></param>
        /// <param name="firstIndex"></param>
        /// <param name="secondIndex"></param>
        /// <returns></returns>
        public static bool SwapList<T>(this List<T> objectArray, int firstIndex, int secondIndex)
        {

            // check for out of range
            if (objectArray.Count <= secondIndex || objectArray.Count <= firstIndex) return false;

            // swap index x and y
            T temp = objectArray[firstIndex];
            objectArray[firstIndex] = objectArray[secondIndex];
            objectArray[secondIndex] = temp;


            return true;
        }
    }
}
