using Geometry;
using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Model.Elements
{
    public abstract class Element2D : Element, IElement2D
    {
        protected ISurface surface;

        public Element2D(int number, Node[] nodes, ObjKind objKind) : base(number)
        {
            InitialColor = Color.FromArgb(0, 0, 255);
            MasterColor = Color.FromArgb(0, 0, 255);
            SlaveColor = Color.FromArgb(0, 0, 255);
            ObjType = "Элементы2D";
            ObjKind = objKind;

            foreach (var node in nodes)
            {
                node.AddElement(this);
                this.nodes.Add(node);
            }
        }

        public Element2D(int number, ISurface surface, ObjKind objKind) : base(number)
        {
            InitialColor = Color.FromArgb(0, 0, 255);
            MasterColor = Color.FromArgb(0, 0, 255);
            SlaveColor = Color.FromArgb(0, 0, 255);
            ObjType = "Элементы2D";
            ObjKind = objKind;
            this.surface = surface;

            foreach (var node in surface.GetNodes())
            {
                node.AddElement(this);
                nodes.Add(node);
            }
        }


        /// <inheritdoc/>

        public override ObjKind ObjKind { get; }

        public override string ObjType { get; }
        /// <inheritdoc/>

        public int NumberOfSides { get { return 1; } }

        public override Point3D CalcCentralPoint()
        {
            var center = new Point3D();

            foreach (var node in GetNodes())
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }

        public Point3D CalcNormal()
        {
            return surface.CalcNormal();
        }

        public abstract float CalcSquare();
     
        /// <inheritdoc/>

        public IEnumerable<ISurface> GetSurface()
        {
            yield return surface;
        }
    }
    public static class FloatOverride
    {
        public static float Square(this float value)
        {
            return value * value;
        }
    }
}
