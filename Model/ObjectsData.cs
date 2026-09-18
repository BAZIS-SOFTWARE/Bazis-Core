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
using Model.Interfaces.ObjectsCollections;
using System.Security.Cryptography;
using System.Drawing;

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
            E2DCollection = new ObjectsCollection<Element2D>();
            E3DCollection = new ObjectsCollection<Element3D>();
            NodesSet = new ObjectsSet<Node>(ObjType.Узел.ToString());
            NodesSet.SetColor(Color.FromArgb(0, 0, 255));
            PointsSet = new ObjectsSet<GeometryPoint>(ObjType.Точка.ToString());
            PointsSet.SetColor(Color.FromArgb(255, 207, 64));
            CurveSet = new ObjectsSet<Curve>(ObjType.Кривая.ToString());
            SurfaceSet = new SurfacesSet(ObjType.Поверхность.ToString());
            //VolumeSet = new ObjectsSet<VolumeFigure>(ObjType.Объем.ToString());
        }

        /// <inheritdoc/>

        public IObjectsSet<Node> NodesSet { get; }
        /// <inheritdoc/>

        public IObjectsCollection<Element3D> E3DCollection { get; }
        /// <inheritdoc/>

        public IObjectsCollection<Element2D> E2DCollection { get; }
        /// <inheritdoc/>

        public IObjectsCollection<Beam> E1DCollection { get; }
        /// <inheritdoc/>

        public IObjectsSet<GeometryPoint> PointsSet { get; }
        /// <inheritdoc/>

        public IObjectsSet<Curve> CurveSet { get; }
        /// <inheritdoc/>

        public IObjectsSet<SurfaceFigure> SurfaceSet { get; }
        /// <inheritdoc/>

        //public VolumeData VolumeData { get; } = new VolumeData();

        /// <inheritdoc/>

        public bool IsReadOnly { get { return false; } }
        /// <inheritdoc/>

        public void ClearAll()
        {
            PointsSet.ClearWithExistState();
            CurveSet.ClearWithExistState();
            SurfaceSet.ClearWithExistState();
            NodesSet.ClearWithExistState();
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
                    return PointsSet.ContainsKey(objNumbr) ? PointsSet[objNumbr] : null;
                case ObjType.Кривая:
                    return CurveSet.ContainsKey(objNumbr) ? CurveSet[objNumbr] : null;
                case ObjType.Поверхность:
                    return SurfaceSet.ContainsKey(objNumbr) ? SurfaceSet[objNumbr] : null;
                case ObjType.Узел:
                    return NodesSet.ContainsKey(objNumbr) ? NodesSet[objNumbr] : null;
                case ObjType.Элемент1D:
                    return E1DCollection.Find(objNumbr);
                case ObjType.Элемент2D:
                    return E2DCollection.Find(objNumbr);
                default:
                    return E3DCollection.Find(objNumbr);
            }
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> FindMany(int objNumbr)
        {
            IModelObject modelObject;
            modelObject = PointsSet.ContainsKey(objNumbr) ? PointsSet[objNumbr] : null;
            if (modelObject != null)
                yield return modelObject;
            modelObject = CurveSet.ContainsKey(objNumbr) ? CurveSet[objNumbr] : null;
            if (modelObject != null)
                yield return modelObject;
            modelObject = SurfaceSet.ContainsKey(objNumbr) ? SurfaceSet[objNumbr] : null;
            if (modelObject != null)
                yield return modelObject;
            modelObject = NodesSet.ContainsKey(objNumbr) ? NodesSet[objNumbr] : null;
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
        }

        /// <inheritdoc/>
        public int GetMaxElementNumber()
        {
            var e1Numb = E1DCollection.Count != 0 ? E1DCollection.GetMaxNumber() : 0;
            var e2Numb = E2DCollection.Count != 0 ? E2DCollection.GetMaxNumber() : 0;
            var e3Numb = E3DCollection.Count != 0 ? E3DCollection.GetMaxNumber() : 0;

            if (e1Numb == e2Numb && e2Numb == e3Numb && e1Numb == 0)
                throw new Exception("Коллекции элементов пустые");

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
                count += SurfaceSet.Count;
                return count;
            }
        }
        int NumberOfObjects
        {
            get
            {
                var count = 0;
                count += NodesSet.Count;
                count += NumberOfElements;
                count += PointsSet.Count;
                count += NumberOfFigures;
                count += CurveSet.Count;
                return count;
            }
        }

        


        /// <inheritdoc/>

        public void ClearEmptySet()
        {
            E1DCollection.ClearEmptySets();
            E2DCollection.ClearEmptySets();
            E3DCollection.ClearEmptySets();
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> GetAllObjects()
        {
            foreach (var item in PointsSet.Values)
                yield return item;
            foreach (var item in CurveSet.Values)
                yield return item;
            foreach (var item in SurfaceSet.Values)
                yield return item;
            foreach (var item in NodesSet.Values)
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
        IEnumerable<SurfaceFigure> GetAllFigures()
        {

            foreach (var item in SurfaceSet.Values)
                yield return item;
        }

/// <inheritdoc/>

        public void ClearNotExisted()
        {
            PointsSet.ClearNotExisted();
            CurveSet.ClearNotExisted();
            SurfaceSet.ClearNotExisted();
            NodesSet.ClearNotExisted();
            E1DCollection.RemoveNotExistedObjects();
            E2DCollection.RemoveNotExistedObjects();
            E3DCollection.RemoveNotExistedObjects();

        }


        /// <inheritdoc/>

        public void Clear(ObjType objType)
        {
            // пробегаем по коллекции с целью синхронизации удаления в группах,
            //где есть эти объекты
            switch (objType)
            {
                case ObjType.Точка:
                    PointsSet.ClearWithExistState();
                    CurveSet.ClearWithExistState();
                    SurfaceSet.ClearWithExistState();
                    //VolumeData.Clear();
                    break;
                case ObjType.Кривая:
                    CurveSet.ClearWithExistState();
                    SurfaceSet.ClearWithExistState();
                    //VolumeData.Clear();
                    break;
                case ObjType.Поверхность:
                    SurfaceSet.ClearWithExistState();
                    //VolumeData.Clear();
                    break;
                case ObjType.Узел:
                    NodesSet.ClearWithExistState();
                    E1DCollection.Clear();
                    E2DCollection.Clear();
                    E3DCollection.Clear();
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
                case ObjType.Точка:
                    return PointsSet.Values;
                case ObjType.Кривая:
                    return CurveSet.Values;
                case ObjType.Поверхность:
                    return SurfaceSet.Values;
                case ObjType.Узел:
                    return NodesSet.Values;
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
                case ObjType.Точка:
                    return PointsSet.Count;
                case ObjType.Кривая:
                    return CurveSet.Count;
                case ObjType.Поверхность:
                    return SurfaceSet.Count;
                case ObjType.Узел:
                    return NodesSet.Count;
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
                    return PointsSet.Max(x => x.Key);
                case ObjType.Кривая:
                    return CurveSet.Max(x => x.Key);
                case ObjType.Поверхность:
                    return SurfaceSet.Max(x => x.Key);
                case ObjType.Узел:
                    return NodesSet.Max(x => x.Key);
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
                        coord._x = rot._x;//Исправление бага после применения оператора Rotate (растяжение и недоворот по углу)
                        coord._y = rot._y;
                        coord._z = rot._z;
                    }
                }
            }

            else throw new Exception("Операция доступна только для узлов или точек");
        }

        /// <inheritdoc/>

        public void Remove(ObjType objType, string objSet)
        {
            //Find(objType, objNumber).ExistState = false;
            switch (objType)
            {
                case ObjType.Элемент1D:
                    E1DCollection.Remove(objSet);
                    break;
                case ObjType.Элемент2D:
                    E2DCollection.Remove(objSet);
                    break;
                case ObjType.Элемент3D:
                    E3DCollection.Remove(objSet);
                    break;
                default:
                    throw new Exception($"Операция не поддерживается для данных типа {objType}");
            }
        }
        /// <inheritdoc/>

        public void Remove(ObjType objType, int objNumber)
        {
            //При удалении геометрии могут быть проблемы с синхронизацией в gmsh
            switch (objType)
            {
                case ObjType.Точка:
                    PointsSet[objNumber].ExistState = false;
                    PointsSet.Remove(objNumber);
                    CurveSet.ClearNotExisted();
                    SurfaceSet.ClearNotExisted();
                    //VolumeData.ClearNotExisted();
                    break;
                case ObjType.Кривая:
                    CurveSet[objNumber].ExistState = false;
                    CurveSet.Remove(objNumber);
                    SurfaceSet.ClearNotExisted();
                    //VolumeData.ClearNotExisted();
                    break;
                case ObjType.Поверхность:
                    SurfaceSet.Remove(objNumber);
                    //VolumeData.ClearNotExisted();
                    break;
                case ObjType.Узел:
                    NodesSet[objNumber].ExistState = false;
                    NodesSet.Remove(objNumber);
                    E1DCollection.RemoveNotExistedObjects();
                    E2DCollection.RemoveNotExistedObjects();
                    E3DCollection.RemoveNotExistedObjects();
                    break;
                case ObjType.Элемент1D:
                    E1DCollection.Remove(objNumber);
                    E1DCollection.RemoveNotExistedObjects();
                    break;
                case ObjType.Элемент2D:
                    E2DCollection.Remove(objNumber);
                    break;
                case ObjType.Элемент3D:
                    E3DCollection.Remove(objNumber);
                    break;
                default:
                    break;
            }
        }
