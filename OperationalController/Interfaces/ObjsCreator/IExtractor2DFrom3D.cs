using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using System.Collections.Generic;

namespace OperationalController.ObjsCreator
{
    /// <summary>
    /// IExtractor2DFrom3D
    /// </summary>
    public interface IExtractor2DFrom3D
    {
        /// <summary>
        /// FindOpenSurfaces
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        IEnumerable<ElementSurface> FindOpenSurfaces(IEnumerable<IElement3D> elements);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="StartNumber"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        IEnumerable<Element2D> Create(int StartNumber, IEnumerable<IElement3D> elements);
    }
}