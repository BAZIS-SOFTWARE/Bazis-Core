using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PrTests
{
    [TestClass]
    public class TestPointsSorter
    {
        public void AssertCollections(float[] unsorted, float[] expected)
        {
            var expectedArr = CreatePoints3D(expected).ToList();
            var sortArr = CreatePoints3D(unsorted).ToList();
            sortArr.RoundSort();
            //PointsSorter.NewRoundSort(sortArr);

            CollectionAssert.AreEqual(expectedArr, sortArr);
        }

        [DataTestMethod]
        [DataRow(
            new float[]
            {
                0, 0, 0,
                1.5f, 1, 0,
                5, 0, 0,
                6, 1, 0
            },
            new float[]
            {
                0, 0, 0,
                1.5f, 1, 0,
                6, 1, 0,
                5, 0, 0
            })]
        [DataRow(
            new float[]
            {
                1,1.5f, 0,
                0.5f, 2, 0,
                -1, -1, 0,
                -2, 1.5f, 0,
                1,-1,0,
                -1,2.5f,0
            },
            new float[]
            {
                1,1.5f, 0,
                0.5f, 2, 0,
                -1,2.5f,0,
                -2, 1.5f,0,
                -1, -1, 0,
                1,-1,0
            })]
        [DataRow(
            new float[]
            {
                0.45f, 0.95f, 7.25f,
                2.5f,  3.05f, 0.1f,
                4.85f, 5.9f,  6.15f,
                1.75f, 1.2f,  0.3f
            },
            new float[]
            {
                0.45f, 0.95f, 7.25f,
                4.85f, 5.9f,  6.15f,
                2.5f,  3.05f, 0.1f,
                1.75f, 1.2f,  0.3f
            })]
        [DataRow(
            new float[]
            {
                -3.8f, -2.35f,  5.7f,
                 1.4f, -1.65f, -1.55f,
                -1.6f, -1.05f,  6.95f,
                 2.1f, -5.9f,   0.15f
            },
            new float[]
            {
                -3.8f, -2.35f,  5.7f,
                -1.6f, -1.05f,  6.95f,
                 1.4f, -1.65f, -1.55f,
                 2.1f, -5.9f,   0.15f
            })]
        [DataRow(
            new float[]
            {
                3.85f,  4.2f,  0.8f,
               -2.25f, -3.75f, 7.3f,
                3.7f,  -4.35f, 1.65f,
                1.4f,   7.6f,  3.45f,
               -1.55f,  2.5f,  6.95f,
                2.05f, -8.9f,  3.15f
            },
            new float[]
            {
               3.85f,  4.2f,  0.8f,
               3.7f,  -4.35f, 1.65f,
               2.05f, -8.9f,  3.15f,
              -2.25f, -3.75f, 7.3f,
              -1.55f,  2.5f,  6.95f,
               1.4f,   7.6f,  3.45f
            })]

        [DataRow(
            new float[]
            {
                0.0f,  1.01f, 0.0f,
                0.0f,  1.0f,  0.0f,
                5.01f, 3.0f,  0.0f,
                5.0f,  3.0f,  0.0f
            },
            new float[]
            {
                0.0f,  1.01f, 0.0f,
                0.0f,  1.0f,  0.0f,
                5.01f, 3.0f,  0.0f,
                5.0f,  3.0f,  0.0f
            })]
        [DataRow(
            new float[]
            {
                0.0f,  0.0001f, 0.0f,
                0.0f,  0.00011f,  0.0f,
                5.01f, 3.0f,  0.0f,
            },
            new float[]
            {
                0.0f,  0.0001f, 0.0f,
                0.0f,  0.00011f,  0.0f,
                5.01f, 3.0f,  0.0f,
            })]
        [DataRow(new float[]
        {
            0.0f, 2.0f, 1.0f,
            3.0f, 3.0f, 1.0f,
            2.0f, 1.0f, 1.0f,
            1.0f, 4.0f, 1.0f
        },
            new float[]
        {
            0.0f, 2.0f, 1.0f,
            2.0f, 1.0f, 1.0f,
            3.0f, 3.0f, 1.0f,
            1.0f, 4.0f, 1.0f
        })]
        [DataRow(
           new float[]
        {
           59003.5039f,  12318.13f,    52106.64f,
           59005.25f,    12318.13f,    52106.64f,
           59003.5f, 12318.13f,    52106.64f,
           59003.5f, 12315.9f, 52104.03f

        },
           new float[]
        {
           59003.5039f,  12318.13f,    52106.64f,
           59005.25f,    12318.13f,    52106.64f,
           59003.5f, 12315.9f, 52104.03f,
           59003.5f, 12318.13f,    52106.64f
        })]

        public void RoundPointsSorter(float[] points, float[] sortedPoints)
        {
            //arrange
            var unsortedNodes = CreatePoints3D(points);
            var sortedNodes = CreatePoints3D(sortedPoints);

            // act sorted points
            unsortedNodes.RoundSort();
            // nodes.RoundSort();

            // assert
            Assert.AreEqual(true, PointsComparison(unsortedNodes, sortedNodes));
        }
        public bool PointsComparison(Point3D[] nodes, Point3D[] sortedNodes)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != sortedNodes[i]) return false;
            }
            return true;
        }
        public Point3D[] CreatePoints3D(IList<float> arPlane)
        {
            var pointsAr = new Point3D[arPlane.Count / 3];
            var j = 0;
            for (int i = 0; i < arPlane.Count; i = i + 3)
            {

                pointsAr[j] = new Point3D(arPlane[i], arPlane[i + 1], arPlane[i + 2]);
                j++;
            }
            return pointsAr;
        }

        [TestMethod]
        [DataRow(new[] 
        {
            1.0f, 1.0f, 1.0f,
            -1.0f, -3.0f, 1.0f,
            -4.0f, -1.0f, 1.0f,
            -3.0f, 2.0f, 1.0f,
            0.0f, 3.0f, 1.0f
        }, 
            new[] 
        { 
            1.0f, 1.0f, 1.0f,
            -1.0f, -3.0f, 1.0f,
            -4.0f, -1.0f, 1.0f,
            -3.0f, 2.0f, 1.0f,
            0.0f, 3.0f, 1.0f,
        })]
        public void PointSorter_ShouldSortCorrect_WhenRightRotation(float[] unsorted, float[] expected)
        {
            AssertCollections(unsorted, expected);
        }

        [TestMethod]
        [DataRow(new[]
        {
            0.0f, 0.0f, 1.0f,
            2.0f, 3.0f, 1.0f,
            6.0f, -4.0f, 1.0f,
            7.0f, -1.0f, 1.0f,
            5.0f, 2.0f, 1.0f,
        },
            new[]
        {
            0.0f, 0.0f, 1.0f,
            2.0f, 3.0f, 1.0f,
            5.0f, 2.0f, 1.0f,
            7.0f, -1.0f, 1.0f,
            6.0f, -4.0f, 1.0f,
        })]
        public void PointSorter_ShouldSortCorrect_WhenLeftRotation(float[] unsorted, float[] expected)
        {
            AssertCollections(unsorted, expected);
        }

        [TestMethod]
        [DataRow(new[]
        {
            1.0f, 1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            1.0f, 1.0f, 1.0f,
            1.0f, 1.0f, 1.0f
        })]
        public void PointSorter_ShouldFall_WhenPointsArePoint(float[] unsorted)
        {
            var sortArr = CreatePoints3D(unsorted).ToList();
            sortArr.RoundSort();
        }     
    }
}
