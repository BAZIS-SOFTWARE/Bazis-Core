using Geometry;
using Geometry.Interfaces;
using Results.ScenePresenter;
using System.Collections.Generic;
using System.Drawing;

namespace PostProc.ScenePresenter
{
    /// <summary>
    /// Представляет поверхность результата для отображения и экспорта.
    /// </summary>
    public class ResultFigure : ICoordinated
    {
        private readonly ResultSurface surface;

        /// <summary>
        /// Создаёт фигуру результата по её координатам.
        /// </summary>
        /// <param name="points">Координаты точек фигуры.</param>
        /// <param name="color">Цвет диапазона результатов.</param>
        public ResultFigure(IEnumerable<Point3D> points, Color color)
        {
            Color = color;

            var resultPoints = new List<ResultPoint>();
            foreach (var point in points)
                resultPoints.Add(new ResultPoint(point));

            surface = new ResultSurface(resultPoints);
        }

        /// <summary>
        /// Возвращает точку фигуры по индексу.
        /// </summary>
        /// <param name="index">Индекс точки.</param>
        /// <returns>Точка результата.</returns>
        public ResultPoint this[int index] => surface[index];

        /// <summary>
        /// Количество поверхностей фигуры.
        /// </summary>
        public int NumberOfSides => 1;

        /// <summary>
        /// Цвет диапазона результатов.
        /// </summary>
        public Color Color { get; }

        /// <inheritdoc/>
        public int NumberOfPoints => surface.NumberOfPoints;

        /// <summary>
        /// Вычисляет центр фигуры.
        /// </summary>
        /// <returns>Координаты центра фигуры.</returns>
        public Point3D CalcCentr()
        {
            return surface.CalcCentr();
        }

        /// <summary>
        /// Вычисляет нормаль фигуры.
        /// </summary>
        /// <returns>Вектор нормали.</returns>
        public Point3D CalcNormal()
        {
            return surface.CalcNormal();
        }

        /// <inheritdoc/>
        public IEnumerable<Point3D> GetCoordinates()
        {
            return surface.GetCoordinates();
        }

        /// <summary>
        /// Возвращает поверхности фигуры.
        /// </summary>
        /// <returns>Последовательность поверхностей результата.</returns>
        public IEnumerable<ResultSurface> GetSurfaces()
        {
            yield return surface;
        }

        /// <summary>
        /// Возвращает точки фигуры.
        /// </summary>
        /// <returns>Последовательность точек результата.</returns>
        public IEnumerable<ResultPoint> GetVertexes()
        {
            return surface.GetVertexes();
        }
    }
}
