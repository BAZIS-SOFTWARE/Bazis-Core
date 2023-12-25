using Functions.Sort;
using ModelInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModelInterfaces.ObjectsComparers;
using ModelInterfaces.ObjectsFinders;

namespace Model
{
    /// <summary>
    /// ObjectsData
    /// </summary>
    public class ObjectsData : IObjectsData
    {
        Dictionary<ObjType,List<IModelObject>> modelObjects;
       
        /// <summary>
        /// ObjectsData
        /// </summary>
        public ObjectsData()
        {
            modelObjects = new Dictionary<ObjType, List<IModelObject>>()
            {
                {ObjType.Точка,new List<IModelObject>() },
                {ObjType.Линия,new List<IModelObject>() },
                {ObjType.Фигура2D,new List<IModelObject>() },
                {ObjType.Фигура3D,new List<IModelObject>() },
                {ObjType.Узел,new List<IModelObject>() },
                {ObjType.Элемент1D,new List<IModelObject>() },
                {ObjType.Элемент2D,new List<IModelObject>() },
                {ObjType.Элемент3D,new List<IModelObject>() }
            };
        }
/// <inheritdoc/>

        public bool IsReadOnly { get { return false; } }
/// <inheritdoc/>

        public void Clear()
        {
            modelObjects.Clear();
        }
        /// <inheritdoc/>

        public IEnumerable<T> FindMany<T>() where T : IModelObject
        {
            foreach (var modelObject in modelObjects)
                foreach (var obj in modelObject.Value)
                    if (obj is T)
                        yield return (T)obj;
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> FindMany(int objNumbr)
        {
            foreach (var modelObject in modelObjects)
            {
                var obj = modelObject.Value.Find(objNumbr);
                if (obj != null)
                    yield return obj;
            }
        }
/// <inheritdoc/>

        public IEnumerable<IModelObject> GetObjects(ObjType objType)
        {
            var temp = new List<IModelObject>();
            if(objType == ObjType.Объект)
            {
                foreach (var item in modelObjects)
                    temp.AddRange(item.Value);
            }
            else if(objType == ObjType.Элемент)
            {
                var e1d = modelObjects[ObjType.Элемент1D];
                var e2d = modelObjects[ObjType.Элемент2D];
                var e3d = modelObjects[ObjType.Элемент3D];

                temp.AddRange(e1d);
                temp.AddRange(e2d);
                temp.AddRange(e3d);
            }
            else if (objType == ObjType.Фигура)
            {
                var f2d = modelObjects[ObjType.Фигура2D];
                var f3d = modelObjects[ObjType.Фигура3D];

                temp.AddRange(f2d);
                temp.AddRange(f3d);
            }
            else
            {
                temp.AddRange(modelObjects[objType]);
            }

            foreach (var item in temp)
            {
                yield return item;
            }
        }



/// <inheritdoc/>

        public int GetLastObjNumber(ObjType objType)
        {
            if (objType == ObjType.Элемент)
            {
                var e1 = modelObjects[ObjType.Элемент1D];
                var e2 = modelObjects[ObjType.Элемент2D];
                var e3 = modelObjects[ObjType.Элемент3D];

                var e1Numb = e1.Count > 0 ? e1.Last().Number : 0;
                var e2Numb = e2.Count > 0 ? e2.Last().Number : 0;
                var e3Numb = e3.Count > 0 ? e3.Last().Number : 0;

                var numb = e1Numb;
                if (e1Numb < e2Numb)
                    numb = e2Numb;
                if (numb < e3Numb)
                    numb = e3Numb;
                return numb;
            }
            else
                return modelObjects[objType].Count > 0 ? modelObjects[objType].Last().Number : 0;
        }
/// <inheritdoc/>

        public int Count
        {
            get 
            {
                var count = 0;

                foreach (var item in modelObjects)
                {
                    count += item.Value.Count;
                }
                return count; 
            }
        }
        /// <inheritdoc/>

        public int NumberOfNodes
        {
            get { return FindMany<INode>().Count(); }
        }
        /// <inheritdoc/>

        public int NumberOf3DElements
        {
            get { return FindMany<IElement3D>().Count(); }
        }
        /// <inheritdoc/>

        public int NumberOf1DElements
        {
            get { return FindMany<IElement1D>().Count(); }
        }
        /// <inheritdoc/>

        public int NumberOf2DElements
        {
            get { return FindMany<IElement2D>().Count(); }
        }
        /// <inheritdoc/>

        public int NumberOfElements
        {
            get { return FindMany<IElement>().Count(); }
        }
  
        /// <inheritdoc/>

        public void ClearNotExisted()
        {
            var temp = new Dictionary<ObjType, List<IModelObject>>();
            foreach (var item in modelObjects)
            {
                var existObjs = item.Value.Where(x => x.ExistState == true).ToList();
                temp.Add(item.Key, existObjs);
            }
            modelObjects = temp;
        }
        /// <inheritdoc/>

        public void Remove(ObjType objType)
        {
            if(objType == ObjType.Объект)
            {
                foreach (var modelObject in modelObjects)
                    foreach (var item in modelObject.Value)
                    {
                        item.ExistState = false;
                    }
            }
            else if (objType == ObjType.Элемент)
            {
                foreach (var item in FindMany<IElement>())
                    item.ExistState = false;
            }

            else
                foreach (var item in modelObjects[objType])
                    item.ExistState = false;

            ClearNotExisted();
        }

/// <inheritdoc/>

        public IModelObject Find(ObjType objType, int objNumbr)
        {
            if (!modelObjects.ContainsKey(objType))
                return null;

            return modelObjects[objType].Find(objNumbr);
        }

   
/// <inheritdoc/>

        public ICollection<ObjType> Keys { get { return modelObjects.Keys; } }
/// <inheritdoc/>

        public ICollection<List<IModelObject>> Values { get { return modelObjects.Values; } }


/// <inheritdoc/>

        public bool Contains(KeyValuePair<ObjType, List<IModelObject>> item)
        {
            return modelObjects.Contains(item);
        }
/// <inheritdoc/>

        public bool ContainsKey(ObjType objType)
        {
                return modelObjects.ContainsKey(objType);
        }

        /// <inheritdoc/>

        public bool TryGetValue(ObjType key, out List<IModelObject> value)
        {
            return modelObjects.TryGetValue(key, out value);
        }

        /// <inheritdoc/>

        public void Add(ObjType objType, IModelObject addObj)
        {
            if (modelObjects[objType].Count > 0)
                if (addObj.Number <= modelObjects[objType].Last().Number)
                    throw new Exception($"Последний элемент в коллекции {objType} больше добавляемого!");
                else
                    modelObjects[objType].Add(addObj);
            else
                modelObjects[objType].Add(addObj);
        }
        /// <inheritdoc/>

        public void Add(ObjType objType,IEnumerable<IModelObject> addObjs)
        {
            var objsList = addObjs.ToList();
            MergeSort<IModelObject>.MergeSortFunction(objsList);

            if (modelObjects[objType].Count > 0)
                if (objsList.First().Number <= modelObjects[objType].Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции {objType} больше существующего последнего!");
                else
                    modelObjects[objType].AddRange(objsList);
            else
                modelObjects[objType].AddRange(objsList);

        }
/// <inheritdoc/>

        public IEnumerator<KeyValuePair<ObjType, List<IModelObject>>> GetEnumerator()
        {
            foreach (var item in modelObjects)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}