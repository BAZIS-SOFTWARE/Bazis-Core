using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.MeshObjects;
using ModelController.MeshObjsCreator.Delone2D;
using ModelController.MeshObjsCreator;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;

namespace TestModel
{
    [TestClass]
    public class Sew3Dfrom2DTests
    {
        [TestMethod]
        [DataRow(0f, 0f, 0f, 0f, 1f, 0f, 1f, 0f, 0f)]
        [DataRow(5f, 7f, 8f, 6f, 15f, 20f, 11f, 20f, 6f)]
        public void TestCalcCircumcenter(
            float x1, float y1, float z1,
            float x2, float y2, float z2,
            float x3, float y3, float z3)
        {
            Node[] nodes = new Node[3]
            {
                new Node(1, new Point3D( x1, y1, z1)),
                new Node(2, new Point3D( x2, y2, z2)),
                new Node(3, new Point3D( x3, y3, z3)),
            };
            var triangle = new Triangle(4, nodes);

            var m = typeof(Sew3DFrom2D).GetMethod("CalcCircumcenter", BindingFlags.NonPublic | BindingFlags.Static);
            var circumcenter = m.Invoke(null, new object[] { triangle }) as Point3D;

            var r1 = circumcenter.Sub(nodes[0].Position).GetSquareSumCoords();
            var r2 = circumcenter.Sub(nodes[1].Position).GetSquareSumCoords();
            var r3 = circumcenter.Sub(nodes[2].Position).GetSquareSumCoords();
            Assert.IsTrue(Math.Abs(r1 - r2) < 10e-6);
            Assert.IsTrue(Math.Abs(r1 - r3) < 10e-6);
            Assert.IsTrue(Math.Abs(r3 - r2) < 10e-6);

            //assert
        }
        [TestMethod]
        public void GetArbitraryTetras()
        {
            var (pointsForFirstPlane, pointsForSecondPlane) = GetPointOn2Planes();

            var instance = new DeloneMesh2DCreator();
            instance.MakeTriangulation(pointsForFirstPlane);

            var triangulation1 = instance.MakeTriangulation(pointsForFirstPlane).ToList();

            var triangulation2 = instance.MakeTriangulation(pointsForSecondPlane).ToList();

            Assert.IsTrue(TestDeloneTriangles(triangulation1));
            Assert.IsTrue(TestDeloneTriangles(triangulation2));

            var tetraMapper = new Sew3DFrom2D();

            var map = tetraMapper.Create(triangulation1.OfType<IElement2D>().ToList(),
                triangulation2.OfType<IElement2D>().ToList());

            Assert.IsTrue(TestDeloneTetras(map.OfType<Tetra>().ToList()));
        }

        private Point3D CalcCircumcenter(IElement2D elem)
        {
            //https://en.wikipedia.org/wiki/Circumcircle
            var vertexes = elem.GetCoordinates().ToList();
            var a2 = vertexes[0].Sub(vertexes[1]).GetSquareSumCoords();
            var b2 = vertexes[1].Sub(vertexes[2]).GetSquareSumCoords();
            var c2 = vertexes[2].Sub(vertexes[0]).GetSquareSumCoords();
            var fa = a2 * (b2 + c2 - a2);
            var fb = b2 * (c2 + a2 - b2);
            var fc = c2 * (a2 + b2 - c2);
            var invSum = 1f / (fa + fb + fc);
            return new Point3D(
                (vertexes[2]._x * fa + vertexes[0]._x * fb + vertexes[1]._x * fc) * invSum,
                (vertexes[2]._y * fa + vertexes[0]._y * fb + vertexes[1]._y * fc) * invSum,
                (vertexes[2]._z * fa + vertexes[0]._z * fb + vertexes[1]._z * fc) * invSum);
        }

        Point3D tetrahedron_circumcenter(Tetra elem)
        {
            //https://rodolphe-vaillant.fr/entry/127/find-a-tetrahedron-circumcenter

            var nodes = elem.GetVertexes().OfType<Node>().ToArray();
            double[] a = { nodes[0]._x, nodes[0]._y, nodes[0]._z };
            double[] b = { nodes[1]._x, nodes[1]._y, nodes[1]._z };
            double[] c = { nodes[2]._x, nodes[2]._y, nodes[2]._z };
            double[] d = { nodes[3]._x, nodes[3]._y, nodes[3]._z };
            double denominator;


            // Use coordinates relative to point 'a' of the tetrahedron.

            // ba = b - a
            double ba_x = b[0] - a[0];
            double ba_y = b[1] - a[1];
            double ba_z = b[2] - a[2];

            // ca = c - a
            double ca_x = c[0] - a[0];
            double ca_y = c[1] - a[1];
            double ca_z = c[2] - a[2];

            // da = d - a
            double da_x = d[0] - a[0];
            double da_y = d[1] - a[1];
            double da_z = d[2] - a[2];

            // Squares of lengths of the edges incident to 'a'.
            double len_ba = ba_x * ba_x + ba_y * ba_y + ba_z * ba_z;
            double len_ca = ca_x * ca_x + ca_y * ca_y + ca_z * ca_z;
            double len_da = da_x * da_x + da_y * da_y + da_z * da_z;

            // Cross products of these edges.

            // c cross d
            double cross_cd_x = ca_y * da_z - da_y * ca_z;
            double cross_cd_y = ca_z * da_x - da_z * ca_x;
            double cross_cd_z = ca_x * da_y - da_x * ca_y;

            // d cross b
            double cross_db_x = da_y * ba_z - ba_y * da_z;
            double cross_db_y = da_z * ba_x - ba_z * da_x;
            double cross_db_z = da_x * ba_y - ba_x * da_y;

            // b cross c
            double cross_bc_x = ba_y * ca_z - ca_y * ba_z;
            double cross_bc_y = ba_z * ca_x - ca_z * ba_x;
            double cross_bc_z = ba_x * ca_y - ca_x * ba_y;

            // Calculate the denominator of the formula.
            denominator = 0.5 / (ba_x * cross_cd_x + ba_y * cross_cd_y + ba_z * cross_cd_z);

            // Calculate offset (from 'a') of circumcenter.
            double circ_x = (len_ba * cross_cd_x + len_ca * cross_db_x + len_da * cross_bc_x) * denominator;
            double circ_y = (len_ba * cross_cd_y + len_ca * cross_db_y + len_da * cross_bc_y) * denominator;
            double circ_z = (len_ba * cross_cd_z + len_ca * cross_db_z + len_da * cross_bc_z) * denominator;

            return new Point3D((float)(circ_x + a[0]), (float)(circ_y + a[1]), (float)(circ_z + a[2]));

        }

