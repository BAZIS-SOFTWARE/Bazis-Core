using Geometry;
using Model.Interfaces;
using Results.ScenePresenter;
using System.Collections.Generic;
using System.Drawing;

namespace PostProc.ScenePresenter
{
    public class ResultFigure : ISurfaceObject<ResultSurface, ResultPoint>
    {
        List<ResultPoint> points = new List<ResultPoint>();
        protected ResultSurface surface;

        public ResultFigure(List<Point3D> points, Color color)
        {
            Color = color;

            foreach (var point in points)
                this.points.Add(new ResultPoint(point));

            surface = new ResultSurface(this.points, color);
        }
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public IPoint this[int ind]
        {
            get { return points[ind]; }
        }
/// <inheritdoc/>

        public int NumberOfSides { get { return 1; } }
/// <inheritdoc/>

        public int Number { get { return 0; } }
/// <inheritdoc/>

        public Color Color { get; set; }
        /// <inheritdoc/>
        public Color SlaveColor { get ; set ; }
        /// <inheritdoc/>
        public Color InitialColor { get ; set; }
        /// <inheritdoc/>
        public bool ViewState { get; set; } = true;
        /// <inheritdoc/>
        public bool ExistState { get; set; } = true;
        /// <inheritdoc/>
        public ObjType ObjType { get; } = ObjType.Фигура2D;
/// <inheritdoc/>

        public int NumberOfPoints { get { return points.Count; } }

        public float _x
        {
            get
            {
                var sum_x = 0.0f;
                foreach (var point in points)
                {
                    sum_x += point._x;
                }
                return sum_x / NumberOfPoints;
            }
        }
        public float _y
        {
            get
            {
                var sum_y = 0.0f;
                foreach (var point in points)
                {
                    sum_y += point._y;
                }
                return sum_y / NumberOfPoints;
            }
        }
        public float _z
        {
            get
            {
                var sum_z = 0.0f;
                foreach (var point in points)
                {
                    sum_z += point._z;
                }
                return sum_z / NumberOfPoints;
            }
        }
/// <inheritdoc/>



        public Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var point in GetCoordinates())
            {
                center = center.Sum(point);
            }
            return center.Div(points.Count);
        }
        /// <summary>
        /// CalcNormal
        /// </summary>
        /// <returns></returns>
        public Point3D CalcNormal()
        {
            var fvectors = points[1].Position.Sub(points[0].Position);
            var svectors = points[2].Position.Sub(points[0].Position);
            return Vector.CrossProd(fvectors, svectors);
        }

        public float CalcSquare()
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public int CompareTo(IModelObject other)
        {
            return this.CompareTo(other);
        }
/// <inheritdoc/>

        public bool Equals(IModelObject x, IModelObject y)
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public bool Equals(IModelObject other)
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var point in points)
            {
                yield return point.Position;
            }
        }

/// <inheritdoc/>

        public int GetHashCode(IModelObject obj)
        {
            return obj.Number;
        }

/// <inheritdoc/>

        public IEnumerable<ResultSurface> GetSurfaces()
        {
            yield return surface;
        }
/// <inheritdoc/>

        public void SetBackColor()
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public void SetNumber(int number)
        {
            throw new System.NotImplementedException();
        }
/// <inheritdoc/>

        public IEnumerable<ResultPoint> GetVertexes()
        {
            foreach (var node in points)
                yield return node;
        }
    }
}