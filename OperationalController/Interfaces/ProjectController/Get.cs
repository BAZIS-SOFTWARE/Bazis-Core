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
        /// GetAdjacentObject
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> GetAdjacentGeometryObjects(IModelObject obj, int direction);
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
        /// GetModelGroup
        /// </summary>
        /// <returns>
        /// Возвращает группу объектов
        /// </returns>
        IGroup GetModelGroup(int grIndex);
        /// <summary>
        /// GetModelGroup
        /// </summary>
        /// <param name="grName"></param>
        /// <returns>
        /// Возвращает группу объектов
        /// </returns>
        IGroup GetModelGroup(string grName);
        /// <summary>
        /// GetCondData
        /// </summary>
        /// <param name="condIndex"></param>
        /// <returns></returns>
        ICondData GetCondData(int condIndex);
        /// <summary>
        /// GetModelElements
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="setName"></param>
        /// <returns></returns>
        IEnumerable<IElement> GetModelElements(int dim, string setName);
        /// <summary>
        /// GetModelSurfaceElements
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        IEnumerable<ISurfaceElement> GetModelSurfaceElements(int dim);
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
