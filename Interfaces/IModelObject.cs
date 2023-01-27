using Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public enum ObjKind
    {
        Node,
        Tetra,
        Hexa,
        Triangle,
        ElementSurface,
        GeometrySurface,
        Quad,
        Point,
        Beam,
        Curve
    }

    public enum ObjType
    {
        Узлы,
        Поверхности,
        Элементы,
        Элементы2D,
        Элементы3D,
        Элементы1D
    }

    public interface IModelObject
    {
        int Number { get; }
        Color MasterColor { get; set; }
        Color SlaveColor { get; set; }
        Color InitialColor { get; set; }

        bool ViewState { get; set; }

        bool ExistState { get; set; }

        ObjKind ObjKind { get; }

        string ObjType { get; }

        int NumberOfSides { get; }

        int NumberOfPoints { get; }

        IEnumerable<Point3D> GetPoints();

        Point3D CalcCentralPoint();

        void SetBackColor();
    }
}
