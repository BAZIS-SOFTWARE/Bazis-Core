using Geometry;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model
{
    public class Element3D : SurfaceElement, IElement3D
    {
        private List<Surface> surfaces;
        public Element3D(int number, Node[] nodes, ObjKind objKind) : base(number, nodes,objKind)
        {

            InitialColor = Color.FromArgb(255, 125, 0);
            MasterColor = Color.FromArgb(255, 125, 0);
            SlaveColor = Color.FromArgb(255, 125, 0);
            ObjKind = objKind;
            ObjType = "Элементы3D";
            surfaces = NodeSorter.GetSurfaces(nodes).ToList();
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

        public override string ObjType { get; }
        /// <inheritdoc/>
        public override ObjKind ObjKind { get;}
/// <inheritdoc/>

        public override int NumberOfSides { get { return surfaces.Count(); } }       

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
        public float CalcVolume()
        {
            //nodes[]
            //var pAr = GetPoints().ToArray();

            var mCoord = new float[4, 4];
            var p = new Point3D[] { nodes[0].Position, nodes[1].Position, nodes[2].Position, nodes[3].Position };
            for (int j = 0; j < p.Length; j++)
            {
                mCoord[j, 0] = 1;
                mCoord[j, 1] = p[j]._x;
                mCoord[j, 2] = p[j]._y;
                mCoord[j, 3] = p[j]._z;
            }

            var cMatr = Math.Abs(Matrix<float>.Build.DenseOfArray(mCoord).Determinant() / 6);
            //var str = cMatr.ToString(4, 4);
            return cMatr;
        }

        public float[][] GetDerivativeFromFunction(Point3D point3D)
        {
            switch (ObjKind)
            {
                case ObjKind.Tetra:
                    throw new NotImplementedException();
                case ObjKind.Penta:
                    return DerivativeFormFunctionPenta(point3D).ToRowArrays();
                case ObjKind.Hexa:
                    return DerivativeFormFunctionHex(point3D).ToRowArrays();
            }

            throw new NotImplementedException();
        }

        private Matrix<float> DerivativeFormFunctionHex(Point3D point3D)
        {
            var center = CalcCentralPoint();
            var supportVector = center.Sub(nodes[0].Position);
            var a = Math.Abs(supportVector._x);
            var b = Math.Abs(supportVector._y);
            var c = Math.Abs(supportVector._z);
            var pointKsi = (point3D._x - center._x) / a;
            var pointEta = (point3D._y - center._y) / b;
            var pointPsi = (point3D._z - center._z) / c;
            var answer = Matrix<float>.Build.Dense(3, 8);
            var num = 0;
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    for (var k = -1; k <= 1; k += 2)
                    {
                        answer[0, num] = (i * (1 + j * pointEta) * (1 + k * pointPsi) / 8);
                        answer[1, num] = (j * (1 + i * pointKsi) * (1 + k * pointPsi) / 8);
                        answer[2, num] = (k * (1 + j * pointEta) * (1 + i * pointKsi) / 8);
                        num++;
                    }
            return answer;
        }

        private Matrix<float> DerivativeFormFunctionPenta(Point3D point3D)
        {
            var topHeight = nodes.Max(x => x.Position._z);
            var botHeight = nodes.Min(x => x.Position._z);

            var bottom = nodes.Where(x => Math.Abs(x.Position._z - botHeight) < 1e-3);
            var top = nodes.Where(z => Math.Abs(z.Position._z - topHeight) < 1e-3);
            var crawlOrder = bottom.Concat(top).ToArray();
            SortCrawlOrder(crawlOrder);
            var dForm = new Dictionary<Node, float[]>();
            var h = (crawlOrder[3].Position._z - crawlOrder[0].Position._z) / 2;
            var dzeta = (point3D._z - crawlOrder[0].Position._z - h) / h;
            var delta = PentaFormDelta(crawlOrder[0].Position, crawlOrder[1].Position, crawlOrder[2].Position);
            var fBottom = PentaFormF(dzeta, -1);
            var fTop = PentaFormF(dzeta, 1);
            for (var i = 0; i < 6; i++)
            {
                var coefficients =
                    PentaCoefficients(crawlOrder[(i + 1) % 3].Position, crawlOrder[(i + 2) % 3].Position);
                var d = new float[3];
                d[0] = coefficients[1] / delta / 2 * (i < 3 ? fTop : fBottom);
                d[1] = coefficients[2] / delta / 2 * (i < 3 ? fTop : fBottom);
                d[2] = PentaFormL(point3D, coefficients, delta) / h / 2 * (i < 3 ? 1 : -1);
                dForm.Add(crawlOrder[i], d);
            }

            var answer = Matrix<float>.Build.Dense(3, 6);
            for (var i = 0; i < nodes.Length; i++)
                for (var j = 0; j < 3; j++)
                    answer[j, i] = dForm[nodes[i]][j];

            return answer;
        }

        public override float[] GetFormFunctions(Point3D point3D)
        {
            switch (ObjKind)
            {
                case ObjKind.Tetra:
                    return FormFunctionTetra(point3D);
                case ObjKind.Hexa:
                    return FormFunctionHex(point3D);
                case ObjKind.Penta:
                    return FormFunctionPenta(point3D);
            }
            throw new NotImplementedException();
        }

        private float[] FormFunctionTetra(Point3D point3D)
        {
            var mCoord = Matrix<float>.Build.DenseOfArray(new float[,]
{
                    { 1, nodes[0].Position._x, nodes[0].Position._y,nodes[0].Position._z },
                    { 1, nodes[1].Position._x, nodes[1].Position._y,nodes[1].Position._z },
                    { 1, nodes[2].Position._x, nodes[2].Position._y,nodes[2].Position._z },
                    { 1, nodes[3].Position._x, nodes[3].Position._y,nodes[3].Position._z }
});
            var dN = mCoord.Inverse(); // differential matrix {dN/dn}

            var vCoord = Vector<float>.Build.Dense(new float[] { 1, point3D._x, point3D._y, point3D._z });
            var result = dN.Multiply(vCoord);
            return result.ToArray();
        }

        private float[] FormFunctionHex(Point3D point3D)
        {
            var center = CalcCentralPoint();
            var supportVector = center.Sub(nodes[0].Position);
            var a = Math.Abs(supportVector._x);
            var b = Math.Abs(supportVector._y);
            var c = Math.Abs(supportVector._z);
            var ksi = (point3D._x - center._x) / a;
            var eta = (point3D._y - center._y) / b;
            var psi = (point3D._z - center._z) / c;
            return FormFunctionInLocalHex(ksi, eta, psi);
        }

        private float[] FormFunctionInLocalHex(float ksi, float eta, float psi)
        {
            var answer = new List<float>();
            for (var i = -1; i <= 1; i += 2)
                for (var j = -1; j <= 1; j += 2)
                    for (var k = -1; k <= 1; k += 2)
                        answer.Add((1 + i * ksi) * (1 + j * eta) * (1 + k * psi) / 8);
            return answer.ToArray();
        }

        private float[] FormFunctionPenta(Point3D point3D)
        {
            var topHeight = nodes.Max(x => x.Position._z);
            var botHeight = nodes.Min(x => x.Position._z);

            var bottom = nodes.Where(x => Math.Abs(x.Position._z - botHeight) < 1e-3);
            var top = nodes.Where(z => Math.Abs(z.Position._z - topHeight) < 1e-3);
            var crawlOrder = bottom.Concat(top).ToArray();
            SortCrawlOrder(crawlOrder);
            var form = MakeFormArray(point3D, crawlOrder);
            var answer = new float[6];
            for (var i = 0; i < nodes.Length; i++)
            {
                var node = nodes[i];
                var index = Array.IndexOf(crawlOrder, node);
                answer[i] = form[index];
            }

            return answer;
        }

        private float[] MakeFormArray(Point3D point3D, Node[] crawlOrder)
        {
            var form = new float[6];
            var h = (crawlOrder[3].Position._z - crawlOrder[0].Position._z) / 2;
            var dzeta = (point3D._z - crawlOrder[0].Position._z - h) / h;
            var delta = PentaFormDelta(crawlOrder[0].Position, crawlOrder[1].Position, crawlOrder[2].Position);
            var fBottom = PentaFormF(dzeta, -1);
            var fTop = PentaFormF(dzeta, 1);
            for (var i = 0; i < 3; i++)
            {
                var coefficients =
                    PentaCoefficients(crawlOrder[(i + 1) % 3].Position, crawlOrder[(i + 2) % 3].Position);
                form[i] = PentaFormL(point3D, coefficients, delta) * fBottom;
                form[i + 3] = PentaFormL(point3D, coefficients, delta) * fTop;
            }

            return form;
        }

        private void SortCrawlOrder(IList<Node> crawlOrder)
        {
            for (var i = 0; i < 3; i++)
            {
                var first = crawlOrder[i];
                var second = crawlOrder[(i + 1) % 3];
                var third = crawlOrder[(i + 2) % 3];
                if (Pseudoscalar(first.Position, second.Position, third.Position) < 0)
                {
                    crawlOrder[(i + 2) % 3] = second;
                    crawlOrder[(i + 1) % 3] = third;
                    (crawlOrder[i + 4], crawlOrder[i + 5]) =
                        (crawlOrder[i + 5], crawlOrder[i + 4]);
                    break;
                }
            }
        }

        private float Pseudoscalar(Point3D i, Point3D j, Point3D k) =>
    (j._x - i._x) * (k._y - j._y) - (k._x - j._x) * (j._y - i._y);

        private float[] PentaCoefficients(Point3D j, Point3D k)
        {
            var coefficients = new float[3];
            coefficients[0] = j._x * k._y - k._x * j._y;
            coefficients[1] = j._y - k._y;
            coefficients[2] = k._x - j._x;
            return coefficients;
        }

        private float PentaFormF(float dzeta, float dzetai) => 1f / 2f * (1f + dzeta * dzetai);

        private float PentaFormL(Point3D point, float[] pentaCoefficients, float square) =>
            (pentaCoefficients[0] + pentaCoefficients[1] * point._x + pentaCoefficients[2] * point._y) / square;

        private float PentaFormDelta(Point3D first, Point3D second, Point3D third)
        {
            return second._x * third._y - third._x * second._y -
                third._x * first._y + first._x * third._y +
                first._x * second._y - first._y * second._x;
        }

        /// <inheritdoc/>

        public override IEnumerable<Surface> GetSurface()
        {
            foreach (var surface in surfaces)
            {
                yield return surface;
            }
        }
    }
}
