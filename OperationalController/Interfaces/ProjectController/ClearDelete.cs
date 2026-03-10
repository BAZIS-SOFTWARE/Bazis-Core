using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// ClearGroupData
        /// </summary>
        void ClearGroupData();
        /// <summary>
        /// ClearTaskData
        /// </summary>
        void ClearTaskData();
        /// <summary>
        /// ClearAllData
        /// </summary>
        void ClearAllData();
        /// <summary>
        /// ClearNotExistedTaskData
        /// </summary>
        /// <param name="group"></param>
        void DeleteCond(IGroup group);
        /// <summary>
        /// ClearNotExistedTaskData
        /// </summary>
        void ClearNotExistedCondData();
        /// <summary>
        /// ClearNotExistedModelData
        /// </summary>
        void ClearNotExistedModelData();
        /// <summary>
        /// ClearNotExistedGroupData
        /// </summary>
        void ClearNotExistedGroupData();
        /// <summary>
        /// ClearEmptySet
        /// </summary>
        void ClearEmptySet();
    }
}
