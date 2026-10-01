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
            var before = CaptureData();
            var changed = TaskData.TaskType != taskType;
            TaskData.TaskType = taskType;
            TaskData.Clear();
            if (changed)
                Publish(new ProjectChange(ProjectChangeKind.PropertyChanged, ProjectProperty.TaskType));
            PublishDataDifference(before);
        }
    }
}
