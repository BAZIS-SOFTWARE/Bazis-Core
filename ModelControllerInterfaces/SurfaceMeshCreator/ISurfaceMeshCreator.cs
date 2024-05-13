using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces.SurfaceMeshCreator
{
    /// <summary>
    /// IDeloneMesh2DCreator
    /// </summary>
    public interface ISurfaceMeshCreator
    {
        /// <summary>
        /// MakeTriangulation
        /// </summary>
        /// <param name="nodes"></param>
        IEnumerable<IElement2D> DeloneTriangulation(IEnumerable<INode> nodes);
    }
}
