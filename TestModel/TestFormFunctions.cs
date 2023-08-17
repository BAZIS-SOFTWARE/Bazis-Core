using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;

namespace TestModel
{
    [TestClass]
    public class TestFormFunctions
    {
        [TestMethod]
        public void Test1d1()
        {
            var node1 = new Node(1, new Point3D(1.5f, 0, 0));
            var node2 = new Node(1, new Point3D(6, 0, 0));
            var elem = new Element1D(2, new[] { node1, node2 }, ObjKind.Beam);
            var ans = elem.GetFormFunctions(new Point3D(4, 0, 0));
            var expect = new[] { 2f / 4.5f, 2.5f / 4.5f };
            var diff = ans.Select((x, i) => x - expect[i]);
            foreach (var e in diff)
            {
                Assert.IsTrue(Math.Abs(e) < 1e-3);
            }
        }

        [TestMethod]
        public void Test1d2()
        {
            var node1 = new Node(1, new Point3D(1.5f, 0, 0));
            var node2 = new Node(1, new Point3D(6, 0, 0));
            var elem = new Element1D(2, new[] { node1, node2 }, ObjKind.Beam);
            var ans1 = elem.GetFormFunctions(new Point3D(1.5f, 0, 0));
            var expect = new[] { 1f, 0f };
            foreach (var e in ans1.Select((x, i) => x - expect[i]))
                Assert.IsTrue(Math.Abs(e) < 1e-3);
            var ans2 = elem.GetFormFunctions(new Point3D(6f, 0, 0));
            expect = new[] { 0f, 1f };
            foreach (var e in ans2.Select((x, i) => x - expect[i]))
                Assert.IsTrue(Math.Abs(e) < 1e-3);
        }

        [TestMethod]
        public void TestSum1000Quad()
        {
            var nodes = new Node[8];
            var num = 0;
            for (var i = 0; i < 2; i++)
            for (var j = 0; j < 2; j++)
            for (var k = 0; k < 2; k++)
            {
                nodes[num] = new Node(0, new Point3D(i, j, k));
                num++;
            }

            var elem = new Element3D(0, nodes, ObjKind.Hexa);
            var rnd = new Random();
            for (var i = 0; i < 10000; i++)
            {
                var x = elem.GetFormFunctions(new Point3D((float)rnd.NextDouble(),
                    (float)rnd.NextDouble(),
                    (float)rnd.NextDouble()));
                var sum = x.Sum();
                Assert.IsTrue(Math.Abs(1- sum) < 1e-3);
            }
        }

        [TestMethod]
        public void TestPentaNode()
        {
            var nodes = new Node[6];
            var num = 0;
            for (var i = 0; i < 2; i++)
            for (var j = 0; j < 2; j++)
            for (var k = 0; k < 2; k++)
            {
                if (k + j > 1)
                    continue;
                nodes[num] = new Node(num, new Point3D(j, k, i));
                num++;
            }

            var penta = new Element3D(0, nodes, ObjKind.Penta);

            for (int i = 0; i < 6; i++)
            {
                var ans1 = penta.GetFormFunctions(nodes[i].Position);
                Assert.IsTrue(Math.Abs(1 - ans1[i])<1e-3);
            }
        }

        [TestMethod]
        public void TestSum1000Penta()
        {
            var nodes = new Node[6];
            var num = 0;
            for (var i = 0; i < 2; i++)
            for (var j = 0; j < 2; j++)
            for (var k = 0; k < 2; k++)
            {
                if (k + j > 1)
                    continue;
                nodes[num] = new Node(num, new Point3D(j, k, i));
                num++;
            }

            var penta = new Element3D(0, nodes, ObjKind.Penta);
            var rnd = new Random();
            for (var i = 0; i < 100000; i++)
            {
                var x = rnd.NextDouble();
                var y = rnd.NextDouble();
                while (x + y > 1)
                {
                    x = rnd.NextDouble(); 
                    y = rnd.NextDouble();
                }
                var func = penta.GetFormFunctions(new Point3D((float)x, (float)y,
                    (float)rnd.NextDouble()));
                var sum = func.Sum();
                Assert.IsTrue(Math.Abs(1- sum) < 1e-3);
            }
        }

