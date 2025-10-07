using Model.ObjectsCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.ObjectsCollections
{
    /// <summary>
    /// IObjectsCollection
    /// </summary>
    public interface IObjectsCollection<T> : IEnumerable<KeyValuePair<string, ObjectsSet<T>>>
         where T : IModelObject
    {
        /// <summary>
        /// this
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        ObjectsSet<T> this[int index] { get; }
        /// <summary>
        /// this
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ObjectsSet<T> this[string name] { get; }
        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="objNumber"></param>
        void Remove(int objNumber);
        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="name"></param>
        bool Remove(string name);
        /// <summary>
        /// Count
        /// </summary>
        int Count { get; }
        /// <summary>
        /// GetMaxNumber
        /// </summary>
        /// <returns></returns>
        int GetMaxNumber();
        /// <summary>
        /// GetMinNumber
        /// </summary>
        /// <returns></returns>
        int GetMinNumber();
        /// <summary>
        /// Append
        /// </summary>
        /// <param name="collection"></param>
        void Append(IObjectsCollection<T> collection);

        /// <summary>
        /// Add set as object
        /// </summary>
        /// <param name="set"></param>
        void Add(ObjectsSet<T> set);

        /// <summary>
        /// Add set use name
        /// </summary>
        /// <param name="setName"></param>
        void Add(string setName);

        /// <summary>
        /// Add set and object
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="addObj"></param>
        void Add(string setName, T addObj);

        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="addObjs"></param>
        void AddRange(string setName, IEnumerable<T> addObjs);
        /// <summary>
        /// Clear
        /// </summary>
        void Clear();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        T Find(int objNumbr);
        /// <summary>
        /// SetsNames
        /// </summary>
        ICollection<string> Names { get; }
        /// <summary>
        /// SetsObjects
        /// </summary>
        ICollection<ObjectsSet<T>> Objects { get; }
        /// <summary>
        /// GetObjects
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetObjects();
        /// <summary>
        /// ClearNotExisted
        /// </summary>
        void RemoveNotExistedObjects();
        /// <summary>
        /// ClearEmpty
        /// </summary>
        void ClearEmptySets();
        /// <summary>
        /// ContainsNumber
        /// </summary>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        bool ContainsNumber(int objNumbr);
        /// <summary>
        /// ContainsName
        /// </summary>
        /// <param name="setName"></param>
        /// <returns></returns>
        bool ContainsName(string setName);
        /// <summary>
        /// ChangeName
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        void ChangeName(string oldName, string newName);
    }
}
