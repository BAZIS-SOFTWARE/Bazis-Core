using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Model.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace TestProject_test.Model
{
    /// <summary>
    /// Проверяет связывание совпадающих поверхностей объёмных элементов.
    /// Грань с двумя живыми владельцами внутренняя, во всех остальных случаях внешняя.
    /// </summary>
    [TestClass]
    public class MarkSurfacesTests
    {
        /// <summary>
        /// Одиночный гексаэдр: соседей нет, все шесть граней внешние.
        /// </summary>
        [TestMethod]
        public void Mark_SingleHexa_LeavesAllSurfacesOuter()
        {
            var nodes = CreateLayer(1, 0);
            var upper = CreateLayer(5, 1);
            var hexa = CreateHexa(1, nodes, upper);

            var marker = new MarkSurfaces();
            var defects = marker.Mark(new ISurfaceElement[] { hexa });

            Assert.AreEqual(0, defects.Count);
            Assert.AreEqual(6, hexa.GetSurfaces().Count());
            Assert.IsFalse(hexa.GetSurfaces().Any(surface => surface.IsInner));
        }

        /// <summary>
        /// Два гексаэдра с общей гранью: из двенадцати граней внешними остаются десять,
        /// у каждого элемента ровно одна внутренняя.
        /// </summary>
        [TestMethod]
        public void Mark_TwoHexasSharingSurface_MarksOneSurfaceInnerInEach()
        {
            var bottom = CreateLayer(1, 0);
            var middle = CreateLayer(5, 1);
            var top = CreateLayer(9, 2);

            var lower = CreateHexa(1, bottom, middle);
            var upper = CreateHexa(2, middle, top);

            var marker = new MarkSurfaces();
            var defects = marker.Mark(new ISurfaceElement[] { lower, upper });

            Assert.AreEqual(0, defects.Count);
            Assert.AreEqual(1, lower.GetSurfaces().Count(surface => surface.IsInner));
            Assert.AreEqual(1, upper.GetSurfaces().Count(surface => surface.IsInner));

            var allSurfaces = lower.GetSurfaces().Concat(upper.GetSurfaces());
            Assert.AreEqual(10, allSurfaces.Count(surface => !surface.IsInner));
        }

        /// <summary>
        /// Внутренняя грань знает обоих владельцев, внешняя — только своего.
        /// </summary>
        [TestMethod]
        public void Mark_TwoHexasSharingSurface_InnerSurfaceKnowsBothOwners()
        {
            var bottom = CreateLayer(1, 0);
            var middle = CreateLayer(5, 1);
            var top = CreateLayer(9, 2);

            var lower = CreateHexa(1, bottom, middle);
            var upper = CreateHexa(2, middle, top);

            var marker = new MarkSurfaces();
            marker.Mark(new ISurfaceElement[] { lower, upper });

            var inner = lower.GetSurfaces().First(surface => surface.IsInner);
            var owners = inner.GetElements().ToList();

            Assert.AreEqual(2, owners.Count);
            Assert.AreEqual(1, owners[0].Number);
            Assert.AreEqual(2, owners[1].Number);
            Assert.AreEqual(1, inner.ElementNumber);

            var outer = lower.GetSurfaces().First(surface => !surface.IsInner);
            Assert.AreEqual(1, outer.GetElements().Count());
        }

        /// <summary>
        /// Погашенный сосед сам обнажает грани: повторный Mark не нужен.
        /// Ради этого сосед хранится ссылкой, а IsInner не хранится вовсе.
        /// </summary>
        [TestMethod]
        public void Mark_AfterNeighbourRemoved_SurfaceBecomesOuterWithoutRemark()
        {
            var bottom = CreateLayer(1, 0);
            var middle = CreateLayer(5, 1);
            var top = CreateLayer(9, 2);

            var lower = CreateHexa(1, bottom, middle);
            var upper = CreateHexa(2, middle, top);

            var marker = new MarkSurfaces();
            marker.Mark(new ISurfaceElement[] { lower, upper });

            Assert.AreEqual(1, lower.GetSurfaces().Count(surface => surface.IsInner));

            upper.ExistState = false;

            Assert.IsFalse(lower.GetSurfaces().Any(surface => surface.IsInner));
        }

        /// <summary>
        /// Повторный Mark разрывает устаревшие связи: элемент, оставшийся в одиночестве,
        /// снова получает все грани внешними.
        /// </summary>
        [TestMethod]
        public void Mark_CalledAgainWithoutNeighbour_ClearsStaleLink()
        {
            var bottom = CreateLayer(1, 0);
            var middle = CreateLayer(5, 1);
            var top = CreateLayer(9, 2);

            var lower = CreateHexa(1, bottom, middle);
            var upper = CreateHexa(2, middle, top);

            var marker = new MarkSurfaces();
            marker.Mark(new ISurfaceElement[] { lower, upper });

            Assert.AreEqual(1, lower.GetSurfaces().Count(surface => surface.IsInner));

            marker.Mark(new ISurfaceElement[] { lower });

            Assert.IsFalse(lower.GetSurfaces().Any(surface => surface.IsInner));
        }

        /// <summary>
        /// Три элемента на одной грани — дефект сетки. Третья поверхность
        /// попадает в отчёт и не затирает уже связанную пару.
        /// </summary>
        [TestMethod]
        public void Mark_ThreeElementsOnSameSurface_ReportsDefect()
        {
            var bottom = CreateLayer(1, 0);
            var middle = CreateLayer(5, 1);
            var top = CreateLayer(9, 2);

            var lower = CreateHexa(1, bottom, middle);
            var upper = CreateHexa(2, middle, top);
            var duplicate = CreateHexa(3, middle, top);

            var marker = new MarkSurfaces();
            var defects = marker.Mark(new ISurfaceElement[] { lower, upper, duplicate });

            Assert.IsTrue(defects.Count > 0);
            Assert.AreEqual(1, lower.GetSurfaces().Count(surface => surface.IsInner));
        }

        /// <summary>
        /// Ключ поверхности не зависит от порядка обхода узлов:
        /// общая грань опознаётся, хотя соседние элементы обходят её в разных направлениях.
        /// </summary>
        [TestMethod]
        public void CreateNodesKey_IgnoresVertexOrder()
        {
            var bottom = CreateLayer(1, 0);
            var middle = CreateLayer(5, 1);
            var top = CreateLayer(9, 2);

            var lower = CreateHexa(1, bottom, middle);
            var upper = CreateHexa(2, middle, top);

            var marker = new MarkSurfaces();
            marker.Mark(new ISurfaceElement[] { lower, upper });

            var lowerInner = lower.GetSurfaces().First(surface => surface.IsInner);
            var upperInner = upper.GetSurfaces().First(surface => surface.IsInner);

            CollectionAssert.AreEqual(lowerInner.CreateNodesKey(), upperInner.CreateNodesKey());
        }

        /// <summary>
        /// Пустая сетка допустима: дефектов нет, исключений нет.
        /// </summary>
        [TestMethod]
        public void Mark_EmptyMesh_ReturnsNoDefects()
        {
            var marker = new MarkSurfaces();
            var defects = marker.Mark(new List<ISurfaceElement>());

            Assert.AreEqual(0, defects.Count);
        }

        /// <summary>
        /// Создаёт слой из четырёх узлов на заданной высоте.
        /// Номера узлов идут подряд начиная с firstNumber.
        /// </summary>
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

        /// <summary>
        /// Собирает гексаэдр из нижнего и верхнего слоёв узлов.
        /// </summary>
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
