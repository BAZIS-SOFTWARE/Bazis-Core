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
        private DeloneTriangulation2D instance;

        /// <summary>
        /// DeloneMesh2DCreator
        /// </summary>
        public DeloneMesh2DCreator()
        {
            instance = new DeloneTriangulation2D();
        }
/// <inheritdoc/>

        public IEnumerable<IElement2D> MakeTriangulation(IEnumerable<INode> nodes)
        {
            var nodesInPlane = GetNodesInLocalPlaneCoordinates(nodes);
            var numberToSourceNode = nodes.ToDictionary(n => n.Number, n => n);
            var resultNodeIndeces = instance.GetTriangles(nodesInPlane);
            return resultNodeIndeces.Select((nodeArray, index) => 
                            new Triangle(index, nodeArray.Select(node => numberToSourceNode[node.Number]).ToArray()));
        }

        private List<DeloneNode2D> GetNodesInLocalPlaneCoordinates(IEnumerable<INode> nodes)
        {
            return nodes.Select(n => new DeloneNode2D(n.Number, new Point2D(n.Position._x, n.Position._y)))
                .ToList();
        }
    }
}