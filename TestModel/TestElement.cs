using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelController.MeshObjsCreator;
using Model.MeshObjects;
using ModelInterfaces.MeshObjects;

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


        [TestMethod]
        public void FindOpenSurfacesTwoElements()
        {
            var nodes = new List<Node>()
            {
                new Node(0,new Point3D(1, 0, 0)),
                new Node(1,new Point3D(0, 1, 0)),
                new Node(2,new Point3D(0, 0, 1)),
                new Node(3,new Point3D(0, 0, 0)),
                new Node(54,new Point3D(-1, 0, 0))
            };

            //act
            var el0 = new Tetra(55, new Node[] { nodes[0], nodes[1], nodes[2], nodes[3] });

            var el1 = new Tetra(56, new Node[] { nodes[1], nodes[2], nodes[3], nodes[4] });


            var model = new ModelData();
            model.ObjectData.E3DCollection.Add(el0);
            model.ObjectData.E3DCollection.Add(el1);


            var creators = new Extract2DFrom3D();

            var lastNumber = model.ObjectData.E2DCollection.Count > 0 ? 
                model.ObjectData.E2DCollection.Last().Number : -1;

            var currentNumber = lastNumber + 1;

            var element2D = creators.Create(currentNumber, model.ObjectData.E3DCollection.ToArray());

            var inside = new ElementSurface(currentNumber,0, new Node[] { nodes[3], nodes[2], nodes[1] });
            foreach (var item in element2D)
            {
                foreach (var surface in item.GetSurfaces())
                {
                    if(surface[0].Number == inside[0].Number &&
                        surface[1].Number == inside[1].Number &&
                        surface[2].Number == inside[2].Number)

                        throw new Exception("Создана внутренняя поверхность!");
                }
            }
      

        }

        [TestMethod]
        public void Extract1DFrom2D_ShouldReturnCorrectElementsNumber()
        {
            var A = new Node(1, new Point3D(2, 2, 0));
            var B = new Node(2, new Point3D(4, 5, 0));
            var C = new Node(3, new Point3D(2, 8, 0));
            var D = new Node(4, new Point3D(9, 9, 0));
            var E = new Node(5, new Point3D(13, 9, 0));
            var F = new Node(6, new Point3D(9, 3, 0));
            var G = new Node(7, new Point3D(17, 3, 0));
            var elements = new IElement2D[]
            {
                new Triangle(1, new[] {A, B, C }),
                new Triangle(2, new[] {A, F, B }),
                new Triangle(3, new[] {D, B, F }),
                new Triangle(4, new[] {F, E, D }),
                new Triangle(5, new[] {F, G, E }),
            };
            var instance = new Extract1DFrom2D();

            var contour = instance.Create(1, elements);

            Assert.AreEqual(7, contour.Count());
        }


        [TestMethod]
        public void SortNodes_ShouldSortNodesCorrectly_OnHexa()
        {
            var A = new Node(1, new Point3D(0, 0, 0));
            var B = new Node(2, new Point3D(0, 1, 0));
            var C = new Node(3, new Point3D(1, 1, 0));
            var D = new Node(4, new Point3D(1, 0, 0));
            var E = new Node(5, new Point3D(0, 0, 1));
            var F = new Node(6, new Point3D(0, 1, 1));
            var G = new Node(7, new Point3D(1, 1, 1));
            var H = new Node(8, new Point3D(1, 0, 1));

            var hexa = new Hexa(1, new INode[] { A, B, C, D, E, F, G, H });

            var actual = hexa.GetVertexes().ToList();
            var expected = new List<INode> { F, B, A, E, G, C, D, H };
            for(var i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void SortNodes_ShouldSortNodesCorrectly_OnPenta()
        {
            var A = new Node(1, new Point3D(0, 0, 0));
            var B = new Node(2, new Point3D(0, 1, 0));
            var D = new Node(4, new Point3D(1, 0, 0));
            var E = new Node(5, new Point3D(0, 0, 1));
            var F = new Node(6, new Point3D(0, 1, 1));
            var H = new Node(8, new Point3D(1, 0, 1));

            var penta = new Penta(1, new INode[] { A, B, D, E, F, H });

            var actual = penta.GetVertexes().ToList();
            var expected = new List<INode> { A, B, D, E, F, H };
            for (var i = 0; i < actual.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
