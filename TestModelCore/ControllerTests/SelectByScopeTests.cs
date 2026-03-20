using Model.Interfaces;
using OperationalController;
using System.Globalization;

namespace ControllerTests
{
    [TestClass]
    public class SelectByScopeTests
    {
        private ProjectController _project;

        [TestInitialize]
        public void Init()
        {
            _project = new ProjectController();
            _project.GmshController.Load(@"..\..\..\..\GmshApi\gmsh.dll");
            _project.ImportCAD(@"..\..\..\Examples\CAD\Tavr.step");
        }

        /// <summary>
        /// Если размерности совпадают — возвращается исходный набор
        /// Проверка раннего выхода при одинаковых размерностях
        /// </summary>
        [TestMethod]
        public void SelectByScope_SameDimension_ReturnsSameSet()
        {
            var input = new[] { 1, 2, 3 };

            var result = _project.SelectByScope(2, input, 2);

            CollectionAssert.AreEquivalent(input, result.ToList());
        }

        /// <summary>
        /// Подъём с кривых до поверхностей и обратно
        /// </summary>
        [TestMethod]
        public void SelectByScope_CurveToSurface_BackToCurve()
        {
            var input = new[] { 1 };

            var result = _project.SelectByScope(1, input, 2);

            Assert.IsTrue(result.Count > 0);
            Assert.IsTrue(result.Contains(1)); // исходная должна остаться
        }

        /// <summary>
        /// Подъём с точек до поверхностей и обратно
        /// Проверяем что область расширяется
        /// </summary>
        [TestMethod]
        public void SelectByScope_PointToSurface_ExpandsScope()
        {
            var input = new[] { 1 };

            var result = _project.SelectByScope(0, input, 2);

            Assert.IsTrue(result.Count >= 1);
        }

        /// <summary>
        /// Несколько входных элементов объединяют область
        /// </summary>
        [TestMethod]
        public void SelectByScope_MultipleInputs_MergesScope()
        {
            var input = new[] { 1, 2 };

            var result = _project.SelectByScope(1, input, 2);

            Assert.IsTrue(result.Count >= input.Length);
        }

        /// <summary>
        /// Проверка отсутствия дубликатов
        /// </summary>
        [TestMethod]
        public void SelectByScope_NoDuplicates()
        {
            var input = new[] { 1 };

            var result = _project.SelectByScope(1, input, 2);

            Assert.AreEqual(result.Count, result.Distinct().Count());
        }

        /// <summary>
        /// Проверяем, что SelectByScope расширяет область до всех связанных сущностей
        /// через верхний уровень (поверхности)
        /// </summary>
        [TestMethod]
        public void SelectByScope_CurveScope_ShouldMatchAdjacencyExpansion()
        {
            int startDim = 1; // кривые
            int targetDim = 2; // поверхности
            int startNumber = 1;

            // act
            var result = _project.SelectByScope(startDim, new[] { startNumber }, targetDim);

            // assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            // 🔹 дополнительная проверка:
            // каждая кривая из результата должна иметь общую поверхность
            foreach (var curve in result)
            {
                var up = _project.GetAdjacentGeometryObjects(
                    _project.GetModelObject(Model.Interfaces.ObjType.Кривая, curve), 2);

                Assert.IsTrue(up.Any(), $"Кривая {curve} не имеет связанных поверхностей");
            }
        }

        /// <summary>
        /// Проверяем, что исходный элемент всегда входит в результат
        /// </summary>
        [TestMethod]
        public void SelectByScope_ResultContainsSource()
        {
            var result = _project.SelectByScope(1, new[] { 1 }, 2);

            Assert.IsTrue(result.Contains(1));
        }

        /// <summary>
        /// Проверка объединения областей
        /// </summary>
        [TestMethod]
        public void SelectByScope_MultipleCurves_ShouldExpandUnion()
        {
            var result1 = _project.SelectByScope(1, new[] { 1 }, 2);
            var result2 = _project.SelectByScope(1, new[] { 2 }, 2);
            var combined = _project.SelectByScope(1, new[] { 1, 2 }, 2);

            Assert.IsTrue(combined.Count >= result1.Count);
            Assert.IsTrue(combined.Count >= result2.Count);
        }

        /// <summary>
        /// Проверка, что при одинаковых размерностях ничего не меняется
        /// </summary>
        [TestMethod]
        public void SelectByScope_SameDim_ReturnsSame()
        {
            var input = new[] { 1, 2, 3 };

            var result = _project.SelectByScope(1, input, 1);

            CollectionAssert.AreEquivalent(input, result.ToList());
        }
    }
}
