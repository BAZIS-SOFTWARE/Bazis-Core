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
            var data = new TaskData();
            ReplaceTaskData(data);
        }

        /// <summary>
        /// Создаёт данные задачи с заданными базами материалов и функций.
        /// </summary>
        public void CreateTask(MaterialDBData materials, FunctionDBData functions)
        {
            var data = new TaskData(materials, functions);
            ReplaceTaskData(data);
        }

        /// <summary>
        /// Заменяет данные задачи и публикует изменения условий и свойств.
        /// </summary>
        private void ReplaceTaskData(ITaskData data)
        {
            if (!hasProject)
                throw new InvalidOperationException("Project is not open.");
            var before = CaptureData();
            var oldKind = TaskData.TaskKind;
            var oldType = TaskData.TaskType;
            var oldMaterials = TaskData.Materials;
            var oldFunctions = TaskData.Functions;

            project.TaskData = data;
            PublishDataDifference(before);
            if (oldKind != TaskData.TaskKind)
                Publish(new ProjectChange(ProjectChangeKind.PropertyChanged, ProjectProperty.TaskKind));
            if (oldType != TaskData.TaskType)
                Publish(new ProjectChange(ProjectChangeKind.PropertyChanged, ProjectProperty.TaskType));
            if (!ReferenceEquals(oldMaterials, TaskData.Materials))
                Publish(new ProjectChange(ProjectChangeKind.PropertyChanged, ProjectProperty.MaterialsDB));
            if (!ReferenceEquals(oldFunctions, TaskData.Functions))
                Publish(new ProjectChange(ProjectChangeKind.PropertyChanged, ProjectProperty.FunctionsDB));
        }
    }
}
