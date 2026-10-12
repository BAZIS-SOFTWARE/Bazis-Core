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
            using (BeginChanges())
            {
                var changed = TaskData.TaskKind != taskKind;
                TaskData.TaskKind = taskKind;
                if (changed)
                    RegisterChange(new ProjectChange(ProjectChange.ProjectChangeKind.PropertyChanged, ProjectChange.ProjectProperty.TaskKind));
                ClearTaskData();
            }
        }
    }
}
