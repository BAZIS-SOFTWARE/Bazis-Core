using Functions.Sort;
using ModelInterfaces;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ModelInterfaces
{
    /// <summary>
    /// IObjectsData
    /// </summary>
    public interface IObjectsData
    {
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
        IObjectsCollection<ILineObject<INode>> E1DCollection { get; }
        /// <summary>
        /// NodeCollection
        /// </summary>
        IObjectsCollection<INode> NodeCollection { get; }
        /// <summary>
        /// PointCollection
        /// </summary>
        IObjectsCollection<IGeometryPoint> PointCollection { get; }
        /// <summary>
        /// LineCollection
        /// </summary>
        IObjectsCollection<ILineObject<IGeometryPoint>> LineCollection { get; }
        /// <summary>
        /// Fig2DCollection
        /// </summary>
        IObjectsCollection<IFigure2D> Fig2DCollection { get; }
        /// <summary>
        /// Fig3DCollection
        /// </summary>
        IObjectsCollection<IFigure3D> Fig3DCollection { get; }
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
        /// ClearNotExisted
        /// </summary>
        void ClearNotExisted();
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
        int GetLastNumber(ObjType objType);
    }
}