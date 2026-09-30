using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Project.Interfaces.Tasks;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет сведения о текущем сеансе проекта и управляет его состоянием.
    /// </summary>
    public interface IProjectSession
    {
        IModelView ModelView { get; }
        bool IsGeometryInitialized { get; }
        FunctionDBData FunctionsDB { get; set; }
        MaterialDBData MaterialsDB { get; set; }
        TaskKind TaskKind { get; }
        TaskType TaskType { get; }
        string Name { get; set; }
        void CreateProject(string projectFile);
        void CreateTask();
        void CreateTask(MaterialDBData materials, FunctionDBData functions);
        void ChangeTaskType(TaskType taskType);
        void ChangeTaskKind(TaskKind taskKind);
        void ClearAllData();
        void UnsubMessasge();
    }
}
