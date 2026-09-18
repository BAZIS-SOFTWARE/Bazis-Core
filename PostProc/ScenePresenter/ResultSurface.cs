using Geometry;
using Geometry.Interfaces;
using Results.ScenePresenter;
using System;
using System.Collections.Generic;

namespace PostProc.ScenePresenter
{
    /// <summary>
    /// Представляет поверхность результата, заданную точками результата.
    /// </summary>
    public class ResultSurface : ICoordinated
    {
        private readonly List<ResultPoint> points;

        /// <summary>
        /// Создаёт поверхность результата.
        /// </summary>
        /// <param name="points">Точки поверхности.</param>
        /// <exception cref="Exception">Возникает, когда точек меньше трёх.</exception>
        public ResultSurface(List<ResultPoint> points)
        {
            if (points.Count < 3)
                throw new Exception("A surface requires at least three points.");

            this.points = points;
        }

        /// <summary>
        /// Возвращает точку поверхности по индексу.
        /// </summary>
        /// <param name="index">Индекс точки.</param>
        /// <returns>Точка результата.</returns>
        public ResultPoint this[int index]
        {
            get { return points[index]; }
            set { points[index] = value; }
        }

        /// <inheritdoc/>
        public int NumberOfPoints => points.Count;

        /// <summary>
        /// Вычисляет центр поверхности.
        /// </summary>
        /// <returns>Координаты центра поверхности.</returns>
        public Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var point in GetCoordinates())
                center = center.Sum(point);

            return center.Div(NumberOfPoints);
        }

        /// <summary>
        /// Вычисляет нормаль поверхности.
        /// </summary>
        /// <returns>Вектор нормали.</returns>
        public Point3D CalcNormal()
        {
            var firstVector = points[1].Position.Sub(points[0].Position);
            var secondVector = points[2].Position.Sub(points[0].Position);
            return Vector.CrossProd(firstVector, secondVector);
        }

        /// <inheritdoc/>
        public IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var point in points)
                yield return point.Position;
        }

        /// <summary>
        /// Возвращает точки поверхности.
        /// </summary>
        /// <returns>Последовательность точек результата.</returns>
        public IEnumerable<ResultPoint> GetVertexes()
        {
            foreach (var point in points)
                yield return point;
        }

        /// <summary>
        /// Меняет направление обхода точек поверхности.
        /// </summary>
        public void ReverseVertexes()
        {
            points.Reverse();
        }
    }
}
