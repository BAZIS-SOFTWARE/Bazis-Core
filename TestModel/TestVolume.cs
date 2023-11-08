using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModel
{
    [TestClass]
    public class TestVolume
    {
        [TestMethod]
        public void TestHex()
        {
            var nodes = new Node[8];
            var num = 0;
            for (var i = 0; i < 4; i += 3)
                for (var j = 0; j < 4; j += 3)
                    for (var k = 0; k < 4; k += 3)
                    {
                        nodes[num] = new Node(0, new Point3D(i, j, k));
                        num++;
                    }

            var elem = new HexElement(0, nodes);
            var x = elem.CalcVolume();
            Assert.IsTrue(x - 27 < 1e-4);
        }

        [TestMethod]
        public void TestPenta()
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

            var penta = new PentaElement(0, nodes);
            var x = penta.CalcVolume();
            Assert.IsTrue(x - 0.5f < 1e-4);
        }

        [TestMethod]
        public void TestQuad()
        {
            var nodes = new Node[4];
            var num = 0;
            for (var i = 0; i < 2; i += 1)
                for (var j = 0; j < 2; j += 1)
                {
                    nodes[num] = new Node(0, new Point3D(i, j, 0));
                    num++;
                }

            var elem = new QuadElement(0, nodes);
            Assert.IsTrue(elem.CalcSquare() - 1f < 1e-4);
        }
    }
}
