using Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using ModelInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model.MeshObjects
{

    public abstract class Element3D : Element, IElement3D
    {
        private List<ElementSurface> surfaces = new List<ElementSurface>();
        public Element3D(int number, Node[] nodes) : base(number, nodes)
        {

            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);

            var surfNumber = 0;
            foreach (var surfNodes in SurfaceFinder.FindSurfaceNodes(nodes))
            {
                surfaces.Add(new ElementSurface(surfNumber, surfNodes));
            }

        }
        /// <inheritdoc/>


        public override bool ViewState
        {
            get => base.ViewState;
            set
            {
                base.ViewState = value;
                foreach (var surface in surfaces)
                {
                    surface.ViewState = value;
                }
            }
        }


        /// <inheritdoc/>
        public override ObjType ObjType { get; } = ObjType.Элемент3D;
        /// <inheritdoc/>

        public int NumberOfSides { get { return surfaces.Count(); } }

        /// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var node in nodes)
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }

        /// <summary>
        /// Calc volume
        /// </summary>
        /// <returns></returns>
        public abstract float CalcVolume();
/// <inheritdoc/>


/// <inheritdoc/>

        public IEnumerable<IElementSurface> GetSurfaces()
        {
            foreach (var surface in surfaces)
            {
                yield return surface;
            }
        }
    }
}
