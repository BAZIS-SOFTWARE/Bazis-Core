using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MathNet.Numerics.LinearAlgebra.Single;
using Model.Interfaces.MeshObjects;
using Geometry.Interfaces;

namespace Model.MeshObjects
{
    public abstract class Element2D : Element, IElement2D
    {
        protected ElementSurface surface;

        public Element2D(int number, Node[] nodes) : base(number, nodes)
        {
            Color = Color.FromArgb(0, 0, 255);

            surface = new ElementSurface(number, 0, nodes);

            //this.nodes = surface.GetVertexes().ToList();

            var basis = CalcTransitionMatrix(GetCoordinates().ToArray());

            TransitionMatrix = Matrix<double>.Build.DenseOfArray(basis).Inverse();
        }
/// <inheritdoc/>

        public double[,] CalcTransitionMatrix( IPoint3D[] points )
        {
            var fPnt = points[0];
            var sPnt = points[1];
            var lPnt = points.Last();

            var v1 = new Point3D(sPnt._x - fPnt._x, sPnt._y - fPnt._y, sPnt._z - fPnt._z);
            var v2 = new Point3D(lPnt._x - fPnt._x, lPnt._y - fPnt._y, lPnt._z - fPnt._z);

            var v3 = Geometry.Vector.CrossProd(v1, v2);

            //для гарантии что СК прямолинейная
            var v4 = Geometry.Vector.CrossProd(v1, v3);

            var xn = Geometry.Vector.GetVectorNorm(v1);
            var yn = Geometry.Vector.GetVectorNorm(v4).Mult(-1);
            var zn = Geometry.Vector.GetVectorNorm(v3);

            var basis = new double[,]
            {
                {xn._x,yn._x,zn._x },
                {xn._y,yn._y,zn._y },
                {xn._z,zn._z,zn._z }
            };
            return basis;
        }

        public Element2D(int number, Node[] nodes, int intPoints) : this(number, nodes)
        {

        }

        internal void UpdateSurfaceAfterCoincidentNodesMerge()
        {
            surface = new ElementSurface(Number, 0, nodes.ToArray());
        }

        /// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Элемент2D;
        /// <inheritdoc/>

        public int NumberOfSides { get { return 1; } }
/// <inheritdoc/>

        public Matrix<double> TransitionMatrix { get; set; }

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

        public abstract double CalcSquare();
/// <inheritdoc/>

        public IEnumerable<Point3D> GetSurfacesNormals()
        {
            yield return surface.CalcNormal();
        }

        /// <inheritdoc/>

        /// <inheritdoc/>

        public IEnumerable<ElementSurface> GetSurfaces()
        {
            yield return surface;
        }

        /// <summary>
        /// CreateIntegrationPoint
        /// </summary>
        /// <param name="w"></param>
        /// <param name="ksi"></param>
        /// <param name="eta"></param>
        /// <returns></returns>
        public IntegralPoint CreateIntegrationPoint(double w, double ksi, double eta)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("ksi", ksi);
            intPoint.AddCoordinate("eta", eta);
            intPoint.Weigt = w;

            intPoint.Jacobi = CalcJacobi(intPoint);

            return intPoint;
        }
    }
}
