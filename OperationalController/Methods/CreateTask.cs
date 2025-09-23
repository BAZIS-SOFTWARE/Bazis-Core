using Model.IO;
using Model.IO.STL;
using Project;
using Project.Tasks;
using System;
using System.IO;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>

        public void CreateTask()
        {
            if (project == null)
                throw new Exception("Создайте проект");
            project.TaskData = new TaskData();
        }
    }
}
