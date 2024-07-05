using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces.MeshObjects
{
    /// <summary>
    /// IElement3D
    /// </summary>
    public interface IElement3D : ISurfaceElement, IVolume
    {
        /// <summary>
        /// SortNodes
        /// </summary>
        /// <param name="mainSurfaceNodesNumber"></param>
        /// <returns></returns>
        IEnumerable<INode> SortNodes(int mainSurfaceNodesNumber);
    }
}
