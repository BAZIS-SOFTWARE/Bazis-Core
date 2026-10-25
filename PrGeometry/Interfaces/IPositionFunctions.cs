using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry.Interfaces
{
    /// <summary>
    /// IPositionFunctions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPositionFunctions<T>
    {
        /// <summary>
        /// Прибавить точку
        /// </summary>
        /// <param name="AddedValue"></param>
        /// <returns></returns>
        T Sum(T AddedValue);
        /// <summary>
        /// Вычесть точку
        /// </summary>
        /// <param name="SubstractValue"></param>
        /// <returns></returns>
        T Sub(T SubstractValue);
        /// <summary>
        /// Разделить точку на число
        /// </summary>
        /// <param name="divide"></param>
        /// <returns></returns>
        T Div(double divide);
        /// <summary>
        /// Умножить точку на число
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        T Mult(double multiplier);
    }
}
