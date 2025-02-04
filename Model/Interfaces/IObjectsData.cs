using Geometry;
using Model.GeometryObjects;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
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
        /// <param name="objNumber"></param>
        void Remove(ObjType objType, int objNumber);
        /// <summary>
        /// E3DCollection
        /// </summary>
        IObjectsCollection<IElement3D> E3DCollection { get; }
        /// <summary>
        /// E2DCollection
        /// </summary>
        IObjectsCollection<IElement2D> E2DCollection { get; }
        /// <summary>
        /// E1DCollection
        /// </summary>
        IObjectsCollection<Beam> E1DCollection { get; }
        /// <summary>
        /// NodeCollection
        /// </summary>
        IObjectsCollection<Node> NodeCollection { get; }
        /// <summary>
        /// PointCollection
        /// </summary>
        IObjectsCollection<GeometryPoint> PointCollection { get; }
        /// <summary>
        /// LineCollection
        /// </summary>
        IObjectsCollection<Line> LineCollection { get; }
        /// <summary>
        /// Fig2DCollection
        /// </summary>
        IObjectsCollection<SurfaceFigure> Fig2DCollection { get; }
        /// <summary>
        /// Fig3DCollection
        /// </summary>
        IObjectsCollection<VolumeFigure> Fig3DCollection { get; }
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
        /// Clear
        /// </summary>
        /// <param name="objType"></param>
        void Clear(ObjType objType);
        /// <summary>
        /// ContainsCollection
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        bool Contains(ObjType objType);
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
        /// ClearNotExisted
        /// </summary>
        void ClearEmpty();
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
        /// GetTypes
        /// </summary>
        ICollection<ObjType> ObjsTypes { get; }
        /// <summary>
        /// GetLastNumber
        /// </summary>
        /// <returns></returns>
        int GetMaxNumber(ObjType objType);
    }
}