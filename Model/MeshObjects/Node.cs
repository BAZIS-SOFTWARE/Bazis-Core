using Geometry;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Model.MeshObjects
{
    /// <summary>
    /// Node
    /// </summary>
    public class Node : ModelObject, INode
    {
        List<IElement> elements = new List<IElement>();
        /// <summary>
        /// Position
        /// </summary>
        public Point3D Position { get; }
        /// <summary>
        /// Normal
        /// </summary>
        [Obsolete(@"Prop ""Normal"" is deprecated, please don't use.")]
        public Point3D Normal { get; set; } = new Point3D();
        /// <summary>
        /// Node
        /// </summary>
        /// <param name="number"></param>
        /// <param name="position"></param>
        public Node(int number, Point3D position) : base(number)
        {
            Position = position;

            InitialColor = Color.FromArgb(0, 0, 255);
            MasterColor = Color.FromArgb(0, 0, 255);
            SlaveColor = Color.FromArgb(0, 0, 255);
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
                    foreach (var element in elements)
                        element.ExistState = false;
                }
            }
        }

        /// <inheritdoc/>

        public void AddElement(IElement element)
        {
            elements.Add(element);
        }
        /// <inheritdoc/>

        public IEnumerable<IElement> GetElements()
        {
            foreach (var element in elements)
            {
                yield return element;
            }
        }

        /// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Узел;
        /// <inheritdoc/>

        public override int NumberOfPoints { get; } = 1;

        /// <inheritdoc/>

        public override IEnumerable<Point3D> GetPoints()
        {
            yield return Position;
        }
        /// <inheritdoc/>

        public override string ToString()
        {
            return string.Format("{0} {1} : {2}", Number, ObjType, Position);
        }
        /// <inheritdoc/>

        public override Point3D CalcCentralPoint()
        {
            return Position;
        }
    }
}