        private (List<Node> pointsForFirstPlane, List<Node> pointsForSecondPlane) GetPointOn2Planes()
        {
            var offset = new Point3D(-0f, -0f, -1);
            return (
                new List<Node>()
                {
                    new Node(0, new Point3D(0, 0, 0)),
                    new Node(1, new Point3D(0, 1, 0)),
                    new Node(2, new Point3D(0, 2, 0)),
                    new Node(3, new Point3D(1, 0, 0)),
                    new Node(4, new Point3D(1, 1, 0)),
                    new Node(5, new Point3D(1, 2, 0)),
                    new Node(6, new Point3D(2, 0, 0)),
                    new Node(7, new Point3D(2, 1, 0)),
                    new Node(8, new Point3D(2, 2, 0)),
                },
                new List<Node>()
                {
                    new Node(100, new Point3D(0, 0, 0).Sub(offset)),
                    new Node(101, new Point3D(0, 1, 0).Sub(offset)),
                    new Node(102, new Point3D(0, 2, 0).Sub(offset)),
                    new Node(103, new Point3D(1, 0, 0).Sub(offset)),
                    new Node(104, new Point3D(1, 1, 0).Sub(offset)),
                    new Node(105, new Point3D(1, 2, 0).Sub(offset)),
                    new Node(106, new Point3D(2, 0, 0).Sub(offset)),
                    new Node(107, new Point3D(2, 1, 0).Sub(offset)),
                    new Node(108, new Point3D(2, 2, 0).Sub(offset)),
                }); 
        }

        private bool TestDeloneTriangles(IEnumerable<IElement2D> triangulation)
        {
            float EPS = 10e-3f;


            foreach (var triangle in triangulation.ToList())
            {
                var center = CalcCircumcenter(triangle);
                var points = triangle.GetCoordinates().ToArray();
                var a2 = center.Sub(points[0]).GetSquareSumCoords();
                var b2 = center.Sub(points[1]).GetSquareSumCoords();
                var c2 = center.Sub(points[2]).GetSquareSumCoords();

                Assert.IsTrue(Math.Abs(a2 - b2) < EPS, "Math.Abs(a2 - b2) > EPS");
                Assert.IsTrue(Math.Abs(a2 - c2) < EPS, "Math.Abs(a2 - b2) > EPS");

                foreach (var point in triangulation.SelectMany(w => w.GetCoordinates()).ToList())
                {
                    // пропускаем точки треугольника
                    if (point.Sub(points[0]).GetSquareSumCoords() < EPS)
                        continue;
                    if (point.Sub(points[1]).GetSquareSumCoords() < EPS)
                        continue;
                    if (point.Sub(points[2]).GetSquareSumCoords() < EPS)
                        continue;

                    var r2 = point.Sub(center).GetSquareSumCoords();
                    Assert.IsTrue(r2 >= a2 - EPS, $"Fail test Delone triangle r2={r2}, a2={a2}");
                }
            }

            return true;
        }

        private bool TestDeloneTetras(List<Tetra> triangulation)
        {
            float EPS = 10e-3f;

            for (int i = 0; i < triangulation.Count; i++)
            {
                var tetra = triangulation[i];
                //}
                //foreach (var tetra in triangulation)
                //{
                Point3D center = tetrahedron_circumcenter(tetra);
                var points = tetra.GetCoordinates().ToArray();
                var a2 = center.Sub(points[0]).GetSquareSumCoords();
                var b2 = center.Sub(points[1]).GetSquareSumCoords();
                var c2 = center.Sub(points[2]).GetSquareSumCoords();
                var d2 = center.Sub(points[3]).GetSquareSumCoords();

                Assert.IsTrue(Math.Abs(a2 - b2) < EPS, "Math.Abs(a2 - b2) > EPS");
                Assert.IsTrue(Math.Abs(a2 - c2) < EPS, "Math.Abs(a2 - c2) > EPS");
                Assert.IsTrue(Math.Abs(a2 - d2) < EPS, "Math.Abs(a2 - d2) > EPS");

                foreach (var point in triangulation.SelectMany(w => w.GetCoordinates()).ToList())
                {
                    // пропускаем точки треугольника
                    if (point.Sub(points[0]).GetSquareSumCoords() < EPS)
                        continue;
                    if (point.Sub(points[1]).GetSquareSumCoords() < EPS)
                        continue;
                    if (point.Sub(points[2]).GetSquareSumCoords() < EPS)
                        continue;
                    if (point.Sub(points[3]).GetSquareSumCoords() < EPS)
                        continue;

                    var r2 = point.Sub(center).GetSquareSumCoords();
                    if (r2 <= a2)
                        ;
                    Assert.IsTrue(r2 >= a2 - EPS, $"Fail test Delone tetra r2={r2}, a2={a2}");
                }
            }

            return true;
        }
    }
}
