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
        /// <param name="key"></param>
        /// <returns></returns>
        ObjectsSet<T> this[string key] { get; }
        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="state"></param>
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
        /// <param name="collName"></param>
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
        void ClearNotExisted();
        /// <summary>
        /// ClearEmpty
        /// </summary>
        void ClearEmpty();
        /// <summary>
        /// ContainsNumber
        /// </summary>
        /// <param name="objNumbr"></param>
        /// <returns></returns>
        bool ContainsNumber(int objNumbr);
        /// <summary>
        /// ContainsName
        /// </summary>
        /// <param name="collName"></param>
        /// <returns></returns>
        bool ContainsName(string setName);
    }
}
