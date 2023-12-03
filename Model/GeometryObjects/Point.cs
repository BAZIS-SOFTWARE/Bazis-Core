using Geometry;
using ModelInterfaces;
using System;
using System.Collections.Generic;

namespace Model.GeometryObjects
{
    /// <summary>
    /// Point
    /// </summary>
    public class Point : ModelObject, IPoint
    {
        List<ILineObject<IPoint>> lines = new List<ILineObject<IPoint>>();
        /// <summary>
        /// Point
        /// </summary>
        /// <param name="number"></param>
        /// <param name="position"></param>
        public Point(int number, Point3D position) : base(number)
        {
 
            Position = position;
        }
        /// <inheritdoc/>

        public override int NumberOfPoints { get; } = 1;
/// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Точка;
/// <inheritdoc/>

        public Point3D Position { get; }
/// <inheritdoc/>

        public Point3D Normal { get; set; }
/// <inheritdoc/>

        public void AddLine(ILineObject<IPoint> line)
        {
            lines.Add(line);
        }
/// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            return Position;
        }
/// <inheritdoc/>

        public IEnumerable<ILineObject<IPoint>> GetLines()
        {
            foreach (var line in lines)
            {
                yield return line;
            }
        }
/// <inheritdoc/>

        public override IEnumerable<Point3D> GetCoordinates()
        {
            yield return Position;
        }
/// <inheritdoc/>

        public override bool ExistState
        {
            get => base.ExistState;
            set
            {
                base.ExistState = value;
                if (!value)
                {
                    foreach (var line in lines)
                        line.ExistState = false;
                }
            }
        }

        /// <inheritdoc/>

        public override string ToString()
        {
            return string.Format("{0} {1} : {2}", Number, ObjType, Position);
        }
    }
}
