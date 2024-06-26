using System.Collections.Generic;

namespace ModelInterfaces
{
    /// <summary>
    /// INodeObject
    /// </summary>
    public interface IVertexObject<T> : IModelObject where T : IPoint
    {
        /// <summary>
        /// GetVertexes
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetVertexes();
    }
}
