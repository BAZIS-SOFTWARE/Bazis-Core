using MaterialDB.Compatibility;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MaterialDB.FunctionData
{
    /// <summary>
    /// База табличных функций.
    /// </summary>
    [Serializable]
    public class FunctionDBData : IDictionary<string, FunctionItem>
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        Dictionary<string, FunctionItem> dic = new Dictionary<string, FunctionItem>();
        /// <inheritdoc/>
        public FunctionItem this[string key] { get { return dic[key]; } set { dic[key] = value; } }
        /// <inheritdoc/>

        public ICollection<string> Keys { get { return dic.Keys; } }
        /// <inheritdoc/>
        public ICollection<FunctionItem> Values { get { return dic.Values; } }
        /// <inheritdoc/>
        public int Count { get { return dic.Count; } }
        /// <inheritdoc/>
        public bool IsReadOnly { get { return false; } }
        /// <inheritdoc/>
        public void Add(string key, FunctionItem value)
        {
            dic.Add(key, value);
        }
        /// <inheritdoc/>
        public void Add(KeyValuePair<string, FunctionItem> item)
        {
            dic.Add(item.Key, item.Value);
        }
        /// <inheritdoc/>
        public void Clear()
        {
            dic.Clear();
        }
        /// <inheritdoc/>
        public bool Contains(KeyValuePair<string, FunctionItem> item)
        {
            return dic.Contains(item);
        }
        /// <inheritdoc/>
        public bool ContainsKey(string key)
        {
            return dic.ContainsKey(key);
        }
        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<string, FunctionItem>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, FunctionItem>> GetEnumerator()
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
        public bool Remove(KeyValuePair<string, FunctionItem> item)
        {
            return dic.Remove(item.Key);
        }
        /// <inheritdoc/>
        public bool TryGetValue(string key, out FunctionItem value)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dic.GetEnumerator();
        }

        public FunctionDBData(string dbName, string dbFolder) : this()
        {

            var filePath = FindFileByPath(dbFolder, dbName);
            if (filePath == null)
            {
                throw new Exception($"Не найдена база {dbName} в папке {dbFolder}");
            }

            else
            {
                var settingsSerializer = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                    Formatting = Formatting.Indented,
                };

                var db = JsonConvert.DeserializeObject<FunctionDBData>
   (File.ReadAllText(filePath), settingsSerializer);

                Name = dbName;
                dic = db.dic;
            }

        }

        public FunctionDBData()
        {
            Name = "newFunctionsDb";
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            var resolver = new ArgumentKindResolver();
            resolver.Resolve(this);
        }

        private string? FindFileByPath(string folder, string fileName)
        {
            var projFiles = Directory.GetFiles(folder, fileName, SearchOption.TopDirectoryOnly);
            if (projFiles.Count() > 0)
            {
                return projFiles[0];
            }

            return null;
        }
    }
}
