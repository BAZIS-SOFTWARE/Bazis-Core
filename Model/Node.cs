using Geometry;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Model
{
    /// <summary>
    /// Node
    /// </summary>
    public class Node : ModelObject,INode
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
        /// <param name="objKind"></param>
        public Node(int number, Point3D position, ObjKind objKind) : this(number, position)
        {
            ObjKind = objKind;

        }
        /// <summary>
        /// Node
        /// </summary>
        /// <param name="number"></param>
        /// <param name="position"></param>
        public Node(int number, Point3D position) : base(number)
        {
            Position = position;
            ObjType = "Узлы";
            NumberOfPoints = 1;

            InitialColor = Color.FromArgb(0, 0, 255);
            MasterColor = Color.FromArgb(0, 0, 255);
            SlaveColor = Color.FromArgb(0, 0, 255);
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

        public void RemoveElement(IElement element)
        {
            elements.Remove(element);
        }
        /// <inheritdoc/>

        public void RemoveElementAt(int index)
        {
            elements.RemoveAt(index);
        }
        /// <inheritdoc/>

        public override ObjKind ObjKind { get; } = ObjKind.Node;
/// <inheritdoc/>

        public override string ObjType { get; }
/// <inheritdoc/>

        public override int NumberOfPoints { get; }

        /// <inheritdoc/>

        public override IEnumerable<Point3D> GetPoints()
        {
            yield return Position;
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return string.Format("{0} {1} : {2}",Number,ObjType,Position);
        }
/// <inheritdoc/>

        public override Point3D CalcCentralPoint()
        {
            return Position;
        }
    }
}
