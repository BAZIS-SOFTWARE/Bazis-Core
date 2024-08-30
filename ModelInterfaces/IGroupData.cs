using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ModelInterfaces
{
    /// <summary>
    /// IGroupData
    /// </summary>
    public interface IGroupData: IList<IGroup>
    {
        /// <summary>
        /// ClearNotExisted
        /// </summary>
        void ClearNotExisted();
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IGroup Find(int index);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="grName"></param>
        /// <returns></returns>
        IGroup Find(string grName);
        /// <summary>
        /// FindMany
        /// </summary>
        /// <param name="objType"></param>
        /// <returns></returns>
        IEnumerable<IGroup> FindMany(ObjType objType);
        /// <summary>
        /// Sort
        /// </summary>
        void Sort();
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objType"></param>
        /// <returns></returns>
        IGroup Create(string name, ObjType objType);

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="modelObjects"></param>
        /// <returns></returns>
        IGroup Create(IEnumerable<IModelObject> modelObjects);
    }
}