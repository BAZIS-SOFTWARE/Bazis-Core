using Project.Interfaces.Tasks;
using System;
using System.IO;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>
        public void ChangeTaskType(TaskType taskType)
        {
            TaskData.TaskType = taskType;
            TaskData.Clear();
        }
    }
}
