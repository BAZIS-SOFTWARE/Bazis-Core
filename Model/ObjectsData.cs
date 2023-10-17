using Functions.Sort;
using Model.Interfaces;
using Model.ObjectsComparers;
using Model.ObjectsFinders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    /// <summary>
    /// ObjectsData
    /// </summary>
    public class ObjectsData : IList<ModelObject>
    {
        List<ModelObject> modelObjects;

        public ModelObject this[int index]
        {
            get { return modelObjects[index]; }
            set { modelObjects[index] = value; }
        }
        /// <summary>
        /// ObjectsData
        /// </summary>
        public ObjectsData()
        {
            modelObjects = new List<ModelObject>();
        }

        public bool IsReadOnly { get { return false; } }

        public void Clear()
        {
            modelObjects.Clear();
        }

        public bool Contains(ModelObject item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(ModelObject[] array, int arrayIndex)
        {
            modelObjects.CopyTo(array, arrayIndex);
        }

        public void Insert(int index, ModelObject item)
        {
            throw new System.NotImplementedException();
        }


        public IEnumerable<T> FindMany<T>() where T : ModelObject
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

        public int NumberOfNodes
        {
            get { return FindMany<Node>().Count(); }
        }

        public int NumberOfVolumeElements
        {
            get { return FindMany<Element3D>().Count(); }
        }

        public int NumberOfLineElements
        {
            get { return FindMany<Element1D>().Count(); }
        }

        public int NumberOfSurfaceElements
        {
            get { return FindMany<Element2D>().Count(); }
        }

        public int NumberOfElements
        {
            get { return FindMany<Element>().Count(); }
        }       

        public IEnumerable<ModelObject> FindMany(ObjKind objKind)
        {
            foreach (var modelObject in modelObjects)
            {
                if (modelObject.ObjKind == objKind)
                    yield return modelObject;
            }
        }

        public IEnumerable<ModelObject> FindMany(string objType)
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

        public bool Remove(ModelObject modelObject)
        {
            return modelObjects.Remove(modelObject);
        }

        public void ClearRemoved()
        {
            var removedObjs = modelObjects.Where(x => x.ExistState == false);


            var existObjects = modelObjects.Except(removedObjs, new EqualityComparerByExistState()).ToList();
            modelObjects = existObjects;
        }

        public void RemoveRange(string objType)
        {
            var listTypedObjs = new List<List<ModelObject>>();
            var objTypes = GetObjectTypes();

            foreach (var type in objTypes)
            {
                if (!type.Equals(objType))
                    listTypedObjs.Add(FindMany(type).ToList());
            }

            var mergeObjsList = new List<ModelObject>();

            foreach (var typedObjs in listTypedObjs)
                mergeObjsList.AddRange(typedObjs);

            MergeSort<ModelObject>.MergeSortFunction(mergeObjsList);

            modelObjects = mergeObjsList;
        }
/// <inheritdoc/>

        public void RemoveAt(int modelObjectInd)
        {
            modelObjects.RemoveAt(modelObjectInd);
        }

        public ModelObject Find(int objNumbr)
        {
            return ByNumberFinder.Find(modelObjects, objNumbr);
        }

        public List<string> GetObjectTypes()
        {
            var objNames = new List<string>();

            foreach (var modelObject in modelObjects)
            {
                if (!objNames.Contains(modelObject.ObjType))
                    objNames.Add(modelObject.ObjType);
            }
            return objNames;
        }

        public void AddRange(IEnumerable<ModelObject> addObjects)
        {
            modelObjects.AddRange(addObjects);
        }

        public void Add(ModelObject addObj)
        {
            modelObjects.Add(addObj);
        }

        public int IndexOf(ModelObject item)
        {
            return modelObjects.IndexOf(item);
        }

        public IEnumerator<ModelObject> GetEnumerator()
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
    }
}