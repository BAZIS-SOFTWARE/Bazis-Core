using Microsoft.VisualStudio.TestTools.UnitTesting;
using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace PrTests
{
    [TestClass]
    public class TestSegment3D
    {
        [DataTestMethod]
        [DataRow(
            new float[]
            {
                1, 0, 0,
                0, 1.5f, 0,
                0, 0, 0.5f
            },
            new float[]
            {
                -1, -1, -1,
                1.5f, 2.0f, 1.0f
            },
            new float[]
            {
                0.3725f, 0.647f, 0.098f
            }
            )]
        [DataRow(
            new float[]
            {
                1, 0, 0,
                0, 1, 0,
                0, 0, 0
            },
            new float[]
            {
                0, -1, -1,
                0, 1, 0.05f
            },
            new float[]
            {
                0, 0.9047f, 0
            }
            )]       
        [DataRow(  // line parallel to plane
            new float[]
            {
                1, -2, 1.4f,
                0, -2, 1.6f,
                -1, 0, -0.6f
            },
            new float[]
            {                
                1, -2, 2,
                3, 1, -2
            },
            new float[] { })]
        [DataRow( //point on the plane
            new float[]
            {
                1, 0, 0,
                0, 1.5f, 0,
                0, 0, 0.5f
            },
            new float[]
            {
                1.5f, 2.0f, 1.0f,
                0, 1.5f, 0                
            },
            new float[] 
            {
                0, 1.5f, 0
            } 
            )]
        public void PlaneIntersection(float[] arPlane, float[] arLine, float[] point)
        {
            //arrange            
            var nodesPlane = new Point3D[3];
            CreateNodes(nodesPlane, arPlane);
            var surface = new Plane(nodesPlane[0], nodesPlane[1], nodesPlane[2]);

            var nodesLine = new Point3D[2];
            CreateNodes(nodesLine, arLine);
            var line = new Segment3D(nodesLine[0], nodesLine[1]);
            var asIntersecPoint = new Point3D();
            if (point.Length != 0)
            {
                asIntersecPoint = new Point3D(point[0], point[1], point[2]);
            }

            //act check intersection and point on surface

            var state = false;
            var actIntersecPoint = line.GetPlaneIntersection(ref state, surface);

            //assert
            Assert.AreEqual(asIntersecPoint, actIntersecPoint);
        }

        public void CreateNodes(Point3D[] nodes, float[] arPlane)
        {
            var j = 0;
            for (int i = 0; i < arPlane.Length; i = i + 3)
            {
                nodes[j] = new Point3D(arPlane[i], arPlane[i + 1], arPlane[i + 2]);
                j++;
            }
        }      
    }
}
