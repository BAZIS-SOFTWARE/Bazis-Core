using Microsoft.VisualStudio.TestTools.UnitTesting;
using PostProc;

namespace TestProject
{
    [TestClass]
    public class TestItemsScale
    {
        [DataTestMethod]
        [DataRow(0.1f, 10.5f, 3, 1, 0)]
        [DataRow(0.1f, 10.5f, 3, 10, 2)]
        [DataRow(0.1f, 10.5f, 3, 11, 2)]
        [DataRow(0.1f, 10.5f, 3, 0, 0)]
        [DataRow(0.1f, 10.5f, 3, 5, 1)]
        public void GetColorFromValue(float min, float max, int ranges, float value, int answer)
        {
            var sc = new ItemsScale(min, max, ranges);

            //assign

            //act
            var color = sc.GetValueColor(value);
            var colorIndex = sc.IndexOf(color);

            //assert
            Assert.AreEqual(answer, colorIndex);
        }

        [DataTestMethod]
        [DataRow(0.1f, 10.5f, 3)]
        [DataRow(0.1f, 10.5f, 5)]
        [DataRow(0.1f, 10.5f, 2)]
        public void GetMinMaxValue(float min, float max, int ranges)
        {
            //assign
            var sc = new ItemsScale(min, max, ranges);

            //act

            //assert
            Assert.AreEqual(min, sc.MinValue);
            Assert.AreEqual(max, sc.MaxValue);
        }
    }
}
