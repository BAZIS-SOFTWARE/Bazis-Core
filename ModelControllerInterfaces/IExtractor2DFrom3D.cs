using Model;
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
        IEnumerable<Surface> FindOpenSurfaces(Element3D[] elements);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="StartNumber"></param>
        /// <returns></returns>
        IEnumerable<Element2D> Create(int StartNumber, Element3D[] elements);
    }
}