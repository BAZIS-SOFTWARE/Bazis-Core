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

        public void Add(string collName, T addObj)
        {
            if (!objects.ContainsKey(collName))
                objects.Add(collName, new ObjectsSet<T>(collName, addObj.ObjType));

            objects[collName].Add(addObj.Number, addObj);
        }
        /// <inheritdoc/>
        public void AddRange(string collName, IEnumerable<T> addObjs)
        {
            if (!objects.ContainsKey(collName))
                objects.Add(collName, new ObjectsSet<T>(collName, addObjs.First().ObjType));

            var dic = addObjs.ToDictionary(x => x.Number);

            foreach (var item in dic)
                objects[collName].Add(item);       
        }
/// <inheritdoc/>


        public override string ToString()
        {
            return objects.Count.ToString();
        }

        /// <inheritdoc/>

        public void Clear()
        {
            objects.Clear();
        }
/// <inheritdoc/>

        public void ClearEmpty()
        {
            objects = objects.Where(x => x.Value.Count != 0).
    ToDictionary(x => x.Key, y => y.Value);
        }
/// <inheritdoc/>

        public bool ContainsName(string collName)
        {
            return objects.ContainsKey(collName) ? true : false;
        }
/// <inheritdoc/>

        public bool ContainsNumber(int objNumbr)
        {
            foreach (var item in objects)
            {
                if (item.Value.ContainsKey(objNumbr))
                    return true;
            }
            return false;
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

        public void Remove(bool state)
        {
            foreach (var item in objects)
            {
                var filtObjs = item.Value.Where(y => y.Value.ExistState != state).ToList();

                objects[item.Key].Clear();

                foreach (var filtObj in filtObjs)
                    objects[item.Key].Add(filtObj);
            }
        }
/// <inheritdoc/>

        public bool Remove(string name)
        {
            return objects.Remove(name);
        }
    }
}
