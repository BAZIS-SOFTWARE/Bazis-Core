using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.MeshObjects;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Model.Interfaces;
using Model.IO;
using System.IO;
using OperationalController.MeshObjsUtility;
using OperationalController.MeshObjsCreator;
using Model.IO.STL;

namespace TestModel.ControllerTests
{
    [TestClass]
    public class MeshUtility
    {
        [DataTestMethod]
        [DataRow(@"..\..\..\Examples\CDB\hex.cdb", 0.001f, 0)]
        [DataRow(@"c:\projs\Matushkin\T_project.bpf",0.001f,2)]
        [DataRow(@"..\..\..\Examples\CDB\hex.cdb", 1.1f, 4)]
        public void FindCoincidentNodes(string path, float dist, int count)
        {
            var ext = Path.GetExtension(path);
            IModelLoader loader;
            if(ext == ".cdb")
                loader = new LoadModelFromCDBTextFile();
            else
                loader = new LoadModelFromBPFTextFile();

            var model = loader.Load(path);
            var finder = new FindCoincidentObjects();
            finder.ProgressEvent += (ar1, ar2) => { };
            var coincidentNodes = finder.Find(model.ObjectData.GetObjects(ObjType.Узел).ToList(), dist);
            var numberOfObjs = coincidentNodes.Count();
            Assert.AreEqual(count, numberOfObjs);
        }

        [DataTestMethod]
        [DataRow(@"c:\projs\Matushkin\T_project.bpf", 0.001f,2)]
        [DataRow(@"..\..\..\Examples\3D\cub_unsorted_freeNodesCoincident.bpf", 0.01f,4)]
        [DataRow(@"C:\projs\AdditiveTest\1n.stl", 0.01f, 3964)]
        public void MergeNodes(string path, float dist, int count)
        {
            //assing
            var controller = new OperationalController.ProjectController();

            //act

            var ext = Path.GetExtension(path);

            if(ext == ".stl")
                controller.ImportMesh(path);
            else
                controller.Load(path);

            var coincidentNodes = controller.FindCoincidentObjects(ObjType.Узел, dist);

            controller.MergeNodes(coincidentNodes);

            //assert

            Assert.AreEqual(coincidentNodes.Count, count);

            // Очень странная проверка ее нужно ускорить
            
            foreach (var elem in controller.GetAllModelElements())
            {
                foreach (var item in elem.GetVertexes())
                {
                    if (!controller.ContainsModelObject(ObjType.Узел, item.Number))
                        throw new Exception($"Узел {item.Number} в элементе {elem.Number} не удален");
                }
            }

            //freeNodes.ToArray()[0] = 14;
            //freeNodes.ToArray()[1] = 15;
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
        public void Extract2DFrom3D()
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
            model.ObjectData.E3DCollection.Add(ObjType.Элемент3D.ToString(), el0);
            model.ObjectData.E3DCollection.Add(ObjType.Элемент3D.ToString(), el1);


            var creators = new Extract2DFrom3D();

            var maxNumber = model.ObjectData.E2DCollection.Count > 0 ?
                model.ObjectData.E2DCollection.GetMaxNumber() : 0;

            var currentNumber = maxNumber + 1;

            var element2D = creators.Create(currentNumber, model.ObjectData.E3DCollection.GetObjects());

            var inside = new ElementSurface(currentNumber, 0, new Node[] { nodes[3], nodes[2], nodes[1] });
            foreach (var item in element2D)
            {
                foreach (var surface in item.GetSurfaces())
                {
                    if (surface[0].Number == inside[0].Number &&
                        surface[1].Number == inside[1].Number &&
                        surface[2].Number == inside[2].Number)

                        throw new Exception("Создана внутренняя поверхность!");
                }
            }


        }
    }
}
