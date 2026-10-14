using Model;
using Model.Interfaces;
using Model.MeshObjects;
using Point3D = global::Geometry.Point3D;

namespace TestModeCore_test.Model
{
    [TestClass]
    public class ModelDataRemovalTests
    {
        [DataTestMethod]
        [DataRow(ObjType.Узел, 0, 0)]
        [DataRow(ObjType.Элемент2D, 4, 0)]
        public void RemoveAllData(ObjType objType, int expectedNodes, int expectedElements)
        {
            var model = new ModelData();
            var nodes = new[]
            {
                new Node(1, new Point3D(0, 0, 0)),
                new Node(2, new Point3D(1, 0, 0)),
                new Node(3, new Point3D(0, 1, 0)),
                new Node(4, new Point3D(1, 1, 0))
            };
            foreach (var node in nodes)
                model.ObjectData.NodesSet.Add(node.Number, node);

            var elements = new Element2D[]
            {
                new Triangle(1, new[] { nodes[0], nodes[1], nodes[2] }),
                new Triangle(2, new[] { nodes[1], nodes[2], nodes[3] })
            };
            model.ObjectData.E2DCollection.AddRange("e2d", elements);

            model.ObjectData.Clear(objType);

            Assert.AreEqual(expectedNodes, model.ObjectData.NodesSet.Count);
            Assert.AreEqual(expectedElements, model.ObjectData.E2DCollection.Count);
        }

        [DataTestMethod]
        [DataRow(1, 1)]
        [DataRow(2, 0)]
        public void DeleteNodeTest(int nodeNumber, int expectedElements)
        {
            var model = new ModelData();
            var nodes = new[]
            {
                new Node(1, new Point3D(0, 0, 0)),
                new Node(2, new Point3D(1, 0, 0)),
                new Node(3, new Point3D(0, 1, 0)),
                new Node(4, new Point3D(1, 1, 0))
            };
            foreach (var node in nodes)
                model.ObjectData.NodesSet.Add(node.Number, node);

            var elements = new Element2D[]
            {
                new Triangle(1, new[] { nodes[0], nodes[1], nodes[2] }),
                new Triangle(2, new[] { nodes[1], nodes[2], nodes[3] })
            };
            model.ObjectData.E2DCollection.AddRange(ObjType.Элемент2D.ToString(), elements);

            model.ObjectData.Remove(ObjType.Узел, nodeNumber);
            model.ObjectData.E2DCollection.RemoveNotExistedObjects();

            Assert.AreEqual(expectedElements, model.ObjectData.E2DCollection.Count);
        }
    }
}
