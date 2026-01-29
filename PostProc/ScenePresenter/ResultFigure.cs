using Geometry;
using Model.Interfaces;
using Results.ScenePresenter;
using System.Collections.Generic;
using System.Drawing;

namespace PostProc.ScenePresenter
{
    public class ResultFigure : ISurfaceObject<ResultSurface, ResultPoint>
    {
        //List<ResultPoint> points = new List<ResultPoint>();
        protected ResultSurface surface;

        public ResultFigure(IEnumerable<Point3D> points, Color color)
        {
            Color = color;

            var resultPoints = new List<ResultPoint>();
            foreach (var point in points)
                resultPoints.Add(new ResultPoint(point));

            surface = new ResultSurface(resultPoints, color);
        }
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public IPoint this[int ind]
        {
            get { return surface[ind]; }
        }
/// <inheritdoc/>

        public int NumberOfSides { get { return 1; } }
/// <inheritdoc/>

        public int Number { get; private set; }
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
        public ObjType ObjType { get; } = ObjType.Поверхность;
/// <inheritdoc/>

        public int NumberOfPoints { get { return surface.NumberOfPoints; } }

        public float _x
        {
            get
            {
                var sum_x = 0.0f;
                foreach (var point in surface.GetCoordinates())
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
                foreach (var point in surface.GetCoordinates())
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
                foreach (var point in surface.GetCoordinates())
                {
                    sum_z += point._z;
                }
                return sum_z / NumberOfPoints;
            }
        }
/// <inheritdoc/>

        public int Dim => 2;

        /// <inheritdoc/>



        public Point3D CalcCentr()
        {
            return surface.CalcCentr();
        }
        /// <summary>
        /// CalcNormal
        /// </summary>
        /// <returns></returns>
        public Point3D CalcNormal()
        {
            return surface.CalcNormal();
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
            return surface.GetCoordinates();
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

        public void SetNumber(int number)
        {
            surface.SetNumber(number);
        }
/// <inheritdoc/>

        public IEnumerable<ResultPoint> GetVertexes()
        {
            return surface.GetVertexes();
        }
    }
}