        [TestMethod]
        public void TestDerivativeHex1000()
        {
            
            var nodes = new Node[8];
            var num = 0;
            for (var i = 0; i < 2; i++)
            for (var j = 0; j < 2; j++)
            for (var k = 0; k < 2; k++)
            {
                nodes[num] = new Node(0, new Point3D(i, j, k));
                num++;
            }

            var elem = new Element3D(0, nodes, ObjKind.Hexa);
            var rnd = new Random();
            for (var i = 0; i < 10000; i++)
            {
                var x = elem.GetDerivativeFromFunction(new Point3D((float)rnd.NextDouble(),
                    (float)rnd.NextDouble(),
                    (float)rnd.NextDouble()));
                var sum = x.Sum(arr => arr.Sum());
                Assert.AreEqual(0, sum);
            }
        }

        [TestMethod]
        public void TestDerivativePenta10000()
        {
            var nodes = new Node[6];
            var num = 0;
            for (var i = 0; i < 2; i++)
            for (var j = 0; j < 2; j++)
            for (var k = 0; k < 2; k++)
            {
                if (k + j > 1)
                    continue;
                nodes[num] = new Node(num, new Point3D(j, k, i));
                num++;
            }

            var penta = new Element3D(0, nodes, ObjKind.Penta);
            var rnd = new Random();
            for (var i = 0; i < 100000; i++)
            {
                var x = rnd.NextDouble();
                var y = rnd.NextDouble();
                while (x + y > 1)
                {
                    x = rnd.NextDouble(); 
                    y = rnd.NextDouble();
                }
                var func = penta.GetDerivativeFromFunction(new Point3D((float)x, (float)y,
                    (float)rnd.NextDouble()));
                var sum = func.Sum(arr => arr.Sum());
                Assert.IsTrue(Math.Abs(sum) < 1e-3);
            }
        }

        [TestMethod]
        public void TestSum1000Triangle()
        {
            var nodes = new Node[3];
            var num = 0;
            for (var j = 0; j < 2; j++)
            for (var k = 0; k < 2; k++)
            {
                if (k + j > 1)
                    continue;
                nodes[num] = new Node(num, new Point3D(j, k, 0));
                num++;
            }

            var triangle = new Element2D(0, nodes, ObjKind.Triangle);
            var rnd = new Random();
            for (var i = 0; i < 100000; i++)
            {
                var x = rnd.NextDouble();
                var y = rnd.NextDouble();
                while (x + y > 1)
                {
                    x = rnd.NextDouble(); 
                    y = rnd.NextDouble();
                }
                var func = triangle.GetFormFunctions(new Point3D((float)x, (float)y, 0));
                var sum = func.Sum();
                Assert.IsTrue(Math.Abs(1- sum) < 1e-3);
            }
        }

        [TestMethod]
        public void TestInNodesTriangle()
        {
            var nodes = new Node[3];
            var num = 0;
            for (var j = 0; j < 2; j++)
            for (var k = 0; k < 2; k++)
            {
                if (k + j > 1)
                    continue;
                nodes[num] = new Node(num, new Point3D(j, k, 0));
                num++;
            }

            var triangle = new Element2D(0, nodes, ObjKind.Triangle);
            for (var i = 0; i < 3; i++)
            {
                var answer = triangle.GetFormFunctions(nodes[i].Position);
                Assert.IsTrue(Math.Abs(1 - answer[i])<1e-3);
            }
        }

        [TestMethod]
        public void Test()
        {
            var nodes = new Node[8];
            var num = 0;
            for (var i = 0; i < 2; i++)
            for (var j = 0; j < 2; j++)
            for (var k = 0; k < 2; k++)
            {
                nodes[num] = new Node(0, new Point3D(i, j, k));
                num++;
            }

            var elem = new Element3D(0, nodes, ObjKind.Hexa);
            var rnd = new Random();

            var x = rnd.NextDouble();
            var y = rnd.NextDouble();
            while (x + y > 1)
            {
                x = rnd.NextDouble(); 
                y = rnd.NextDouble();
            }

            var z = (float)rnd.NextDouble();
            var func = elem.GetFormFunctions(new Point3D((float)x, (float)y, z));
            var sumX = func.Select((t, i) => t * nodes[i].Position._x).Sum();
            var sumY = func.Select((t, i) => t * nodes[i].Position._y).Sum();
            var sumZ = func.Select((t, i) => t * nodes[i].Position._z).Sum();
        }
    }
}