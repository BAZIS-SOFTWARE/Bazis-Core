using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
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
        /// <param name="groupData"></param>
        void ClearNotExistedTaskData(IGroupData groupData);
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
