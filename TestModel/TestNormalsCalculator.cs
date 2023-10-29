using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Interfaces;
using Geometry;
using ModelController.ModelScenePresentator;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ModelController.MeshObjsUtility;

namespace TestModel
{
    [TestClass]
    public class TestNormalsCalculator
    {
        [DataTestMethod]
        [DataRow(90, 4, new int[] { 1, 1, 1, 1 })]
        [DataRow(110, 4, new int[] { 3, 3, 3, 3 })]
        [DataRow(180, 4,  new int[] { 3, 3, 3, 3 })]
        public void TestTetraSmoothNormals(float minAngle, int totalGroups, int[] expGroups)
        {
            //Правильный тетраэдр
            var nodes = new Node[] { new Node(1, new Point3D(-1.5f,0,0)), new Node(2, new Point3D(1.5f, 0, 0)),
                                     new Node(3, new Point3D(0,0, - 3 / 2f * (float)Math.Sqrt(3)) ),
                                     new Node(4, new Point3D(0, (float)Math.Sqrt(6), -(float)Math.Sqrt(3) / 2f))};
            var tetra = new Element3D(1, nodes, ObjKind.Tetra);
            var modelData = new ModelData();
            modelData.ObjectData.Add(tetra);
            var presenter = new ModelScenePresentator(modelData.ObjectData);
            var tuple = presenter["Элементы3D"].CreateIndexes();
            var glNormals = presenter["Элементы3D"].CreateVertexes(tuple.Item2, "сглаженная нормаль " + minAngle.ToString());
            var set = TotalNormals(glNormals);
            Assert.AreEqual(totalGroups, set.Count);
            foreach (var group in GroupNormals(glNormals).Select((v, i) => (v, i)))
                Assert.AreEqual(expGroups[group.i], group.v.Value.Count);
        }

        [DataTestMethod]
        [DataRow(30, 1.5f ,new int[] { 1, 1, 1 })]
        [DataRow(100, 1.5f ,new int[] { 2, 2, 2 })]
        [DataRow(140, 0.7f, new int[] { 2, 2, 2 })]
        public void Test2DElements(float minAngle, float yCoord ,int[] expGroups)
        {
            var nodesFirst = new Node[] { new Node(1, new Point3D(-1.5f, 0, 0)), new Node(2, new Point3D(1.5f, 0, 0)),
                                          new Node(3, new Point3D(1.5f, yCoord, 0 )), new Node(4 ,new Point3D(-1.5f, 1 , 0))};
            var first = new Element2D(1, nodesFirst, ObjKind.Quad);
            var second = new Element2D(2, new Node[] {nodesFirst[3], nodesFirst[2], new Node (5, new Point3D(0, 1.1f, -1)) }, ObjKind.Triangle);
            var modelData = new ModelData();
            modelData.ObjectData.Add(first);
            modelData.ObjectData.Add(second);
            var presenter = new ModelScenePresentator(modelData.ObjectData);
            var tuple = presenter["Элементы2D"].CreateIndexes();
            var glNormals = presenter["Элементы2D"].CreateVertexes(tuple.Item2, "сглаженная нормаль " + minAngle.ToString());
            foreach (var group in GroupNormals(glNormals).Select((v, i) => (v, i)))
                Assert.AreEqual(expGroups[group.i], group.v.Value.Count);
        }

        private HashSet<string> TotalNormals(float[] glNormals)
        {
            var result = new HashSet<string>();
            var sb = new StringBuilder(100);
            for (int i = 0, j = 0; i < glNormals.Length; i += 9, ++j)
            {
                for (var k = 0; k < 3; ++k)
                {
                    var normal = new Point3D(glNormals[i + k * 3], glNormals[i + k * 3 + 1], glNormals[i + k * 3 + 2]);
                    normal = Vector.GetVectorNorm(normal);
                    sb.Append(normal._x.ToString("0.0000"));
                    sb.Append(normal._y.ToString("0.0000"));
                    sb.Append(normal._z.ToString("0.0000"));
                    var hash = sb.ToString();
                    if(!result.Contains(hash))
                        result.Add(hash);
                    sb.Clear();
                }
            }
            return result;
        }

        private Dictionary<int, HashSet<string>> GroupNormals(float[] glNormals)
        {
            var groups = new Dictionary<int, HashSet<string>>();
            var sb = new StringBuilder(100);
            for (int i = 0, j = 0; i < glNormals.Length; i += 9, ++j)
            {
                groups.Add(j, new HashSet<string>());
                for (var k = 0; k < 3; ++k)
                {
                    var normal = new Point3D(glNormals[i + k * 3], glNormals[i + k * 3 + 1], glNormals[i + k * 3 + 2]);
                    normal = Vector.GetVectorNorm(normal);
                    sb.Append(normal._x.ToString("0.0000"));
                    sb.Append(normal._y.ToString("0.0000"));
                    sb.Append(normal._z.ToString("0.0000"));
                    var hash = sb.ToString();
                    if (!groups[j].Contains(hash))
                        groups[j].Add(hash);
                    sb.Clear();
                }
            }
            return groups;
        }
    }
}
