using Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    /// <summary>
    /// ObjKind
    /// </summary>
    public enum ObjKind
    {
        Node,
        Penta,
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
    /// <summary>
    /// ObjType
    /// </summary>
    public enum ObjType
    {
        /// <summary>
        /// Узлы
        /// </summary>
        Узлы,
        /// <summary>
        /// Поверхности
        /// </summary>
        Поверхности,
        /// <summary>
        /// Элементы
        /// </summary>
        Элементы,
        /// <summary>
        /// Элементы2D
        /// </summary>
        Элементы2D,
        /// <summary>
        /// Элементы3D
        /// </summary>
        Элементы3D,
        /// <summary>
        /// Элементы1D
        /// </summary>
        Элементы1D
    }
    /// <summary>
    /// Model object interface
    /// </summary>
    public interface IModelObject : IComparable<IModelObject>, IEqualityComparer<IModelObject>, IEquatable<IModelObject>
    {
        /// <summary>
        /// Number
        /// </summary>
        int Number { get; }
        /// <summary>
        /// MasterColor
        /// </summary>
        Color MasterColor { get; set; }
        /// <summary>
        /// SlaveColor
        /// </summary>
        Color SlaveColor { get; set; }
        /// <summary>
        /// InitialColor
        /// </summary>
        Color InitialColor { get; set; }
        /// <summary>
        /// View state flag
        /// </summary>
        bool ViewState { get; set; }
        /// <summary>
        /// Exist state flag
        /// </summary>
        bool ExistState { get; set; }
        /// <summary>
        /// ObjKind
        /// </summary>
        ObjKind ObjKind { get; }
        /// <summary>
        /// ObjType
        /// </summary>
        string ObjType { get; }
        /// <summary>
        /// NumberOfPoints
        /// </summary>
        int NumberOfPoints { get; }
        /// <summary>
        /// GetPoints
        /// </summary>
        IEnumerable<Point3D> GetPoints();
        /// <summary>
        /// CalcCentralPoint
        /// </summary>
        Point3D CalcCentralPoint();
        /// <summary>
        /// SetBackColor
        /// </summary>
        void SetBackColor();
    }
}
