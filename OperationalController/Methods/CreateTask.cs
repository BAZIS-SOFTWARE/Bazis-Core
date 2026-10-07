using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Project.Interfaces.Tasks;
using Project.Tasks;
using System;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>

        public void CreateTask()
        {
            using (BeginChanges())
            {
                var data = new TaskData();
                ReplaceTaskData(data);
            }
        }

        /// <summary>
        /// Создаёт данные задачи с заданными базами материалов и функций.
        /// </summary>
        public void CreateTask(MaterialDBData materials, FunctionDBData functions)
        {
            using (BeginChanges())
            {
                var data = new TaskData(materials, functions);
                ReplaceTaskData(data);
            }
        }

        /// <summary>
        /// Заменяет данные задачи и публикует изменения условий и свойств.
        /// </summary>
        private void ReplaceTaskData(ITaskData data)
        {
            if (!hasProject)
                throw new InvalidOperationException("Project is not open.");
            var oldKind = TaskData.TaskKind;
            var oldType = TaskData.TaskType;
            var oldMaterials = TaskData.Materials;
            var oldFunctions = TaskData.Functions;

            ClearTaskData();
            project.TaskData = data;
            if (oldKind != TaskData.TaskKind)
                RegisterChange(new ProjectChange(ProjectChange.ProjectChangeKind.PropertyChanged, ProjectChange.ProjectProperty.TaskKind));
            if (oldType != TaskData.TaskType)
                RegisterChange(new ProjectChange(ProjectChange.ProjectChangeKind.PropertyChanged, ProjectChange.ProjectProperty.TaskType));
            if (!ReferenceEquals(oldMaterials, TaskData.Materials))
                RegisterChange(new ProjectChange(ProjectChange.ProjectChangeKind.PropertyChanged, ProjectChange.ProjectProperty.MaterialsDB));
            if (!ReferenceEquals(oldFunctions, TaskData.Functions))
                RegisterChange(new ProjectChange(ProjectChange.ProjectChangeKind.PropertyChanged, ProjectChange.ProjectProperty.FunctionsDB));
        }
    }
}
