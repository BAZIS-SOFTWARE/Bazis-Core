# OperationalController-3 — Регистрация изменений в ProjectController

[Диаграмма](OperationalController-3.mdpuml). [Обзор проекта](OperationalController.md).

Это целевой состав API с сохранением существующих операций. События вне `IProjectEvents.Message` исключены из целевого контракта; код ещё может их содержать. [Расхождения](../../Documents/ArchitectureDifferences.md).

Для классов члены реализуемых интерфейсов не повторяются. Внешние стандартные интерфейсы .NET не раскрываются. Полный состав типа при разбиении приведён на перечисленных ниже диаграммах.

## `ProjectController`

Пространство имён: `OperationalController`.

Исходники: [ProjectController.cs](../../OperationalController/ProjectController.cs), [ChangeMeshSetName.cs](../../OperationalController/Methods/ChangeMeshSetName.cs), [ChangeMeshSetOrder.cs](../../OperationalController/Methods/ChangeMeshSetOrder.cs), [ChangeTaskKind.cs](../../OperationalController/Methods/ChangeTaskKind.cs), [ChangeTaskType.cs](../../OperationalController/Methods/ChangeTaskType.cs), [ConnectByBeams.cs](../../OperationalController/Methods/ConnectByBeams.cs), [ContainsData.cs](../../OperationalController/Methods/ContainsData.cs), [CopyFile.cs](../../OperationalController/Methods/CopyFile.cs), [CreateBoundaryEdges.cs](../../OperationalController/Methods/CreateBoundaryEdges.cs), [CreateBoundaryMesh.cs](../../OperationalController/Methods/CreateBoundaryMesh.cs), [CreateData.cs](../../OperationalController/Methods/CreateData.cs), [CreateGroup.cs](../../OperationalController/Methods/CreateGroup.cs), [CreateModelObjectsPresentor.cs](../../OperationalController/Methods/CreateModelObjectsPresentor.cs), [CreateProject.cs](../../OperationalController/Methods/CreateProject.cs), [CreateTask.cs](../../OperationalController/Methods/CreateTask.cs), [DelData .cs](../../OperationalController/Methods/DelData .cs), [DeleteGeometry.cs](../../OperationalController/Methods/DeleteGeometry.cs), [ExportMesh.cs](../../OperationalController/Methods/ExportMesh.cs), [FindInterfacedNodes.cs](../../OperationalController/Methods/FindInterfacedNodes.cs), [GenerateMesh.cs](../../OperationalController/Methods/GenerateMesh.cs), [GeometryMeshingSettings.cs](../../OperationalController/Methods/GeometryMeshingSettings.cs), [GetData.cs](../../OperationalController/Methods/GetData.cs), [GetGeometryFilePath.cs](../../OperationalController/Methods/GetGeometryFilePath.cs), [GetGeometryMeshNodes.cs](../../OperationalController/Methods/GetGeometryMeshNodes.cs), [GetMeshSettings.cs](../../OperationalController/Methods/GetMeshSettings.cs), [HideMeshByGeo.cs](../../OperationalController/Methods/HideMeshByGeo.cs), [ImportCAD.cs](../../OperationalController/Methods/ImportCAD.cs), [ImportMesh.cs](../../OperationalController/Methods/ImportMesh.cs), [IO.cs](../../OperationalController/Methods/IO.cs), [MarkModelSurfaces.cs](../../OperationalController/Methods/MarkModelSurfaces.cs), [Merge.cs](../../OperationalController/Methods/Merge.cs), [ProjectSession.cs](../../OperationalController/Methods/ProjectSession.cs), [ProjectView.cs](../../OperationalController/Methods/ProjectView.cs), [PublishChanges.cs](../../OperationalController/Methods/PublishChanges.cs), [RecombineMesh.cs](../../OperationalController/Methods/RecombineMesh.cs), [RefineMesh.cs](../../OperationalController/Methods/RefineMesh.cs), [RenumberElements.cs](../../OperationalController/Methods/RenumberElements.cs), [SynchronizeGeometryMesh.cs](../../OperationalController/Methods/SynchronizeGeometryMesh.cs), [TransformMesh.cs](../../OperationalController/Methods/TransformMesh.cs), [UpdateCondition.cs](../../OperationalController/Methods/UpdateCondition.cs).

Базовые типы: `IProjectController`.

```text
CreateProject(string projectFile) : void
ImportCAD(string projectFile) : void
ImportMesh(string filePath) : void
Load(string path) : void
ProjectController()
Publish(ChangeMessage change) : void
PublishConditionChange(DataKind kind, ChangeAction action, int index) : void
PublishGeometryChange() : void
PublishGroupChange(IGroup group, ChangeAction action) : void
PublishSetChange(ISetInfo setInfo, ChangeAction action) : void
PublishAppendedModel(IModelData appendedModel, IReadOnlyList<string> appendedNames, IReadOnlyCollection<ISetInfo> surfaceChanges) : void
MarkModelSurfaces(IModelData model) : IReadOnlyCollection<ISetInfo>
AddCollectionObject<T>(IObjectsCollection<T> collection, string name, T item) : void where T : IModelObject
AddCollectionObjects<T>(IObjectsCollection<T> collection, string name, IEnumerable<T> items) : void where T : IModelObject
AddSetObjects<T>(IObjectsSet<T> set, IEnumerable<T> items) : bool where T : IModelObject
AddCollectionSets<T>(IObjectsCollection<T> collection, IEnumerable<Model.ObjectsCollections.ObjectsSet<T>> sets) : IReadOnlyCollection<ISetInfo> where T : IModelObject
RemoveDeadObjects<T>(IObjectsSet<T> set) : bool where T : IModelObject
RemoveDeadObjects<T>(IObjectsCollection<T> collection) : IReadOnlyCollection<ISetInfo> where T : IModelObject
RemoveGeometryEntry<T>(IObjectsSet<T> set, int number) : bool where T : IModelObject
Replace(IProjectData data, string path, ProjectChangeKind kind, bool isOpen) : void
ReplaceTaskData(ITaskData data) : void
ToEnum<T>(string value) : T where T : struct, Enum
```
