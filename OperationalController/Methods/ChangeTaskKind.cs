using Project.Interfaces.Tasks;
using System;
using System.IO;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void ChangeTaskKind(TaskKind taskKind)
        {
            var before = CaptureData();
            var changed = TaskData.TaskKind != taskKind;
            TaskData.TaskKind = taskKind;
            TaskData.Clear();
            if (changed)
                Publish(new ProjectChange(ProjectChangeKind.PropertyChanged, ProjectProperty.TaskKind));
            PublishDataDifference(before);
        }
    }
}
