using ModelInterfaces.MeshObjects;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IInterfacedNodesFinder
    /// </summary>
    public interface IInterfacedNodesFinder
    {
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        Dictionary<int, List<int>> Find(List<int> nodes, IEnumerable<IElement> elements);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        Dictionary<int, List<int>> Find(IEnumerable<IElement> elements);
    }
}