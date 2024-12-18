using Geometry;
using Model.Interfaces;
using Model.Interfaces.GeometryObjects;
using System.Collections.Generic;
using System.Drawing;

namespace Model.GeometryObjects
{
    /// <summary>
    /// Point
    /// </summary>
    public class GeometryPoint : ModelObject, IPoint
    {
        List<ILineObject<IPoint>> lines = new List<ILineObject<IPoint>>();
        /// <summary>
        /// Point
        /// </summary>
        /// <param name="number"></param>
        /// <param name="position"></param>
        public GeometryPoint(int number, Point3D position) : base(number)
        {
 
            Position = position;

            InitialColor = Color.FromArgb(255, 207, 64);
            MasterColor = Color.FromArgb(255, 207, 64);
            SlaveColor = Color.FromArgb(255, 207, 64);
        }
        /// <inheritdoc/>

        public override int NumberOfPoints { get; } = 1;
/// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Точка;
/// <inheritdoc/>

        public Point3D Position { get; set; }
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
            return $"{base.ToString()} : {Position}";
        }
    }
}
