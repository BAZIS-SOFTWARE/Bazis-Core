using Geometry;
using System.Collections.Generic;
using System.Linq;
using Model.MeshObjects;
using ModelControllerInterfaces.Delone2D;
using ModelInterfaces.MeshObjects;

namespace ModelController.MeshObjsCreator.Delone2D
{
    /// <summary>
    /// DeloneMesh2DCreator
    /// </summary>
    public class DeloneMesh2DCreator : IDeloneMesh2DCreator
    {
        private DeloneTriangulation2D triangulation;
        private Dictionary<int, INode> numberToNode;

/// <inheritdoc/>

        public IEnumerable<IElement2D> GetElements()
        {
            var resultNodeIndeces = triangulation.GetTriangles();
            return resultNodeIndeces.Select(
                (t, i) => new Triangle(i, t.Select(node => numberToNode[node.Number]).ToArray()));
        }
        /// <inheritdoc/>

        public void MakeTriangulation(IEnumerable<INode> nodes)
        {
            triangulation = new DeloneTriangulation2D(GetNodesInPlaneCoordinates(nodes));
            numberToNode = nodes.ToDictionary(n => n.Number, n => n);
        }

        private List<DeloneNode2D> GetNodesInPlaneCoordinates(IEnumerable<INode> nodes)
        {
            return nodes.Select(n => new DeloneNode2D(n.Number, new Point2D(n.Position._x, n.Position._y)))
                .ToList();
        }
    }
}