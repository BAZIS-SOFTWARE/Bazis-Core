using System.Collections.Generic;

namespace Geometry.Interfaces
{
    /// <summary>
    /// Описывает объект, заданный набором пространственных координат.
    /// </summary>
    public interface ICoordinated
    {
        /// <summary>
        /// Количество точек, задающих объект.
        /// </summary>
        int NumberOfPoints { get; }

        /// <summary>
        /// Возвращает координаты точек объекта.
        /// </summary>
        /// <returns>Последовательность координат точек.</returns>
        IEnumerable<Point3D> GetCoordinates();
    }
}
