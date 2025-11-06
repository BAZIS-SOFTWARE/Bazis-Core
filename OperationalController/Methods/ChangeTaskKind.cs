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
            TaskData.TaskKind = taskKind;
            TaskData.Clear();
        }
    }
}
