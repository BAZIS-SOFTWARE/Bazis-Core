using System;
using System.Collections.Generic;

namespace SurfaceMesherInterfaces
{
    public interface IDeloneTriangulation
    {
        void BuildTriangulation(List<INode2D> nodes, IEnumerable<IEdge> constraints, List<List<INode2D>> contour);

        [Obsolete]
        bool IsNodeInsideContour(INode2D node);

        IEnumerable<ITriangle2D> GetTriangles();

        IEnumerable<INode2D> RefineMesh();
    }
}