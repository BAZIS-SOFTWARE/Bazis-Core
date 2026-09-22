using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Model.Utilities;
using OperationalController.ModelScenePresentator;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TestProject.ControllerTests
{
    /// <summary>
    /// Проверяет, что презентер поверхностей не кладёт в буферы грани,
    /// закрытые соседними элементами, когда отрисовка внутренних граней выключена.
    /// </summary>
    [TestClass]
    public class InnerSurfacesPresenterTests
    {
        /// <summary>
        /// Два гексаэдра с общей гранью: с отрисовкой внутренних граней
        /// в буфер идут все 12, без неё - только 10 внешних.
        /// </summary>
        [TestMethod]
        public void CreateIndexes_WithoutInnerSurfaces_SkipsSharedSurface()
        {
            var elements = CreateTwoHexas();

            var withInner = CreatePresenter(elements, true).CreateIndexes();
            var withoutInner = CreatePresenter(elements, false).CreateIndexes();

            // квадратная грань даёт 2 треугольника, треугольник - 3 указателя
            Assert.AreEqual(12 * 2 * 3, withInner.Item1);
            Assert.AreEqual(10 * 2 * 3, withoutInner.Item1);
        }

        /// <summary>
        /// Размеры всех буферов должны сходиться с CreateIndexes, иначе
        /// сборщики вершин выйдут за границы массива.
        /// </summary>
        [TestMethod]
        public void CreateBuffers_WithoutInnerSurfaces_MatchDeclaredSizes()
        {
            var presenter = CreatePresenter(CreateTwoHexas(), false);
            var indexes = presenter.CreateIndexes();

            var coordinates = presenter.CreateVertexes(indexes.Item2, "координаты");
            var colors = presenter.CreateVertexes(indexes.Item3, "цвет");
            var normals = presenter.CreateVertexes(indexes.Item2, "нормали");
            var edges = presenter.CreateEdgeFlags(indexes.Item4);

            Assert.AreEqual(10 * 2 * 9, coordinates.Length);
            Assert.AreEqual(10 * 2 * 12, colors.Length);
            Assert.AreEqual(10 * 2 * 9, normals.Length);
            Assert.AreEqual(10 * 2 * 3, edges.Length);
        }

        /// <summary>
        /// Сепараторы отображают диапазоны буфера обратно на элементы,
        /// поэтому они обязаны считать те же грани, что и буферы.
        /// </summary>
        [TestMethod]
        public void CreateSeparators_WithoutInnerSurfaces_CountOnlyDrawnSurfaces()
        {
            var elements = CreateTwoHexas();
            var presenter = CreatePresenter(elements, false);

            var separators = presenter.CreateSeparators();

            Assert.AreEqual(3, separators.Length);
            Assert.AreEqual(0, separators[0]);
            Assert.AreEqual(5 * 2, separators[1]);
            Assert.AreEqual(10 * 2, separators[2]);
            Assert.AreEqual(elements.Count, presenter.Count());
        }

        /// <summary>
        /// Погашенный сосед обнажает грань, и она возвращается в буфер
        /// без повторного связывания.
        /// </summary>
        [TestMethod]
        public void CreateIndexes_AfterNeighbourRemoved_ReturnsSurfaceToBuffer()
        {
            var elements = CreateTwoHexas();
            var presenter = CreatePresenter(elements, false);

            Assert.AreEqual(10 * 2 * 3, presenter.CreateIndexes().Item1);

            ((Element3D)elements[1]).ExistState = false;

            // у первого элемента все 6 граней снова внешние; второй остаётся
            // в списке презентера - отбор погашенных объектов не его забота
            Assert.AreEqual(11 * 2 * 3, presenter.CreateIndexes().Item1);
        }

        /// <summary>
        /// Собирает два гексаэдра с общей гранью и связывает их поверхности.
        /// </summary>
        private List<ISurfaceObject<ElementSurface, Node>> CreateTwoHexas()
        {
            var bottom = CreateLayer(1, 0);
            var middle = CreateLayer(5, 1);
            var top = CreateLayer(9, 2);

            var lower = CreateHexa(1, bottom, middle);
            var upper = CreateHexa(2, middle, top);

            new MarkSurfaces().Mark(new ISurfaceElement[] { lower, upper });

            return new List<ISurfaceObject<ElementSurface, Node>> { lower, upper };
        }

        private OperationalController.ISurfaceObjsPresenter CreatePresenter(
            IReadOnlyList<ISurfaceObject<ElementSurface, Node>> elements, bool drawInnerSurfaces)
        {
            var creator = new PresentersCreator();
            var colors = elements.Select(element => Color.Blue).ToList();
            return creator.CreateSurfaceObjectsPresenter<ElementSurface, Node>(elements, colors, drawInnerSurfaces);
        }

        private Node[] CreateLayer(int firstNumber, int level)
        {
            return new[]
            {
                new Node(firstNumber, new Point3D(0, 0, level)),
                new Node(firstNumber + 1, new Point3D(1, 0, level)),
                new Node(firstNumber + 2, new Point3D(1, 1, level)),
                new Node(firstNumber + 3, new Point3D(0, 1, level))
            };
        }

        private Hexa CreateHexa(int number, Node[] lower, Node[] upper)
        {
            var nodes = new[]
            {
                lower[0], lower[1], lower[2], lower[3],
                upper[0], upper[1], upper[2], upper[3]
            };

            return new Hexa(number, nodes);
        }
    }
}
