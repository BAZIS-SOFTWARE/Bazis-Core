using OperationalController;

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
            _project.InitializeGeometry(@"..\..\..\..\GmshApi\gmsh.dll");
            _project.ImportCAD(@"..\..\..\Examples\CAD\Tavr.step");
        }

        [DataTestMethod]
        [DataRow(2, new int[] { 1, 2, 3 }, 2)]     // одинаковые размерности: поверхности → поверхности
        [DataRow(1, new int[] { 1 }, 2)]           // кривые → поверхность
        [DataRow(0, new int[] { 1 }, 2)]           // точка → поверхность
        [DataRow(1, new int[] { 1, 2 }, 2)]        // несколько кривых → поверхность
        [DataRow(0, new int[] { 1, 2, 3 }, 1)]     // несколько точек → кривые
        [DataRow(2, new int[] { 1 }, 1)]           // поверхность → кривые
        [DataRow(1, new int[] { 5, 6, 7 }, 1)]     // кривые → кривые (проверка с другими числами)
        [DataRow(0, new int[] { 4, 5 }, 2)]        // несколько точек → поверхность (расширяем область)
        [DataRow(1, new int[] { 3, 4 }, 2)]        // несколько кривых → поверхность (объединение областей)
        [DataRow(2, new int[] { 10 }, 0)]          // поверхность → точка (редкий обратный кейс)
        public void SelectByScope_CombinedTests(int startDim, int[] input, int targetDim)
        {
            // act
            var result = _project.SelectByScope(startDim, input, targetDim);

            // базовые проверки
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            // 1. если размерности совпадают — возвращается исходный набор
            if (startDim == targetDim)
            {
                CollectionAssert.AreEquivalent(input, result.ToList());
                return; 
            }

            // 2. результат всегда содержит исходные элементы
            foreach (var item in input)
                Assert.IsTrue(result.Contains(item), $"Нет исходного элемента {item}");

            // 3. нет дубликатов
            Assert.AreEqual(result.Count, result.Distinct().Count(), "Есть дубликаты");

            // 4. при множественном вводе результат не меньше входа
            Assert.IsTrue(result.Count >= input.Length);

            // 5. каждая кривая должна иметь связанные поверхности (если старт — кривые)
            if (startDim == 1 && targetDim == 2)
            {
                foreach (var curve in result)
                {
                    var up = _project.GetAdjacentGeometryObjects(
                        _project.GetModelObject(Model.Interfaces.ObjType.Кривая, curve), 2);

                    Assert.IsTrue(up.Any(), $"Кривая {curve} не имеет связанных поверхностей");
                }
            }
        }
    }
}
