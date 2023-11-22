using System.Collections.Generic;

namespace ModelInterfaces
{
    /// <summary>
    /// INodeObject
    /// </summary>
    public interface INodeObject : IModelObject
    {
        /// <summary>
        /// GetNodes
        /// </summary>
        /// <returns></returns>
        IEnumerable<INode> GetNodes();
    }
}
