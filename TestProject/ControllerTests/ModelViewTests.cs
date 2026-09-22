using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.MeshObjects;
using OperationalController;
using OperationalController.ModelScenePresentator.GlObjsPresenters;
using System.Drawing;
using System.Linq;

namespace TestProject.ControllerTests
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
            var view = new ModelView(new ModelData());
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
            var view = new ModelView(new ModelData());
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
            var view = new ModelView(new ModelData());
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
            var view = new ModelView(model);
            view.Select(ObjType.Узел, new[] { 7 });

            view.HideSelected();

            Assert.IsFalse(view.GetVisible(ObjType.Узел, 7));
            Assert.AreEqual(0, view.SelectedCount);

            model.ObjectData.Remove(ObjType.Узел, 7);
            view.Prune();

            Assert.IsTrue(view.GetVisible(ObjType.Узел, 7));
        }

        /// <summary>
        /// Проверяет приоритет цвета выделения и альфа-канал представления.
        /// </summary>
        [TestMethod]
        public void GetColor_UsesSelectionColorBeforeSetColor()
        {
            var model = new ModelData();
            model.ObjectData.NodesSet.Add(3, new Node(3, new Point3D()));
            var view = new ModelView(model)
            {
                SelectionColor = Color.Red,
                Transparency = 128
            };
            var setInfo = model.ObjectData.GetSetInfo(ObjType.Узел, 3);
            view.SetSetColor(setInfo, Color.Blue);

            view.Select(ObjType.Узел, new[] { 3 });

            var color = view.GetColor(ObjType.Узел, 3);

            Assert.AreEqual(Color.FromArgb(128, Color.Red), color);
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
            var view = new ModelView(model);
            var expectedColor = Color.FromArgb(155, Color.SteelBlue);
            view.SetSetColor(setInfo, expectedColor);

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
            var view = new ModelView(model)
            {
                SelectionColor = Color.Red,
                Transparency = 128
            };
            view.SetSetColor(setInfo, Color.Blue);
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
            var view = new ModelView(model);

            view.SetSetColor(setInfo, Color.MediumPurple);
            view.SetViewMode(setInfo, ViewMode.Surface);
            collection.ChangeName("initial", "renamed");
            var renamedSet = collection["renamed"];

            Assert.AreEqual(Color.MediumPurple, view.GetSetColor(renamedSet));
            Assert.AreEqual(ViewMode.Surface, view.GetViewMode(renamedSet));
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
            var view = new ModelView(model);
            view.SetSetColor(setInfo, Color.Green);
            view.Select(ObjType.Узел, new[] { 5 });
            view.SetVisible(ObjType.Узел, new[] { 5 }, false);

            view.Reset();

            Assert.IsFalse(view.IsSelected(ObjType.Узел, 5));
            Assert.IsTrue(view.GetVisible(ObjType.Узел, 5));
            Assert.AreEqual(Color.Green, view.GetSetColor(setInfo));
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
            var view = new ModelView(model);
            view.SetSetColor(setInfo, Color.Fuchsia);
            collection.Remove("deleted");

            view.Prune();

            Assert.AreNotEqual(Color.Fuchsia, view.GetSetColor(setInfo));
        }

        /// <summary>
        /// Проверяет общемодельный признак отрисовки внутренних граней:
        /// по умолчанию они рисуются, смена значения уведомляет вид,
        /// а повторная запись того же значения - нет.
        /// </summary>
        [TestMethod]
        public void HideInsideSurfaces_RaisesChangedOnlyWhenValueDiffers()
        {
            var view = new ModelView(new ModelData());
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