/// <inheritdoc/>

        public IEnumerable<IModelObject> GetObjects(ObjType objType, string setName)
        {
            switch (objType)
            {
                case ObjType.Точка:
                    return PointsSet.Values;
                case ObjType.Кривая:
                    return CurveSet.Values;
                case ObjType.Поверхность:
                    return SurfaceSet.Values;
                case ObjType.Узел:
                    return NodesSet.Values;
                case ObjType.Элемент1D:
                    return E1DCollection[setName].Values;
                case ObjType.Элемент2D:
                    return E2DCollection[setName].Values;
                case ObjType.Элемент3D:
                    return E3DCollection[setName].Values;
                default:
                    return null;
            }
        }
/// <inheritdoc/>

        public ISetInfo GetSetInfo(ObjType objType, string setName)
        {
            switch (objType)
            {
                case ObjType.Точка:
                    return PointsSet;
                case ObjType.Кривая:
                    return CurveSet;
                case ObjType.Поверхность:
                    return SurfaceSet;
                case ObjType.Узел:
                    return NodesSet;
                case ObjType.Элемент1D:
                    {
                        return
    E1DCollection.ContainsName(setName) ?
    E1DCollection[setName] : null;
                    }
                case ObjType.Элемент2D:
                    {
                        return
E2DCollection.ContainsName(setName) ?
E2DCollection[setName] : null;
                    }
                default:
                    {
                        return
E3DCollection.ContainsName(setName) ?
E3DCollection[setName] : null;
                    }
            }
        }
