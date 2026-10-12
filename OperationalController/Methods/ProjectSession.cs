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
        private readonly List<ChangeMessage> pendingChanges = new List<ChangeMessage>();
        private int changeScopeDepth;

        public event EventHandler<ProjectMessageEventArgs> Message;

        public bool HasProject => hasProject;
        public string FilePath => filePath;
        public bool IsModified => isModified;

        /// <summary>
        /// Открывает проект или сеточную модель по расширению файла.
        /// </summary>
        public void Open(string path, IProgress<int> progress = null)
        {
            using (BeginChanges())
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentException("File path is not specified.", nameof(path));

                var extension = Path.GetExtension(path);
                if (extension.Equals(".bpf", StringComparison.OrdinalIgnoreCase) || extension.Equals(".bpf2", StringComparison.OrdinalIgnoreCase))
                {
                    var loader = new LoadProjectFromTextFormat();
                    var data = loader.Load(path, progress);
                    MarkModelSurfaces(data.ModelData);
                    Replace(data, path, ProjectChange.ProjectChangeKind.Loaded);
                }
                else
                    OpenModel(path, progress);
            }
        }

        /// <summary>
        /// Читает сеточную модель и добавляет её в текущий проект.
        /// </summary>
        public void Append(string path, IProgress<int> progress = null)
        {
            using (BeginChanges())
            {
                if (!hasProject)
                    throw new InvalidOperationException("Project is not open.");

                var modelData = LoadModel(path, progress);
                var appendedNames = ModelData.Append(modelData);
                var surfaceChanges = MarkModelSurfaces(ModelData);
                RegisterAppendedModelChanges(modelData, appendedNames, surfaceChanges);
            }
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
            Replace(data, projectPath, ProjectChange.ProjectChangeKind.Loaded);
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
            using (BeginChanges())
            {
                if (string.IsNullOrWhiteSpace(projectFile))
                    throw new ArgumentException("Project file path is not specified.", nameof(projectFile));
                var data = new ProjectData(projectFile);
                Replace(data, projectFile, ProjectChange.ProjectChangeKind.Created);
            }
        }

        /// <summary>
        /// Заменяет текущие данные и представление после успешного чтения.
        /// </summary>
        private void Replace(IProjectData data, string path, ProjectChange.ProjectChangeKind kind)
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
            hasProject = true;
            isModified = false;
            RegisterChange(new ProjectChange(kind));
        }

        /// <summary>
        /// Открывает область накопления изменений проекта.
        /// </summary>
        private IDisposable BeginChanges()
        {
            changeScopeDepth++;
            return new ChangeScope(this);
        }

        /// <summary>
        /// Завершает область изменений и публикует пакет на внешней границе операции.
        /// </summary>
        private void EndChanges()
        {
            if (changeScopeDepth == 0)
                throw new InvalidOperationException("Change scope is not active.");

            changeScopeDepth--;
            if (changeScopeDepth != 0 || pendingChanges.Count == 0)
                return;

            var changes = pendingChanges.ToArray();
            pendingChanges.Clear();
            PublishChanges(changes);
        }

        /// <summary>
        /// Накапливает изменение или сразу передаёт одиночный пакет вне области операции.
        /// </summary>
        private void RegisterChange(ChangeMessage change)
        {
            if (change == null)
                throw new ArgumentNullException(nameof(change));
            if (changeScopeDepth == 0)
                throw new InvalidOperationException("Change scope is not active.");

            if (change is GeoChange || change is MeshChange || change is CondChange || change is SetDelete || change is SetRename || change is GroupRename)
                isModified = true;
            if (change is ProjectChange projectChange && projectChange.ChangeKind == ProjectChange.ProjectChangeKind.PropertyChanged && projectChange.Property != ProjectChange.ProjectProperty.FilePath)
                isModified = true;

            pendingChanges.Add(change);
        }

        /// <summary>
        /// Передаёт подписчикам готовый пакет изменений.
        /// </summary>
        private void PublishChanges(IReadOnlyList<ChangeMessage> changes)
        {
            Message?.Invoke(this, new ProjectMessageEventArgs(changes));
        }

        /// <summary>
        /// Пересылает изменение внутреннего представления через событие контроллера.
        /// </summary>
        private void OnViewChanged(object sender, ModelViewChangedEventArgs e)
        {
            var changes = new Dictionary<ISetInfo, ModelViewChange>(ReferenceEqualityComparer.Instance);
            foreach (var setInfo in e.GetChangedSets())
                changes.Add(setInfo, e.GetChanges(setInfo));
            var change = new ViewChange(changes);
            if (changeScopeDepth == 0)
            {
                var viewChanges = new[] { change };
                PublishChanges(viewChanges);
                return;
            }
            RegisterChange(change);
        }

        /// <summary>
        /// Гарантирует завершение области изменений при любом выходе из операции.
        /// </summary>
        private sealed class ChangeScope : IDisposable
        {
            private readonly ProjectController controller;
            private bool disposed;

            public ChangeScope(ProjectController controller)
            {
                this.controller = controller;
            }

            /// <summary>
            /// Завершает связанную область изменений один раз.
            /// </summary>
            public void Dispose()
            {
                if (disposed)
                    return;
                disposed = true;
                controller.EndChanges();
            }
        }
    }
}
