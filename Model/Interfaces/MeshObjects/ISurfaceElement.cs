using Geometry;
using System.Collections;
using System.Collections.Generic;

namespace Model.Interfaces.MeshObjects
{
    public interface ISurfaceElement : IElement, ISurfaceObject<IElementSurface, INode>
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
