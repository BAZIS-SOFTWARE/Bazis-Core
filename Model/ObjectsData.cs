using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.MeshObjects;
using Model.ObjectsCollections;
using System.Collections.ObjectModel;

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
            E1DCollection = new Element1DCollection();
            E2DCollection = new Element2DCollection();
            E3DCollection = new Element3DCollection();
            NodeCollection = new NodeCollection();
            PointCollection = new PointCollection();
            LineCollection = new LineCollection();
            Fig2DCollection = new SurfaceFiguresCollection();
            Fig3DCollection = new VolumeFiguresCollection();

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

        public IObjectsCollection<INode> NodeCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<IElement3D> E3DCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<IElement2D> E2DCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<ILineObject<INode>> E1DCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<IGeometryPoint> PointCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<ILineObject<IGeometryPoint>> LineCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<ISurfaceFigure> Fig2DCollection { get; }
/// <inheritdoc/>

        public IObjectsCollection<IVolumeFigure> Fig3DCollection { get; }

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


        int GetLastElementNumber()
        {
            var e1Numb = E1DCollection.Count > 0 ? E1DCollection.Last().Number : -1;
            var e2Numb = E2DCollection.Count > 0 ? E2DCollection.Last().Number : -1;
            var e3Numb = E3DCollection.Count > 0 ? E3DCollection.Last().Number : -1;

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

        public void ClearNotExisted()
        {
            PointCollection.ClearNotExisted();
            LineCollection.ClearNotExisted();
            Fig2DCollection.ClearNotExisted();
            Fig3DCollection.ClearNotExisted();
            NodeCollection.ClearNotExisted();
            E1DCollection.ClearNotExisted();
            E2DCollection.ClearNotExisted();
            E3DCollection.ClearNotExisted();
        }
/// <inheritdoc/>

        public bool Contains(ObjType objType)
        {
            return ObjsTypes.Contains(objType);
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> GetAllObjects()
        {
            foreach (var item in PointCollection)
                yield return item;
            foreach (var item in LineCollection)
                yield return item;
            foreach (var item in Fig2DCollection)
                yield return item;
            foreach (var item in Fig3DCollection)
                yield return item;
            foreach (var item in NodeCollection)
                yield return item;
            foreach (var item in E1DCollection)
                yield return item;
            foreach (var item in E2DCollection)
                yield return item;
            foreach (var item in E3DCollection)
                yield return item;
        }
        /// <inheritdoc/>

        public IEnumerable<IElement> GetAllElements()
        {
            foreach (var item in E1DCollection)
                yield return (IElement)item;
            foreach (var item in E2DCollection)
                yield return item;
            foreach (var item in E3DCollection)
                yield return item;
        }
        /// <inheritdoc/>
        IEnumerable<IModelObject> GetAllFigures()
        {

            foreach (var item in Fig2DCollection)
                yield return item;
            foreach (var item in Fig3DCollection)
                yield return item;
        }

        int GetLastFigureNumber()
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
                    foreach (var item in PointCollection)
                        item.ExistState = false;
                    PointCollection.Clear();
                    LineCollection.Clear();
                    ClearFigures();
                    break;
                case ObjType.Линия:
                    foreach (var item in LineCollection)
                        item.ExistState = false;
                    LineCollection.Clear();
                    break;
                case ObjType.Фигура:
                    foreach (var item in GetAllFigures())
                        item.ExistState = false;
                    ClearFigures();
                    break;
                case ObjType.Фигура2D:
                    foreach (var item in Fig2DCollection)
                        item.ExistState = false;
                    Fig2DCollection.Clear();
                    break;
                case ObjType.Фигура3D:
                    foreach (var item in Fig3DCollection)
                        item.ExistState = false;
                    Fig3DCollection.Clear();
                    break;
                case ObjType.Узел:
                    foreach (var item in NodeCollection)
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
                    foreach (var item in E1DCollection)
                        item.ExistState = false;
                    E1DCollection.Clear();
                    break;
                case ObjType.Элемент2D:
                    foreach (var item in E2DCollection)
                        item.ExistState = false;
                    E2DCollection.Clear();
                    break;
                case ObjType.Элемент3D:
                    foreach (var item in E3DCollection)
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
                    return PointCollection;
                case ObjType.Линия:
                    return LineCollection;
                case ObjType.Фигура:
                    return GetAllFigures();
                case ObjType.Фигура2D:
                    return Fig2DCollection;
                case ObjType.Фигура3D:
                    return Fig3DCollection;
                case ObjType.Узел:
                    return NodeCollection;
                case ObjType.Элемент:
                    return GetAllElements();
                case ObjType.Элемент1D:
                    return E1DCollection;
                case ObjType.Элемент2D:
                    return E2DCollection;
                case ObjType.Элемент3D:
                    return E3DCollection;
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

        public int GetLastNumber(ObjType objType)
        {
            switch (objType)
            {
                case ObjType.Точка:
                    return PointCollection.Count > 0 ? PointCollection.Last().Number : -1;
                case ObjType.Линия:
                    return LineCollection.Count > 0 ? LineCollection.Last().Number : -1;
                case ObjType.Фигура:
                    return GetLastFigureNumber();
                case ObjType.Фигура2D:
                    return Fig2DCollection.Count > 0 ? Fig2DCollection.Last().Number :  -1;
                case ObjType.Фигура3D:
                    return Fig3DCollection.Count > 0 ? Fig3DCollection.Last().Number : -1;
                case ObjType.Узел:
                    return NodeCollection.Count > 0 ? NodeCollection.Last().Number : -1;
                case ObjType.Элемент:
                    return GetLastElementNumber();
                case ObjType.Элемент1D:
                    return E1DCollection.Count > 0 ? E1DCollection.Last().Number : -1;
                case ObjType.Элемент2D:
                    return E2DCollection.Count > 0 ? E2DCollection.Last().Number : -1;             
                default:
                    return E3DCollection.Count > 0 ? E3DCollection.Last().Number : -1;
            }
        }       
    }
}