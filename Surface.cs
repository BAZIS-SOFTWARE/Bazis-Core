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
    /// <summary>
    /// Surface class
    /// </summary>
    public class Surface : ModelObject, ISurface, IEquatable<Surface>
    {
        Node[] nodes;
        /// <summary>
        /// Surface constructor
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <param name="objKind"></param>
        public Surface(int number, Node[] nodes, ObjKind objKind) : base(number)
        {
            ObjKind = objKind;
            this.nodes = Anticlockwise(nodes);

            ObjType = "Поверхность";

            InitialColor = Color.FromArgb(224, 0, 177);
            MasterColor = Color.FromArgb(224, 0, 177);
            SlaveColor = Color.FromArgb(224, 0, 177);
        }
        /// <summary>
        /// Surface constructor
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <param name="objKind"></param>
        /// <param name="color"></param>
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
            var ar = new List<Node>();
            var arInd = new List<int>();

            var fInd = 0;
            var sInd = 1;

            var firstNode = nodes[fInd];
            var secondNode = nodes[sInd];
            //var baseVector = secondNode.Position.Sub(firstNode.Position);

            //baseVector.Round(2);
            var lenght = nodes.Length;

            ar.Add(nodes[fInd]);
            arInd.Add(fInd);
            var iterationCounter = 0;
            var iterationMax = lenght * lenght;
            while (arInd.Count != lenght)
            {
                if (iterationCounter > iterationMax) break;

                var resu = CheckVectorsOrientation(nodes, firstNode, secondNode);

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
                //baseVector = nodes[sInd].Position.Sub(firstNode.Position);
                secondNode = nodes[sInd];

                iterationCounter++;
            }

            if (ar.Count < nodes.Length) // check arrays 
                return nodes;

            return ar.ToArray();
        }

        private bool CheckVectorsOrientation(Node[] nodes, Node firstNode, Node secondNode)
        {
            //bool isPointInside = false;
            var temp = new List<Point3D>();
            var baseVector = secondNode.Position.Sub(firstNode.Position);

            foreach (var node in nodes)
            {
                if (!node.Equals(firstNode) & !node.Equals(secondNode))
                {
                    var actVector = node.Position.Sub(firstNode.Position);

                    var prod = Vector.CrossProd(baseVector, actVector);

                    if (Math.Abs(prod._x) < 1e-6)
                        prod._x = 0;
                    if (Math.Abs(prod._y) < 1e-6)
                        prod._y = 0;
                    if (Math.Abs(prod._z) < 1e-6)
                        prod._z = 0;
                    //if (prod == new Point3D(0, 0, 0))
                    //{
                    //    var baseLength = Vector.GetVectorLenght(baseVector);
                    //    var actLength = Vector.GetVectorLenght(actVector);
                    //    if (baseLength - actLength > 1e-4)
                    //    {
                    //        isPointInside = true; break;
                    //    }
                    //}
                    temp.Add(prod);
                }

            }
            //var resu = false;
            //if (!isPointInside) resu = Vector.IsVectorsCoDirectional(temp);
            //return resu;
            return Vector.IsVectorsCoDirectional(temp);
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

        public override int NumberOfPoints { get { return nodes.Length; } }

        public int NumberOfSides { get { return 1; } }

        //public int NumberOfSides { get; internal set; }

        /// <inheritdoc/>

        public Point3D CalcNormal()
        {
            var fvectors = nodes[1].Position.Sub(nodes[0].Position);
            var svectors = nodes[2].Position.Sub(nodes[0].Position);
            return Vector.CrossProd(fvectors,svectors);
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
            var square = 0.0f;
            for (int i = 0; i < NumberOfPoints - 2; i++)
            {
                var v1 = this[i + 1].Position.Sub(this[0].Position);
                var v2 = this[i + 2].Position.Sub(this[0].Position);

                var vProd = Vector.CrossProd(v1, v2);
                square += Math.Abs(Vector.GetVectorLenght(vProd) / 2);
            }


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
            yield return this;
        }
        /// <summary>
        /// GetDistance
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
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
