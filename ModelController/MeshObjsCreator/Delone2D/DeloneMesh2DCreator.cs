using Geometry;
using System.Collections.Generic;
using System.Linq;
using Model.MeshObjects;
using ModelControllerInterfaces.Delone2D;
using ModelInterfaces.MeshObjects;
using Mesher;

namespace ModelController.MeshObjsCreator.Delone2D
{
    /// <summary>
    /// DeloneMesh2DCreator
    /// </summary>
    public class DeloneMesh2DCreator : IDeloneMesh2DCreator
    {
        private DeloneTriangulation instance;

        /// <summary>
        /// DeloneMesh2DCreator
        /// </summary>
        public DeloneMesh2DCreator()
        {
            instance = new DeloneTriangulation();
        }
/// <inheritdoc/>

        public IEnumerable<IElement2D> MakeTriangulation(IEnumerable<INode> nodes)
        {
            var nodesInPlane = GetNodesInLocalPlaneCoordinates(nodes);
            var numberToSourceNode = nodes.ToDictionary(n => n.Number, n => n);
            var resultNodeIndeces = instance.GetMesh(nodesInPlane);

            var counter = 0;
            foreach (var item in resultNodeIndeces.Elements)
            {
                var nds = item.GetNodes().Select(x => numberToSourceNode[x.Index]);
                yield return new Triangle(counter++, nds.ToArray());
            }

        }

        private List<Node2D> GetNodesInLocalPlaneCoordinates(IEnumerable<INode> nodes)
        {
            return nodes.Select(n => new Node2D(n.Number, new Mesher.Point2D(n.Position._x, n.Position._y)))
                .ToList();
        }
    }
}