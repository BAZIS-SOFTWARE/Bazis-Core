using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.MeshObjects;
using Model.ObjectsCollections;
using OperationalController;
using OperationalController.ModelScenePresentator.GlObjsPresenters;
using System.Drawing;
using System.Linq;

namespace TestProject_test.OperationalController
{
    /// <summary>
    /// Проверяет хранение состояния представления отдельно от объектов модели.
    /// </summary>
    [TestClass]
    public class ModelViewTests
    {
        /// <summary>
        /// Проверяет операции над выделением одного типа объектов.
        /// </summary>
        [DataTestMethod]
        [DataRow("Select", new[] { 1, 2 }, new[] { 1, 2 })]
        [DataRow("Deselect", new[] { 1 }, new[] { 2 })]
        [DataRow("Toggle", new[] { 2, 3 }, new[] { 1, 3 })]
        public void SelectionCommands_ChangeOnlyRequestedNumbers(string command, int[] numbers, int[] expected)
        {
            var view = new global::OperationalController.ModelView(new ModelData());
            view.Select(ObjType.Узел, new[] { 1, 2 });

            if (command == "Select")
                view.ClearSelection();
            else if (command == "Deselect")
                view.Deselect(ObjType.Узел, numbers);
            else
                view.ToggleSelection(ObjType.Узел, numbers);

            if (command == "Select")
                view.Select(ObjType.Узел, numbers);

            CollectionAssert.AreEquivalent(expected, view.GetSelected(ObjType.Узел).ToArray());
        }

        /// <summary>
        /// Проверяет, что пакет изменений генерирует одно уведомление.
        /// </summary>
        [TestMethod]
        public void BeginUpdate_RaisesOneChangedEvent()
        {
            var view = new global::OperationalController.ModelView(new ModelData());
            var changesCount = 0;
            view.Changed += (sender, args) => changesCount++;

            using (view.BeginUpdate())
            {
                view.Select(ObjType.Узел, new[] { 1 });
                view.Select(ObjType.Элемент2D, new[] { 2 });
            }

            Assert.AreEqual(1, changesCount);
        }

        /// <summary>
        /// Проверяет уведомление при пакетном изменении настроек представления.
        /// </summary>
        [TestMethod]
        public void DisplaySettings_RaiseOneChangedEventInUpdate()
        {
            var view = new global::OperationalController.ModelView(new ModelData());
            var changesCount = 0;
            view.Changed += (sender, args) => changesCount++;

            using (view.BeginUpdate())
            {
                view.SelectionColor = Color.Green;
                view.Transparency = 128;
            }

            Assert.AreEqual(1, changesCount);
        }

        /// <summary>
        /// Проверяет скрытие выделения и очистку устаревших номеров.
        /// </summary>
        [TestMethod]
        public void HideSelectedAndPrune_KeepViewStateConsistentWithModel()
        {
            var model = new ModelData();
            model.ObjectData.NodesSet.Add(7, new Node(7, new Point3D()));
            var view = new global::OperationalController.ModelView(model);
            view.Select(ObjType.Узел, new[] { 7 });

            view.HideSelected();

            Assert.IsFalse(view.GetVisible(ObjType.Узел, 7));
            Assert.AreEqual(0, view.SelectedCount);

            model.ObjectData.Remove(ObjType.Узел, 7);
            view.Prune();

            Assert.IsTrue(view.GetVisible(ObjType.Узел, 7));
        }

        /// <summary>
        /// Общая очистка модели удаляет из представления состояние удалённых объектов.
        /// </summary>
        [TestMethod]
        public void ClearNotExistedModelData_PrunesModelView()
        {
            var controller = new ProjectController();
            controller.CreateProject("model-view-cleanup.bpf2");
            controller.CreateNode(7, 0, 0, 0);
            var node = controller.GetModelObject(ObjType.Узел, 7);
            controller.SetVisible(ObjType.Узел, new[] { node.Number }, false);

            node.ExistState = false;
            controller.ClearNotExistedModelData();

            Assert.IsNull(controller.GetModelObject(ObjType.Узел, node.Number));
            Assert.IsTrue(controller.GetVisible(ObjType.Узел, node.Number));
        }

