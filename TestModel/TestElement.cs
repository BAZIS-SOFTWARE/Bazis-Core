using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using ModelController;
using ModelController.MeshObjsCreator;

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

            var elem = new Element3D(0, nodes, ObjKind.Tetra);

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

            var elem = new Element3D(0, nodes, ObjKind.Tetra);

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
            var el0 = new Element3D(55, new Node[] { nodes[0], nodes[1], nodes[2], nodes[3] }, ObjKind.Tetra);

            var el1 = new Element3D(56, new Node[] { nodes[1], nodes[2], nodes[3], nodes[4] }, ObjKind.Tetra);


            var model = new ModelData();
            model.ObjectData.Add(el0);
            model.ObjectData.Add(el1);


            var creators = new Extract2DFrom3D(model.ObjectData.FindMany<Element3D>().ToArray());

            var startNumber = model.ObjectData.GetLastObjNumber() + 1;
            var element2D = creators.Create(startNumber);

            model.ObjectData.AddRange(element2D);    

            //var findSurfaces = model.FindOpenSurfaces(model.FindObjs<Element3D>().ToArray());


            //assert

            //var compareSurfaces = new List<Surface>()
            //{
            //    new Surface(0,new Node[]{ nodes[0], nodes[1], nodes[2] },ObjKind.ElementSurface),
            //    new Surface(1,new Node[]{ nodes[0], nodes[1], nodes[3] },ObjKind.ElementSurface),
            //    new Surface(2,new Node[]{ nodes[0], nodes[3], nodes[2] },ObjKind.ElementSurface),
            //    new Surface(3,new Node[]{ nodes[1], nodes[4], nodes[3] },ObjKind.ElementSurface),
            //    new Surface(4,new Node[]{ nodes[1], nodes[4], nodes[2] },ObjKind.ElementSurface),
            //    new Surface(5,new Node[]{ nodes[2], nodes[3], nodes[4] },ObjKind.ElementSurface),
            //};

            //foreach (var surface in findSurfaces)
            //{
            //    if (!compareSurfaces.Contains(surface, new ElementSurfaceEqualityComparer()))
            //        throw new Exception();
            //}

        }

        [TestMethod]
        public void FindInsideSurfacesTwoElements()
        {
            var nodes = new List<Node>()
            {
                new Node(1,new Point3D(0, 0, 0)),
                new Node(2,new Point3D(1.5f, 0, 0)),
                new Node(4,new Point3D(0, 1.5f, 0)),
                new Node(5,new Point3D(0, 0, 1.5f)),
                new Node(6,new Point3D(-1.5f, 0, 0)),
                new Node(7,new Point3D(0, -1.5f, 0)),
                new Node(8,new Point3D(0, 0, -1.5f))
            };

            //act
            var el0 = new Element3D(
                0, nodes.Where(x => x.Number == 6 | x.Number == 5 | x.Number == 1 | x.Number == 4).ToArray(),
                ObjKind.Tetra);
            var el1 = new Element3D(
                1, nodes.Where(x => x.Number == 1 | x.Number == 4 | x.Number == 5 | x.Number == 2).ToArray(),
                 ObjKind.Tetra);
            var el2 = new Element3D(
                2, nodes.Where(x => x.Number == 2 | x.Number == 1 | x.Number == 4 | x.Number == 8).ToArray(),
                ObjKind.Tetra);
            var el3 = new Element3D(
                3, nodes.Where(x => x.Number == 1 | x.Number == 7 | x.Number == 2 | x.Number == 5).ToArray(),
                ObjKind.Tetra);
            var el4 = new Element3D(
                4, nodes.Where(x => x.Number == 7 | x.Number == 1 | x.Number == 6 | x.Number == 5).ToArray(),
                ObjKind.Tetra);
            var el5 = new Element3D(
                5, nodes.Where(x => x.Number == 8 | x.Number == 1 | x.Number == 7 | x.Number == 2).ToArray(),
                 ObjKind.Tetra);
            var el6 = new Element3D(
                6, nodes.Where(x => x.Number == 7 | x.Number == 6 | x.Number == 1 | x.Number == 8).ToArray(),
                ObjKind.Tetra);
            var el7 = new Element3D(
                7, nodes.Where(x => x.Number == 1 | x.Number == 8 | x.Number == 6 | x.Number == 4).ToArray(),
                ObjKind.Tetra);

            var els = new Element3D[]{ el0, el1, el2, el3, el4, el5, el6, el7 };
            var model = new ModelData();
            model.ObjectData.AddRange(els);

            var presenter = new ModelScenePresentator(model.ObjectData.ToArray());
            //presenter.GetSurfaceObjsCreator();
            
        }
    }
}
