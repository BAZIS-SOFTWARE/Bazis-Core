using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// ContainsModelObject
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumb"></param>
        /// <returns></returns>
        bool ContainsModelObject(ObjType objType, int objNumb);
        /// <summary>
        /// GetModelElements
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="setName"></param>
        /// <returns></returns>
        IEnumerable<IElement> GetModelElements(int dim, string setName);
        /// <summary>
        /// GetSurfaceFigure
        /// </summary>
        /// <returns></returns>
        SurfaceFigure GetModelSurface(int objNumb);
        /// <summary>
        /// GetModelVolumes
        /// </summary>
        /// <returns></returns>
        IEnumerable<VolumeFigure> GetModelVolumes();
        /// <summary>
        /// GetModelCurve
        /// </summary>
        /// <returns></returns>
        Curve GetModelCurve(int objNumb);
        /// <summary>
        /// GetModelPoint
        /// </summary>
        /// <returns></returns>
        GeometryPoint GetModelPoint(int objNumb);
    }
}
