using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using ModelInterfaces;
using ModelController.MeshObjsCreator;
using ModelController.ModelScenePresentator;
using Model.Elements;

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
            var normals = elem.GetSurface().Select(x => x.CalcNormal()).ToList();

            //assert
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
            var points = elem.GetPoints().ToList();

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
            model.ObjectData.Add(el0);
            model.ObjectData.Add(el1);


            var creators = new Extract2DFrom3D();

            var startNumber = model.ObjectData.GetLastObjNumber("Элементы2D") + 1;

            var element2D = creators.Create(startNumber, model.ObjectData.FindMany<Element3D>().ToArray());

            var inside = new Surface(0, new Node[] { nodes[3], nodes[2], nodes[1] },ObjKind.ElementSurface);
            foreach (var item in element2D)
            {
                foreach (var surface in item.GetSurface())
                {
                    if (surface.Equals(inside))
                        throw new Exception("Создана внутренняя поверхность!");
                }
            }
      

        }
       
    }
}
