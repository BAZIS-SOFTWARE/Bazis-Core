using System.Collections.Generic;
using System.Linq;
using Model.MeshObjects;
using ModelControllerInterfaces.SurfaceMeshCreator;
using SurfaceMesherInterfaces;

namespace ModelController.MeshObjsCreator.Delone2D
{
    /// <summary>
    /// DeloneMesh2DCreator
    /// </summary>
    public class SurfaceMeshCreator : ISurfaceMeshCreator
    {
        private IDeloneTriangulation instance;

        /// <summary>
        /// DeloneMesh2DCreator
        /// </summary>
        public SurfaceMeshCreator(IDeloneTriangulation deloneMesh2DGenerator)
        {
            instance = deloneMesh2DGenerator;
        }
/// <inheritdoc/>

        public IEnumerable<ModelInterfaces.MeshObjects.IElement2D> DeloneTriangulation(IEnumerable<ModelInterfaces.MeshObjects.INode> nodes)
        {
            var nodesInPlane = GetNodesInLocalPlaneCoordinates(nodes);
            var numberToSourceNode = nodes.ToDictionary(n => n.Number, n => n);
            var resultNodeIndeces = instance.GetMesh(nodesInPlane);

            var counter = 0;
            foreach (var item in resultNodeIndeces)
            {
                var nds = item.GetNodes().Select(x => numberToSourceNode[x.Index]);
                yield return new Triangle(counter++, nds.ToArray());
            }

        }

        private List<SurfaceMesherInterfaces.INode2D> GetNodesInLocalPlaneCoordinates(IEnumerable<ModelInterfaces.MeshObjects.INode> nodes)
        {
            return nodes.Select(n => instance.CreateNode(n.Number, n.Position._x, n.Position._y)).ToList();
        }
    }
}