/// <inheritdoc/>


        public ISetInfo GetSetInfo(ObjType objType, int objNumber)
        {
            switch (objType)
            {
                case ObjType.Точка:
                    return PointsSet;
                case ObjType.Кривая:
                    return CurveSet;
                case ObjType.Поверхность:
                    return SurfaceSet;
                case ObjType.Узел:
                    return NodesSet;
                case ObjType.Элемент1D:
                    {
                        return
                        E1DCollection.
                        Select(x => x.Value).
                        FirstOrDefault(x => x.ContainsKey(objNumber));
                    }
                case ObjType.Элемент2D:
                    {
                        return
                        E2DCollection.
                        Select(x => x.Value).
                        FirstOrDefault(x => x.ContainsKey(objNumber));
                    }
                default:
                    {
                        return
                        E3DCollection.
                        Select(x => x.Value).
                        FirstOrDefault(x => x.ContainsKey(objNumber));
                    }
            }
        }


        /// <inheritdoc/>

        public IEnumerable<ISetInfo> GetSetsInfo(ObjType objType)
        {
            switch (objType)
            {
                case ObjType.Точка:
                    return new ISetInfo[] { PointsSet };
                case ObjType.Кривая:
                    return new ISetInfo[] { CurveSet };
                case ObjType.Поверхность:
                    return new ISetInfo[] { SurfaceSet };
                case ObjType.Узел:
                    return new ISetInfo[] { NodesSet };
                case ObjType.Элемент1D:
                    return E1DCollection.Objects;
                case ObjType.Элемент2D:
                    return E2DCollection.Objects;
                default:
                    return E3DCollection.Objects;
            }
        }
    }
}
