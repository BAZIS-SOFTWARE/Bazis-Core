using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// GetModelObject
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="setNumber"></param>
        /// <returns></returns>
        IModelObject GetModelObject(ObjType objType, int setNumber);
        /// <summary>
        /// GetModelObjects
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> GetModelObjects(ObjType objType);
        /// <summary>
        /// GetAllModelObjects
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelObject> GetAllModelObjects();
        /// <summary>
        /// GetModelGroup
        /// </summary>
        /// <returns>
        /// Возвращает группу объектов
        /// </returns>
        IGroup GetModelGroup(int grIndex);
        /// <summary>
        /// GetAllModelGroups
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGroup> GetAllModelGroups();
        /// <summary>
        /// GetAllCondData
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICondData> GetAllCondData();
        /// <summary>
        /// GetModelElements
        /// </summary>
        /// <returns></returns>
        IEnumerable<Node> GetAllModelNodes();
        /// <summary>
        /// GetModelElements
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElement> GetAllModelElements();
        /// <summary>
        /// GetCondData
        /// </summary>
        /// <param name="grIndex"></param>
        /// <returns></returns>
        ICondData GetCondData(int grIndex);
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
