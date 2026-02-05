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
        /// <param name="group"></param>
        void ClearNotExistedTaskData(IGroup group);
        /// <summary>
        /// ClearNotExistedTaskData
        /// </summary>
        /// <param name="groups"></param>
        void ClearNotExistedTaskData(IEnumerable<IGroup> groups);
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
