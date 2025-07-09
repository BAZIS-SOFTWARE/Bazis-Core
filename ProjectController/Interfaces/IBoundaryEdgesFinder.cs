using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
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
        /// CreateBoundaryEdges
        /// </summary>
        /// <param name="linesNodes. Nodes written one by one splitted by space n1 n2"></param>
        /// <param name="modelData"></param>
        /// <returns></returns>
        IEnumerable<ILineObject<Node>> CreateBoundaryEdges(List<string> linesNodesб, IModelData modelData);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="surfaceElements"></param>
        /// <returns></returns>
        List<string> Find(IEnumerable<ISurfaceElement> surfaceElements);
    }
}
