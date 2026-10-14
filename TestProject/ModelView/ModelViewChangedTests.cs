using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
using OperationalController;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TestProject_test.ModelView
{
    [TestClass]
    public class ModelViewChangedTests
    {
        [TestMethod]
        public void Select_GivesSelectionForTypeSets_AndNoneForOtherTypeSets()
        {
            var model = CreateModel(out var nodes, out var setA, out var setB);
            var view = new global::OperationalController.ModelView(model);
            var args = RaiseOnce(view, () => view.Select(ObjType.Узел, new[] { nodes[0].Number }));

            Assert.AreEqual(ModelViewChange.Selection, args.GetChanges(model.ObjectData.NodesSet));
            Assert.AreEqual(ModelViewChange.None, args.GetChanges(setA));
            Assert.AreEqual(ModelViewChange.None, args.GetChanges(setB));
        }

        [TestMethod]
        public void Select_SameNumbersAgain_DoesNotRaiseEvent()
        {
            var model = CreateModel(out var nodes, out _, out _);
            var view = new global::OperationalController.ModelView(model);
            view.Select(ObjType.Узел, new[] { nodes[0].Number });

            var raised = false;
            view.Changed += (sender, e) => raised = true;
            view.Select(ObjType.Узел, new[] { nodes[0].Number });

            Assert.IsFalse(raised);
        }

        [TestMethod]
        public void BatchedUpdate_SelectNodesAndHideElements_RaisesOneEvent_WithSeparateChangesPerSet()
        {
            var model = CreateModel(out var nodes, out var setA, out _);
            var view = new global::OperationalController.ModelView(model);
            var raisedCount = 0;
            view.Changed += (sender, e) => raisedCount++;

            ModelViewChangedEventArgs? args = null;
            view.Changed += (sender, e) => args = e;

            using (view.BeginUpdate())
            {
                view.Select(ObjType.Узел, new[] { nodes[0].Number });
                view.SetVisible(ObjType.Элемент2D, setA.GetNumbers(), false);
            }

            Assert.AreEqual(1, raisedCount);
            Assert.IsNotNull(args);
            Assert.AreEqual(ModelViewChange.Selection, args!.GetChanges(model.ObjectData.NodesSet));
            Assert.AreEqual(ModelViewChange.Visibility, args.GetChanges(setA));
        }

        [TestMethod]
        public void SetSetColor_GivesSetColorOnlyForThatSet_AndNoneForOtherSetsOfSameType()
        {
            var model = CreateModel(out _, out var setA, out var setB);
            var view = new global::OperationalController.ModelView(model);
            var args = RaiseOnce(view, () => view.SetColor(setA, Color.Aquamarine));

            Assert.AreEqual(ModelViewChange.SetColor, args.GetChanges(setA));
            Assert.AreEqual(ModelViewChange.None, args.GetChanges(setB));
        }

        [TestMethod]
        public void SetObjectColor_GivesObjectColorOnlyForThatSet()
        {
            var model = CreateModel(out var nodes, out var setA, out var setB);
            var view = new global::OperationalController.ModelView(model);
            var args = RaiseOnce(view, () => view.SetColor(ObjType.Узел, new[] { nodes[0].Number }, Color.Aquamarine));

            Assert.AreEqual(ModelViewChange.ObjectColor, args.GetChanges(model.ObjectData.NodesSet));
            Assert.AreEqual(ModelViewChange.None, args.GetChanges(setA));
            Assert.AreEqual(ModelViewChange.None, args.GetChanges(setB));
        }

        /// <summary>
        /// Проверяет обновление прежнего и нового наборов при замене выделения объектов одного типа.
        /// </summary>
        [TestMethod]
        public void SetSelection_GivesSelectionForPreviousAndNewSetsOfSameType()
        {
            var model = CreateModel(out _, out var setA, out var setB);
            var view = new global::OperationalController.ModelView(model);
            view.SetSelection(ObjType.Элемент2D, setA.GetNumbers());

            var args = RaiseOnce(view, () => view.SetSelection(ObjType.Элемент2D, setB.GetNumbers()));

            Assert.AreEqual(ModelViewChange.Selection, args.GetChanges(setA));
            Assert.AreEqual(ModelViewChange.Selection, args.GetChanges(setB));
        }

        [TestMethod]
        public void Transparency_GivesTransparencyForAllSets()
        {
            var model = CreateModel(out _, out var setA, out var setB);
            var view = new global::OperationalController.ModelView(model);
            var args = RaiseOnce(view, () => view.Transparency = 128);

            Assert.AreEqual(ModelViewChange.Transparency, args.GetChanges(model.ObjectData.NodesSet));
            Assert.AreEqual(ModelViewChange.Transparency, args.GetChanges(setA));
            Assert.AreEqual(ModelViewChange.Transparency, args.GetChanges(setB));
        }

        [TestMethod]
        public void HideSelected_GivesVisibilityAndSelection()
        {
            var model = CreateModel(out var nodes, out _, out _);
            var view = new global::OperationalController.ModelView(model);
            view.Select(ObjType.Узел, new[] { nodes[0].Number });

            var args = RaiseOnce(view, () => view.HideSelected());

            Assert.AreEqual(ModelViewChange.Visibility | ModelViewChange.Selection, args.GetChanges(model.ObjectData.NodesSet));
        }

        [TestMethod]
        public void NestedBeginUpdate_RaisesOneEventOnOuterExit()
        {
            var model = CreateModel(out var nodes, out _, out _);
            var view = new global::OperationalController.ModelView(model);
            var raisedCount = 0;
            view.Changed += (sender, e) => raisedCount++;

            using (view.BeginUpdate())
            {
                using (view.BeginUpdate())
                    view.Select(ObjType.Узел, new[] { nodes[0].Number });

                Assert.AreEqual(0, raisedCount);
                view.Select(ObjType.Узел, new[] { nodes[1].Number });
            }

            Assert.AreEqual(1, raisedCount);
        }

        [TestMethod]
        public void HandlerChangingViewInsideChanged_GetsNextEventSeparately()
        {
            var model = CreateModel(out var nodes, out _, out _);
            var view = new global::OperationalController.ModelView(model);
            var raisedCount = 0;
            view.Changed += (sender, e) =>
            {
                raisedCount++;
                if (raisedCount == 1)
                    view.Select(ObjType.Узел, new[] { nodes[1].Number });
            };

            view.Select(ObjType.Узел, new[] { nodes[0].Number });

            Assert.AreEqual(2, raisedCount);
        }

        /// <summary>
        /// Подписывает временный обработчик, выполняет действие и возвращает единственные полученные аргументы.
        /// </summary>
        private ModelViewChangedEventArgs RaiseOnce(IModelView view, System.Action action)
        {
            ModelViewChangedEventArgs? args = null;
            var raisedCount = 0;
            void Handler(object? sender, ModelViewChangedEventArgs e)
            {
                raisedCount++;
                args = e;
            }

            view.Changed += Handler;
            action();
            view.Changed -= Handler;

            Assert.AreEqual(1, raisedCount);
            Assert.IsNotNull(args);
            return args!;
        }

        /// <summary>
        /// Создаёт модель с узлами и двумя наборами элементов одного типа.
        /// </summary>
        private ModelData CreateModel(out List<Node> nodes, out ISetInfo setA, out ISetInfo setB)
        {
            var a = new Node(1, new Point3D(0, 0, 0));
            var b = new Node(2, new Point3D(1, 0, 0));
            var c = new Node(3, new Point3D(0, 1, 0));
            var d = new Node(4, new Point3D(1, 1, 0));

            var model = new ModelData();
            nodes = new List<Node> { a, b, c, d };
            foreach (var node in nodes)
                model.ObjectData.NodesSet.Add(node.Number, node);

            var trianglesA = new List<Element2D> { new Triangle(100, new[] { a, b, c }) };
            var trianglesB = new List<Element2D> { new Triangle(200, new[] { b, c, d }) };
            model.ObjectData.E2DCollection.AddRange("SetA", trianglesA);
            model.ObjectData.E2DCollection.AddRange("SetB", trianglesB);

            setA = model.ObjectData.GetSetInfo(ObjType.Элемент2D, "SetA");
            setB = model.ObjectData.GetSetInfo(ObjType.Элемент2D, "SetB");
            return model;
        }
    }
}
