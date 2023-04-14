using Functions.Sort;
using Model.Comparers;
using Model.Interfaces;
using Model.ObjectsComparers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class ObjectsData : IList<ModelObject>
    {
        List<ModelObject> modelObjects;

        public ModelObject this[int index]
        {
            get { return modelObjects[index]; }
            set { modelObjects[index] = value; }
        }

        public ObjectsData()
        {
            modelObjects = new List<ModelObject>();
        }

        public bool IsReadOnly => throw new System.NotImplementedException();

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

        public int NumberOfSurfaceElements
        {
            get { return FindMany<Element2D>().Count(); }
        }

        public int NumberOfElements
        {
            get { return FindMany<Element>().Count(); }
        }

        public void HideModelObjs<T>(List<int> objNumbers) where T : ModelObject
        {
            foreach (var objNumber in objNumbers)
            {
                var obj = Find(modelObjects, objNumber);
                obj.ViewState = false;
            }
        }

        public void HideModelObjs<T>(T[] objects) where T : ModelObject
        {
            foreach (var obj in objects)
            {
                obj.ViewState = false;
            }
        }

        public void HideModelObj<T>(T objects) where T : ModelObject
        {
            objects.ViewState = false;
        }

        public void ShowModelObjs<T>(T[] objects, List<int> objNumbers) where T : ModelObject
        {
            foreach (var objNumber in objNumbers)
            {
                Find(modelObjects, objNumber).ViewState = true;
            }
        }

        public IEnumerable<ModelObject> FindMany(ObjKind objKind)
        {
            return modelObjects.Where(x => x.ObjKind == objKind);
        }

        public IEnumerable<ModelObject> FindMany(string objType)
        {
            //var list = new List<ModelObject>();

            if (objType == "Элементы")
            {
                return modelObjects.Where(x =>
                x.ObjType == "Элементы2D" |
                x.ObjType == "Элементы3D" |
                x.ObjType == "Элементы1D");
            }

            else
            {
                return modelObjects.Where(x => x.ObjType == objType);
            }
        }

        public T Find<T>(List<T> objs, int objNumbr) where T : ModelObject
        {
            if (objs.Count - 1 == -1) { return null; }
            var left = 0;
            var right = objs.Count - 1;
            while (left < right)
            {
                var middle = (right + left) / 2;
                if (objNumbr <= objs[middle].Number)
                    right = middle;
                else left = middle + 1;
            }
            if (objs[right].Number == objNumbr)
                return objs[right];
            return null;
        }

        public ModelObject Find(int objNumbr)
        {
            if (modelObjects.Count - 1 == -1) { return null; }
            var left = 0;
            var right = modelObjects.Count - 1;
            while (left < right)
            {
                var middle = (right + left) / 2;
                if (objNumbr <= modelObjects[middle].Number)
                    right = middle;
                else left = middle + 1;
            }
            if (modelObjects[right].Number == objNumbr)
                return modelObjects[right];
            return null;
        }

        public bool Remove(ModelObject modelObject)
        {
            return modelObjects.Remove(modelObject);
        }

        public void RemoveRange(string objType,IEnumerable<int> selectedObjects)
        {
            var tempSelectedObjects = new ModelObject[selectedObjects.Count()];
            var counter = 0;
            foreach (var selectedObject in selectedObjects)
            {
                var obj = Find(selectedObject);
                obj.ExistState = false;
                tempSelectedObjects[counter] = obj;
                counter++;
            }

            var existObjects = modelObjects.Except(tempSelectedObjects, new EqualityComparerByExistState()).ToList();
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

        public void RemoveAt(int modelObjectInd)
        {
            modelObjects.RemoveAt(modelObjectInd);
        }

        public T Find<T>(T[] objs, int objNumbr) where T : ModelObject
        {
            if (objs.Length - 1 == -1) { return null; }
            var left = 0;
            var right = objs.Length - 1;
            while (left < right)
            {
                var middle = (right + left) / 2;
                if (objNumbr <= objs[middle].Number)
                    right = middle;
                else left = middle + 1;
            }
            if (objs[right].Number == objNumbr)
                return objs[right];
            return null;
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