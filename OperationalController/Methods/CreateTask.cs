using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Model.IO;
using Model.IO.STL;
using Project;
using Project.Tasks;
using System;
using System.IO;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>

        public void CreateTask()
        {
            if (project == null)
                throw new Exception("Создайте проект");
            project.TaskData = new TaskData();
        }

        public void CreateTask(MaterialDBData materials, FunctionDBData functions)
        {
            if (project == null)
                throw new Exception("Создайте проект");           

            project.TaskData = new TaskData(materials, functions);
        }
    }
}
