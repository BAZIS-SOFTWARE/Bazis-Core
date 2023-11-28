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

        public IEnumerable<IModelObject> FindMany(ObjKind objKind)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IModelObject> Find(int objNumbr)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> FindMany(ObjType objType, ObjKind objKind)
        {
            foreach (var modelObject in modelObjects[objType])
            {
                if (modelObject.ObjKind == objKind)
                    yield return modelObject;
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

        public int GetLastObjNumber(ObjType objType)
        {
            return modelObjects[objType].Last().Number;
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

        public bool Remove(IModelObject modelObject)
        {
            foreach (var item in modelObjects)
            {
                if (item.Value.Remove(modelObject))
                    return true;
            }
            return false;
        }
        /// <inheritdoc/>

        public void ClearNotExisted()
        {
            foreach (var item in modelObjects)
            {
                var removedObjs = item.Value.Where(x => x.ExistState == false);

                var existObjects = item.Value.Except(removedObjs, new EqualityComparerByExistState()).ToList();

                item.Value.Clear();
                item.Value.AddRange(existObjects);
            }

        }
        /// <inheritdoc/>

        public bool Remove(string objType)
        {
            return modelObjects.Remove(objType);
        }
        /// <inheritdoc/>

        public void RemoveAt(string objType,int objInd)
        {
            modelObjects[objType].RemoveAt(objInd);
        }

/// <inheritdoc/>

        public IModelObject Find(string objType, int objNumbr)
        {
            return modelObjects[objType].Find(objNumbr);
        }
        /// <inheritdoc/>

        public void Sort(string objType)
        {
            MergeSort<IModelObject>.MergeSortFunction(modelObjects[objType]);
        }    
/// <inheritdoc/>

        public ICollection<string> Keys { get { return modelObjects.Keys; } }
/// <inheritdoc/>

        public ICollection<List<IModelObject>> Values { get { return modelObjects.Values; } }
/// <inheritdoc/>

        public void Add(string key, List<IModelObject> value)
        {
            modelObjects[key].AddRange(value);
        }
/// <inheritdoc/>

        public void Add(KeyValuePair<string, List<IModelObject>> item)
        {
            modelObjects[item.Key].AddRange(item.Value);
        }

        /// <inheritdoc/>

        public void Add(IModelObject addObj)
        {
            modelObjects[addObj.ObjType].Add(addObj);
        }
/// <inheritdoc/>

        public bool Contains(KeyValuePair<string, List<IModelObject>> item)
        {
            return modelObjects.Contains(item);
        }
/// <inheritdoc/>

        public bool ContainsKey(string objType)
        {
            return modelObjects.ContainsKey(objType);
        }

        public void CopyTo(KeyValuePair<string, List<IModelObject>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, List<IModelObject>>> GetEnumerator()
        {
            foreach (var objItem in modelObjects)
            {
                yield return objItem;
            }
        }
/// <inheritdoc/>

        public bool Remove(KeyValuePair<string, List<IModelObject>> item)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public bool TryGetValue(string key, out List<IModelObject> value)
        {
            return modelObjects.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}