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
        /// this
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objIndex"></param>
        /// <returns></returns>
        public IModelObject this[ObjType objType, int objIndex]
        {
            get { return modelObjects[objType][objIndex]; }
            set { modelObjects[objType][objIndex] = value; }
        }
        /// <summary>
        /// this
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        public List<IModelObject> this[ObjType objType]
        {
            get { return modelObjects[objType]; }
            set { modelObjects[objType] = value; }
        }
        /// <summary>
        /// ObjectsData
        /// </summary>
        public ObjectsData()
        {
            modelObjects = new Dictionary<ObjType, List<IModelObject>>();
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
            if (modelObjects.ContainsKey(objType))
                return modelObjects[objType].Last();           
            else return null;
        }
/// <inheritdoc/>

        public int Count
        {
            get { return modelObjects.Count; }
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

        public bool Remove(ObjType objType)
        {
            return modelObjects.Remove(objType);
        }
        /// <inheritdoc/>

        public void RemoveAt(ObjType objType,int objInd)
        {
            modelObjects[objType].RemoveAt(objInd);

            if (modelObjects[objType].Count == 0)
                modelObjects.Remove(objType);
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

        public void Add(ObjType key, List<IModelObject> value)
        {
            if (modelObjects.ContainsKey(key))
            {
                MergeSort<IModelObject>.MergeSortFunction(value);
                if (value.First().Number <= modelObjects[key].Last().Number)
                    throw new Exception($"Последний элемент в коллекции {key} больше добавляемого!");
                else
                    modelObjects[key].AddRange(value);
            }
            else
                modelObjects[key].AddRange(value);
        }
/// <inheritdoc/>

        public void Add(KeyValuePair<ObjType, List<IModelObject>> item)
        {
            modelObjects[item.Key].AddRange(item.Value);
        }

        /// <inheritdoc/>

        public void Add(IModelObject addObj)
        {
            var objType = addObj.ObjType;

            if (modelObjects.ContainsKey(objType))
                if (addObj.Number <= modelObjects[objType].Last().Number)
                    throw new Exception($"Последний элемент в коллекции {objType} больше добавляемого!");
                else
                    modelObjects[objType].Add(addObj);
            else
                modelObjects.Add(objType, new List<IModelObject>() { addObj });         
        }
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

        public void CopyTo(KeyValuePair<ObjType, List<IModelObject>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
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

        public bool Remove(KeyValuePair<ObjType, List<IModelObject>> item)
        {
            throw new NotImplementedException();
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

        public void AddRange(IEnumerable<IModelObject> addObjs)
        {

            var objsList = addObjs.ToList();
            MergeSort<IModelObject>.MergeSortFunction(objsList);

            var objType = objsList.First().ObjType;

            if (modelObjects.ContainsKey(objType))
                if (objsList.First().Number <= modelObjects[objType].Last().Number)
                    throw new Exception($"Первый элемент в добавляемой коллекции {objType} больше существующего последнего!");
                else
                    modelObjects[objType].AddRange(objsList);
            else
                modelObjects.Add(objType, objsList);


        }
    }
}