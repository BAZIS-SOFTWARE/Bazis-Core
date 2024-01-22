using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ModelInterfaces
{
    /// <summary>
    /// IGroupData
    /// </summary>
    public interface IGroupData: IList<IGroup>, IComparable<IGroup>
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
    }
}