using Geometry;
using System.Collections;
using System.Collections.Generic;

namespace ModelInterfaces.MeshObjects
{
    public interface ISurfaceElement : IElement, ISurfaceObject<IElementSurface, INode>
    {
        /// <summary>
        /// Calc surfaces normals
        /// </summary>
        /// <returns>
        /// Lazy collections of vector normals
        /// </returns>
        IEnumerable<Point3D> CalcSurfacesNormals();
    }
}
