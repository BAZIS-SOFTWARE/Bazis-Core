using System.Collections.Generic;

namespace ModelInterfaces
{
    public interface IGroup : IList<IModelObject>
    {
        /// <summary>
        /// ViewState
        /// </summary>
        bool ViewState { get; set; }
        /// <summary>
        /// GroupName
        /// </summary>
        string GroupName { get; }
        /// <summary>
        /// ObjType
        /// </summary>
        string ObjType { get; }
        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="items"></param>
        void AddRange(IEnumerable<IModelObject> items);
        /// <summary>
        /// SwapList
        /// </summary>
        /// <param name="firstInd"></param>
        /// <param name="secInd"></param>
        /// <returns></returns>
        bool SwapList(int firstInd, int secInd);
        /// <summary>
        /// SortByDistance
        /// </summary>
        void SortByDistance();
    }
}