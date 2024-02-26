using Geometry;
using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Single;
using ModelInterfaces.MeshObjects;

namespace Model.MeshObjects
{
    public abstract class Element2D : Element, IElement2D
    {
        protected IElementSurface surface;

        public Element2D(int number, INode[] nodes) : base(number, nodes)
        {
            InitialColor = Color.FromArgb(0, 0, 255);
            MasterColor = Color.FromArgb(0, 0, 255);
            SlaveColor = Color.FromArgb(0, 0, 255);

        }


        public Element2D(int number, INode[] nodes, int intPoints) : this(number, nodes)
        {

        }
        /// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Элемент2D;
        /// <inheritdoc/>

        public int NumberOfSides { get { return 1; } }
/// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var node in GetVertexes())
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }
/// <inheritdoc/>

        public Point3D CalcNormal()
        {
            return surface.CalcNormal();
        }
/// <inheritdoc/>

        public abstract float CalcSquare();
/// <inheritdoc/>

        /// <inheritdoc/>

        public IEnumerable<IElementSurface> GetSurfaces()
        {
            yield return surface;
        }
    }
}
