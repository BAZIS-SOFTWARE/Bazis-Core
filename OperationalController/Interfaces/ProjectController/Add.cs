using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// AddTaskData
        /// </summary>
        /// <param name="cond"></param>
        void AddTaskData(ICondData cond);


    }
}
