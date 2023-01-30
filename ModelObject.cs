using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public abstract class ModelObject : IModelObject, IComparable<ModelObject>, IEqualityComparer<ModelObject>
    {
        public ModelObject(int number)
        {
            Number = number;
            ExistState = true;
        }
        public int Number { get; }

        public Color MasterColor { get ; set; }
        public Color SlaveColor { get; set; }
        public Color InitialColor { get; set; }

        bool viewState;
        bool existState;
        public bool ViewState
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

        public bool ExistState
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

        public abstract ObjKind ObjKind { get; }

        public int NumberOfSides { get; }

        public abstract int NumberOfPoints { get; }

        public abstract string ObjType { get; }

        public void SetInitialColor()
        {
            var temp = InitialColor;
            SlaveColor = temp;
            MasterColor = temp;
        }

        public void SetBackColor()
        {
            var temp = SlaveColor;
            MasterColor = temp;
        }

        public void SetNewColor(Color objColor)
        {
            SlaveColor = objColor;
            MasterColor = objColor;
        }

        public abstract IEnumerable<Point3D> GetPoints();

        public int CompareTo(ModelObject other)
        {
            if (this.Number > other.Number)
                return 1;
            else if (this.Number < other.Number)
                return -1;
            else return 0;
        }

        public Point3D CalcCentralPoint()
        {
            var center = new Point3D();
            foreach (var point in GetPoints())
            {
                center = center.Sum(point);
            }
            return center.Div(GetPoints().Count());
        }

        public bool Equals(ModelObject x, ModelObject y)
        {
            if (x.Number == y.Number)
                return true;
            else return false;
        }

        public int GetHashCode(ModelObject obj)
        {
            return obj.Number;
        }
    }
}
