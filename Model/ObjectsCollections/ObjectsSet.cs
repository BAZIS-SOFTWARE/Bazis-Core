using Model.Interfaces.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.ObjectsCollections
{
    public class ObjectsSet<T>: IDictionary<int,T>
    {
        /// <summary>
        /// Имя набора
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ObjectsSet
        /// </summary>
        /// <param name="name"></param>
        public ObjectsSet(string name)
        {
            Name = name;
        }

        Dictionary<int,T> objectsSet = new Dictionary<int, T>();

        public T this[int key] { get => objectsSet[key]; set => objectsSet[key] = value; }

        public ICollection<int> Keys => objectsSet.Keys;

        public ICollection<T> Values => objectsSet.Values;
/// <inheritdoc/>

        public int Count => objectsSet.Count;
/// <inheritdoc/>

        public bool IsReadOnly => false;
/// <inheritdoc/>


        public override string ToString()
        {
            return $@"{Name} {objectsSet.Count}";
        }

        /// <inheritdoc/>

        public void Add(int key, T value)
        {
            objectsSet.Add(key, value);
        }
/// <inheritdoc/>

        public void Add(KeyValuePair<int, T> item)
        {
            objectsSet.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            objectsSet.Clear();
        }

        public bool Contains(KeyValuePair<int, T> item)
        {
            return objectsSet.Contains(item);
        }
/// <inheritdoc/>

        public bool ContainsKey(int key)
        {
            return objectsSet.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<int, T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public IEnumerator<KeyValuePair<int, T>> GetEnumerator()
        {
            foreach (var item in objectsSet)
            {
                yield return item;
            }
        }
/// <inheritdoc/>

        public bool Remove(int key)
        {
            return objectsSet.Remove(key);
        }
/// <inheritdoc/>

        public bool Remove(KeyValuePair<int, T> item)
        {
            return objectsSet.Remove(item.Key);
        }

        public bool TryGetValue(int key, out T value)
        {
            return objectsSet.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
