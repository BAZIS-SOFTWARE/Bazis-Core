using MaterialDB.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// PropertyData
    /// </summary>
    [Serializable]
    public class PropertyData : IDictionary<string, PropertyItem>
    {
        Dictionary<string, PropertyItem> dic = new Dictionary<string, PropertyItem>();
        /// <inheritdoc/>
        public PropertyItem this[string key] { get { return dic[key]; } set { dic[key] = value; } }
/// <inheritdoc/>

        public ICollection<string> Keys { get { return dic.Keys; } }
        /// <inheritdoc/>
        public ICollection<PropertyItem> Values { get { return dic.Values; } }
        /// <inheritdoc/>
        public int Count { get { return dic.Count; } }
        /// <inheritdoc/>
        public bool IsReadOnly { get { return false; } }
        /// <inheritdoc/>
        public void Add(string key, PropertyItem value)
        {
            dic.Add(key, value);
        }
        /// <inheritdoc/>
        public void Add(KeyValuePair<string, PropertyItem> item)
        {
            dic.Add(item.Key, item.Value);
        }
        /// <inheritdoc/>
        public void Clear()
        {
            dic.Clear();
        }
        /// <inheritdoc/>
        public bool Contains(KeyValuePair<string, PropertyItem> item)
        {
            return dic.Contains(item);
        }
        /// <inheritdoc/>
        public bool ContainsKey(string key)
        {
            return dic.ContainsKey(key);
        }
        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<string, PropertyItem>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, PropertyItem>> GetEnumerator()
        {
            foreach (var item in dic)
            {
                yield return item;
            }
        }
        /// <inheritdoc/>
        public bool Remove(string key)
        {
            return dic.Remove(key);
        }
        /// <inheritdoc/>
        public bool Remove(KeyValuePair<string, PropertyItem> item)
        {
            return dic.Remove(item.Key);
        }
        /// <inheritdoc/>
        public bool TryGetValue(string key, out PropertyItem value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dic.GetEnumerator();
        }
    }
}
