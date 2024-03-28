using Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using ModelInterfaces;
using ModelInterfaces.MeshObjects;
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
        public Element3D(int number, INode[] nodes) : base(number, nodes)
        {

            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);
            
            var centr = CalcCentr();
            var surfNumber = 0;
            foreach (var surfNodes in SurfaceFinder.FindSurfaceNodes(nodes))
            {
                var surface = new ElementSurface(surfNumber, surfNodes);

                var normal = surface.CalcNormal();
                var plane = new Plane(surface[0].Position, normal);
                var proj = centr.GetPointProectionOnPlane(plane);
                var vec = proj.Sub(centr);
                var res = Geometry.Vector.DotProd(normal, vec);

                if (res < 0)
                    surface.ReverseVertexes();

                surfaces.Add(surface);
                surfNumber++;
            }
        }

        public Element3D(int number, INode[] nodes,int point) : this(number, nodes)
        {


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
/// <inheritdoc/>

        public IEnumerable<Point3D> GetSurfacesNormals()
        {
            foreach (var surface in surfaces)
            {
                yield return surface.CalcNormal();
            }
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
