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
        Dictionary<string,List<IModelObject>> modelObjects;
        /// <summary>
        /// this
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public IModelObject this[string objType, int index]
        {
            get { return modelObjects[type][index]; }
            set { modelObjects[type][index] = value; }
        }
        /// <summary>
        /// ObjectsData
        /// </summary>
        public ObjectsData()
        {
            modelObjects = new Dictionary<string, List<IModelObject>>();
        }
/// <inheritdoc/>

        public bool IsReadOnly { get { return false; } }
/// <inheritdoc/>

        public void Clear()
        {
            modelObjects.Clear();
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> FindMany(string objType, ObjKind objKind)
        {
            foreach (var modelObject in modelObjects[objType])
            {
                if (modelObject.ObjKind == objKind)
                    yield return modelObject;
            }
        }
        /// <inheritdoc/>

        public IEnumerable<IModelObject> FindMany(string objType)
        {
            //var list = new List<ModelObject>();

            if (objType == "Элементы")
            {
                foreach (var modelObject in modelObjects)
                {
                    if (modelObject.Key == "Элементы2D" |
                        modelObject.Key == "Элементы3D" |
                        modelObject.Key == "Элементы1D")
                        foreach (var obj in modelObject.Value)
                            yield return obj;
                }
            }

            else
            {
                foreach (var obj in modelObjects[objType])
                    yield return obj;
            }
        }


        public IEnumerable<T> FindMany<T>() where T : IModelObject
        {
            foreach (var modelObject in modelObjects)
                foreach (var obj in modelObject.Value)
                    if (obj is T)
                        yield return (T)obj;
        }

        public int GetLastObjNumber(string objType)
        {
            return modelObjects[objType].Last().Number;
        }

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

        public void ClearRemoved()
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

        public void Remove(string objType)
        {
            if (modelObjects.Count == 0)
                return;

            modelObjects.Remove(objType);
        }
        /// <inheritdoc/>

        public void RemoveAt(string objType,int objInd)
        {
            modelObjects[objType].RemoveAt(objInd);
        }
        /// <inheritdoc/>

        public IModelObject Find(int objNumbr)
        {
            foreach (var item in modelObjects)
            {
                return item.Value.Find(objNumbr);
            }
            return null;
        }
        /// <inheritdoc/>

        public void Sort(string objType)
        {
            MergeSort<IModelObject>.MergeSortFunction(modelObjects[objType]);
        }
        public List<IModelObject> this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<string> Keys { get { return modelObjects.Keys; } }

        public ICollection<List<IModelObject>> Values => throw new NotImplementedException();

        public void Add(string key, List<IModelObject> value)
        {
            if (modelObjects.ContainsKey(key))
                modelObjects[key].AddRange(value);
            else modelObjects.Add(key, value);
        }

        public void Add(KeyValuePair<string, List<IModelObject>> item)
        {
            if (modelObjects.ContainsKey(item.Key))
                modelObjects[item.Key].AddRange(item.Value);
            else modelObjects.Append(item);
        }

        public void AddRange(IEnumerable<IModelObject> addObjects)
        {
            if(modelObjects.ContainsKey(addObjects.First().ObjType))
                modelObjects[addObjects.First().ObjType].AddRange(addObjects);
        }

        public void Add(IModelObject addObj)
        {
            modelObjects[addObj.ObjType].Add(addObj);
        }

        public bool Contains(KeyValuePair<string, List<IModelObject>> item)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, List<IModelObject>>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, List<IModelObject>>> GetEnumerator()
        {
            throw new NotImplementedException();
        }


        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, List<IModelObject>> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out List<IModelObject> value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IModelObject> FindMany(ObjKind objKind)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetObjectTypes()
        {
            throw new NotImplementedException();
        }
    }
}