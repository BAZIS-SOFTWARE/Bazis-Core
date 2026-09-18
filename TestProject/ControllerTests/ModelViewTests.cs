using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model;
using Model.Interfaces;
using Model.MeshObjects;
using OperationalController;
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
            model.ObjectData.NodesSet.SetColor(Color.Blue);
            var view = new ModelView(model)
            {
                SelectionColor = Color.Red,
                Transparency = 128
            };

            view.Select(ObjType.Узел, new[] { 3 });

            var color = view.GetColor(ObjType.Узел, 3);

            Assert.AreEqual(Color.FromArgb(128, Color.Red), color);
        }
    }
}
