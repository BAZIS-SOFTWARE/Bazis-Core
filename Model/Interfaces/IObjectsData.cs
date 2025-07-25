using Geometry;
using Model.GeometryObjects;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
using Model.ObjectsCollections;
using System.Collections.Generic;
using System.Numerics;

namespace Model.Interfaces
{
    /// <summary>
    /// IObjectsData
    /// </summary>
    public interface IObjectsData
    {
        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objSet"></param>
        void Remove(ObjType objType, string objSet);
        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumber"></param>
        void Remove(ObjType objType, int objNumber);


        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="objType"></param>
        void Clear(ObjType objType);
        /// <summary>
        /// E3DCollection
        /// </summary>
        IObjectsCollection<Element3D> E3DCollection { get; }
        /// <summary>
        /// E2DCollection
        /// </summary>
        IObjectsCollection<Element2D> E2DCollection { get; }
        /// <summary>
        /// E1DCollection
        /// </summary>
        IObjectsCollection<Beam> E1DCollection { get; }
        /// <summary>
        /// NodeCollection
        /// </summary>
        IObjectsSet<Node> NodesSet { get; }
        /// <summary>
        /// PointCollection
        /// </summary>
        IObjectsSet<GeometryPoint> PointsSet { get; }
        /// <summary>
        /// LineCollection
        /// </summary>
        IObjectsSet<Curve> CurveSet { get; }
        /// <summary>
        /// Fig2DCollection
        /// </summary>
        IObjectsCollection<SurfaceFigure> SurfaceCollection{ get; }
        /// <summary>
        /// Count
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        int Count(ObjType objType);
        /// <summary>
        /// ClearAll
        /// </summary>
        void ClearAll();
        /// <summary>
        /// ClearEmpty
        /// </summary>
        void ClearEmptySet();
        /// <summary>
        /// ClearNotExisted
        /// </summary>
        void ClearNotExisted();

        /// <summary>
        /// Move
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="vector"></param>
        /// <returns></returns>
        void Move(ObjType objType, Point3D vector);
        /// <summary>
        /// Rotate
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        void Rotate(ObjType objType, Point3D axis, float angle);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> FindMany(int objNumbr);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        IModelObject Find(ObjType objType, int objNumbr);
        /// <summary>
        /// GetAllObjects
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelObject> GetAllObjects();
        /// <summary>
        /// GetAllElements
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElement> GetAllElements();
        /// <summary>
        /// GetObjects
        /// </summary>
        /// <returns></returns>
        IEnumerable<IModelObject> GetObjects(ObjType objType);
        /// <summary>
        /// GetObjects
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="setName"></param>
        /// <returns></returns>
        IEnumerable<IModelObject> GetObjects(ObjType objType, string setName);
        /// <summary>
        /// GetSetsInfo
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        IEnumerable<ISetInfo> GetSetsInfo(ObjType objType);
        /// <summary>
        /// GetLastNumber
        /// </summary>
        /// <returns></returns>
        int GetMaxNumber(ObjType objType);
        /// <summary>
        /// SetBackColor
        /// </summary>
        /// <param name="objType"></param>
        void SetBackColor(ObjType objType);
        /// <summary>
        /// GetMaxElementNumber
        /// </summary>
        /// <returns></returns>
        int GetMaxElementNumber();
        /// <summary>
        /// GetSetInfo
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="setName"></param>
        /// <returns></returns>
        ISetInfo GetSetInfo(ObjType objType, string setName);
    }
}