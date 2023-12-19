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
        /// <inheritdoc/>
        public IModelObject this[ObjType objType, int objIndex]
        {
            get { return modelObjects[objType][objIndex]; }
            set { modelObjects[objType][objIndex] = value; }
        }
        /// <inheritdoc/>
        public List<IModelObject> this[ObjType objType]
        {
            get 
            {
                if(objType == ObjType.Элемент1D |
                    objType == ObjType.Элемент2D |
                    objType == ObjType.Элемент3D)
                return modelObjects[ObjType.Элемент].Where(x => x.ObjType == objType).ToList();
                else
                    return modelObjects[objType];
            }
        }
        /// <summary>
        /// ObjectsData
        /// </summary>
        public ObjectsData()
        {
            modelObjects = new Dictionary<ObjType, List<IModelObject>>()
            {
                {ObjType.Точка,new List<IModelObject>() },
                {ObjType.Линия,new List<IModelObject>() },
                {ObjType.Фигура,new List<IModelObject>() },
                {ObjType.Узел,new List<IModelObject>() },
                {ObjType.Элемент,new List<IModelObject>() }
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


        public IEnumerable<T> FindMany<T>() where T : IModelObject
        {
            foreach (var modelObject in modelObjects)
                foreach (var obj in modelObject.Value)
                    if (obj is T)
                        yield return (T)obj;
        }
/// <inheritdoc/>

        public IModelObject GetLastObj(ObjType objType)
        {
            if (objType == ObjType.Элемент1D |
       objType == ObjType.Элемент2D |
       objType == ObjType.Элемент3D)
                return modelObjects[ObjType.Элемент].FindLast(x => x.ObjType == objType);
            else if (objType == ObjType.Фигура2D |
                objType == ObjType.Фигура3D)
                return modelObjects[ObjType.Фигура].FindLast(x => x.ObjType == objType);
            else
                return modelObjects[objType].FindLast(x => x.ObjType == objType);
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

        public int NumberOfVolumeElements
        {
            get { return FindMany<IElement3D>().Count(); }
        }
        /// <inheritdoc/>

        public int NumberOfLineElements
        {
            get { return FindMany<IElement1D>().Count(); }
        }
        /// <inheritdoc/>

        public int NumberOfSurfaceElements
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
                var removedObjs = item.Value.Where(x => x.ExistState == false);

                var existObjects = item.Value.Except(removedObjs, new EqualityComparerByExistState()).ToList();

                if (existObjects.Count > 0)
                    temp.Add(item.Key, existObjects);
            }
            modelObjects = temp;
        }
        /// <inheritdoc/>

        public void Remove(ObjType objType)
        {
            foreach (var item in modelObjects[objType])
                item.ExistState = false;
            ClearNotExisted();
        }
        /// <inheritdoc/>

        public void RemoveAt(ObjType objType,int objInd)
        {
                modelObjects[objType][objInd].ExistState = false;
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

        public void Sort(ObjType objType)
        {
            if (!modelObjects.ContainsKey(objType))
                throw new Exception($"Объектов {objType} не найдено в коллекции!");

            MergeSort<IModelObject>.MergeSortFunction(modelObjects[objType]);
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
            if (objType == ObjType.Элемент1D |
                objType == ObjType.Элемент2D |
                objType == ObjType.Элемент3D)
                return modelObjects[ObjType.Элемент].Find(x => x.ObjType == objType)
    != null ? true : false;
            else if (objType == ObjType.Фигура2D |
                objType == ObjType.Фигура3D)
                return modelObjects[ObjType.Фигура].Find(x => x.ObjType == objType)
!= null ? true : false;
            else
                return modelObjects.ContainsKey(objType);
        }

/// <inheritdoc/>

        public IEnumerator<KeyValuePair<ObjType, List<IModelObject>>> GetEnumerator()
        {
            foreach (var objItem in modelObjects)
            {
                yield return objItem;
            }
        }

/// <inheritdoc/>

        public bool TryGetValue(ObjType key, out List<IModelObject> value)
        {
            return modelObjects.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
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
    }
}