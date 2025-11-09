using Geometry;
using Model.Interfaces;
using Results.ScenePresenter;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PostProc.ScenePresenter
{
    public class ResultSurface : ISurface<ResultPoint>
    {
        List<ResultPoint> points = new List<ResultPoint>();
        public ResultSurface(List<ResultPoint> points, Color color)
        {
            if (points.Count < 3)
                throw new Exception("Для создания поверхности требуется не меньше 3-х узлов!");

            //points.RoundSort(); //Anticlockwise(nodes);

            this.points = points;

            Color = color;
        }
        /// <summary>
        /// this
        /// </summary>
        /// <param name="ind"></param>
        /// <returns></returns>
        public ResultPoint this[int ind] 
        {
            get { return points[ind]; } 
        }

        ResultPoint ISurface<ResultPoint>.this[int ind] 
        { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }

        public int Number => throw new NotImplementedException();

        public Color Color { get ; set ; }
        public Color SlaveColor { get ; set ; }
        public Color InitialColor { get; set ; }
        public bool ViewState { get; set; } = true;
        public bool ExistState { get; set; } = true;

        public ObjType ObjType { get; } = ObjType.Поверхность;

        public int NumberOfPoints { get { return points.Count; } }

        public float _x => throw new NotImplementedException();

        public float _y => throw new NotImplementedException();

        public float _z => throw new NotImplementedException();

        public Point3D CalcCentr()
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public Point3D CalcNormal()
        {
            var fvectors = points[1].Position.Sub(points[0].Position);
            var svectors = points[2].Position.Sub(points[0].Position);
            return Vector.CrossProd(fvectors, svectors);
        }
/// <inheritdoc/>

        public double CalcSquare()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IModelObject other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IModelObject x, IModelObject y)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IModelObject other)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Point3D> GetCoordinates()
        {
            foreach (var point in points)
                yield return point.Position;
        }

        public float GetDistance(Point3D point3D)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(IModelObject obj)
        {
            throw new NotImplementedException();
        }

        public Segment3D[] GetLinesFromSurface()
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public IEnumerable<ResultPoint> GetVertexes()
        {
            foreach (var item in points)
            {
                yield return item;
            }
        }

        public int GetVertexIndex(ResultPoint vertex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>

        public void ReverseVertexes()
        {
            points.Reverse();
        }
/// <inheritdoc/>

        public void SetBackColor()
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public void SetNumber(int number)
        {
            throw new NotImplementedException();
        }
    }
}
