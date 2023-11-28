using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace ModelInterfaces
{

    /// <summary>
    /// ObjKind
    /// </summary>
    public enum ObjKind
    {
        /// <summary>
        /// Вершина
        /// </summary>
        Вершина,
        Пента,
        Тетра,
        Гекса,
        Треугольник,
        Поверхность,
        Квадрат,
        Узел,
        Стержень,
        Линия
    }
    /// <summary>
    /// ObjType
    /// </summary>
    public enum ObjType
    {
        /// <summary>
        /// Точка
        /// </summary>
        Точка,
        /// <summary>
        /// Поверхности
        /// </summary>
        Линия,
        /// <summary>
        /// Поверхности
        /// </summary>
        Поверхность,
        /// <summary>
        /// Элементы2D
        /// </summary>
        Элемент2D,
        /// <summary>
        /// Элементы3D
        /// </summary>
        Элемент3D,
        /// <summary>
        /// Элементы1D
        /// </summary>
        Элемент1D
    }
    /// <summary>
    /// Model object interface
    /// </summary>
    public interface IModelObject : IComparable<IModelObject>, IEqualityComparer<IModelObject>, IEquatable<IModelObject>, IPosition3D
    {
        //Position3D Position
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
        /// ObjType
        /// </summary>
        ObjType ObjType { get; }
        /// <summary>
        /// ObjKind
        /// </summary>
        ObjKind ObjKind { get; }
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
        /// <summary>
        /// SetNumber
        /// </summary>
        /// <param name="number"></param>
        void SetNumber(int number);
    }
}
