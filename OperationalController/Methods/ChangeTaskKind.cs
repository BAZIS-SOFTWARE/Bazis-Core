using Project.Interfaces.Tasks;
using System;
using System.IO;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>
        public void ChangeTaskKind(TaskKind taskKind)
        {
            TaskData.TaskKind = taskKind;
            TaskData.Clear();
        }
    }
}
