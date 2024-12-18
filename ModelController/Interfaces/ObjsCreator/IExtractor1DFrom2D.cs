using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using System.Collections.Generic;

namespace ModelControllerInterfaces.ObjsCreator
{
    /// <summary>
    /// IExtractor1DFrom2D
    /// </summary>
    public interface IExtractor1DFrom2D
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="startNumber"></param>
        /// /// <param name="elements"></param>
        /// <returns></returns>
        IEnumerable<ILineObject<Node>> Create(int startNumber, IElement2D[] elements);
    }
}
