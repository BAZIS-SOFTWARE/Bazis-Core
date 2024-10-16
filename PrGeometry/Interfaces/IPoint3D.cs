using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry.Interfaces
{
    /// <summary>
    /// IPoint3D
    /// </summary>
    public interface IPoint3D : IPosition3D
    {
        /// <summary>
        /// Рассчитать растояние до линии
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        float GetDistanceToLine(Segment3D line);
        /// <summary>
        /// Найти проекцию на плоскость
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        Point3D GetPointProectionOnPlane(Plane plane);
        /// <summary>
        /// Проверить принадлежность
        /// </summary>
        /// <param name="plane"></param>
        /// <returns></returns>
        bool IsBelongTo(Plane plane);
    }
}
