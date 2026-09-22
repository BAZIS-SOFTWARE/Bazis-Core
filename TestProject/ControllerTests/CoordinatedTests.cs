using Geometry;
using Geometry.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using Model.MeshObjects;
using OperationalController.ModelScenePresentator;
using PostProc.ScenePresenter;
using System.Drawing;
using System.Linq;

namespace TestProject.ControllerTests
{
    /// <summary>
    /// Проверяет общий координатный контракт объектов модели и результатов.
    /// </summary>
    [TestClass]
    public class CoordinatedTests
    {
        /// <summary>
        /// Проверяет, что модель и результат используют координатный контракт без связи результата с моделью.
        /// </summary>
        [TestMethod]
        public void CoordinatedObjects_ExposeCoordinatesWithoutMakingResultFigureModelObject()
        {
            var node = new Node(1, new Point3D(1, 2, 3));
            var figure = new ResultFigure(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 0),
                new Point3D(0, 1, 0)
            }, Color.Red);

            Assert.IsInstanceOfType(node, typeof(ICoordinated));
            Assert.IsInstanceOfType(figure, typeof(ICoordinated));
            Assert.IsFalse(figure is IModelObject);
            Assert.AreEqual(3, figure.NumberOfPoints);
            Assert.AreEqual(3, figure.GetCoordinates().Count());
        }

        /// <summary>
        /// Проверяет построение буферов для сцены результатов без модельных интерфейсов.
        /// </summary>
        [TestMethod]
        public void ResultFigurePresenter_CreatesBuffersFromCoordinatedObject()
        {
            var figure = new ResultFigure(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 0),
                new Point3D(0, 1, 0)
            }, Color.Red);
            var creator = new PresentersCreator();
            var figures = new[] { figure };
            var colorsByFigure = figures.Select(item => item.Color).ToList();
            var presenter = creator.CreateSurfaceObjectsPresenter(figures, colorsByFigure);

            var indexes = presenter.CreateIndexes();
            var coordinates = presenter.CreateVertexes(indexes.Item2, "координаты");
            var colors = presenter.CreateVertexes(indexes.Item3, "цвет");

            Assert.AreEqual(3, indexes.Item1);
            Assert.AreEqual(9, coordinates.Length);
            Assert.AreEqual(12, colors.Length);
            Assert.AreEqual(1.0f, colors[0]);
            Assert.AreEqual(0.0f, colors[1]);
            Assert.AreEqual(0.0f, colors[2]);
        }
    }
}
