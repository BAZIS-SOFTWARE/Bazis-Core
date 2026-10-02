using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Project;
using Project.IO;
using Project.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        private readonly ModelLoaderFactory modelLoaderFactory = new ModelLoaderFactory();

        public event EventHandler<ProjectMessageEventArgs> Message;

        public bool HasProject => hasProject;
        public string FilePath => filePath;
        public bool IsModified => isModified;

        /// <summary>
        /// Открывает проект или сеточную модель по расширению файла.
        /// </summary>
        public void Open(string path, IProgress<int> progress = null)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("File path is not specified.", nameof(path));

            var extension = Path.GetExtension(path);
            if (extension.Equals(".bpf", StringComparison.OrdinalIgnoreCase) || extension.Equals(".bpf2", StringComparison.OrdinalIgnoreCase))
            {
                var loader = new LoadProjectFromTextFormat();
                var data = loader.Load(path, progress);
                MarkModelSurfaces(data.ModelData);
                Replace(data, path, ProjectChangeKind.Loaded, true);
            }
            else
                OpenModel(path, progress);
        }

        /// <summary>
        /// Читает сеточную модель и добавляет её в текущий проект.
        /// </summary>
        public void Append(string path, IProgress<int> progress = null)
        {
            if (!hasProject)
                throw new InvalidOperationException("Project is not open.");

            var modelData = LoadModel(path, progress);
            var appendedNames = ModelData.Append(modelData);
            var surfaceChanges = MarkModelSurfaces(ModelData);
            PublishAppendedModel(modelData, appendedNames, surfaceChanges);
        }

        /// <summary>
        /// Открывает сеточную модель как новый проект.
        /// </summary>
        private void OpenModel(string path, IProgress<int> progress)
        {
            var modelData = LoadModel(path, progress);
            var projectPath = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, "новый_проект.bpf2");
            var data = new ProjectData(projectPath);
            data.ModelData = modelData;
            Replace(data, projectPath, ProjectChangeKind.Loaded, true);
        }

        /// <summary>
        /// Читает модель выбранным по расширению загрузчиком.
        /// </summary>
        private IModelData LoadModel(string path, IProgress<int> progress)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Model file path is not specified.", nameof(path));

            var extension = Path.GetExtension(path);
            var loader = modelLoaderFactory.Create(extension);
            var data = loader.Load(path, progress);
            MarkModelSurfaces(data);
            return data;
        }

        /// <summary>
        /// Создаёт новый пустой проект.
        /// </summary>
        public void Create(string projectFile)
        {
            if (string.IsNullOrWhiteSpace(projectFile))
                throw new ArgumentException("Project file path is not specified.", nameof(projectFile));
            var data = new ProjectData(projectFile);
            Replace(data, projectFile, ProjectChangeKind.Created, true);
        }

        /// <summary>
        /// Закрывает текущий проект и создаёт пустые данные.
        /// </summary>
        public void Close()
        {
            var data = new ProjectData("untitled.bpf2");
            Replace(data, null, ProjectChangeKind.Closed, false);
        }

        /// <summary>
        /// Заменяет текущие данные и представление после успешного чтения.
        /// </summary>
        private void Replace(IProjectData data, string path, ProjectChangeKind kind, bool isOpen)
        {
            if (modelView.HasOpenUpdate)
                throw new InvalidOperationException("Complete the view update before replacing the project.");

            var newView = new ModelView(data.ModelData);
            newView.SelectionColor = modelView.SelectionColor;
            newView.Transparency = modelView.Transparency;
            newView.HideInsideSurfaces = modelView.HideInsideSurfaces;

            modelView.Changed -= OnViewChanged;
            project = data;
            modelView = newView;
            modelView.Changed += OnViewChanged;
            filePath = path;
            hasProject = isOpen;
            isModified = false;
            Publish(new ProjectChange(kind));
        }

        /// <summary>
        /// Передаёт подписчикам одно согласованное изменение.
        /// </summary>
        private void Publish(ChangeMessage change)
        {
            if (change is GeoChange || change is MeshChange || change is CondChange)
                isModified = true;
            if (change is ProjectChange projectChange && projectChange.ChangeKind == ProjectChangeKind.PropertyChanged && projectChange.Property != ProjectProperty.FilePath)
                isModified = true;
            Message?.Invoke(this, new ProjectMessageEventArgs(change));
        }

        /// <summary>
        /// Пересылает изменение внутреннего представления через событие контроллера.
        /// </summary>
        private void OnViewChanged(object sender, ModelViewChangedEventArgs e)
        {
            var changes = new Dictionary<ISetInfo, ModelViewChange>(ReferenceEqualityComparer.Instance);
            foreach (var setInfo in e.GetChangedSets())
                changes.Add(setInfo, e.GetChanges(setInfo));
            Publish(new ViewChange(changes));
        }
    }
}
