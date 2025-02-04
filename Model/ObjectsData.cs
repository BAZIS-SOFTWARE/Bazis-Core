using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Model.Interfaces.MeshObjects;
using Model.ObjectsCollections;
using System.Collections.ObjectModel;
using Model.MeshObjects;
using Model.GeometryObjects;
using Geometry;
using System.Reflection;

namespace Model
{
    /// <summary>
    /// ObjectsData
    /// </summary>
    public class ObjectsData : IObjectsData
    {
        /// <summary>
        /// ObjectsData
        /// </summary>
        public ObjectsData()
        {
            E1DCollection = new ObjectsCollection<Beam>();
            E2DCollection = new ObjectsCollection<IElement2D>();
            E3DCollection = new ObjectsCollection<IElement3D>();
            NodeCollection = new ObjectsCollection<Node>();
            PointCollection = new ObjectsCollection<GeometryPoint>();
            LineCollection = new ObjectsCollection<Line>();
            Fig2DCollection = new ObjectsCollection<SurfaceFigure>();
            Fig3DCollection = new ObjectsCollection<VolumeFigure>();

            ObjsTypes = new Collection<ObjType>()
                {
                    ObjType.Точка,
                    ObjType.Линия,
                    ObjType.Фигура2D,
                    ObjType.Фигура3D,
                    ObjType.Узел,
                    ObjType.Элемент1D,
                    ObjType.Элемент2D,
                    ObjType.Элемент3D
                };

        }

/// <inheritdoc/>

        public IObjectsCollection<Node> NodeCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<IElement3D> E3DCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<IElement2D> E2DCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<Beam> E1DCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<GeometryPoint> PointCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<Line> LineCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<SurfaceFigure> Fig2DCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<VolumeFigure> Fig3DCollection { get; }

        /// <inheritdoc/>

        public bool IsReadOnly { get { return false; } }
        /// <inheritdoc/>

        public void ClearAll()
        {
            PointCollection.Clear();
            LineCollection.Clear();
            Fig2DCollection.Clear();
            Fig3DCollection.Clear();
            NodeCollection.Clear();
            E1DCollection.Clear();
            E2DCollection.Clear();
            E3DCollection.Clear();
        }

        /// <inheritdoc/>

