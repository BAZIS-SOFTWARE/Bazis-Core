using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.MeshObjsCreator;
using Model.MeshObjects;
using ModelInterfaces.MeshObjects;
using Model.Utilities;

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
            var nodes = new INode[]
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
            var nodes = new INode[]
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
