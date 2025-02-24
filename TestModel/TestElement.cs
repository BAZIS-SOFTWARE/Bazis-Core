using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.MeshObjsCreator;
using Model.MeshObjects;
using Model.Interfaces.MeshObjects;
using Model.Utilities;
using MathNet.Numerics.LinearAlgebra;

namespace TestModel
{
    [TestClass]
    public class TestElement
    {
        [TestMethod]
        public void CalcTetraNormals()
        {
            //assing
            var nodes = new Node[4]
{
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
};

            var elem = new Tetra(0, nodes);

            //act
            var normals = elem.GetSurfaces().Select(x => x.CalcNormal()).ToList();

            //assert
            var ptr = 0;
            foreach (var surface in elem.GetSurfaces())
            {
                var angle = Vector.GetCosAngleVectors(surface.CalcNormal(), normals[ptr]);
                Assert.AreEqual(1,Vector.GetCosAngleVectors(surface.CalcNormal(), normals[ptr]),0.1f);
                    ptr++;
            }
        }

        [DataTestMethod]
        [DataRow(new float[] { -1.5f, 0, 15.25f, 0, 11, 5}, 15.11f)]
        [DataRow(new float[] { 15, 0, 0, 0, 10, 5}, 18.7082f)]
        [DataRow(new float[] { 8.5f, 4, 0, 7.5f, 4, 0 }, 1)]
        public void BeamLength(float[] arCoord, float length)
        {
            var nodes = new Node[2];
            var num = 0;
            for (var i = 0; i < 2; i += 1)
            {
                nodes[num] = new Node(num, new Point3D(arCoord[3 * i + 0], arCoord[3 * i + 1], arCoord[3 * i + 2]));
                num++;
            }

            var elem = new Beam(0, nodes,2);

            Assert.AreEqual(length,elem.CalcLength(), 0.15f);
        }


        [DataTestMethod]
        [DataRow(new float[] { -1.5f, 0, 15.25f, 0, 11, 5, 5.025f, 5.75f }, 61.5f)]
        [DataRow(new float[] { 0, 0, 15, 0, 10, 5, 5, 5 }, 50)]
        [DataRow(new float[] { 15, 0, 0, 0, 5, 5,10, 5 }, 50)]
        [DataRow(new float[] { 5, 5, 0, 0, 15, 0, 10, 5 }, 50)]
        public void QuadSquare(float[] arCoord, float square)
        {
            var nodes = new Node[4];
            var num = 0;
            for (var i = 0; i < 4; i += 1)
            {
                nodes[num] = new Node(num, new Point3D(arCoord[2 * i + 0], arCoord[2 * i + 1], 0));
                num++;
            }

            var elem = new Quad(0, nodes, 4);
            Assert.AreEqual(elem.CalcSquare(), square, 0.15f);
        }

        [DataTestMethod]
        [DataRow(new float[] { 0, 0, 3,2, 1, 6 }, new double[] { -1, 2, -6, -4 })]

        public void TriangleJacobyMatrix(float[] arCoord, double[] J)
        {
            var nodes = new Node[3];
            var num = 0;
            for (var i = 0; i < 3; i += 1)
            {
                nodes[num] = new Node(num, new Point3D(arCoord[2 * i + 0], arCoord[2 * i + 1], 0));
                num++;
            }

            var elem = new Triangle(0, nodes, 1);
            
            var derFormFunctions = elem.GetDerivativesFormFunctions(elem.GetIntegralPoints().First());
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(3, 2);

            for (var i = 0; i < nodes.Length; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
            }

            var s = matrixPoints.ToString();
            var s1 = matrixDFF.ToString();

            var result = matrixDFF.Multiply(matrixPoints).ToColumnMajorArray();

            for (int i = 0; i < result.Length; i++)
                Assert.AreEqual(J[i], result[i], 0.025);

        }

