using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    /// <summary>
    /// IObjectsCollection
    /// </summary>
    public interface IObjectsCollection<T> : IEnumerable<T>
    {
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="addObj"></param>
        void Add(T addObj);

        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="addObjs"></param>
        void AddRange(IEnumerable<T> addObjs);
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
        /// Count
        /// </summary>
        int Count { get; }
        /// <summary>
        /// ClearNotExisted
        /// </summary>
        void ClearNotExisted();
    }
}
