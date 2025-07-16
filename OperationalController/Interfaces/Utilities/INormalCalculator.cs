using Geometry;
using Model.GeometryObjects;
using Model.Interfaces.MeshObjects;
using System.Collections.Generic;

namespace OperationalController.MeshObjsUtility
{
    public interface INormalCalculator
    {
        /// <summary>
        /// CalcNodeNormals
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        Dictionary<int, Point3D> CalcNodeNormals(IEnumerable<IElement3D> elements);

        /// <summary>
        /// CalcElemsNormals
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        IEnumerable<Curve> CalcElemsNormals(IEnumerable<ISurfaceElement> elements);

    }
}