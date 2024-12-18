using Geometry;
using Model.MeshObjects;
using System.Collections;
using System.Collections.Generic;

namespace Model.Interfaces.MeshObjects
{
    public interface ISurfaceElement : IElement, ISurfaceObject<ElementSurface, Node>
    {
        /// <summary>
        /// Get surfaces normals
        /// </summary>
        /// <returns>
        /// Lazy collections of vector normals
        /// </returns>
        IEnumerable<Point3D> GetSurfacesNormals();
    }
}
