using ModelInterfaces;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IExtractor2DFrom3D
    /// </summary>
    public interface IExtractor2DFrom3D
    {
        /// <summary>
        /// FindOpenSurfaces
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElementSurface> FindOpenSurfaces(IElement3D[] elements);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="StartNumber"></param>
        /// <returns></returns>
        IEnumerable<IElement2D> Create(int StartNumber, IElement3D[] elements);
    }
}