using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Events;
using ModelController.IO;
using System;
using System.Drawing;

namespace TestModel
{
    [TestClass]
    public class TestSorter
    {
        [TestMethod]
        public void SortObjectsByNumber()
        {
            //assing
            var model = new ModelData();
            var loader = new LoadModelFromProjectTextFile();
            loader.LoadEvent += (ar1, ar2) => { };

            //act
            var modelBPF = loader.Load(@"C:\Projs\TestProjTerm\Cub\cub_unsorted.bpf");
            model.Load(modelBPF);

            //assert
            var number = 0;
            foreach (var modelObject in model.ObjectData)
            {
                Assert.AreEqual(number, modelObject.Number);
                number++;
            }
        }

        [TestMethod]
        public void SortNodeByAnticlockwise_XPositiveYPositive()
        {
            //assing

            var nodes = new Node[]
            {
                new Node(0, new Point3D(0,0,0)),
                new Node(1, new Point3D(5,0,0)),
                new Node(2, new Point3D(1.5f,1,0)),
                new Node(3, new Point3D(6,1,0))
            };

            //act
            var surf = new Surface(0, nodes, Model.Interfaces.ObjKind.ElementSurface);

            //assert
            if (!surf[0].Equals(nodes[0]))
                throw new Exception();
            if (!surf[1].Equals(nodes[1]))
                throw new Exception();
            if (!surf[2].Equals(nodes[3]))
                throw new Exception();
            if (!surf[3].Equals(nodes[2]))
                throw new Exception();
        }

        [TestMethod]
        public void SortNodeByAnticlockwise_XDifferentYDifferent()
        {
            //assing

            var nodes = new Node[]
            {
                new Node(0, new Point3D(1,-1,0)),
                new Node(1, new Point3D(0,1,0)),
                new Node(2, new Point3D(3,1,0)),
                new Node(3, new Point3D(-3,-1,0))
            };

            //act
            var surf = new Surface(0, nodes, Model.Interfaces.ObjKind.ElementSurface);

            //assert
            //assert
            if (!surf[0].Equals(nodes[0]))
                throw new Exception();
            if (!surf[1].Equals(nodes[2]))
                throw new Exception();
            if (!surf[2].Equals(nodes[1]))
                throw new Exception();
            if (!surf[3].Equals(nodes[3]))
                throw new Exception();
        }
    }
}
