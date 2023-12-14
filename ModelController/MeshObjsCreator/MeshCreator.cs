using Geometry;
using System.Collections.Generic;
using System.Linq;
using Model.MeshObjects;
using ModelController.MeshObjsCreator.Delone2D;

namespace ModelController.MeshObjsCreator
{
    public class MeshCreator
    {
        private readonly DeloneTriangulation2D triangulation;
        private readonly Dictionary<int, Node> numberToNode;

        public MeshCreator(List<Node> nodes)
        {
            triangulation = new DeloneTriangulation2D(GetNodesInPlaneCoordinates(nodes));
            numberToNode = nodes.ToDictionary(n => n.Number, n => n);
        }

        public IEnumerable<Triangle> GetElements()
        {
            var resultNodeIndeces = triangulation.GetTriangles();
            return resultNodeIndeces.Select(
                (t, i) => new Triangle(i, t.Select(node => numberToNode[node.Number]).ToArray()));
        }

        private List<DeloneNode2D> GetNodesInPlaneCoordinates(List<Node> nodes)
        {
            return nodes.Select(n => new DeloneNode2D(n.Number, new Point2D(n.Position._x, n.Position._y)))
                .ToList();
        }
    }
}