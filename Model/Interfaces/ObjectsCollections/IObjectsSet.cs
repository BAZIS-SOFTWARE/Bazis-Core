using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Model.Interfaces.ObjectsCollections
{
    /// <summary>
    /// IObjectsSet
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObjectsSet<T> : ISetInfo,IDictionary<int, T> where T : IModelObject
    {
        /// <summary>
        /// ClearNotExisted
        /// </summary>
        void ClearNotExisted();

    }
}
