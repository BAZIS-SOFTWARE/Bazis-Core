# OperationalController

`ProjectController` реализует `IProjectController` и координирует изменения проекта, геометрии, сетки и представления. Только этот класс вызывает событие `IProjectEvents.Message`; зависимые библиотеки и внутренний `ModelView` событий в целевой архитектуре не содержат.

[Обзорная диаграмма](OperationalController.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [OperationalController-1](OperationalController-1.mdpuml) | Представление и сообщения об изменениях; ChangeAction, ChangeMessage, ChangedSets, CondChange, GeoChange, GeometryEntityKind |
| [OperationalController-2](OperationalController-2.mdpuml) | Создание объектов, операции и презентаторы; CurveMeshingSettings, GradientVolumeMeshingSettings, MeshSettings, ModelLoaderFactory, PresenterType, SurfaceMeshingSettings |
| [OperationalController-3](OperationalController-3.mdpuml) | Регистрация изменений в ProjectController; ProjectController |
| [OperationalController-4](OperationalController-4.mdpuml) | Специализированные контракты контроллера; IBoundaryEdgesFinder, ICoincidentObjectsFinder, IConditionCommands, IConditionQueries, ICrossSectionMaker, IFreeNodesFinder |
| [OperationalController-5](OperationalController-5.mdpuml) | Представление и сообщения об изменениях; MeshChange, MeshDataKind, ModelView, ModelViewChange, ModelViewChangedEventArgs, ObjectsView |
| [OperationalController-6](OperationalController-6.mdpuml) | Представление и сообщения об изменениях; ProjectChange, ProjectChangeKind, ProjectMessageEventArgs, ProjectProperty, SetStyle, UpdateScope |
| [OperationalController-7](OperationalController-7.mdpuml) | Представление и сообщения об изменениях; ViewChange, ViewMode |
| [OperationalController-8](OperationalController-8.mdpuml) | Создание объектов, операции и презентаторы; VolumeMeshingSettings, VolumeMeshingType, ChamferGeometryBuilder, Convert2DMeshTo2DGeo, CurveLoop, CurveLoopNode |
| [OperationalController-9](OperationalController-9.mdpuml) | Создание объектов, операции и презентаторы; MiddleSurfaceBuilder, Orient, GmshController, BeamGroupConnection, Extract1DFrom2D, Extract2DFrom3D |
| [OperationalController-10](OperationalController-10.mdpuml) | Создание объектов, операции и презентаторы; OctreeBeamGroupConnection, QuadMeshCreator, SimpleBeamGroupConnection, VolumeMeshCreator, CollectionEmptyArgumentException, NegativeRadiusArgumentException |
| [OperationalController-11](OperationalController-11.mdpuml) | Создание объектов, операции и презентаторы; CurveExtruder, Element2DSorter, Extrude3DStrategy, ExtrudeBeamStrategy, MeshExtruder, RotateExtruder |
| [OperationalController-12](OperationalController-12.mdpuml) | Создание объектов, операции и презентаторы; TranslateExtruder, CrossSection, ElementData, FindBoundaryEdges, FindCoincidentObjects, FindOpenSurfaceNodes |
| [OperationalController-13](OperationalController-13.mdpuml) | Создание объектов, операции и презентаторы; FreeNodesFinder, InterfacedNodesFinder, MergeObjects, NormalsCalculator, NormalsChecker, ObjectsRenumber |
| [OperationalController-14](OperationalController-14.mdpuml) | Создание объектов, операции и презентаторы; SelectionHelper, EmptyCollectionException, ObjectsDimensionException, PresentersCreator, ElementSurfaceObjsPresenter, LineObjsPresenter |
| [OperationalController-15](OperationalController-15.mdpuml) | Создание объектов, операции и презентаторы; ManyColorsObjsColors, ObjsColorsType, ObjsPresenter, PointObjsPresenter, SingleColorObjsColors |
| [OperationalController-16](OperationalController-16.mdpuml) | Создание объектов, операции и презентаторы; SurfaceObjsPresenter, ColorLineObjsCreator, ColorPointObjsCreator, ColorSurfaceObjsCreator, CoordsLineObjsCreator |
| [OperationalController-17](OperationalController-17.mdpuml) | Создание объектов, операции и презентаторы; CoordsPointObjsCreator, CoordsSurfaceObjsCreator, EdgesSurfaceObjsCreator, NormalsLineObjsCreator, NormalsPointObjsCreator, NormalsSurfaceObjsCreator |
| [OperationalController-18](OperationalController-18.mdpuml) | Создание объектов, операции и презентаторы; BrigthnessController |
| [OperationalController-19](OperationalController-19.mdpuml) | Специализированные контракты контроллера; IGeometryCommands, IGeometryQueries, IGroupCommands, IGroupQueries, IInterfacedNodesFinder |
| [OperationalController-20](OperationalController-20.mdpuml) | Специализированные контракты контроллера; IMeshCommands |
| [OperationalController-21](OperationalController-21.mdpuml) | Специализированные контракты контроллера; IMeshCommands, IMeshQueries |
| [OperationalController-22](OperationalController-22.mdpuml) | Специализированные контракты контроллера; IMeshQueries, IModelView |
| [OperationalController-23](OperationalController-23.mdpuml) | Специализированные контракты контроллера; IModelView, IObjectsMerger, IObjectsRenumber, IObjectsView, IObjsPresenter |
| [OperationalController-24](OperationalController-24.mdpuml) | Специализированные контракты контроллера; IPresentersCreator |
| [OperationalController-25](OperationalController-25.mdpuml) | Специализированные контракты контроллера; IProjectController |
| [OperationalController-26](OperationalController-26.mdpuml) | Специализированные контракты контроллера; IProjectEvents, IProjectSession, IProjectStorage |
| [OperationalController-27](OperationalController-27.mdpuml) | Специализированные контракты контроллера; IProjectView |
| [OperationalController-28](OperationalController-28.mdpuml) | Специализированные контракты контроллера; IProjectView, ISelectionHelper, ISurfaceObjsPresenter, IVolumeMeshCreator, IGmshController |
| [OperationalController-29](OperationalController-29.mdpuml) | Специализированные контракты контроллера; IGmshController, IExtrusionStrategy, INormalCalculator, IObjsColors, ILineObjsVertexCreator, IPointObjsVertexCreator |
| [OperationalController-30](OperationalController-30.mdpuml) | Специализированные контракты контроллера; ISurfaceObjsVertexCreator, IExtractor1DFrom2D, IExtractor2DFrom3D, ISurfaceMeshCreator, IMesh2DRefiner |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `OperationalController.ChangeAction` | [OperationalController-1](OperationalController-1.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.ChangeMessage` | [OperationalController-1](OperationalController-1.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.ChangedSets` | [OperationalController-1](OperationalController-1.mdpuml) | [ChangedSets.cs](../../OperationalController/ModelView/ChangedSets.cs) |
| `OperationalController.CondChange` | [OperationalController-1](OperationalController-1.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.CurveMeshingSettings` | [OperationalController-2](OperationalController-2.mdpuml) | [CurveMeshingSettings.cs](../../OperationalController/Meshing/CurveMeshingSettings.cs) |
| `OperationalController.GeoChange` | [OperationalController-1](OperationalController-1.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.GeometryEntityKind` | [OperationalController-1](OperationalController-1.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.GradientVolumeMeshingSettings` | [OperationalController-2](OperationalController-2.mdpuml) | [VolumeMeshingSettings.cs](../../OperationalController/Meshing/VolumeMeshingSettings.cs) |
| `OperationalController.IBoundaryEdgesFinder` | [OperationalController-4](OperationalController-4.mdpuml) | [IBoundaryEdgesFinder.cs](../../OperationalController/Interfaces/IBoundaryEdgesFinder.cs) |
| `OperationalController.ICoincidentObjectsFinder` | [OperationalController-4](OperationalController-4.mdpuml) | [ICoincidentObjectsFinder.cs](../../OperationalController/Interfaces/ICoincidentObjectsFinder.cs) |
| `OperationalController.IConditionCommands` | [OperationalController-4](OperationalController-4.mdpuml) | [IConditionCommands.cs](../../OperationalController/Interfaces/IConditionCommands.cs) |
| `OperationalController.IConditionQueries` | [OperationalController-4](OperationalController-4.mdpuml) | [IConditionQueries.cs](../../OperationalController/Interfaces/IConditionQueries.cs) |
| `OperationalController.ICrossSectionMaker` | [OperationalController-4](OperationalController-4.mdpuml) | [ICrossSectionMaker.cs](../../OperationalController/Interfaces/ICrossSectionMaker.cs) |
| `OperationalController.IFreeNodesFinder` | [OperationalController-4](OperationalController-4.mdpuml) | [IFreeNodesFinder.cs](../../OperationalController/Interfaces/IFreeNodesFinder.cs) |
| `OperationalController.IGeometryCommands` | [OperationalController-19](OperationalController-19.mdpuml) | [IGeometryCommands.cs](../../OperationalController/Interfaces/IGeometryCommands.cs) |
| `OperationalController.IGeometryQueries` | [OperationalController-19](OperationalController-19.mdpuml) | [IGeometryQueries.cs](../../OperationalController/Interfaces/IGeometryQueries.cs) |
| `OperationalController.IGroupCommands` | [OperationalController-19](OperationalController-19.mdpuml) | [IGroupCommands.cs](../../OperationalController/Interfaces/IGroupCommands.cs) |
| `OperationalController.IGroupQueries` | [OperationalController-19](OperationalController-19.mdpuml) | [IGroupQueries.cs](../../OperationalController/Interfaces/IGroupQueries.cs) |
| `OperationalController.IInterfacedNodesFinder` | [OperationalController-19](OperationalController-19.mdpuml) | [IInterfacedNodesFinder.cs](../../OperationalController/Interfaces/IInterfacedNodesFinder.cs) |
| `OperationalController.IMeshCommands` | [OperationalController-20](OperationalController-20.mdpuml), [OperationalController-21](OperationalController-21.mdpuml) | [IMeshCommands.cs](../../OperationalController/Interfaces/IMeshCommands.cs) |
| `OperationalController.IMeshQueries` | [OperationalController-21](OperationalController-21.mdpuml), [OperationalController-22](OperationalController-22.mdpuml) | [IMeshQueries.cs](../../OperationalController/Interfaces/IMeshQueries.cs) |
| `OperationalController.IModelView` | [OperationalController-22](OperationalController-22.mdpuml), [OperationalController-23](OperationalController-23.mdpuml) | [IModelView.cs](../../OperationalController/Interfaces/IModelView.cs) |
| `OperationalController.IObjectsMerger` | [OperationalController-23](OperationalController-23.mdpuml) | [IObjectsMerger.cs](../../OperationalController/Interfaces/IObjectsMerger.cs) |
| `OperationalController.IObjectsRenumber` | [OperationalController-23](OperationalController-23.mdpuml) | [IObjectsRenumber.cs](../../OperationalController/Interfaces/IObjectsRenumber.cs) |
| `OperationalController.IObjectsView` | [OperationalController-23](OperationalController-23.mdpuml) | [IObjectsView.cs](../../OperationalController/Interfaces/IObjectsView.cs) |
| `OperationalController.IObjsPresenter` | [OperationalController-23](OperationalController-23.mdpuml) | [IObjsPresenter.cs](../../OperationalController/Interfaces/IObjsPresenter.cs) |
| `OperationalController.IPresentersCreator` | [OperationalController-24](OperationalController-24.mdpuml) | [IPresentersCreator.cs](../../OperationalController/Interfaces/IPresentersCreator.cs) |
| `OperationalController.IProjectController` | [OperationalController-25](OperationalController-25.mdpuml) | [IProjectController.cs](../../OperationalController/Interfaces/IProjectController.cs) |
| `OperationalController.IProjectEvents` | [OperationalController-26](OperationalController-26.mdpuml) | [IProjectEvents.cs](../../OperationalController/Interfaces/IProjectEvents.cs) |
| `OperationalController.IProjectSession` | [OperationalController-26](OperationalController-26.mdpuml) | [IProjectSession.cs](../../OperationalController/Interfaces/IProjectSession.cs) |
| `OperationalController.IProjectStorage` | [OperationalController-26](OperationalController-26.mdpuml) | [IProjectStorage.cs](../../OperationalController/Interfaces/IProjectStorage.cs) |
| `OperationalController.IProjectView` | [OperationalController-27](OperationalController-27.mdpuml), [OperationalController-28](OperationalController-28.mdpuml) | [IProjectView.cs](../../OperationalController/Interfaces/IProjectView.cs) |
| `OperationalController.ISelectionHelper` | [OperationalController-28](OperationalController-28.mdpuml) | [ISelectionHelper.cs](../../OperationalController/Interfaces/ISelectionHelper.cs) |
| `OperationalController.ISurfaceObjsPresenter` | [OperationalController-28](OperationalController-28.mdpuml) | [ISurfaceObjsPresenter.cs](../../OperationalController/Interfaces/ISurfaceObjsPresenter.cs) |
| `OperationalController.IVolumeMeshCreator` | [OperationalController-28](OperationalController-28.mdpuml) | [IVolumeMeshCreator.cs](../../OperationalController/Interfaces/IVolumeMeshCreator.cs) |
| `OperationalController.MeshChange` | [OperationalController-5](OperationalController-5.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.MeshDataKind` | [OperationalController-5](OperationalController-5.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.MeshSettings` | [OperationalController-2](OperationalController-2.mdpuml) | [GetMeshSettings.cs](../../OperationalController/Methods/GetMeshSettings.cs) |
| `OperationalController.ModelLoaderFactory` | [OperationalController-2](OperationalController-2.mdpuml) | [ModelLoaderFactory.cs](../../OperationalController/ModelLoaderFactory.cs) |
| `OperationalController.ModelView` | [OperationalController-5](OperationalController-5.mdpuml) | [ModelView.cs](../../OperationalController/ModelView/ModelView.cs) |
| `OperationalController.ModelViewChange` | [OperationalController-5](OperationalController-5.mdpuml) | [ModelViewChange.cs](../../OperationalController/ModelView/ModelViewChange.cs) |
| `OperationalController.ModelViewChangedEventArgs` | [OperationalController-5](OperationalController-5.mdpuml) | [ModelViewChangedEventArgs.cs](../../OperationalController/ModelView/ModelViewChangedEventArgs.cs) |
| `OperationalController.ObjectsView` | [OperationalController-5](OperationalController-5.mdpuml) | [ObjectsView.cs](../../OperationalController/ModelView/ObjectsView.cs) |
| `OperationalController.PresenterType` | [OperationalController-2](OperationalController-2.mdpuml) | [IObjsPresenter.cs](../../OperationalController/Interfaces/IObjsPresenter.cs) |
| `OperationalController.ProjectChange` | [OperationalController-6](OperationalController-6.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.ProjectChangeKind` | [OperationalController-6](OperationalController-6.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.ProjectController` | [OperationalController-3](OperationalController-3.mdpuml) | [ProjectController.cs](../../OperationalController/ProjectController.cs) |
| `OperationalController.ProjectMessageEventArgs` | [OperationalController-6](OperationalController-6.mdpuml) | [ProjectMessageEventArgs.cs](../../OperationalController/Events/ProjectMessageEventArgs.cs) |
| `OperationalController.ProjectProperty` | [OperationalController-6](OperationalController-6.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.SetStyle` | [OperationalController-6](OperationalController-6.mdpuml) | [SetStyle.cs](../../OperationalController/ModelView/SetStyle.cs) |
| `OperationalController.SurfaceMeshingSettings` | [OperationalController-2](OperationalController-2.mdpuml) | [SurfaceMeshingSettings.cs](../../OperationalController/Meshing/SurfaceMeshingSettings.cs) |
| `OperationalController.UpdateScope` | [OperationalController-6](OperationalController-6.mdpuml) | [UpdateScope.cs](../../OperationalController/ModelView/UpdateScope.cs) |
| `OperationalController.ViewChange` | [OperationalController-7](OperationalController-7.mdpuml) | [ChangeMessage.cs](../../OperationalController/Events/ChangeMessage.cs) |
| `OperationalController.ViewMode` | [OperationalController-7](OperationalController-7.mdpuml) | [ViewMode.cs](../../OperationalController/ModelView/ViewMode.cs) |
| `OperationalController.VolumeMeshingSettings` | [OperationalController-8](OperationalController-8.mdpuml) | [VolumeMeshingSettings.cs](../../OperationalController/Meshing/VolumeMeshingSettings.cs) |
| `OperationalController.VolumeMeshingType` | [OperationalController-8](OperationalController-8.mdpuml) | [VolumeMeshingSettings.cs](../../OperationalController/Meshing/VolumeMeshingSettings.cs) |
| `OperationalController.GeomObjsCreator.ChamferGeometryBuilder` | [OperationalController-8](OperationalController-8.mdpuml) | [ChamferGeometryBuilder.cs](../../OperationalController/GeomObjsCreator/ChamferGeometryBuilder.cs) |
| `OperationalController.GeomObjsCreator.Convert2DMeshTo2DGeo` | [OperationalController-8](OperationalController-8.mdpuml) | [Convert2DMeshTo2DGeo.cs](../../OperationalController/GeomObjsCreator/Convert2DMeshTo2DGeo.cs) |
| `OperationalController.GeomObjsCreator.CurveLoop` | [OperationalController-8](OperationalController-8.mdpuml) | [CurveLoop.cs](../../OperationalController/GeomObjsCreator/CurveLoop.cs) |
| `OperationalController.GeomObjsCreator.CurveLoop.CurveLoopNode` | [OperationalController-8](OperationalController-8.mdpuml) | [CurveLoop.cs](../../OperationalController/GeomObjsCreator/CurveLoop.cs) |
| `OperationalController.GeomObjsCreator.MiddleSurfaceBuilder` | [OperationalController-9](OperationalController-9.mdpuml) | [MiddleSurfaceBuilder.cs](../../OperationalController/GeomObjsCreator/MiddleSurfaceBuilder.cs) |
| `OperationalController.GeomObjsCreator.CurveLoop.Orient` | [OperationalController-9](OperationalController-9.mdpuml) | [CurveLoop.cs](../../OperationalController/GeomObjsCreator/CurveLoop.cs) |
| `OperationalController.GmshController.GmshController` | [OperationalController-9](OperationalController-9.mdpuml) | [GmshController.cs](../../OperationalController/GmshController/GmshController.cs) |
| `OperationalController.GmshController.IGmshController` | [OperationalController-28](OperationalController-28.mdpuml), [OperationalController-29](OperationalController-29.mdpuml) | [IGmshController.cs](../../OperationalController/GmshController/IGmshController.cs) |
| `OperationalController.MeshObjsCreator.BeamGroupConnection` | [OperationalController-9](OperationalController-9.mdpuml) | [BeamGroupConnection.cs](../../OperationalController/MeshObjsCreator/BeamGroupConnection.cs) |
| `OperationalController.MeshObjsCreator.Extract1DFrom2D` | [OperationalController-9](OperationalController-9.mdpuml) | [Extract1DFrom2D.cs](../../OperationalController/MeshObjsCreator/Extract1DFrom2D.cs) |
| `OperationalController.MeshObjsCreator.Extract2DFrom3D` | [OperationalController-9](OperationalController-9.mdpuml) | [Extract2DFrom3D.cs](../../OperationalController/MeshObjsCreator/Extract2DFrom3D.cs) |
| `OperationalController.MeshObjsCreator.OctreeBeamGroupConnection` | [OperationalController-10](OperationalController-10.mdpuml) | [OctreeBeamGroupConnection.cs](../../OperationalController/MeshObjsCreator/OctreeBeamGroupConnection.cs) |
| `OperationalController.MeshObjsCreator.QuadMeshCreator` | [OperationalController-10](OperationalController-10.mdpuml) | [QuadMeshCreator.cs](../../OperationalController/MeshObjsCreator/QuadMeshCreator.cs) |
| `OperationalController.MeshObjsCreator.SimpleBeamGroupConnection` | [OperationalController-10](OperationalController-10.mdpuml) | [SimpleBeamGroupConnection.cs](../../OperationalController/MeshObjsCreator/SimpleBeamGroupConnection.cs) |
| `OperationalController.MeshObjsCreator.VolumeMeshCreator` | [OperationalController-10](OperationalController-10.mdpuml) | [VolumeMeshCreator.cs](../../OperationalController/MeshObjsCreator/VolumeMeshCreator.cs) |
| `OperationalController.MeshObjsCreator.Exceptions.CollectionEmptyArgumentException` | [OperationalController-10](OperationalController-10.mdpuml) | [CollectionArgumentException.cs](../../OperationalController/MeshObjsCreator/Exceptions/CollectionArgumentException.cs) |
| `OperationalController.MeshObjsCreator.Exceptions.NegativeRadiusArgumentException` | [OperationalController-10](OperationalController-10.mdpuml) | [RadiusArgumentException.cs](../../OperationalController/MeshObjsCreator/Exceptions/RadiusArgumentException.cs) |
| `OperationalController.MeshObjsCreator.Extrude.CurveExtruder` | [OperationalController-11](OperationalController-11.mdpuml) | [CurveExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/CurveExtruder.cs) |
| `OperationalController.MeshObjsCreator.Extrude.MeshExtruder.Element2DSorter` | [OperationalController-11](OperationalController-11.mdpuml) | [MeshExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/MeshExtruder.cs) |
| `OperationalController.MeshObjsCreator.Extrude.Extrude3DStrategy` | [OperationalController-11](OperationalController-11.mdpuml) | [Extrude3DStrategy.cs](../../OperationalController/MeshObjsCreator/Extrude/Extrude3DStrategy.cs) |
| `OperationalController.MeshObjsCreator.Extrude.ExtrudeBeamStrategy` | [OperationalController-11](OperationalController-11.mdpuml) | [ExtrudeBeamStrategy.cs](../../OperationalController/MeshObjsCreator/Extrude/ExtrudeBeamStrategy.cs) |
| `OperationalController.MeshObjsCreator.Extrude.IExtrusionStrategy<TResult>` | [OperationalController-29](OperationalController-29.mdpuml) | [IExtrusionStrategy.cs](../../OperationalController/MeshObjsCreator/Extrude/IExtrusionStrategy.cs) |
| `OperationalController.MeshObjsCreator.Extrude.MeshExtruder` | [OperationalController-11](OperationalController-11.mdpuml) | [MeshExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/MeshExtruder.cs) |
| `OperationalController.MeshObjsCreator.Extrude.RotateExtruder` | [OperationalController-11](OperationalController-11.mdpuml) | [RotateExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/RotateExtruder.cs) |
| `OperationalController.MeshObjsCreator.Extrude.TranslateExtruder` | [OperationalController-12](OperationalController-12.mdpuml) | [TranslateExtruder.cs](../../OperationalController/MeshObjsCreator/Extrude/TranslateExtruder.cs) |
| `OperationalController.MeshObjsUtility.CrossSection` | [OperationalController-12](OperationalController-12.mdpuml) | [CrossSection.cs](../../OperationalController/MeshObjsUtility/CrossSection.cs) |
| `OperationalController.MeshObjsUtility.NormalsChecker.ElementData` | [OperationalController-12](OperationalController-12.mdpuml) | [NormalsChecker.cs](../../OperationalController/MeshObjsUtility/NormalsChecker.cs) |
| `OperationalController.MeshObjsUtility.FindBoundaryEdges` | [OperationalController-12](OperationalController-12.mdpuml) | [FindBoundaryEdges.cs](../../OperationalController/MeshObjsUtility/FindBoundaryEdges.cs) |
| `OperationalController.MeshObjsUtility.FindCoincidentObjects` | [OperationalController-12](OperationalController-12.mdpuml) | [FindCoincidentObjects.cs](../../OperationalController/MeshObjsUtility/FindCoincidentObjects.cs) |
| `OperationalController.MeshObjsUtility.FindOpenSurfaceNodes` | [OperationalController-12](OperationalController-12.mdpuml) | [FindOpenSurfaceNodes.cs](../../OperationalController/MeshObjsUtility/FindOpenSurfaceNodes.cs) |
| `OperationalController.MeshObjsUtility.FreeNodesFinder` | [OperationalController-13](OperationalController-13.mdpuml) | [FreeNodesFinder.cs](../../OperationalController/MeshObjsUtility/FreeNodesFinder.cs) |
| `OperationalController.MeshObjsUtility.INormalCalculator` | [OperationalController-29](OperationalController-29.mdpuml) | [INormalCalculator.cs](../../OperationalController/Interfaces/Utilities/INormalCalculator.cs) |
| `OperationalController.MeshObjsUtility.InterfacedNodesFinder` | [OperationalController-13](OperationalController-13.mdpuml) | [InterfacedNodesFinder.cs](../../OperationalController/MeshObjsUtility/InterfacedNodesFinder.cs) |
| `OperationalController.MeshObjsUtility.MergeObjects` | [OperationalController-13](OperationalController-13.mdpuml) | [MergeObjects.cs](../../OperationalController/MeshObjsUtility/MergeObjects.cs) |
| `OperationalController.MeshObjsUtility.NormalsCalculator` | [OperationalController-13](OperationalController-13.mdpuml) | [NormalsCalculator.cs](../../OperationalController/MeshObjsUtility/NormalsCalculator.cs) |
| `OperationalController.MeshObjsUtility.NormalsChecker` | [OperationalController-13](OperationalController-13.mdpuml) | [NormalsChecker.cs](../../OperationalController/MeshObjsUtility/NormalsChecker.cs) |
| `OperationalController.MeshObjsUtility.ObjectsRenumber` | [OperationalController-13](OperationalController-13.mdpuml) | [ObjectsRenumber.cs](../../OperationalController/MeshObjsUtility/ObjectsRenumber.cs) |
| `OperationalController.MeshObjsUtility.SelectionHelper` | [OperationalController-14](OperationalController-14.mdpuml) | [SelectionHelper.cs](../../OperationalController/MeshObjsUtility/SelectionHelper.cs) |
| `OperationalController.Methods.Exceptions.EmptyCollectionException` | [OperationalController-14](OperationalController-14.mdpuml) | [EmptyCollectionException.cs](../../OperationalController/Methods/Exceptions/EmptyCollectionException.cs) |
| `OperationalController.Methods.Exceptions.ObjectsDimensionException` | [OperationalController-14](OperationalController-14.mdpuml) | [ObjectsDimensionException.cs](../../OperationalController/Methods/Exceptions/ObjectsDimensionException.cs) |
| `OperationalController.ModelScenePresentator.PresentersCreator` | [OperationalController-14](OperationalController-14.mdpuml) | [PresentersCreator.cs](../../OperationalController/ModelScenePresentator/PresentersCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.ElementSurfaceObjsPresenter` | [OperationalController-14](OperationalController-14.mdpuml) | [ElementSurfaceObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/ElementSurfaceObjsPresenter.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.IObjsColors` | [OperationalController-29](OperationalController-29.mdpuml) | [IObjsColors.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/IObjsColors.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.LineObjsPresenter<T>` | [OperationalController-14](OperationalController-14.mdpuml) | [LineObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/LineObjsPresenter.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.ManyColorsObjsColors` | [OperationalController-15](OperationalController-15.mdpuml) | [IObjsColors.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/IObjsColors.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.ObjsColorsType` | [OperationalController-15](OperationalController-15.mdpuml) | [IObjsColors.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/IObjsColors.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.ObjsPresenter<T>` | [OperationalController-15](OperationalController-15.mdpuml) | [ObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/ObjsPresenter.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.PointObjsPresenter<T>` | [OperationalController-15](OperationalController-15.mdpuml) | [PointObjsPresenter.Coordinated.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/PointObjsPresenter.Coordinated.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.PointObjsPresenter` | [OperationalController-15](OperationalController-15.mdpuml) | [PointObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/PointObjsPresenter.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.SingleColorObjsColors` | [OperationalController-15](OperationalController-15.mdpuml) | [IObjsColors.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/IObjsColors.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.SurfaceObjsPresenter<T>` | [OperationalController-16](OperationalController-16.mdpuml) | [SurfaceObjsPresenter.Coordinated.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/SurfaceObjsPresenter.Coordinated.cs) |
| `OperationalController.ModelScenePresentator.GlObjsPresenters.SurfaceObjsPresenter<T, U>` | [OperationalController-16](OperationalController-16.mdpuml) | [SurfaceObjsPresenter.cs](../../OperationalController/ModelScenePresentator/GlObjsPresenters/SurfaceObjsPresenter.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.ColorLineObjsCreator<T>` | [OperationalController-16](OperationalController-16.mdpuml) | [ColorLineObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/ColorLineObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.ColorPointObjsCreator` | [OperationalController-16](OperationalController-16.mdpuml) | [ColorPointObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/ColorPointObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.ColorSurfaceObjsCreator<T,U>` | [OperationalController-16](OperationalController-16.mdpuml) | [ColorSurfaceObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/ColorSurfaceObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.CoordsLineObjsCreator<T>` | [OperationalController-16](OperationalController-16.mdpuml) | [CoordsLineObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/CoordsLineObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.CoordsPointObjsCreator` | [OperationalController-17](OperationalController-17.mdpuml) | [CoordsPointObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/CoordsPointObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.CoordsSurfaceObjsCreator<T, U>` | [OperationalController-17](OperationalController-17.mdpuml) | [CoordsSurfaceObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/CoordsSurfaceObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.EdgesSurfaceObjsCreator<T, U>` | [OperationalController-17](OperationalController-17.mdpuml) | [EdgesSurfaceObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/EdgesSurfaceObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.NormalsLineObjsCreator<T>` | [OperationalController-17](OperationalController-17.mdpuml) | [NormalsLineObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/NormalsLineObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.NormalsPointObjsCreator` | [OperationalController-17](OperationalController-17.mdpuml) | [NormalsPointObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/NormalsPointObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.GlObjsVertexCreators.NormalsSurfaceObjsCreator<T, U>` | [OperationalController-17](OperationalController-17.mdpuml) | [NormalsSurfaceObjsCreator.cs](../../OperationalController/ModelScenePresentator/GlObjsVertexCreators/NormalsSurfaceObjsCreator.cs) |
| `OperationalController.ModelScenePresentator.Interfaces.ILineObjsVertexCreator<T>` | [OperationalController-29](OperationalController-29.mdpuml) | [ILineObjsVertexCreator.cs](../../OperationalController/ModelScenePresentator/Interfaces/ILineObjsVertexCreator.cs) |
| `OperationalController.ModelScenePresentator.Interfaces.IPointObjsVertexCreator` | [OperationalController-29](OperationalController-29.mdpuml) | [IPointObjsVertexCreator.cs](../../OperationalController/ModelScenePresentator/Interfaces/IPointObjsVertexCreator.cs) |
| `OperationalController.ModelScenePresentator.Interfaces.ISurfaceObjsVertexCreator<T,U>` | [OperationalController-30](OperationalController-30.mdpuml) | [ISurfaceObjsVertexCreator.cs](../../OperationalController/ModelScenePresentator/Interfaces/ISurfaceObjsVertexCreator.cs) |
| `OperationalController.ObjsCreator.IExtractor1DFrom2D` | [OperationalController-30](OperationalController-30.mdpuml) | [IExtractor1DFrom2D.cs](../../OperationalController/Interfaces/ObjsCreator/IExtractor1DFrom2D.cs) |
| `OperationalController.ObjsCreator.IExtractor2DFrom3D` | [OperationalController-30](OperationalController-30.mdpuml) | [IExtractor2DFrom3D.cs](../../OperationalController/Interfaces/ObjsCreator/IExtractor2DFrom3D.cs) |
| `OperationalController.SceneBrightnessController.BrigthnessController` | [OperationalController-18](OperationalController-18.mdpuml) | [BrigthnessController.cs](../../OperationalController/SceneBrightnessController/BrigthnessController.cs) |
| `OperationalController.SurfaceMeshCreator.ISurfaceMeshCreator` | [OperationalController-30](OperationalController-30.mdpuml) | [ISurfaceMeshCreator.cs](../../OperationalController/Interfaces/SurfaceMeshCreator/ISurfaceMeshCreator.cs) |
| `OperationalController.Utilities.IMesh2DRefiner` | [OperationalController-30](OperationalController-30.mdpuml) | [IMesh2DRefiner.cs](../../OperationalController/Interfaces/Utilities/IMesh2DRefiner.cs) |

## Уведомления

Публичный контракт и правила публикации: [ProjectController.md](ProjectController.md). Сообщения: [ProjectEvents.mdpuml](ProjectEvents.mdpuml). Целевой протокол вида: [ModelView.md](ModelView.md). Существующий `ModelView.Changed` и события IO перечислены в реестре расхождений; отсутствие этих событий на целевых диаграммах не означает изменения исходников.

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
