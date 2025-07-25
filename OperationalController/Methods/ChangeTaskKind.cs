using Project.Interfaces.Tasks;
using System;
using System.IO;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>
        /// <inheritdoc/>
        public void ChangeTaskKind(TaskKind taskKind)
        {
            GeneralData.TaskKind = taskKind;
            TaskData.Clear();
        }
    }
}
