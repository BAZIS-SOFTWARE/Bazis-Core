using Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using ModelInterfaces;
using Model.ObjectsSorters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model.MeshObjects
{

    public abstract class Element3D : Element, IElement3D
    {
        private List<ISurface> surfaces;
        public Element3D(int number, Node[] nodes) : base(number, nodes)
        {

            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);

            surfaces = SurfaceFinder.FindSurfaces(nodes).ToList();
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

        public override Point3D CalcCentralPoint()
        {
            var center = new Point3D();

            foreach (var node in GetNodes())
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

        public IEnumerable<ISurface> GetSurface()
        {
            foreach (var surface in surfaces)
            {
                yield return surface;
            }
        }
    }
}
