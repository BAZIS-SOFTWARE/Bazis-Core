using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry.Interfaces
{
    /// <summary>
    /// IPointsFunctions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPointFunctions<T> : IPositionFunctions<T>, IEquatable<T>
    {
        /// <summary>
        /// Вычислить сумму квадрата координат
        /// </summary>
        /// <returns></returns>
        float GetSquareSumCoords();
    }
}
