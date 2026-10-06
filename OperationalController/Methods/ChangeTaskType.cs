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
            var changed = TaskData.TaskType != taskType;
            TaskData.TaskType = taskType;
            if (changed)
                Publish(new ProjectChange(ProjectChange.ProjectChangeKind.PropertyChanged, ProjectChange.ProjectProperty.TaskType));
            ClearTaskData();
        }
    }
}
