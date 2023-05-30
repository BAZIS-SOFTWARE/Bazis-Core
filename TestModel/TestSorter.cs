using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Events;
using ModelController.IO;
using System;
using System.Collections.Generic;
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
            var modelBPF = loader.Load(@"..\..\Examples\3D\cub_unsorted.bpf");
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
        public void SortNodeByClockwise_XPositiveYPositive()
        {
            //assing

            var nodes = new Node[]
            {
                new Node(0, new Point3D(0,0,0)),
                new Node(1, new Point3D(1.5f,1,0)),
                new Node(2, new Point3D(5,0,0)),
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

        [DataTestMethod]
        [DataRow(new float[] 
        { 
            258,
            -0.025f,-0.04588125f,0.1662952f,
            233,
            -0.036f,-0.03793374f,0.1649841f,
            229,
            -0.036f,-0.04588125f,0.1662952f,
            257,
            -0.025f,-0.03793373f,0.1649841f,
        })]
        [DataRow(new float[]
        {
            147,
            -0.058f,-0.05212305f,0.1608275f,
            18,
            -0.047f,-0.04709208f,0.1510405f,
            146,
            -0.047f,-0.05212305f,0.1608275f,
            19,
            -0.058f,-0.04709209f,0.1510406f
        })]
        
        public void OtherSortNode(float[] ar)
        {
            //assing
            var nodes = new List<Node>();
            for (int i = 0; i < ar.Length; i+=4)
            {
                nodes.Add(new Node((int)ar[i], new Point3D(ar[i + 1], ar[i + 2], ar[i + 3])));
            }


            //act
            var surf = new Surface(0, nodes.ToArray(), Model.Interfaces.ObjKind.ElementSurface);

            //assert
            Assert.AreEqual(surf.NumberOfPoints, nodes.Count);
        }
    }
}