        public IModelObject Find(ObjType objType, int objNumbr)
        {
            switch (objType)
            {
                case ObjType.Точка:
                    return PointCollection.Find(objNumbr);
                case ObjType.Линия:
                    return LineCollection.Find(objNumbr);
                case ObjType.Фигура:
                    return GetAllFigures().First(x => x.Number == objNumbr);
                case ObjType.Фигура2D:
                    return Fig2DCollection.Find(objNumbr);
                case ObjType.Фигура3D:
                    return Fig3DCollection.Find(objNumbr);
                case ObjType.Узел:
                    return NodeCollection.Find(objNumbr);
                case ObjType.Элемент:
                    return GetAllElements().First(x => x.Number == objNumbr);
                case ObjType.Элемент1D:
                    return E1DCollection.Find(objNumbr);
                case ObjType.Элемент2D:
                    return E2DCollection.Find(objNumbr);
                case ObjType.Элемент3D:
                    return E3DCollection.Find(objNumbr);
                default:
                    return GetAllObjects().First(x => x.Number == objNumbr);
            }
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> FindMany(int objNumbr)
        {
            IModelObject modelObject;
            modelObject = PointCollection.Find(objNumbr);
            if(modelObject != null)
                yield return modelObject;
            modelObject = LineCollection.Find(objNumbr);
            if (modelObject != null)
                yield return modelObject;
            modelObject = Fig2DCollection.Find(objNumbr);
            if (modelObject != null)
                yield return modelObject;
            modelObject = Fig3DCollection.Find(objNumbr);
            if (modelObject != null)
                yield return modelObject;
            modelObject = NodeCollection.Find(objNumbr);
            if (modelObject != null)
                yield return modelObject;
            modelObject = E1DCollection.Find(objNumbr);
            if (modelObject != null)
                yield return modelObject;
            modelObject = E2DCollection.Find(objNumbr);
            if (modelObject != null)
                yield return modelObject;
            modelObject = E3DCollection.Find(objNumbr);
            if (modelObject != null)
                yield return modelObject;
            modelObject = LineCollection.Find(objNumbr);
            if (modelObject != null)
                yield return modelObject;     
        }


        int GetMaxElementNumber()
        {
            var e1Numb = E1DCollection.GetMaxNumber();
            var e2Numb = E2DCollection.GetMaxNumber();
            var e3Numb = E3DCollection.GetMaxNumber();

            var numb = e1Numb;
            if (e1Numb < e2Numb)
                numb = e2Numb;
            if (numb < e3Numb)
                numb = e3Numb;
            return numb;
        }

        /// <inheritdoc/>

        int NumberOfElements
        {
            get 
            {
                var count = 0;
                count += E1DCollection.Count;
                count += E2DCollection.Count;
                count += E3DCollection.Count;
                return count;
            }
        }

        int NumberOfFigures
        {
            get
            {
                var count = 0;
                count += Fig2DCollection.Count;
                count += Fig3DCollection.Count;
                return count;
            }
        }
        int NumberOfObjects
        {
            get
            {
                var count = 0;
                count += NodeCollection.Count;
                count += NumberOfElements;
                count += PointCollection.Count;
                count += NumberOfFigures;
                count += LineCollection.Count;
                return count;
            }
        }
        /// <inheritdoc/>

        public ICollection<ObjType> ObjsTypes { get; }

        /// <inheritdoc/>

        public void ClearEmpty()
        {
            PointCollection.ClearEmpty();
            LineCollection.ClearEmpty();
            Fig2DCollection.ClearEmpty();
            Fig3DCollection.ClearEmpty();
            NodeCollection.ClearEmpty();
            E1DCollection.ClearEmpty();
            E2DCollection.ClearEmpty();
            E3DCollection.ClearEmpty();
        }
/// <inheritdoc/>

        public bool Contains(ObjType objType)
        {
            return ObjsTypes.Contains(objType);
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> GetAllObjects()
        {
            foreach (var item in PointCollection.GetObjects())
                yield return item;
            foreach (var item in LineCollection.GetObjects())
                yield return item;
            foreach (var item in Fig2DCollection.GetObjects())
                yield return item;
            foreach (var item in Fig3DCollection.GetObjects())
                yield return item;
            foreach (var item in NodeCollection.GetObjects())
                yield return item;
            foreach (var item in E1DCollection.GetObjects())
                yield return item;
            foreach (var item in E2DCollection.GetObjects())
                yield return item;
            foreach (var item in E3DCollection.GetObjects())
                yield return item;
        }
        /// <inheritdoc/>

        public IEnumerable<IElement> GetAllElements()
        {
            foreach (var item in E1DCollection.GetObjects())
                yield return item;
            foreach (var item in E2DCollection.GetObjects())
                yield return item;
            foreach (var item in E3DCollection.GetObjects())
                yield return item;
        }
        /// <inheritdoc/>
        IEnumerable<IModelObject> GetAllFigures()
        {

            foreach (var item in Fig2DCollection.GetObjects())
                yield return item;
            foreach (var item in Fig3DCollection.GetObjects())
                yield return item;
        }

        int GetMaxFigureNumber()
        {
            throw new NotImplementedException();
        }

        void ClearElements()
        {
            E1DCollection.Clear();
            E2DCollection.Clear();
            E3DCollection.Clear();
        }

        void ClearFigures()
        {
            Fig2DCollection.Clear();
            Fig3DCollection.Clear();
        }
/// <inheritdoc/>

        public void Clear(ObjType objType)
        {
            switch (objType)
            {
                case ObjType.Объект:
                    foreach (var item in GetAllObjects())
                        item.ExistState = false;
                    ClearAll();
                    break;
                case ObjType.Точка:
                    foreach (var item in PointCollection.GetObjects())
                        item.ExistState = false;
                    PointCollection.Clear();
                    LineCollection.Clear();
                    ClearFigures();
                    break;
                case ObjType.Линия:
                    foreach (var item in LineCollection.GetObjects())
                        item.ExistState = false;
                    LineCollection.Clear();
                    break;
                case ObjType.Фигура:
                    foreach (var item in GetAllFigures())
                        item.ExistState = false;
                    ClearFigures();
                    break;
                case ObjType.Фигура2D:
                    foreach (var item in Fig2DCollection.GetObjects())
                        item.ExistState = false;
                    Fig2DCollection.Clear();
                    break;
                case ObjType.Фигура3D:
                    foreach (var item in Fig3DCollection.GetObjects())
                        item.ExistState = false;
                    Fig3DCollection.Clear();
                    break;
                case ObjType.Узел:
                    foreach (var item in NodeCollection.GetObjects())
                        item.ExistState = false;
                    NodeCollection.Clear();
                    ClearElements();
                    break;
                case ObjType.Элемент:
                    foreach (var item in GetAllElements())
                        item.ExistState = false;
                    ClearElements();
                    break;
                case ObjType.Элемент1D:
                    foreach (var item in E1DCollection.GetObjects())
                        item.ExistState = false;
                    E1DCollection.Clear();
                    break;
                case ObjType.Элемент2D:
                    foreach (var item in E2DCollection.GetObjects())
                        item.ExistState = false;
                    E2DCollection.Clear();
                    break;
                case ObjType.Элемент3D:
                    foreach (var item in E3DCollection.GetObjects())
                        item.ExistState = false;
                    E3DCollection.Clear();
                    break;
                default:
                    break;
            }
        }
/// <inheritdoc/>

        public IEnumerable<IModelObject> GetObjects(ObjType objType)
        {
            switch (objType)
            {
                case ObjType.Объект:
                    return GetAllObjects();
                case ObjType.Точка:
                    return PointCollection.GetObjects();
                case ObjType.Линия:
                    return LineCollection.GetObjects();
                case ObjType.Фигура:
                    return GetAllFigures();
                case ObjType.Фигура2D:
                    return Fig2DCollection.GetObjects();
                case ObjType.Фигура3D:
                    return Fig3DCollection.GetObjects();
                case ObjType.Узел:
                    return NodeCollection.GetObjects();
                case ObjType.Элемент:
                    return GetAllElements();
                case ObjType.Элемент1D:
                    return E1DCollection.GetObjects();
                case ObjType.Элемент2D:
                    return E2DCollection.GetObjects();
                case ObjType.Элемент3D:
                    return E3DCollection.GetObjects();
                default:
                    return null;
            }
        }
/// <inheritdoc/>

        public int Count(ObjType objType)
        {
            switch (objType)
            {
                case ObjType.Объект:
                    return NumberOfObjects;
                case ObjType.Точка:
                    return PointCollection.Count;
                case ObjType.Линия:
                    return LineCollection.Count;
                case ObjType.Фигура:
                    return NumberOfFigures;
                case ObjType.Фигура2D:
                    return Fig2DCollection.Count;
                case ObjType.Фигура3D:
                    return Fig3DCollection.Count;
                case ObjType.Узел:
                    return NodeCollection.Count;
                case ObjType.Элемент:
                    return NumberOfElements;
                case ObjType.Элемент1D:
                    return E1DCollection.Count;
                case ObjType.Элемент2D:
                    return E2DCollection.Count;
                case ObjType.Элемент3D:
                    return E3DCollection.Count;
                default:
                    return 0;
            }
        }
/// <inheritdoc/>

        public int GetMaxNumber(ObjType objType)
        {
            switch (objType)
            {
                case ObjType.Точка:
                    return PointCollection.GetMaxNumber();
                case ObjType.Линия:
                    return LineCollection.GetMaxNumber();
                case ObjType.Фигура:
                    return GetMaxFigureNumber();
                case ObjType.Фигура2D:
                    return Fig2DCollection.GetMaxNumber();
                case ObjType.Фигура3D:
                    return Fig3DCollection.GetMaxNumber();
                case ObjType.Узел:
                    return NodeCollection.GetMaxNumber();
                case ObjType.Элемент:
                    return GetMaxElementNumber();
                case ObjType.Элемент1D:
                    return E1DCollection.GetMaxNumber();
                case ObjType.Элемент2D:
                    return E2DCollection.GetMaxNumber();             
                default:
                    return E3DCollection.GetMaxNumber();
            }
        }
/// <inheritdoc/>

        public void Move(ObjType objType, Point3D vector)
        {
            if (objType == ObjType.Точка | objType == ObjType.Узел)
            {
                var objs = GetObjects(objType);
                foreach (var item in objs)
                {
                    foreach (var coord in item.GetCoordinates())
                    {
                        coord._x += vector._x;
                        coord._y += vector._y;
                        coord._z += vector._z;
                    }
                }
            }

            else throw new Exception("Операция доступна только для узлов или точек");
        }
/// <inheritdoc/>

        public void Rotate(ObjType objType, Point3D axis, float angle)
        {
            if (objType == ObjType.Точка | objType == ObjType.Узел)
            {
                var objs = GetObjects(objType);
                foreach (var item in objs)
                {
                    foreach (var coord in item.GetCoordinates())
                    {
                        var rot = Geometry.Vector.GetVectorRotation(coord, axis, angle);
                        coord._x += rot._x;
                        coord._y += rot._y;
                        coord._z += rot._z;
                    }
                }
            }

            else throw new Exception("Операция доступна только для узлов или точек");
        }
/// <inheritdoc/>

        public void Remove(ObjType objType, int objNumber)
        {
            Find(objType, objNumber).ExistState = false;

            NodeCollection.Remove(false);
            LineCollection.Remove(false);
            Fig2DCollection.Remove(false);
            Fig3DCollection.Remove(false);
            NodeCollection.Remove(false);
            E1DCollection.Remove(false);
            E2DCollection.Remove(false);
            E3DCollection.Remove(false);
        }
    }
}