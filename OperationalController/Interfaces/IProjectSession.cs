using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Project.Interfaces.Tasks;
using System;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет сведения о текущем сеансе проекта и управляет его состоянием.
    /// </summary>
    public interface IProjectSession
    {
        bool HasProject { get; }
        string FilePath { get; }
        bool IsModified { get; }
        bool IsGeometryInitialized { get; }
        FunctionDBData FunctionsDB { get; set; }
        MaterialDBData MaterialsDB { get; set; }
        TaskKind TaskKind { get; }
        TaskType TaskType { get; }
        string Name { get; }
        /// <summary>Изменяет имя текущего проекта.</summary>
        void ChangeProjectName(string name);
        /// <summary>Открывает проект или сеточную модель по расширению файла.</summary>
        void Open(string path, IProgress<int> progress = null);
        /// <summary>Читает сеточную модель и добавляет её в текущий проект.</summary>
        void Append(string path, IProgress<int> progress = null);
        /// <summary>Импортирует геометрическую модель как новый проект.</summary>
        void ImportGeometry(string path);
        /// <summary>Создаёт новый пустой проект.</summary>
        void Create(string projectFile);
        /// <summary>Закрывает текущий проект.</summary>
        void Close();
        /// <summary>Сохраняет текущий проект.</summary>
        void Save(string path);
        void CreateTask();
        void CreateTask(MaterialDBData materials, FunctionDBData functions);
        void ChangeTaskType(TaskType taskType);
        void ChangeTaskKind(TaskKind taskKind);
        void ClearAllData();
    }
}
