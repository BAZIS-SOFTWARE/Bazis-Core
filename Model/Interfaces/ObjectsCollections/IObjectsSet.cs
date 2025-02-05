using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Interfaces.ObjectsCollections
{
    /// <summary>
    /// IObjectsSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectsSet<T> : IDictionary<int, T> where T : IModelObject
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// ObjType
        /// </summary>
        ObjType ObjType { get; }
    }
}
