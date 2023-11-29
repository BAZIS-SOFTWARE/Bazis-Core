using System.Collections.Generic;

namespace ModelInterfaces
{
    /// <summary>
    /// IGroup
    /// </summary>
    public interface IGroup : IList<IModelObject>
    {
        /// <summary>
        /// ViewState
        /// </summary>
        bool ViewState { get; set; }
        /// <summary>
        /// GroupName
        /// </summary>
        string GroupName { get; set; }
        /// <summary>
        /// ObjType
        /// </summary>
        ObjType ObjType { get; }
        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="items"></param>
        void AddRange(IEnumerable<IModelObject> items);
        /// <summary>
        /// SortByDistance
        /// </summary>
        void SortByDistance();
        /// <summary>
        /// Swap objects
        /// </summary>
        /// <param name="fInd"></param>
        /// <param name="sInd"></param>
        /// <returns></returns>
        bool Swap(int fInd, int sInd);
        /// <summary>
        /// ClearRemoved
        /// </summary>
        void ClearRemoved();
    }
}