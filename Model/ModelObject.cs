using Geometry;
using ModelInterfaces;
using System.Collections.Generic;
using System.Drawing;

namespace Model
{
    /// <summary>
    ///Model object class
    /// </summary>
    public abstract class ModelObject : IModelObject
    {
        /// <summary>
        /// ModelObject constructor
        /// </summary>
        public ModelObject(int number)
        {
            Number = number;
            ExistState = true;
        }
        /// <summary>
        /// Number
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// SetNumber
        /// </summary>
        public void SetNumber (int number)
        {
            Number = number;
        }
        /// <summary>
        /// MasterColor
        /// </summary>
        public Color MasterColor { get ; set; }
        /// <summary>
        /// SlaveColor
        /// </summary>
        public Color SlaveColor { get; set; }
        /// <summary>
        /// InitialColor
        /// </summary>
        public Color InitialColor { get; set; }

        bool viewState;
        bool existState;
        /// <summary>
        /// View state flag
        /// </summary>
        public virtual bool ViewState
        {
            get { return viewState; }
            set
            {
                if(existState)
                {
                    if (!value)
                        SetBackColor();
                    viewState = value;
                }
            }
        }
        /// <summary>
        /// Exist state flag
        /// </summary>
        public virtual bool ExistState
        {
            get { return existState; }
            set
            {
                if (!value)
                    viewState = false;
                else viewState = true;
                existState = value;
            }
        }
        /// <summary>
        /// NumberOfPoints
        /// </summary>
        public abstract int NumberOfPoints { get; }
/// <inheritdoc/>

        public abstract ObjType ObjType { get; }

        public float _x
        {
            get
            {
                var sum_x = 0.0f;
                foreach (var point in GetCoordinates())
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
                foreach (var point in GetCoordinates())
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
                foreach (var point in GetCoordinates())
                {
                    sum_z += point._z;
                }
                return sum_z / NumberOfPoints;
            }
        }




        /// <summary>
        /// Set initial color
        /// </summary>
        public void SetInitialColor()
        {
            var temp = InitialColor;
            SlaveColor = temp;
            MasterColor = temp;
        }
        /// <summary>
        /// Set back color
        /// </summary>
        public void SetBackColor()
        {
            var temp = SlaveColor;
            MasterColor = temp;
        }
        /// <summary>
        /// Set new color
        /// </summary>
        public void SetNewColor(Color objColor)
        {
            SlaveColor = objColor;
            MasterColor = objColor;
        }
        /// <summary>
        /// Get points
        /// </summary>
        public abstract IEnumerable<Point3D> GetCoordinates();
        /// <summary>
        /// Compare to
        /// </summary>
        public int CompareTo(IModelObject other)
        {
            if (this.Number > other.Number)
                return 1;
            else if (this.Number < other.Number)
                return -1;
            else return 0;
        }
        /// <summary>
        /// Calc central point
        /// </summary>
        public virtual Point3D CalcCentr()
        {

            var center = new Point3D();

            foreach (var node in GetCoordinates())
            {
                center = center.Sum(node);
            }
            return center.Div(NumberOfPoints);

        }
        /// <summary>
        /// Equal two objects
        /// </summary>
        public bool Equals(IModelObject x, IModelObject y)
        {
            if (x.Number == y.Number)
                return true;
            else return false;
        }
        /// <summary>
        ///Get hash code
        /// </summary>
        public int GetHashCode(IModelObject obj)
        {
            return obj.Number;
        }
        /// <summary>
        /// Equal with object
        /// </summary>
        public bool Equals(IModelObject other)
        {
            if (Number == other.Number)
                return true;
            else return false;
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{Number} {ObjType}";
        }
    }
}