        [DataTestMethod]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                1, 0, 0,
                1, 1,0,
                0,1,0,
                0,0,1,
                1.2f,0,1,
                1,1,0.7f,
                0,1.5f,1,
            }, 8)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                1, 0, 0,
                1, 1,0,
                0,1,0,
                0,0,1,
                1.2f,0,1,
                1,1,0.7f,
                0,1.5f,1,
            }, 1)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                1.5f, 0, 0,
                1, 1,0,
                0,1,0,
                0,0,1,
                1.2f,0,1,
                1,1,0.7f,
                0,1.5f,1,
            }, 8)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                3, 0, 0,
                1, 3,0,
                0,3,0,
                0,0,3,
                1.2f,0,1,
                1,1,0.7f,
                0,1.5f,1,
            }, 8)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                1.75f, 0, 0,
                1, 2,0,
                0,1.75f,0,
                0,0,1.75f,
                1.2f,0,1,
                1,1,0.7f,
                0,1.5f,1,
            }, 1)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                1, 0, 0,
                1, 1,0,
                0,1,0,
                0,0,1,
                1,0,1,
                1,1,1,
                0,1,1,
            }, 1)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                10, 0, 0,
                10, 10,0,
                0,10,0,
                0,0,10,
                10,0,10,
                10,10,10,
                0,10,10,
            }, 1)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                10, 0, 0,
                10, 10,0,
                0,10,0,
                0,0,10,
                10,0,10,
                10,10,10,
                0,10,10,
            }, 8)]
        public void HexVolume(float[] coords, int intPoints)
        {
            //assign
            var nodes = new Node[8];
            var num = 0;
            for (var i = 0; i < coords.Length; i += 3)
            {
                var x = coords[i + 0];
                var y = coords[i + 1];
                var z = coords[i + 2];
                nodes[num] = new Node(num, new Point3D(x, y, z));
                num++;
            }

            var hexa = new Hexa(0, nodes, intPoints);

            //act
            var hNodes = hexa.GetVertexes().ToArray();

            var hVol = new Tetra(0, new Node[] { hNodes[0], hNodes[1], hNodes[2], hNodes[5] }).CalcVolume();
            hVol += new Tetra(0, new Node[] { hNodes[0], hNodes[2], hNodes[5], hNodes[6] }).CalcVolume();
            hVol += new Tetra(0, new Node[] { hNodes[0], hNodes[2], hNodes[3], hNodes[6] }).CalcVolume();

            hVol += new Tetra(0, new Node[] { hNodes[0], hNodes[3], hNodes[6], hNodes[7] }).CalcVolume();
            hVol += new Tetra(0, new Node[] { hNodes[0], hNodes[4], hNodes[6], hNodes[7] }).CalcVolume();
            hVol += new Tetra(0, new Node[] { hNodes[0], hNodes[4], hNodes[5], hNodes[6] }).CalcVolume();
            
            //assert
            Assert.AreEqual(hVol, hexa.CalcVolume(), 0.1f);
        }

        [DataTestMethod]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                0.5f, 0, 0,
                0, 1.9f,0,
                0,0,0.1f,

            }, 19f / 1200)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                0.5f, 0, 0,
                0, 1.9f,0,
                0,0,100.1f,

            }, 19019f / 1200)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                1, 0, 0,
                0, 1,0,
                0,0,1,

            }, 1f/6)]
        public void TetraVolume(float[] coords, float vol)
        {
            var nodes = new Node[4];
            var num = 0;
            for (var i = 0; i < coords.Length; i += 3)
            {
                var x = coords[i + 0];
                var y = coords[i + 1];
                var z = coords[i + 2];
                nodes[num] = new Node(num, new Point3D(x, y, z));
                num++;
            }

            var tetra = new Tetra(0, nodes, 1);

            Assert.AreEqual(tetra.CalcVolume(), vol, 0.05f);
        }

        [DataTestMethod]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                0.5f, 0, 0,
                0, 1.9f,0,
                0,0,0.1f,

            }, 19f / 1200)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                0.5f, 0, 0,
                0, 1.9f,0,
                0,0,100.1f,

            }, 19019f / 1200)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                1, 0, 0,
                0, 1,0,
                0,0,1,

            }, 1f / 6)]
        public void Tetra_c_Volume(float[] coords, float vol)
        {
            var nodes = new Node[4];
            var num = 0;
            for (var i = 0; i < coords.Length; i += 3)
            {
                var x = coords[i + 0];
                var y = coords[i + 1];
                var z = coords[i + 2];
                nodes[num] = new Node(num, new Point3D(x, y, z));
                num++;
            }

            var tetra = new Tetra_c(0, nodes, 1);

            Assert.AreEqual(tetra.CalcVolume(), vol, vol / 100);
        }

        [DataTestMethod]
        [DataRow(
            new float[] 
            { 
                0, 0, 0, 
                1, 0, 0, 
                1, 1,0,
                0,0,1,
                1.2f,0,1,
                1,1,0.7f
            }, 0.495f)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                5, 0, 0,
                5, 1,0,
                0,0,1,
                1.2f,0,1,
                1,1,0.7f
            }, 1.395f)]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                5, 0, 0,
                1, 5,0,
                0,0,1,
                1.2f,0,1,
                1,1,0.7f
            }, 4.5083f)]
        public void PentaVolume(float[] coords, float vol)
        {
            var nodes = new Node[6];
            var num = 0;
            for (var i = 0; i < coords.Length; i+=3)
            {
                var x = coords[i + 0];
                var y = coords[i + 1];
                var z = coords[i + 2];
                nodes[num] = new Node(num, new Point3D(x, y, z));
                num++;
            }
               
            var penta = new Penta(0, nodes,2);

            Assert.AreEqual(penta.CalcVolume(), vol, 0.25f);
        }


        [TestMethod]
        public void CalcHexaNormals()
        {
            //assing
            var nodes = new Node[8]
{
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
                new Node(4,new Point3D(0,10,10)),
                new Node(5,new Point3D(10,10,10)),
                new Node(6,new Point3D(10,0,10)),
                new Node(7,new Point3D(10,10,0)),
};

            var elem = new Hexa(0, nodes);

            //act
            var normals = elem.GetSurfaces().Select(x => x.CalcNormal()).ToList();

            //assert
            var ptr = 0;
            foreach (var surface in elem.GetSurfaces())
            {
                var angle = Vector.GetCosAngleVectors(surface.CalcNormal(), normals[ptr]);
                Assert.AreEqual(1, Vector.GetCosAngleVectors(surface.CalcNormal(), normals[ptr]), 0.1f);
                ptr++;
            }
        }

        [TestMethod]
        public void GetSurfaceElementPoints()
        {
            //assing
            var nodes = new Node[4]
{
                new Node(0,new Point3D(0,0,0)),
                new Node(1,new Point3D(10,0,0)),
                new Node(2,new Point3D(0,10,0)),
                new Node(3,new Point3D(0,0,10)),
};

            var elem = new Tetra(0, nodes);

            //act
            var points = elem.GetCoordinates().ToList();

            //assert

            Assert.AreEqual(points.Count(), 4);
        }


        [DataTestMethod]
        [DataRow(0, new int[] { 1, 2, 3, 4, 5, 6, 7, 8 })]
        [DataRow(1, new int[] { 5, 6, 2, 1, 8, 7, 3, 4 })]
        [DataRow(2, new int[] { 1, 4, 8, 5, 2, 3, 7, 6 })]
        [DataRow(3, new int[] { 6, 7, 3, 2, 5, 8, 4, 1 })]
        public void SortNodes_ShouldSortNodesCorrectly_OnHexa(int mainSurfaceIndex, int[] expectedNodesNumbers)
        {
            var nodes = new Node[]
            {
                new Node(1, new Point3D(0, 0, 0)),
                new Node(2, new Point3D(0, 1, 0)),
                new Node(3, new Point3D(1, 1, 0)),
                new Node(4, new Point3D(1, 0, 0)),
                new Node(5, new Point3D(0, 0, 1)),
                new Node(6, new Point3D(0, 1, 1)),
                new Node(7, new Point3D(1, 1, 1)),
                new Node(8, new Point3D(1, 0, 1)),
            };

            var surfaces = SurfacesCreator.CreateSurfacesFromSetOfNodes(0, nodes);

            var sortedNodes = Element3D.SortNodes(mainSurfaceIndex, surfaces);

            for (var i = 0; i < sortedNodes.Count(); i++)
            {
                Assert.AreEqual(expectedNodesNumbers[i], sortedNodes[i].Number);
            }
        }

        [DataTestMethod]
        [DataRow(0, new int[] { 1, 2, 3, 4, 5, 6 })]
        [DataRow(4, new int[] { 6,5,4,3,2,1})]
        public void SortNodes_ShouldSortNodesCorrectly_OnPenta(int mainSurfaceIndex, int[] expectedNodesNumbers)
        {
            var nodes = new Node[]
{
                new Node(1, new Point3D(0, 0, 0)),
                new Node(2, new Point3D(0, 1, 0)),
                new Node(3, new Point3D(1, 0, 0)),
                new Node(4, new Point3D(0, 0, 1)),
                new Node(5, new Point3D(0, 1, 1)),
                new Node(6, new Point3D(1, 0, 1)),
};

            var surfaces = SurfacesCreator.CreateSurfacesFromSetOfNodes(0, nodes);

            var sortedNodes = Element3D.SortNodes(mainSurfaceIndex, surfaces);

            for (var i = 0; i < sortedNodes.Count(); i++)
            {
                Assert.AreEqual(expectedNodesNumbers[i], sortedNodes[i].Number);
            }
        }
    }
}
