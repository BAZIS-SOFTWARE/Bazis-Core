using ModelInterfaces;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IBoundaryEdgesFinder
    /// </summary>
    public interface IBoundaryEdgesFinder
    {
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="modelData"></param>
        /// <returns></returns>
        List<string> Find(IModelData modelData);

        /// <summary>
        /// CreateBoundaryEdges
        /// </summary>
        /// <param name="modelData"></param>
        /// <returns></returns>
        IEnumerable<ILineObject<INode>> CreateBoundaryEdges(IModelData modelData);
    }
}
