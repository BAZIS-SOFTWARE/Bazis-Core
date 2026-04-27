using System;
using System.Collections.Generic;

namespace SurfaceMesherInterfaces
{
    public interface ITriangleFilter
    {
        [Obsolete]
        IEnumerable<ITriangle2D> GetInflictedTriangles(List<INode2D> layerOneNodes, 
            List<ITriangle2D> trianglesLayerOne, 
            List<List<INode2D>> layerTwoContour,
            List<INode2D> layerTwoNodes);

        IEnumerable<ITriangle2D> GetInflictedTriangles(List<ITriangle2D> triangles,
            List<INode2D> outerBoundary, 
            List<INode2D>[] innerBoundary,
            bool specialSelection = true);
    }
}
