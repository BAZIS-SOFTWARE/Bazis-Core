using Geometry.Exteisions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeometryTests
{
    [TestClass]
    public class TestExtensions
    {
        [DataTestMethod]
        [DataRow(new float[] { 20, 300, 210, 190, 100.0f, 204.2857f })]
        [DataRow(new float[] { 600, 1500, 210, 190, 700.0f, 207.7778f })]
        public void BigramsTestReturnsFirstElementsInLastCouple(float[] ar)
        {
            //arrange
            var objList = new List<float>();
            //act
            foreach (var big in ar.Bigrams(true))
            {
                objList.Add(big.Item1);
                objList.Add(big.Item2);
            }

            //assert
            Assert.AreEqual(objList[objList.Count - 1], ar.First(), 0.0001f);
        }
    }
}
