using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.MeshObjects;
using System;

namespace TestModel
{
    [TestClass]
    public class TestSurfaceElements
    {
		[DataTestMethod]
		[DataRow(new float[] { -1.5f, 0, 15.25f, 0, 11, 5, 5.025f, 5.75f }, 61.5f)]
		[DataRow(new float[] { 0, 0, 15, 0, 10, 5, 5, 5 }, 50)]
		[DataRow(new float[] { 15, 0, 0, 0, 10, 5, 5, 5 }, 50)]
		[DataRow(new float[] { 5, 5, 0, 0, 15, 0, 10, 5 }, 50)]
		public void QuadSquare(float[] arCoord, float square)
		{
			var nodes = new Node[4];
			var num = 0;
			for (var i = 0; i < 4; i += 1)
			{
				nodes[num] = new Node(num, new Point3D(arCoord[2 * i + 0], arCoord[2 * i + 1],0));
				num++;
			}

			var elem = new Quad(0, nodes,4);
			Assert.AreEqual(elem.CalcSquare(), square, 0.15f);
		}
	}
}
