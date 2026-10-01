using Microsoft.VisualStudio.TestTools.UnitTesting;
using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestModeCore_test.Geometry
{
    [TestClass]
    public class TestVector
    {
        [TestMethod]
        public void VectorZAxisRotation()
        {
            //arrange
            var vecIni = new Point3D(1, 0, 0);
            var angles = new float[] { 0, 30.0f, 45.0f, 88.0f, 180, 210 };

            var vecRot = new List<Point3D>();
            //act points on surface

            foreach (var angle in angles)
                vecRot.Add(Vector.GetVectorRotation(vecIni, new Point3D(0, 0, 1), angle));


            //assert
            var vecAssert = new Point3D[]
            {
                new Point3D(1, 0, 0),
                new Point3D(0.866f, 0.5f, 0),
                new Point3D(0.7071f, 0.7071f, 0),
                new Point3D(0.0349f, 0.9994f, 0),
                new Point3D(-1, 0, 0),
                new Point3D(-0.866f, -0.5f, 0),
            };

            for (int i = 0; i < vecRot.Count; i++)
            {
                if (!vecRot[i].Equals(vecAssert[i]))
                    throw new Exception();
            }

        }

        [DataTestMethod]
        [DataRow(new float[] { -21, 0, -3, -9.3f, 10, -1 })]
        public void DotProduct(float[] arr)
        {
            //arrange
            var v1 = new Point3D(arr[0], arr[1], arr[2]);
            var v2 = new Point3D(arr[3], arr[4], arr[5]);

            //act points on surface

            var prod1 = Vector.DotProd(v1, v2);
            //assert
            Assert.AreEqual(prod1, 198.3f);
        }

        [DataTestMethod]
        [DataRow(new float[] 
        {
            146,
            -0.047f,
            -0.05212305f,
            0.1608275f,
            18,
            -0.047f,
            -0.04709208f,
            0.1510405f
        })]
        // use loader
        //-3.645939E-11|0.0001076569|5.534064E-05
        //2.048473E-12|0.0001076569|5.534064E-05
        // use other
        //0|0.0001076571|5.534068E-05
        //4.153928E-10|0.0001076571|5.534068E-05


        public void CheckCodirectional(float[] arr)
        {
            //arrange
            var v1 = new Point3D(arr[0], arr[1], arr[2]);
            var v2 = new Point3D(arr[3], arr[4], arr[5]);

            //act points on surface

            var prod1 = Vector.DotProd(v1, v2);
            //assert
            Assert.AreEqual(prod1, 22.6372662f, 0.01f);
        }

        [DataTestMethod]   
        [DataRow(
            new[] { -0.125f, -0.275f, -3.66f },
            new[] { 0.25f, 0.55f, 7.32f })]
        [DataRow(
            new[] { 1.0f, 2.12f, 3.0f },
            new[] { -1.0f, -2.12f, -3.0f })]
        [DataRow(
            new[] { -2.6f, -0.19f, -2.05f },
            new[] { 2.6f, 0.19f, 2.05f })]
        [DataRow(
            new[] { -12.885f, -13.333f, -16.1785f },
            new[] { 12.885f, 13.333f, 16.1785f })]
        [DataRow(
            new[] { 0.0f, 0.0f, -2.515f },
            new[] { 0.0f, 0.0f, 2.515f })]
        [DataRow(
            new[] { 0.0f, 0.0f, -2.0f },
            new[] { 0.0f, 0.0f, -4.0f })]
        [DataRow(
            new[] { 12.6f, 0.9f, 2.0f },
            new[] { -12.6f, -0.9f, -2.0f })]
        [DataRow(
            new[] { 0.0f, 1.19f, 5.0f },
            new[] { -0.0f, -1.19f, -5.0f })]
        [DataRow(
            new[] { 3.12f, 2.0f, 1.675f },
            new[] { -3.12f, -2.0f, -1.675f })]
        [TestMethod]
        public void CheckCollinearityVectors_ShouldPass_WhenCounterDirectional(float[] vec1, float[] vec2)
        {
            var vector1 = new Point3D(vec1[0], vec1[1], vec1[2]);
            var vector2 = new Point3D(vec2[0], vec2[1], vec2[2]);

            Assert.IsTrue(Vector.CheckCollinearityVectors(vector1, vector2));
        }

        [DataRow(
            new[] { 12.089f, 0.0f, 0.0f },
            new[] { 24.089f, 0.0f, 0.0f })]
        [DataRow(
            new[] { 0.0f, 66.66f, 0.0f },
            new[] { 0.0f, 66.66f, 0.0f })]
        [DataRow(
            new[] { 0.0f, 0.0f, 31.12f },
            new[] { 0.0f, 0.0f, 31.12f })]
        [DataRow(
            new[] { 128.0f, 256.0f, 32.0f },
            new[] { 256.0f, 512.0f, 64.0f })]
        [DataRow(
            new[] { 0.0f, -0.0f, 0.0f },
            new[] { -0.0f, 0.0f, -0.0f })]
        [DataRow(
            new[] { 13.17f, 11.1f, 23.2f },
            new[] { 26.34f, 22.2f, 46.4f })]
        [TestMethod]
        public void CheckCollinearityVectors_ShouldPass_WhenParallel(float[] vec1, float[] vec2)
        {
            var vector1 = new Point3D(vec1[0], vec1[1], vec1[2]);
            var vector2 = new Point3D(vec2[0], vec2[1], vec2[2]);

            Assert.IsTrue(Vector.CheckCollinearityVectors(vector1, vector2));
        }

        [DataRow(
            new[] { -1.2f, -5.06f, -3.885f },
            new[] { 1.2f, 5.06f, 3.885f })]
        [DataRow(
            new[] { 9.185f, -11.066f, 10.01f },
            new[] { 9.185f, -11.066f, 10.01f })]
        [DataRow(
            new[] { 0.0f, 0.0f, -0.0f },
            new[] { 0.0f, 0.0f, -0.0f })]
        [DataRow(
            new[] { 12.222f, 6.666f, 9.18f },
            new[] { -12.222f, -6.666f, -9.18f })]
        [DataRow(
            new[] { -12.222f, -6.666f, -9.18f },
            new[] { 12.222f, 6.666f, 9.18f })]
        [TestMethod]
        public void CheckCollinearityVectors_ShouldPass_WhenVectorsOnOneLine(float[] vec1, float[] vec2)
        {
            var vector1 = new Point3D(vec1[0], vec1[1], vec1[2]);
            var vector2 = new Point3D(vec2[0], vec2[1], vec2[2]);

            Assert.IsTrue(Vector.CheckCollinearityVectors(vector1, vector2));
        }

        [TestMethod]
        public void CheckCollinearityVectors_ShouldPass_WhenBothNullable()
        {
            var vector1 = new Point3D(0.0f, 0.0f, 0.0f);
            var vector2 = new Point3D(0.0f, 0.0f, 0.0f);

            Assert.IsTrue(Vector.CheckCollinearityVectors(vector1, vector2));
        }

        [DataRow(
            new[] { 1.5f, -2.75f, 2.66f },
            new[] { 0.0f, 0.0f, 0.0f })]
        [DataRow(
            new[] { 0.0f, 0.0f, 0.0f },
            new[] { -1.0f, 1.0f, 1.0f })]
        [TestMethod]
        public void CheckCollinearityVectors_ShouldFail_WhenOneNullableVector(float[] vec1, float[] vec2)
        {
            var vector1 = new Point3D(vec1[0], vec1[1], vec1[2]);
            var vector2 = new Point3D(vec2[0], vec2[1], vec2[2]);

            Assert.IsTrue(Vector.CheckCollinearityVectors(vector1, vector2));
        }

        [DataRow(
            new[] { 1.5f, -2.75f, 2.66f },
            new[] { -1.5f, 2.25f, 2.33f })]
        [DataRow(
            new[] { 1.0f, 1.0f, 1.0f },
            new[] { -1.0f, 1.0f, 1.0f })]
        [TestMethod]
        public void CheckCollinearityVectors_ShouldFail_WhenUnColinearityVectors(float[] vec1, float[] vec2)
        {
            var vector1 = new Point3D(vec1[0], vec1[1], vec1[2]);
            var vector2 = new Point3D(vec2[0], vec2[1], vec2[2]);

            Assert.IsFalse(Vector.CheckCollinearityVectors(vector1, vector2));
        }
    }
}
