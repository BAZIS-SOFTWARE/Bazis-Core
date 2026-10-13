using Microsoft.VisualStudio.TestTools.UnitTesting;
using OperationalController.ModelScenePresentator.GlObjsPresenters;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TestProject_test.ModelView
{
    /// <summary>
    /// Проверяет источники цветов презентаторов.
    /// </summary>
    [TestClass]
    public class ObjsColorsTests
    {
        /// <summary>
        /// Проверяет повторение единого цвета для всех объектов.
        /// </summary>
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(3)]
        public void CreateColors_SingleColor_ReturnsColorForEveryObject(int objectsCount)
        {
            var source = new SingleColorObjsColors(Color.CornflowerBlue);
            var colors = source.CreateColors(objectsCount).ToArray();

            Assert.AreEqual(ObjsColorsType.Single, source.Type);
            Assert.AreEqual(objectsCount, colors.Length);

            foreach (var color in colors)
                Assert.AreEqual(Color.CornflowerBlue, color);
        }

        /// <summary>
        /// Проверяет контроль соответствия количества индивидуальных цветов объектам.
        /// </summary>
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(2)]
        public void CreateColors_ManyColorsWithDifferentCount_ThrowsArgumentException(int objectsCount)
        {
            var source = new ManyColorsObjsColors(new List<Color> { Color.CornflowerBlue });

            Assert.AreEqual(ObjsColorsType.Many, source.Type);
            Assert.ThrowsException<ArgumentException>(() => source.CreateColors(objectsCount).ToArray());
        }
    }
}
