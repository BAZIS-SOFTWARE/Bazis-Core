using Geometry;
using Model.Interfaces.GeometryObjects;
using Model.Interfaces.MeshObjects;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    public interface ICrossSectionMaker
    {
        /// <summary>
        /// Find a sections of the surfaces of given elements by the given plane
        /// </summary>
        ISurfaceFigure GetSectionSurfaces(IEnumerable<IElement3D> elements, Plane plane);
    }
}
