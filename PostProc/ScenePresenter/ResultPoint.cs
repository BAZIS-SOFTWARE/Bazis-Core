using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Results.ScenePresenter
{
    /// <summary>
    /// ResultNode
    /// </summary>
    public class ResultPoint : IPoint
    {
        Point3D point;
        /// <summary>
        /// ResultNode
        /// </summary>
        /// <param name="point"></param>
        public ResultPoint(Point3D point)
        {
            this.point = point;
        }
/// <inheritdoc/>

        public Point3D Position 
        { 
            get { return point; }
            set { point = value; }
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{_x} | {_y} | {_z}";
        }
        /// <inheritdoc/>

        public Point3D Normal { get; set; }
/// <inheritdoc/>

        public int Number { get; }

        public Color Color { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
  
        /// <inheritdoc/>
        public bool ViewState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        /// <inheritdoc/>
        public bool ExistState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
/// <inheritdoc/>

        public ObjType ObjType { get; } = ObjType.Точка;
/// <inheritdoc/>

        public int NumberOfPoints { get; } = 1;

        public float _x { get { return Position._x; } }

        public float _y { get { return Position._y; } }

        public float _z { get { return Position._z; } }
/// <inheritdoc/>

        public int Dim => 0;

        /// <inheritdoc/>

        public Point3D CalcCentr()
        {
            return new Point3D(_x, _y, _z);
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
/// <inheritdoc/>

        public IEnumerable<Point3D> GetCoordinates()
        {
            throw new NotImplementedException();
        }

        public int GetHashCode(IModelObject obj)
        {
            return obj.Number;
        }

        public void SetBackColor()
        {
            throw new NotImplementedException();
        }

        public void SetNumber(int number)
        {
            throw new NotImplementedException();
        }
    }
}
