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
         
            this.nodes = Anticlockwise(nodes);
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
        /// <summary>
        /// Sort nodes anticlockwise
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public Node[] Anticlockwise(Node [] nodes)
        {
            if (nodes.Length < 4) return nodes;

            var ar = new List<Node>();
            var arInd = new List<int>();

            var fInd = 0;
            var sInd = 1;

            var firstNode = nodes[fInd];
            var baseVector = nodes[sInd].Position.Sub(firstNode.Position);

            //baseVector.Round(2);
            var lenght = nodes.Length;

            ar.Add(nodes[fInd]);
            arInd.Add(fInd);
            var iterationCounter = 0;
            var iterationMax = lenght * lenght;
            while (arInd.Count != lenght)
            {
                if (iterationCounter > iterationMax) break;

                var resu = CheckVectorsOrientation(nodes, firstNode, baseVector);

                if (resu)
                {
                    if (!arInd.Contains(sInd))
                    {
                        ar.Add(nodes[sInd]);
                        arInd.Add(sInd);
                        var tempInd = sInd;
                        fInd = tempInd;
                    }
                    //if (sInd == arInd[0] && )
                    //    break;
                }

                sInd++;
                if (sInd == fInd) sInd++;
                if (sInd == nodes.Length) sInd = 0;

                firstNode = nodes[fInd];
                baseVector = nodes[sInd].Position.Sub(firstNode.Position);

                iterationCounter++;
            }

            return ar.ToArray();
        }

        private bool CheckVectorsOrientation(Node[] nodes, Node firstNode, Point3D baseVector)
        {
            bool isPointInside = false;
            var temp = new List<Point3D>();

            foreach (var node in nodes)
            {
                if (!node.Equals(firstNode))
                {

                    var actVector = node.Position.Sub(firstNode.Position);

                    var prod = Vector.GetVectorsProd(baseVector, actVector);
                    if (prod == new Point3D(0, 0, 0))
                    {
                        var baseLength = Vector.GetVectorLenght(baseVector);
                        var actLength = Vector.GetVectorLenght(actVector);
                        if (baseLength - actLength > 1e-4)
                        {
                            isPointInside = true; break;
                        }
                    }
                    temp.Add(prod);
                }

            }
            var resu = false;
            if (!isPointInside) resu = Vector.IsVectorsCoDirectional(temp);
            return resu;
        }

        public Node this [int ind]
        {
            get { return nodes[ind]; }
        }
/// <inheritdoc/>

        public override string ToString()
        {
            var strArr = GetNodes().Select(x => x.Number.ToString()).ToArray();

            var str = string.Join(" ", strArr);

            return string.Format("{0} {1} : {2}", Number, ObjType, str);
        }
/// <inheritdoc/>

        public override ObjKind ObjKind { get; }
/// <inheritdoc/>

        public override string ObjType { get; }
/// <inheritdoc/>

        public override int NumberOfPoints { get; }
/// <inheritdoc/>

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
/// <inheritdoc/>

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
        /// <summary>
        /// Get surface
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Surface> GetSurface()
        {
            yield return new Surface(Number, nodes.ToArray(), ObjKind,MasterColor);
        }

        public float GetDistance(Point3D point)
        {
            var normal = CalcNormal();
            var nNormal = Vector.GetVectorNorm(normal);
            var temp = point.Sub(nodes[0].Position);

            return nNormal._x * temp._x + nNormal._y * temp._y + nNormal._z * temp._z;
        }
/// <inheritdoc/>

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
