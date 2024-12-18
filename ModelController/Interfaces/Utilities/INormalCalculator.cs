using Geometry;
using Model.Interfaces;
using Model.Interfaces.GeometryObjects;
using Model.Interfaces.MeshObjects;
using System.Collections.Generic;

namespace ModelControllerInterfaces.MeshObjsUtility
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
        IEnumerable<ILine> CalcElemsNormals(IEnumerable<ISurfaceElement> elements);

    }
}