        /// <summary>
        /// Проверяет приоритет цвета выделения и альфа-канал представления.
        /// </summary>
        [TestMethod]
        public void GetColor_UsesSelectionColorBeforeSetColor()
        {
            var model = new ModelData();
            model.ObjectData.NodesSet.Add(3, new Node(3, new Point3D()));
            var view = new global::OperationalController.ModelView(model)
            {
                SelectionColor = Color.Red,
                Transparency = 128
            };
            var setInfo = model.ObjectData.GetSetInfo(ObjType.Узел, 3);
            view.SetColor(setInfo, Color.Blue);

            view.Select(ObjType.Узел, new[] { 3 });

            var color = view.GetColor(ObjType.Узел, 3);

            Assert.AreEqual(Color.FromArgb(128, Color.Red), color);
        }

        /// <summary>
        /// Проверяет приоритет выделения над индивидуальной окраской и её сохранение после снятия выделения.
        /// </summary>
        [TestMethod]
        public void GetColor_UsesSelectionColorBeforeObjectColor()
        {
            var model = new ModelData();
            model.ObjectData.NodesSet.Add(8, new Node(8, new Point3D()));
            var setInfo = model.ObjectData.GetSetInfo(ObjType.Узел, 8);
            var view = new global::OperationalController.ModelView(model)
            {
                SelectionColor = Color.Red
            };
            view.SetColor(setInfo, Color.Blue);
            view.SetColor(ObjType.Узел, new[] { 8 }, Color.Green);

            Assert.AreEqual(Color.Green.ToArgb(), view.GetColor(ObjType.Узел, 8).ToArgb());

            view.Select(ObjType.Узел, new[] { 8 });

            Assert.AreEqual(Color.Red.ToArgb(), view.GetColor(ObjType.Узел, 8).ToArgb());

            view.Deselect(ObjType.Узел, new[] { 8 });

            Assert.AreEqual(Color.Green.ToArgb(), view.GetColor(ObjType.Узел, 8).ToArgb());

            view.ClearColor(ObjType.Узел, new[] { 8 });

            Assert.AreEqual(Color.Blue.ToArgb(), view.GetColor(ObjType.Узел, 8).ToArgb());
        }

        /// <summary>
        /// Проверяет сохранение стиля набора, выделения и видимости при очистке цветов.
        /// </summary>
        [DataTestMethod]
        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        public void ClearColor_KeepsSetStyleSelectionAndVisibility(bool selected, bool visible)
        {
            var model = new ModelData();
            model.ObjectData.NodesSet.Add(1, new Node(1, new Point3D()));
            model.ObjectData.NodesSet.Add(2, new Node(2, new Point3D()));
            var setInfo = model.ObjectData.NodesSet;
            var view = new global::OperationalController.ModelView(model);
            var numbers = new[] { 1, 2 };
            view.SetColor(setInfo, Color.Blue);
            view.SetViewMode(setInfo, ViewMode.Surface);
            view.SetColor(ObjType.Узел, numbers, Color.Green);
            view.SetVisible(ObjType.Узел, numbers, visible);
            if (selected)
                view.Select(ObjType.Узел, numbers);

            view.ClearColor();

            Assert.AreEqual(Color.Blue, view.GetColor(setInfo));
            Assert.AreEqual(ViewMode.Surface, view.GetViewMode(setInfo));
            foreach (var number in numbers)
            {
                Assert.AreEqual(selected, view.IsSelected(ObjType.Узел, number));
                Assert.AreEqual(visible, view.GetVisible(ObjType.Узел, number));
            }

            view.ClearSelection();
            foreach (var number in numbers)
                Assert.AreEqual(Color.Blue.ToArgb(), view.GetColor(ObjType.Узел, number).ToArgb());
        }

