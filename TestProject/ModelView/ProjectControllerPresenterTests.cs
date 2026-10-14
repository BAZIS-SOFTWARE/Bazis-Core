using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Model.IO;
using System.Collections.Generic;
using Model.MeshObjects;
using Model.GeometryObjects;
using OperationalController;
using System.IO;
using Geometry;
using System.Linq;
using Model.IO.STL;
using OperationalController.ModelScenePresentator.GlObjsPresenters;
using OperationalController.ModelScenePresentator;
using OperationalController.MeshObjsUtility;
using OperationalController.GmshController;
using System;
using System.Drawing;

namespace TestProject_test.ModelView
{
    [TestClass]
    public class ProjectControllerPresenterTests
    {
        [TestMethod]
        public void TestSeparators()
        {
            var controller = new ProjectController();
            controller.Load(@"../../../../TestModelCore/Examples/3D/mixElements.bpf");

            var setInfo = controller.GetModelSetsInfo(ObjType.Элемент2D).First();
            var presenter = controller.CreateModelObjectsPresentor(setInfo);
            var surfPresenter = (ISurfaceObjsPresenter)presenter;
            var inds = surfPresenter.CreateIndexes();
            var coords = surfPresenter.CreateVertexes(inds.Item2, "координаты");
            var separators = surfPresenter.CreateSeparators();

            Assert.IsTrue(surfPresenter.Count() == separators.Length - 1);
        }

        [DataTestMethod]
        [DataRow(@"../../../../TestModelCore/Examples/3D/Lazer.inp", 10, ObjType.Элемент3D)]
        [DataRow(@"../../../../TestModelCore/Examples/3D/untitled.inp", 47, ObjType.Элемент3D)]
        [DataRow(@"../../../../TestModelCore/Examples/3D/Lazer.inp", 35, ObjType.Элемент2D)]
        [DataRow(@"../../../../TestModelCore/Examples/3D/untitled.inp", 6, ObjType.Элемент2D)]
        public void TestSeparatorsAfterHideElementsByColor(string fileName, int hideElemsBySet, ObjType objType)
        {
            var controller = new ProjectController();
            controller.ImportMesh(fileName);

            var sets = controller.GetModelSetsInfo(objType);
            var numbersToHide = sets.SelectMany(set => set.GetNumbers()).Take(hideElemsBySet).ToArray();
            controller.SetVisible(objType, numbersToHide, false);

            var expectedShown = sets.Sum(set => set.NumberOfObjects) - numbersToHide.Length;
            var actualShown = 0;
            foreach (var set in controller.GetModelSetsInfo(objType))
            {
                foreach (var item in set.GetNumbers())
                {
                    if (controller.GetVisible(objType, item))
                        actualShown++;
                }

            }

            Assert.AreEqual(expectedShown, actualShown);

            var setInfo = sets.First();
            expectedShown = setInfo.GetNumbers().Count(number => controller.GetVisible(objType, number));
            var surfPresenter = (ISurfaceObjsPresenter)controller.CreateModelObjectsPresentor(setInfo);
            
            var inds = surfPresenter.CreateIndexes();
            var coords = surfPresenter.CreateVertexes(inds.Item2, "координаты");
            var separators = surfPresenter.CreateSeparators();

            actualShown = 0;
            var maxSeparator = separators[0];//Для проверки "дружелюбности" сепараторов с индексами
            for (var i = 1; i < separators.Length; ++i)
            {
                maxSeparator = Math.Max(maxSeparator, separators[i]);
                var viewState = Convert.ToInt32(separators[i] - separators[i - 1] > 0);
                actualShown += viewState;
            }

            Assert.AreEqual(expectedShown, actualShown);
            Assert.AreEqual(inds.Item1, maxSeparator * 3);
        }

        [DataTestMethod]
        [DataRow(@"../../../../TestModelCore/Examples/3D/Lazer.inp", 155)]
        public void ChangeElement1DSetColorTest(string fileName, int alpha)
        {
            var alphaExp = alpha / 255f;

            var controller = new ProjectController();
            controller.ImportMesh(fileName);

            var sets = controller.GetModelSetsInfo(ObjType.Элемент1D);

            foreach (var set in sets)
            {
                var color = controller.GetColor(set);
                var setColor = Color.FromArgb(alpha, color);
                controller.SetColor(set, setColor);
                var actualColor = controller.GetColor(set);
                Assert.AreEqual(alpha, actualColor.A);
            }

            var setInfo = sets.First();
            var pres = controller.CreateModelObjectsPresentor(setInfo);

            var indices = pres.CreateIndexes();
            var colors = pres.CreateVertexes(indices.Item3, "цвет");

            //Сделал исправление в ColorLineObjsCreator.cs стр 14, т.к набор не подхватывал альфа-канал
            for (var i = 0; i < colors.Length; i+=4)
                Assert.AreEqual(alphaExp, colors[i + 3], 1e-4);
        }
    }
}
