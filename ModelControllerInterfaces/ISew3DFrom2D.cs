using ModelInterfaces.MeshObjects;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// Построение 3D сетки путем соединения сторон 2D сетки
    /// </summary>
    public interface ISew3DFrom2D
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="firstSet"></param>
        /// <param name="secSet"></param>
        /// <returns></returns>
        List<IElement3D> Create(List<IElement2D> firstSet,List<IElement2D> secSet);
    }
}