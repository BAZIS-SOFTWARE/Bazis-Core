using Functions.Sort;
using ModelInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModelInterfaces.ObjectsComparers;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces.GeometryObjects;
using ModelInterfaces.MeshObjects;
using Model.ObjectsCollections;
using System.Security.Cryptography;
using System.Collections.ObjectModel;

namespace Model
{
    /// <summary>
    /// ObjectsData
    /// </summary>
    public class ObjectsData : IObjectsData
    {

        public IObjectsCollection<INode> NodeCollection => new NodeCollection();

        public IObjectsCollection<IElement3D> E3DCollection => new Element3DCollection();

        public IObjectsCollection<IElement2D> E2DCollection => new Element2DCollection();

        public IObjectsCollection<ILineObject<INode>> E1DCollection => new Element1DCollection();

        public IObjectsCollection<IGeometryPoint> PointCollection => new PointCollection();

        public IObjectsCollection<ILineObject<IGeometryPoint>> LineCollection => new LineCollection();

        public IObjectsCollection<IFigure2D> Fig2DCollection => new Fig2DCollection();

        public IObjectsCollection<IFigure3D> Fig3DCollection => new Fig3DCollection();

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
            if (Contains(objType))
                return null;

            switch (objType)
            {
                case ObjType.Точка:
                    return PointCollection.Find(objNumbr);
                case ObjType.Линия:
                    return LineCollection.Find(objNumbr);
                case ObjType.Фигура2D:
                    return Fig2DCollection.Find(objNumbr);
                case ObjType.Фигура3D:
                    return Fig3DCollection.Find(objNumbr);
                case ObjType.Узел:
                    return NodeCollection.Find(objNumbr);
                case ObjType.Элемент1D:
                    return E1DCollection.Find(objNumbr);
                case ObjType.Элемент2D:
                    return E2DCollection.Find(objNumbr);
                case ObjType.Элемент3D:
                    return E3DCollection.Find(objNumbr);
                default:
                    return null;
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
            yield return LineCollection.Find(objNumbr);      
        }
        /// <inheritdoc/>

        IEnumerable<IModelObject> GetObjects()
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

        int GetLastElementNumber()
        {
            var e1Numb = E1DCollection.Count > 0 ? E1DCollection.Last().Number : -1;
            var e2Numb = E2DCollection.Count > 0 ? E1DCollection.Last().Number : -1;
            var e3Numb = E3DCollection.Count > 0 ? E1DCollection.Last().Number : -1;

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



        public ICollection<ObjType> GetTypes
        {
            get 
            {
                return new Collection<ObjType>()
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
        }

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
            return GetTypes.Contains(objType);
        }
/// <inheritdoc/>

        public IEnumerable<IElement> GetElements()
        {
            foreach (var item in E1DCollection)
                yield return (IElement)item;
            foreach (var item in E2DCollection)
                yield return item;
            foreach (var item in E3DCollection)
                yield return item;
        }
        /// <inheritdoc/>
        IEnumerable<IModelObject> GetFigures()
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
                    ClearAll();
                    break;
                case ObjType.Точка:
                    PointCollection.Clear();
                    ClearFigures();
                    break;
                case ObjType.Линия:
                    LineCollection.Clear();
                    break;
                case ObjType.Фигура:
                    ClearFigures();
                    break;
                case ObjType.Фигура2D:
                    Fig2DCollection.Clear();
                    break;
                case ObjType.Фигура3D:
                    Fig3DCollection.Clear();
                    break;
                case ObjType.Узел:
                    NodeCollection.Clear();
                    ClearElements();
                    break;
                case ObjType.Элемент:
                    ClearElements();
                    break;
                case ObjType.Элемент1D:
                    E1DCollection.Clear();
                    break;
                case ObjType.Элемент2D:
                    E2DCollection.Clear();
                    break;
                case ObjType.Элемент3D:
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
                    return GetObjects();
                case ObjType.Точка:
                    return PointCollection;
                case ObjType.Линия:
                    return LineCollection;
                case ObjType.Фигура:
                    return GetFigures();
                case ObjType.Фигура2D:
                    return Fig2DCollection;
                case ObjType.Фигура3D:
                    return Fig3DCollection;
                case ObjType.Узел:
                    return NodeCollection;
                case ObjType.Элемент:
                    return GetElements();
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