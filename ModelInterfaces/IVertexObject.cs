using System.Collections.Generic;

namespace ModelInterfaces
{
    /// <summary>
    /// INodeObject
    /// </summary>
    public interface IVertexObject<T> : IModelObject where T : IGeometryPoint
    {
        /// <summary>
        /// GetNodes
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetVertexes();
    }
}
