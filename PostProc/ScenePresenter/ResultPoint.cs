using Geometry;
using Geometry.Interfaces;
using System.Collections.Generic;

namespace Results.ScenePresenter
{
    /// <summary>
    /// Представляет точку результата.
    /// </summary>
    public class ResultPoint : ICoordinated, IPosition3D
    {
        private Point3D point;

        /// <summary>
        /// Создаёт точку результата.
        /// </summary>
        /// <param name="point">Координаты точки.</param>
        public ResultPoint(Point3D point)
        {
            this.point = point;
        }

        /// <summary>
        /// Координаты точки.
        /// </summary>
        public Point3D Position
        {
            get { return point; }
            set { point = value; }
        }

        /// <summary>
        /// Нормаль в точке результата.
        /// </summary>
        public Point3D Normal { get; set; }

        /// <inheritdoc/>
        public int NumberOfPoints => 1;

        /// <inheritdoc/>
        public float _x => Position._x;

        /// <inheritdoc/>
        public float _y => Position._y;

        /// <inheritdoc/>
        public float _z => Position._z;

        /// <summary>
        /// Вычисляет центр точки.
        /// </summary>
        /// <returns>Координаты точки.</returns>
        public Point3D CalcCentr()
        {
            return new Point3D(_x, _y, _z);
        }

        /// <inheritdoc/>
        public IEnumerable<Point3D> GetCoordinates()
        {
            yield return Position;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{_x} | {_y} | {_z}";
        }
    }
}
