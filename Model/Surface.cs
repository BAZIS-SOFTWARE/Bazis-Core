using Geometry;
using Model.MeshObjects;
using ModelInterfaces;
using ModelInterfaces.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Model
{
    /// <summary>
    /// Surface class
    /// </summary>
    public class Surface : ModelObject, ISurface
    {
        Node[] nodes;
        /// <summary>
        /// Surface constructor
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <param name="objKind"></param>
        public Surface(int number, Node[] nodes) : base(number)
        {
            if (nodes.Length < 3)
                throw new Exception("Для создания поверхности требуется не меньше 3-х узлов!");
            
            nodes.RoundSort(); //Anticlockwise(nodes);
            this.nodes = nodes;

            InitialColor = Color.FromArgb(224, 0, 177);
            MasterColor = Color.FromArgb(224, 0, 177);
            SlaveColor = Color.FromArgb(224, 0, 177);
        }
        /// <summary>
        /// Sort nodes anticlockwise
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        [Warning("Предлагаю ипользовать метод RoundSort() из Geometry")]
        public Node[] Anticlockwise(Node[] nodes)
        {
            var ar = new List<Node>();
            var arInd = new List<int>();
            var plane = new Plane(nodes[0].Position, nodes[1].Position, nodes[2].Position);
            plane.Shifting = 0;

            var fInd = 0;
            var sInd = 1;

            var lenght = nodes.Length;

            ar.Add(nodes[fInd]);
            arInd.Add(fInd);
            var iterationCounter = 0;
            var iterationMax = lenght * lenght;
            while (arInd.Count != lenght)
            {
                if (iterationCounter > iterationMax) break;

                var resu = CheckVectorsOrientation(nodes, fInd, sInd, plane);

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

                iterationCounter++;
            }

            if (ar.Count < nodes.Length) // check arrays 
                return nodes;

            return ar.ToArray();
        }
        [Warning("Предлагаю ипользовать метод RoundSort() из Geometry")]
        private bool CheckVectorsOrientation(Node[] nodes, int firstInd, int secondInd, Plane plane)
        {
            //bool isPointInside = false;
            var temp = new List<float>();
            var baseVector = nodes[secondInd].Position.Sub(nodes[firstInd].Position);
            //var nBaseVector = Vector.GetVectorNorm(baseVector);
            foreach (var node in nodes)
            {
                if (!node.Equals(nodes[firstInd]) & !node.Equals(nodes[secondInd]))
                {
                    var actVector = node.Position.Sub(nodes[firstInd].Position);

                    var prod = Vector.CrossProd(baseVector, actVector);

                    var dist = plane.PointPosition(prod);
                    temp.Add(dist);
                }

            }

            if (temp.All(x => Math.Sign(x) >= 0))
                return true;
            else if (temp.All(x => Math.Sign(x) < 0))
                return true;
            else return false;
        }

        public INode this[int ind]
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

        public override ObjType ObjType { get; } = ObjType.Поверхность;
        /// <inheritdoc/>

        public override int NumberOfPoints { get { return nodes.Length; } }

        public int NumberOfSides { get { return 1; } }        

        /// <inheritdoc/>

        public Point3D CalcNormal()
        {
            var fvectors = nodes[1].Position.Sub(nodes[0].Position);
            var svectors = nodes[2].Position.Sub(nodes[0].Position);
            return Vector.CrossProd(fvectors, svectors);
        }
/// <inheritdoc/>

        public IEnumerable<INode> GetNodes()
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
/// <inheritdoc/>

        public bool Equals(ISurface other)
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
        public IEnumerable<ISurface> GetSurface()
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

        public Segment3D[] GetLinesFromSurface(ISurface surface)
        {
            var n = surface.GetPoints().Count();
            var points = surface.GetPoints().ToArray();
            var lines = new Segment3D[n];
            for (int i = 0; i < n - 1; i++)
            {
                lines[i] = new Segment3D(points[i], points[i + 1]);
            }
            lines[n - 1] = new Segment3D(points[n - 1], points[0]);
            return lines;
        }
    }
}