using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Model.Interfaces
{
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
        /// Поверхность
        /// </summary>
        Поверхность,
        /// <summary>
        /// Фигура2D
        /// </summary>
        Фигура2D,
        /// <summary>
        /// Фигура3D
        /// </summary>
        Фигура3D,
        /// <summary>
        /// Узел
        /// </summary>
        Узел,
        /// <summary>
        /// Элемент1D
        /// </summary>
        Элемент1D,
        /// <summary>
        /// Элемент2D
        /// </summary>
        Элемент2D,
        /// <summary>
        /// Элемент3D
        /// </summary>
        Элемент3D
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
        Color Color { get; set; }
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
        /// NumberOfPoints
        /// </summary>
        int NumberOfPoints { get; }
        /// <summary>
        /// GetCoordinates
        /// </summary>
        IEnumerable<Point3D> GetCoordinates();
        /// <summary>
        /// CalcCentralPoint
        /// </summary>
        Point3D CalcCentr();
        /// <summary>
        /// SetNumber
        /// </summary>
        /// <param name="number"></param>
        void SetNumber(int number);
    }
}
