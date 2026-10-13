using Geometry;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Model.MeshObjects
{
    /// <summary>
    /// Node
    /// </summary>
    public class Node : ModelObject, IPoint
    {
        List<IElement> elements = new List<IElement>();
  /// <inheritdoc/>
  
        public Point3D Position { get; set; }
/// <inheritdoc/>


        public IElement this[int elemInd] => elements[elemInd];
/// <inheritdoc/>

        public Point3D Normal { get; set; } = new Point3D();
        /// <summary>
        /// Node
        /// </summary>
        /// <param name="number"></param>
        /// <param name="position"></param>
        public Node(int number, Point3D position) : base(number)
        {
            Position = position;

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
                    // Важное изменение. Теперь используем цикл с индексом,
                    // чтобы предоотвратить Exception, так как коллекция изменяется методом
                    // RemoveElement
                    var count = elements.Count;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        elements[i].ExistState = false;
                    }
                    //foreach (var element in elements)
                       // element.ExistState = false;
                }
            }
        }

        /// <inheritdoc/>

        public void AddElement(IElement element)
        {
            elements.Add(element);
        }
        /// <summary>
        /// RemoveElement
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElement(IElement element)
        {
            elements.Remove(element);
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

        public override IEnumerable<Point3D> GetCoordinates()
        {
            yield return Position;
        }
        /// <inheritdoc/>

        public override string ToString()
        {
            
            return $"{base.ToString()} : {Position}";
        }
        /// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            return new Point3D(Position._x, Position._y, Position._z);
        }
    }
}