        /// <summary>
        /// Проверяет уведомления при очистке цветов и последующей покраске в пакете.
        /// </summary>
        [DataTestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void ClearColor_RaisesOneEventAndSupportsRecoloringInUpdate(bool recolor)
        {
            var model = new ModelData();
            var nodes = new[] { new Node(1, new Point3D()), new Node(2, new Point3D(1, 0, 0)), new Node(3, new Point3D(0, 1, 0)) };
            foreach (var node in nodes)
                model.ObjectData.NodesSet.Add(node.Number, node);
            var elements = new[] { new Triangle(1, nodes) };
            model.ObjectData.E2DCollection.AddRange("colored", elements);
            var view = new global::OperationalController.ModelView(model);
            var numbers = new[] { 1 };
            view.SetColor(ObjType.Узел, numbers, Color.Green);
            view.SetColor(ObjType.Элемент2D, numbers, Color.Green);
            var elementSet = model.ObjectData.GetSetsInfo(ObjType.Элемент2D).Single();
            var changesCount = 0;
            view.Changed += (sender, args) =>
            {
                changesCount++;
                Assert.AreEqual(2, args.Count);
                Assert.AreEqual(ModelViewChange.ObjectColor, args.GetChanges(model.ObjectData.NodesSet));
                Assert.AreEqual(ModelViewChange.ObjectColor, args.GetChanges(elementSet));
            };

            if (recolor)
            {
                using (view.BeginUpdate())
                {
                    view.ClearColor();
                    Assert.AreEqual(0, changesCount);
                    view.SetColor(ObjType.Узел, numbers, Color.Orange);
                    Assert.AreEqual(0, changesCount);
                }

                Assert.AreEqual(Color.Orange.ToArgb(), view.GetColor(ObjType.Узел, 1).ToArgb());
                Assert.AreEqual(1, changesCount);
            }
            else
            {
                view.ClearColor();
                Assert.AreEqual(1, changesCount);
                view.ClearColor();
                Assert.AreEqual(1, changesCount);
            }

            var expectedElementColor = view.GetColor(elementSet);
            Assert.AreEqual(expectedElementColor.ToArgb(), view.GetColor(ObjType.Элемент2D, 1).ToArgb());
        }

        /// <summary>
        /// Проверяет сохранение альфа-канала стиля при полной прозрачности представления.
        /// </summary>
        [TestMethod]
        public void GetColor_KeepsSetColorAlphaAtFullTransparency()
        {
            var model = new ModelData();
            model.ObjectData.NodesSet.Add(4, new Node(4, new Point3D()));
            var setInfo = model.ObjectData.GetSetInfo(ObjType.Узел, 4);
            var view = new global::OperationalController.ModelView(model);
            var expectedColor = Color.FromArgb(155, Color.SteelBlue);
            view.SetColor(setInfo, expectedColor);

            var color = view.GetColor(ObjType.Узел, 4);

            Assert.AreEqual(expectedColor, color);
        }

        /// <summary>
        /// Проверяет формирование цвета presenter без поиска набора для каждого объекта.
        /// </summary>
        [TestMethod]
        public void PointPresenter_UsesSetStyleAndSelectionColor()
        {
            var model = new ModelData();
            var node = new Node(12, new Point3D());
            model.ObjectData.NodesSet.Add(node.Number, node);
            var setInfo = model.ObjectData.GetSetInfo(ObjType.Узел, node.Number);
            var view = new global::OperationalController.ModelView(model)
            {
                SelectionColor = Color.Red,
                Transparency = 128
            };
            view.SetColor(setInfo, Color.Blue);
            var pointObjects = new IModelObject[] { node };
            var colorsByPoint = new[] { view.GetColor(setInfo, node.Number) };
            var presenter = new PointObjsPresenter(pointObjects, colorsByPoint);

            var colors = presenter.CreateVertexes(4, "цвет");

            Assert.AreEqual(0.0f, colors[0]);
            Assert.AreEqual(0.0f, colors[1]);
            Assert.AreEqual(1.0f, colors[2]);
            Assert.AreEqual(128.0f / 255.0f, colors[3]);

            view.Select(ObjType.Узел, new[] { node.Number });
            colorsByPoint = new[] { view.GetColor(setInfo, node.Number) };
            presenter = new PointObjsPresenter(pointObjects, colorsByPoint);
            colors = presenter.CreateVertexes(4, "цвет");

            Assert.AreEqual(1.0f, colors[0]);
            Assert.AreEqual(0.0f, colors[1]);
            Assert.AreEqual(0.0f, colors[2]);
            Assert.AreEqual(128.0f / 255.0f, colors[3]);
        }

        /// <summary>
        /// Проверяет сохранение стиля набора после его переименования.
        /// </summary>
        [TestMethod]
        public void SetStyle_IsKeptWhenSetIsRenamed()
        {
            var model = new ModelData();
            var collection = model.ObjectData.E2DCollection;
            collection.Add("initial");
            var setInfo = collection["initial"];
            var view = new global::OperationalController.ModelView(model);

            view.SetColor(setInfo, Color.MediumPurple);
            view.SetViewMode(setInfo, ViewMode.Surface);
            collection.ChangeName("initial", "renamed");
            var renamedSet = collection["renamed"];

            Assert.AreEqual(Color.MediumPurple, view.GetColor(renamedSet));
            Assert.AreEqual(ViewMode.Surface, view.GetViewMode(renamedSet));
        }

        /// <summary>
        /// Проверяет, что стандартные цвета определяются порядком наборов в модели,
        /// а не порядком первого запроса их состояния.
        /// </summary>
        [TestMethod]
        public void DefaultSetColors_DoNotDependOnReadOrder()
        {
            var model = new ModelData();
            var collection = model.ObjectData.E2DCollection;
            collection.Add("first");
            collection.Add("second");
            var first = collection["first"];
            var second = collection["second"];
            var view = new global::OperationalController.ModelView(model);

            view.SetViewMode(second, ViewMode.Surface);
            var secondColor = view.GetColor(second);
            var firstColor = view.GetColor(first);

            Assert.AreEqual(Color.Orange, secondColor);
            Assert.AreEqual(Color.SteelBlue, firstColor);
            Assert.AreEqual(ViewMode.LineSurface, view.GetViewMode(first));
        }

        /// <summary>
        /// Проверяет назначение стабильных номеров наборам одного типа.
        /// </summary>
        [TestMethod]
        public void SetNumbers_AreNotChangedAfterNeighborSetRemoval()
        {
            var model = new ModelData();
            var collection = model.ObjectData.E2DCollection;
            collection.Add("first");
            collection.Add("second");
            collection.Add("third");

            collection.Remove("second");
            collection.Add("fourth");

            Assert.AreEqual(0, collection["first"].Number);
            Assert.AreEqual(2, collection["third"].Number);
            Assert.AreEqual(3, collection["fourth"].Number);
        }

        /// <summary>
        /// Проверяет назначение номера при разных способах создания набора.
        /// </summary>
        [DataTestMethod]
        [DataRow("EmptySet")]
        [DataRow("PreparedSet")]
        [DataRow("SingleObject")]
        [DataRow("ObjectRange")]
        public void Add_AssignsSetNumber(string addMode)
        {
            var collection = new ObjectsCollection<Node>();
            if (addMode == "EmptySet")
                collection.Add("first");
            else if (addMode == "PreparedSet")
                collection.Add(new ObjectsSet<Node>("first"));
            else if (addMode == "SingleObject")
                collection.Add("first", new Node(1, new Point3D()));
            else
                collection.AddRange("first", new[] { new Node(1, new Point3D()) });

            collection.Add("second");

            Assert.AreEqual(0, collection["first"].Number);
            Assert.AreEqual(1, collection["second"].Number);
        }

        /// <summary>
        /// Проверяет сохранение стандартного цвета после удаления соседнего набора.
        /// </summary>
        [TestMethod]
        public void DefaultSetColor_IsNotChangedAfterNeighborSetRemoval()
        {
            var model = new ModelData();
            var collection = model.ObjectData.E2DCollection;
            collection.Add("first");
            collection.Add("second");
            var second = collection["second"];
            var view = new global::OperationalController.ModelView(model);

            collection.Remove("first");

            Assert.AreEqual(1, second.Number);
            Assert.AreEqual(Color.Orange, view.GetColor(second));
        }

        /// <summary>
        /// Проверяет начало нумерации наборов заново после полной очистки коллекции.
        /// </summary>
        [TestMethod]
        public void Clear_ResetsSetNumbering()
        {
            var model = new ModelData();
            var collection = model.ObjectData.E2DCollection;
            collection.Add("beforeClear");

            collection.Clear();
            collection.Add("afterClear");

            Assert.AreEqual(0, collection["afterClear"].Number);
        }

        /// <summary>
        /// Проверяет номера одиночных наборов модели.
        /// </summary>
        [DataTestMethod]
        [DataRow(ObjType.Точка)]
        [DataRow(ObjType.Кривая)]
        [DataRow(ObjType.Поверхность)]
        [DataRow(ObjType.Узел)]
        public void SingleSet_NumberIsZero(ObjType objType)
        {
            var model = new ModelData();
            var setInfo = model.ObjectData.GetSetsInfo(objType).Single();

            Assert.AreEqual(0, setInfo.Number);
        }

        /// <summary>
        /// Проверяет независимое хранение переопределений цвета и режима набора.
        /// </summary>
        [TestMethod]
        public void ObjectsView_SetStylePropertiesAreIndependent()
        {
            var model = new ModelData();
            var setInfo = model.ObjectData.NodesSet;
            var view = new ObjectsView();

            view.SetSetColor(setInfo, Color.Green);

            Assert.IsTrue(view.TryGetSetColor(setInfo, out var color));
            Assert.AreEqual(Color.Green, color);
            Assert.IsFalse(view.TryGetViewMode(setInfo, out _));

            view.SetViewMode(setInfo, ViewMode.Surface);

            Assert.IsTrue(view.TryGetViewMode(setInfo, out var viewMode));
            Assert.AreEqual(ViewMode.Surface, viewMode);
        }

        /// <summary>
        /// Проверяет, что сброс номеров не сбрасывает стиль набора.
        /// </summary>
        [TestMethod]
        public void Reset_ClearsObjectStateAndKeepsSetStyle()
        {
            var model = new ModelData();
            model.ObjectData.NodesSet.Add(5, new Node(5, new Point3D()));
            var setInfo = model.ObjectData.GetSetInfo(ObjType.Узел, 5);
            var view = new global::OperationalController.ModelView(model);
            view.SetColor(setInfo, Color.Green);
            view.Select(ObjType.Узел, new[] { 5 });
            view.SetVisible(ObjType.Узел, new[] { 5 }, false);
            view.SetColor(ObjType.Узел, new[] { 5 }, Color.Red);

            view.Reset();

            Assert.IsFalse(view.IsSelected(ObjType.Узел, 5));
            Assert.IsTrue(view.GetVisible(ObjType.Узел, 5));
            Assert.AreEqual(Color.Green, view.GetColor(setInfo));
            Assert.AreEqual(Color.Green.ToArgb(), view.GetColor(ObjType.Узел, 5).ToArgb());
        }

        /// <summary>
        /// Проверяет, что удалённый номер не передаёт индивидуальный цвет новому объекту.
        /// </summary>
        [TestMethod]
        public void Prune_RemovesObjectColorOfDeletedObject()
        {
            var model = new ModelData();
            model.ObjectData.NodesSet.Add(6, new Node(6, new Point3D()));
            var setInfo = model.ObjectData.GetSetInfo(ObjType.Узел, 6);
            var view = new global::OperationalController.ModelView(model);
            view.SetColor(setInfo, Color.Blue);
            view.SetColor(ObjType.Узел, new[] { 6 }, Color.Red);

            model.ObjectData.Remove(ObjType.Узел, 6);
            view.Prune();
            model.ObjectData.NodesSet.Add(6, new Node(6, new Point3D()));

            Assert.AreEqual(Color.Blue.ToArgb(), view.GetColor(ObjType.Узел, 6).ToArgb());
        }

        /// <summary>
        /// Проверяет очистку стиля удалённого набора.
        /// </summary>
        [TestMethod]
        public void Prune_RemovesStyleOfDeletedSet()
        {
            var model = new ModelData();
            var collection = model.ObjectData.E2DCollection;
            collection.Add("deleted");
            var setInfo = collection["deleted"];
            var view = new global::OperationalController.ModelView(model);
            view.SetColor(setInfo, Color.Fuchsia);
            collection.Remove("deleted");

            view.Prune();

            Assert.AreNotEqual(Color.Fuchsia, view.GetColor(setInfo));
        }

        /// <summary>
        /// Проверяет общемодельный признак отрисовки внутренних граней:
        /// по умолчанию они рисуются, смена значения уведомляет вид,
        /// а повторная запись того же значения - нет.
        /// </summary>
        [TestMethod]
        public void HideInsideSurfaces_RaisesChangedOnlyWhenValueDiffers()
        {
            var view = new global::OperationalController.ModelView(new ModelData());
            var changesCount = 0;
            view.Changed += (sender, args) => changesCount++;

            Assert.IsFalse(view.HideInsideSurfaces);

            view.HideInsideSurfaces = true;
            view.HideInsideSurfaces = true;

            Assert.IsTrue(view.HideInsideSurfaces);
            Assert.AreEqual(1, changesCount);
        }
    }
}
