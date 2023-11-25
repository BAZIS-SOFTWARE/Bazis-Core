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
        List<IModelObject> modelObjects;

        public IModelObject this[int index]
        {
            get { return modelObjects[index]; }
            set { modelObjects[index] = value; }
        }
        /// <summary>
        /// ObjectsData
        /// </summary>
        public ObjectsData()
        {
            modelObjects = new List<IModelObject>();           
        }

        public bool IsReadOnly { get { return false; } }

        public void Clear()
        {
            modelObjects.Clear();
        }

        public bool Contains(IModelObject item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(IModelObject[] array, int arrayIndex)
        {
            modelObjects.CopyTo(array, arrayIndex);
        }

        public void Insert(int index, IModelObject item)
        {
            throw new System.NotImplementedException();
        }


        public IEnumerable<T> FindMany<T>() where T : IModelObject
        {
            //var list = new List<T>();
            foreach (var obj in modelObjects)
            {
                if (obj is T)
                    yield return (T)obj;
                //list.Add((T)obj);
            }
            //return list.ToArray();
        }

        public int GetLastObjNumber()
        {
            return modelObjects.Last().Number;
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

        public IEnumerable<IModelObject> FindMany(ObjKind objKind)
        {
            foreach (var modelObject in modelObjects)
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
                    if (modelObject.ObjType == "Элементы2D" |
                        modelObject.ObjType == "Элементы3D" |
                        modelObject.ObjType == "Элементы1D")
                        yield return modelObject;
                }
            }

            else
            {
                foreach (var modelObject in modelObjects)
                {
                    if (modelObject.ObjType == objType)
                        yield return modelObject;
                }
            }
        }       
/// <inheritdoc/>

        public bool Remove(IModelObject modelObject)
        {
            return modelObjects.Remove(modelObject);
        }
/// <inheritdoc/>

        public void ClearRemoved()
        {
            var removedObjs = modelObjects.Where(x => x.ExistState == false);

            var existObjects = modelObjects.Except(removedObjs, new EqualityComparerByExistState()).ToList();
            modelObjects = existObjects;
        }
/// <inheritdoc/>

        public void RemoveRange(string objType)
        {
            if (modelObjects.Count == 0)
                return;

            var existedObjs = new List<IModelObject>();
            var objTypes = GetObjectTypes();

            foreach (var type in objTypes)
            {
                if (!type.Equals(objType))
                    existedObjs.AddRange(FindMany(type));
            }

            modelObjects = existedObjs;
        }
/// <inheritdoc/>

        public void RemoveAt(int modelObjectInd)
        {
            modelObjects.RemoveAt(modelObjectInd);
        }
/// <inheritdoc/>

        public IModelObject Find(int objNumbr)
        {
            return modelObjects.Find(objNumbr);
        }
/// <inheritdoc/>

        public IEnumerable<string> GetObjectTypes()
        {
            HashSet<string> objsNames = new HashSet<string>();

            foreach (var modelObject in modelObjects)
            {
                if (!objsNames.Contains(modelObject.ObjType))
                {
                    objsNames.Add(modelObject.ObjType);
                    yield return modelObject.ObjType;
                }

            }
        }

        public void AddRange(IEnumerable<IModelObject> addObjects)
        {
            modelObjects.AddRange(addObjects);
        }

        public void Add(IModelObject addObj)
        {
            modelObjects.Add(addObj);
        }

        public int IndexOf(IModelObject item)
        {
            return modelObjects.IndexOf(item);
        }

        public IEnumerator<IModelObject> GetEnumerator()
        {
            foreach (var modelObject in modelObjects)
            {
                yield return modelObject;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public void Sort()
        {
            MergeSort<IModelObject>.MergeSortFunction(modelObjects);
        }
    }
}