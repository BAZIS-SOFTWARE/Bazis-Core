using System;
using System.Collections.Generic;
using System.Linq;
using ModelInterfaces;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class PentaElement : Element3D
    {
        public PentaElement(int number, Node[] nodes) : base(number, nodes, ObjKind.Penta)
        {
        }

        public override float[] GetFormFunctions(Point3D point3D)
        {
            return FormFunctionPenta(point3D);
        }

        public override float[] GetDerivativesFormFunctions(Point3D point3D)
        {
            return DerivativeFormFunctionPenta(point3D).ToColumnMajorArray();
        }

        public override float DetJ(Point3D point3D)
        {
            return detJPenta();
        }

        public float[][] CoefficientKPenta(float k, float h, Node[] points)
        {
            return VolumeIntegralBdbPenta(k).Add(SquareIntegralNnPenta(points)).Multiply(h).ToRowArrays();
        }

        public float[] CoefficientFPenta(float h, float tInf, Node[] points)
        {
            return SquareIntegralNPenta(points).Multiply(h * tInf).ToArray();
        }

        public float[][] CoefficientCPenta()
        {
            return VolumeIntegralNnPenta().ToRowArrays();
        }

        public override float CalcVolume()
        {
            var sum = 0f;

            for (int i = 0; i < nodes.Count - 3; i++)
            {
                for (int j = i + 1; j < nodes.Count - 2; j++)
                {
                    for (int k = j + 1; k < nodes.Count - 1; k++)
                    {
                        for (int l = k + 1; l < nodes.Count; l++)
                        {
                            var x = new Node[] { (Node)nodes[i], (Node)nodes[j], (Node)nodes[k], (Node)nodes[l] };
                            var tetra = new TetraElement(0, x);
                            sum += tetra.CalcVolume();
                        }
                    }
                }
            }
            return sum / 4;
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
            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                var index = Array.IndexOf(crawlOrder, node);
                answer[i] = form[index];
            }

            return answer;
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

        private float Pseudoscalar(Point3D i, Point3D j, Point3D k) =>
            (j._x - i._x) * (k._y - j._y) - (k._x - j._x) * (j._y - i._y);

        private float PentaFormF(float dzeta, float dzetai) => 1f / 2f * (1f + dzeta * dzetai);

        private float PentaFormL(Point3D point, float[] pentaCoefficients, float square) =>
            (pentaCoefficients[0] + pentaCoefficients[1] * point._x + pentaCoefficients[2] * point._y) / square;

        private float PentaFormDelta(Point3D first, Point3D second, Point3D third)
        {
            return second._x * third._y - third._x * second._y -
                third._x * first._y + first._x * third._y +
                first._x * second._y - first._y * second._x;
        }

        private float[] PentaCoefficients(Point3D j, Point3D k)
        {
            var coefficients = new float[3];
            coefficients[0] = j._x * k._y - k._x * j._y;
            coefficients[1] = j._y - k._y;
            coefficients[2] = k._x - j._x;
            return coefficients;
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

            var answer = new DenseMatrix(3, 6);
            for (var i = 0; i < nodes.Count; i++)
                for (var j = 0; j < 3; j++)
                    answer[j, i] = dForm[nodes[i]][j];

            return answer;
        }

        private float detJPenta()
        {
            return CountHPenta();
        }

        private Matrix<float> VolumeIntegralBdbPenta(float k)
        {
            var supPoints = MakeSupPointsVolumePenta();

            var d = new DiagonalMatrix(3, 3, k);
            var sum = new DenseMatrix(6, 6).Multiply(0);
            foreach (var point in supPoints)
            {
                var b = DerivativeFormFunctionPenta(point);
                var bt = b.Transpose();
                var jacobian = detJPenta();
                sum = sum.Add(bt.Multiply(d).Multiply(b).Multiply(jacobian * 1 / 6f));
            }

            return sum;
        }

        private Matrix<float> SquareIntegralNnPenta(Node[] points)
        {
            var supPoints = MakeSupPointsSquarePenta(points);
            var jacobian = detJPenta();
            var sum = new DenseMatrix(6, 6).Multiply(0);

            foreach (var point in supPoints)
            {
                var x = FormFunctionPenta(point);
                var form = new DenseMatrix(1, 6);
                for (var j = 0; j < x.Length; j++) form[0, j] = x[j];
                sum = sum.Add(form.Transpose().Multiply(form)
                    .Multiply(jacobian * (supPoints.Length == 2 ? 1 : 1 / 6f)));
            }

            return sum;
        }


        private Vector<float> SquareIntegralNPenta(Node[] points)
        {
            var supPoints = MakeSupPointsSquarePenta(points);
            var jacobian = detJPenta();
            var sum = new DenseVector(6).Multiply(0);
            return supPoints.Aggregate(sum,
                (current, point) =>
                    current.Add(
                        new DenseVector(FormFunctionPenta(point)).Multiply(jacobian *
                                                                           (supPoints.Length == 2 ? 1 : 1 / 6f))));
        }

        private Matrix<float> VolumeIntegralNnPenta()
        {
            var supPoints = MakeSupPointsVolumePenta();
            var sum = new DenseMatrix(6, 6).Multiply(0);
            var jacobian = detJPenta();
            foreach (var point in supPoints)
            {
                var m = new DenseMatrix(1, 6, FormFunctionPenta(point));
                sum = sum.Add(m.Transpose().Multiply(m).Multiply(jacobian * 1 / 6f));
            }

            return sum;
        }

        private float CountHPenta()
        {
            var topHeight = nodes.Max(x => x.Position._z);
            var botHeight = nodes.Min(x => x.Position._z);
            var bottom = nodes.Where(x => Math.Abs(x.Position._z - botHeight) < 1e-3);
            var top = nodes.Where(z => Math.Abs(z.Position._z - topHeight) < 1e-3);
            var crawlOrder = bottom.Concat(top).ToArray();
            return (crawlOrder[3].Position._z - crawlOrder[0].Position._z) / 2;
        }

        private Point3D[] MakeSupPointsSquarePenta(Node[] points)
        {
            Point3D[] supPoints;
            Point3D vector;
            switch (points.Length)
            {
                case 3:
                    supPoints = new Point3D[3];
                    var height = points[0].Position._z;
                    for (var i = 0; i < 3; i++)
                    {
                        var first = points[i];
                        var second = points[(i + 1) % 3];
                        var third = points[(i + 2) % 3];
                        if (Pseudoscalar(first.Position, second.Position, third.Position) < 0)
                        {
                            points[(i + 2) % 3] = second;
                            points[(i + 1) % 3] = third;
                            break;
                        }
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        vector = points[(i + 1) % 3].Position.Sub(points[i].Position).Mult(1 / 2f);
                        supPoints[i] = points[i].Position.Sum(vector);
                        supPoints[i]._z = height;
                    }

                    break;
                case 4:
                    supPoints = new Point3D[2];
                    var topHeight = points.Max(x => x.Position._z);
                    var botHeight = points.Min(x => x.Position._z);
                    var bottom = points.Where(x => Math.Abs(x.Position._z - botHeight) < 1e-3).ToArray();
                    var top = points.Where(z => Math.Abs(z.Position._z - topHeight) < 1e-3).ToArray();
                    if (Math.Abs(top[0].Position._x - bottom[0].Position._x) > 1e-3)
                        (top[0], top[1]) = (top[1], top[0]);

                    var h = (top[0].Position._z - bottom[0].Position._z) / 2;
                    vector = bottom[0].Position.Sub(bottom[1].Position).Mult(1 / 2f);
                    supPoints[0] = bottom[0].Position.Sum(vector);
                    supPoints[0]._z = botHeight + h * (1 - 1 / (float)Math.Sqrt(3));
                    supPoints[1] = top[0].Position.Sum(vector);
                    supPoints[1]._z = botHeight + h * (1 + 1 / (float)Math.Sqrt(3));
                    break;
                default:
                    throw new NotSupportedException();
            }

            return supPoints;
        }

        private Point3D[] MakeSupPointsVolumePenta()
        {
            var topHeight = nodes.Max(x => x.Position._z);
            var botHeight = nodes.Min(x => x.Position._z);
            var bottom = nodes.Where(x => Math.Abs(x.Position._z - botHeight) < 1e-3);
            var top = nodes.Where(z => Math.Abs(z.Position._z - topHeight) < 1e-3);
            var crawlOrder = bottom.Concat(top).ToArray();
            SortCrawlOrder(crawlOrder);
            var h = CountHPenta();
            var supPoints = new Point3D[6];
            for (var i = 0; i < 3; i++)
            {
                var vector = crawlOrder[(i + 1) % 3].Position.Sub(crawlOrder[i].Position).Mult(1 / 2f);
                supPoints[i] = crawlOrder[i].Position.Sum(vector);
                supPoints[i]._z = botHeight + h * (1 - 1 / (float)Math.Sqrt(3));
                supPoints[i + 3] = crawlOrder[i + 3].Position.Sum(vector);
                supPoints[i + 3]._z = topHeight + h * (1 + 1 / (float)Math.Sqrt(3));
            }

            return supPoints;
        }
    }
}
