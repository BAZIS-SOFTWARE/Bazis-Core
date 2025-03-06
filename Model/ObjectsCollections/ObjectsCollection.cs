using Model.Interfaces.MeshObjects;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using MathNet.Numerics;
using System.Xml.Linq;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;

namespace Model.ObjectsCollections
{
    /// <summary>
    /// ObjectsCollection. Класс содержащий наборы объектов.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectsCollection<T> : IObjectsCollection<T> where T : IModelObject
    {

        public ObjectsSet<T> this[string key]
        {
            get { return objects[key]; }
        }

/// <inheritdoc/>

        public ICollection<string> Names => objects.Keys;
/// <inheritdoc/>

        public ICollection<ObjectsSet<T>> Objects => objects.Values;
/// <inheritdoc/>

        public int Count => objects.Values.Sum(x => x.Count);

        /// <inheritdoc/> 

        Dictionary<string, ObjectsSet<T>> objects = new Dictionary<string, ObjectsSet<T>>();
        /// <inheritdoc/>

        public void Add(string setName)
        {
            objects.Add(setName, new ObjectsSet<T>(setName));
        }

        public void Add(ObjectsSet<T> set)
        {
            objects.Add(set.Name, set);
        }

        /// <inheritdoc/>

        public void Add(string setName, T addObj)
        {
            if (!objects.ContainsKey(setName))
                objects.Add(setName, new ObjectsSet<T>(setName));
            if (!ContainsNumber(addObj.Number))
                objects[setName].Add(addObj.Number, addObj);
            else 
                throw new Exception("Объект с таким номером уже добавлен в коллекцию!");
        }
        /// <inheritdoc/>
        public void AddRange(string setName, IEnumerable<T> addObjs)
        {
            if (!objects.ContainsKey(setName))
                objects.Add(setName, new ObjectsSet<T>(setName));

            foreach (var addObj in addObjs)
            {
                if (!ContainsNumber(addObj.Number))
                    objects[setName].Add(addObj.Number, addObj);
                else throw new Exception("Объект с таким номером уже добавлен в коллекцию!");
            }    
        }
/// <inheritdoc/>


        public override string ToString()
        {
            return objects.Count.ToString();
        }

        /// <inheritdoc/>

        public void Clear()
        {
            foreach (var item in objects)
                item.Value.Clear();
            objects.Clear();
        }
/// <inheritdoc/>

        public void ClearEmpty()
        {
            objects = objects.Where(x => x.Value.Count != 0).
    ToDictionary(x => x.Key, y => y.Value);
        }
/// <inheritdoc/>

        public void ClearNotExisted()
        {
            foreach (var item in objects)
                item.Value.ClearNotExisted();
        }


/// <inheritdoc/>

public bool ContainsName(string collName)
        {
            return objects.ContainsKey(collName);
        }
/// <inheritdoc/>

        public bool ContainsNumber(int objNumbr)
        {
            return objects.Any(x => x.Value.ContainsKey(objNumbr));
        }
/// <inheritdoc/>

        public T Find(int objNumbr)
        {
            foreach (var objsSet in objects)
            {
                if (objsSet.Value.ContainsKey(objNumbr))
                    return objsSet.Value[objNumbr];
            }
            return default(T);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, ObjectsSet<T>>> IEnumerable<KeyValuePair<string, ObjectsSet<T>>>.GetEnumerator()
        {
            foreach (var item in objects)
            {
                yield return item;
            }
        }
        /// <summary>
        /// GetMaxNumber
        /// </summary>
        /// <returns></returns>
        public int GetMaxNumber()
        {
            return objects.Max(x => x.Value.Max(y => y.Key));
        }
        /// <summary>
        /// GetMinNumber
        /// </summary>
        /// <returns></returns>
        public int GetMinNumber()
        {
            return objects.Min(x => x.Value.Min(y => y.Key));
        }
/// <inheritdoc/>

        public IEnumerable<T> GetObjects()
        {
            foreach (var item in objects)
            {
                foreach (var itemValue in item.Value)
                {
                    yield return itemValue.Value;
                }
            }
        }
/// <inheritdoc/>

        public void Remove(int objNumber)
        {
            foreach (var item in objects)
            {
                item.Value[objNumber].ExistState = false;
                item.Value.Remove(objNumber);
            }
        }
/// <inheritdoc/>

        public bool Remove(string name)
        {
            foreach (var item in objects[name])
                item.Value.ExistState = false;

            return objects.Remove(name);
        }
    }
}
