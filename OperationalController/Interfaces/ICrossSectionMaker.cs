using Geometry;
using Model.GeometryObjects;
using Model.Interfaces.MeshObjects;
using System.Collections.Generic;

namespace OperationalController
{
    public interface ICrossSectionMaker
    {
        /// <summary>
        /// Find a sections of the surfaces of given elements by the given plane
        /// </summary>
        SurfaceFigure GetSectionSurfaces(IEnumerable<IElement3D> elements, Plane plane);
    }
}
