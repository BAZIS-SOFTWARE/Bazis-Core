using Project.Interfaces.Tasks;
using System;
using System.IO;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void ChangeTaskType(TaskType taskType)
        {
            TaskData.TaskType = taskType;
            TaskData.Clear();
        }
    }
}
