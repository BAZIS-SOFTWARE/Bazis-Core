using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Surface : ModelObject, ISurface, IEquatable<Surface>
    {
        Node[] nodes;

        public Surface(int number, Node[] nodes, ObjKind objKind) : base(number)
        {
            ObjKind = objKind;
            this.nodes = nodes;
            ObjType = "Поверхность";
            NumberOfPoints = nodes.Length;
            InitialColor = Color.FromArgb(224, 0, 177);
            MasterColor = Color.FromArgb(224, 0, 177);
            SlaveColor = Color.FromArgb(224, 0, 177);
        }
        public Surface(int number, Node[] nodes, ObjKind objKind, Color color) : this(number,nodes,objKind)
        {
            InitialColor = color;
            MasterColor = color;
            SlaveColor = color;
        }

        public Node this [int ind]
        {
            get { return nodes[ind]; }
        }

        public override string ToString()
        {
            var strArr = GetNodes().Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return string.Format("{0} {1} : {2}", Number, ObjType, str);
        }

        public override ObjKind ObjKind { get; }

        public override string ObjType { get; }

        public override int NumberOfPoints { get; }

        public Point3D CalcNormal()
        {
            var x1 = nodes[0].Position._x; var x2 = nodes[1].Position._x; var x3 = nodes[2].Position._x;
            var y1 = nodes[0].Position._y; var y2 = nodes[1].Position._y; var y3 = nodes[2].Position._y;
            var z1 = nodes[0].Position._z; var z2 = nodes[1].Position._z; var z3 = nodes[2].Position._z;

            var fvectors = new Point3D(x2 - x1, y2 - y1, z2 - z1);
            var svectors = new Point3D(x3 - x1, y3 - y1, z3 - z1);


            return Vector.GetVectorsProd(fvectors,svectors);
        }

        public IEnumerable<Node> GetNodes()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }

        public float CalcSquare()
        {
            var pAr = GetPoints().ToArray();

            var v1 = new Point3D(pAr[1]._x - pAr[0]._x, pAr[1]._y - pAr[0]._y, pAr[1]._z - pAr[0]._z);
            var v2 = new Point3D(pAr[2]._x - pAr[0]._x, pAr[2]._y - pAr[0]._y, pAr[2]._z - pAr[0]._z);

            var vProd = Vector.GetVectorsProd(v1, v2);
            var square = Math.Abs(Vector.GetVectorLenght(vProd) / 2);
            return square;
        }

        public override IEnumerable<Point3D> GetPoints()
        {
            foreach (var node in nodes)
            {
                yield return node.Position;
            }
        }

        public bool Equals(Surface other)
        {
            var points = other.GetPoints().ToArray();
            if (points.Contains(nodes[0].Position, new Point3DEqualityComparer()) &&
points.Contains(nodes[1].Position, new Point3DEqualityComparer()) &&
points.Contains(nodes[2].Position, new Point3DEqualityComparer()))
                return true;
            else
                return false;
        }

        public IEnumerable<Surface> GetSurface()
        {
            yield return new Surface(Number, nodes.ToArray(), ObjKind);
        }

        public float GetDistance(Point3D point)
        {
            var normal = CalcNormal();
            var nNormal = Vector.GetVectorNorm(normal);
            var temp = point.Sub(nodes[0].Position);

            return nNormal._x * temp._x + nNormal._y * temp._y + nNormal._z * temp._z;
        }

        public override Point3D CalcCentralPoint()
        {
            var center = new Point3D();

            foreach (var point in GetPoints())
            {
                center = center.Sum(point);
            }
            return center.Div(NumberOfPoints);
        }
    }
}
