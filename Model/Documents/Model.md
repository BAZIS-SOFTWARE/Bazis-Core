# Model

[Итоговый PDF](Model.pdf) объединяет активные диаграммы и полные сигнатуры API. Ссылки на схемах ведут к страницам внутри документа, включая используемые типы Geometry. [Состав и результаты проверки PDF](Model-PDF.md).

Хранит геометрические и сеточные объекты, наборы и группы. Изменения выполняются методами; библиотека не публикует события. `Append` не принимает делегатов уведомления. `Renumber` возвращает признак изменения, `MarkSurfaces.Mark` — уникальных изменённых владельцев через `out`; делегатов уведомления у этих операций нет.

Загрузка сообщает условный процент через необязательный `IProgress<int>` в [IModelLoader](Model-2.md).
`LoadEvent`, `ILoaderEventArgs` и `LoaderEventArgs` удалены из Model; старый `ILoader.Load(path)` сохранён.

[Обзорная диаграмма](Model.mdpuml). [Архитектура решения](../../Documents/Architecture.md). [Расхождения с кодом](../../Documents/ArchitectureDifferences.md).

## Поддиаграммы

Основная схема показывает связи и адреса подробностей. В каждой поддиаграмме есть обратный переход к `Model.mdpuml`; у типов `reference` указан адрес подробного описания. Легенда и раздел «Переходы к связанным типам» в соседнем Markdown-файле дают переходы по базовым типам и типам в сигнатурах. Для типов `Geometry` указан путь к диаграмме соответствующей библиотеки. Ссылки интерактивны в SVG при поддержке просмотрщиком, а названия диаграмм видны и в статическом изображении. На поддиаграммах раскрыты методы, свойства и события интерфейсов; классы содержат собственные методы без повторения интерфейсного контракта. Методы в диаграммах показывают имена параметров; полные сигнатуры и ссылки на код находятся в соседних Markdown-файлах.

| Диаграмма | Область |
| --- | --- |
| [Model-1](Model-1.mdpuml) | Геометрические и сеточные сущности; IModelObject, IIntegralObject, IIntegralPoint, IElement, IElement1D, IElement2D |
| [Model-2](Model-2.mdpuml) | Загрузка и сохранение; ILoader, IModelLoader, IModelSaver, ISaver, ISaverEventArgs |
| [Model-3](Model-3.mdpuml) | Модель, наборы и группы; IGroup, IGroupData, ILineObject, IModelData, INormal |
| [Model-4](Model-4.mdpuml) | Грани и вспомогательные алгоритмы; ListExtentions, ElementSurfaceEqualityComparer, ComparerBySqCoords, EqualityComparerByCentralPoint, EqualityComparerByExistState, EqualityComparerByNumber |
| [Model-5](Model-5.mdpuml) | Геометрические и сеточные сущности; IElement3D, ISurfaceElement, Curve, GeometryPoint, GeometrySurface, SurfaceFigure |
| [Model-6](Model-6.mdpuml) | Геометрические и сеточные сущности; VolumeData, VolumeFigure, ObjType, ElementKind, Beam |
| [Model-7](Model-7.mdpuml) | Геометрические и сеточные сущности; Element, Element2D, Element3D, ElementSurface, Hexa, IntegralPoint |
| [Model-8](Model-8.mdpuml) | Геометрические и сеточные сущности; Node, Penta, Quad, Tetra, Tetra_c, Triangle |
| [Model-9](Model-9.mdpuml) | Геометрические и сеточные сущности; Triangle_c |
| [Model-10](Model-10.mdpuml) | Загрузка и сохранение; SaverEventArgs, LoadModelFromASCIITextFile, LoadModelFromASCIITextFile_v2, LoadModelFromBPF2TextFile, LoadModelFromBPFTextFile |
| [Model-11](Model-11.mdpuml) | Загрузка и сохранение; LoadModelFromCDBTextFile, LoadModelFromINPTextFile, LoadModelFromINPTextFile_v2, LoadModelFromSalomeFile, ProgressTextReader, SaveModelToASCIITextFile |
| [Model-12](Model-12.mdpuml) | Загрузка и сохранение; SaveModelToBPF2TextFile, SaveModelToBPFTextFile, LoadFromSTLFile, SaveModelToBinarySTLFile, SaveToTxtSTLFile, StlFileLoadExeption |
| [Model-13](Model-13.mdpuml) | Загрузка и сохранение; StlFileWriteExeption |
| [Model-14](Model-14.mdpuml) | Модель, наборы и группы; IObjectsData |
| [Model-15](Model-15.mdpuml) | Модель, наборы и группы; IObjectsData, IPoint, ISquare, ISurface, ISurfaceObject, IVertexObject |
| [Model-16](Model-16.mdpuml) | Модель, наборы и группы; IVolume, IObjectsCollection, IObjectsSet, ISetInfo, ModelData |
| [Model-17](Model-17.mdpuml) | Модель, наборы и группы; ModelObject, ObjectsData, Group, GroupData, NameAndTypeGroupComparer |
| [Model-18](Model-18.mdpuml) | Модель, наборы и группы; NumberGroupComparer, StringNumberGroupComparer, WarningAttribute, ObjectsCollection, ObjectsSet, SurfacesSet |
| [Model-19](Model-19.mdpuml) | Грани и вспомогательные алгоритмы; ByNumberFinder, DistanceSorter, MergeSort, MergeSortWithComparer, DefaultModelObjsComparer |
| [Model-20](Model-20.mdpuml) | Грани и вспомогательные алгоритмы; DefaultSetInfoComparer, MarkSurfaces, SurfaceNodesComparer, SurfacesCreator, SurfacesFinder |

## Указатель типов

| Тип | Диаграммы | Исходник |
| --- | --- | --- |
| `Model.ModelData` | [Model-16](Model-16.mdpuml) | [ModelData.cs](../../Model/ModelData.cs) |
| `Model.ModelObject` | [Model-17](Model-17.mdpuml) | [ModelObject.cs](../../Model/ModelObject.cs) |
| `Model.ObjectsData` | [Model-17](Model-17.mdpuml) | [ObjectsData.cs](../../Model/ObjectsData.cs) |
| `Model.Events.SaverEventArgs` | [Model-10](Model-10.mdpuml) | [SaverEventArgs.cs](../../Model/Events/SaverEventArgs.cs) |
| `Model.Extensions.ListExtentions` | [Model-4](Model-4.mdpuml) | [ListExtentions.cs](../../Model/Extensions/ListExtentions.cs) |
| `Model.GeometryObjects.Curve` | [Model-5](Model-5.mdpuml) | [Curve.cs](../../Model/GeometryObjects/Curve.cs) |
| `Model.GeometryObjects.GeometryPoint` | [Model-5](Model-5.mdpuml) | [GeometryPoint.cs](../../Model/GeometryObjects/GeometryPoint.cs) |
| `Model.GeometryObjects.GeometrySurface` | [Model-5](Model-5.mdpuml) | [GeometrySurface.cs](../../Model/GeometryObjects/GeometrySurface.cs) |
| `Model.GeometryObjects.SurfaceFigure` | [Model-5](Model-5.mdpuml) | [SurfaceFigure.cs](../../Model/GeometryObjects/SurfaceFigure.cs) |
| `Model.GeometryObjects.VolumeData` | [Model-6](Model-6.mdpuml) | [VolumeData.cs](../../Model/GeometryObjects/VolumeData.cs) |
| `Model.GeometryObjects.VolumeFigure` | [Model-6](Model-6.mdpuml) | [VolumeFigure.cs](../../Model/GeometryObjects/VolumeFigure.cs) |
| `Model.GroupsData.Group` | [Model-17](Model-17.mdpuml) | [Group.cs](../../Model/GroupsData/Group.cs) |
| `Model.GroupsData.GroupData` | [Model-17](Model-17.mdpuml) | [GroupData.cs](../../Model/GroupsData/GroupData.cs) |
| `Model.GroupsData.NameAndTypeGroupComparer` | [Model-17](Model-17.mdpuml) | [NameAndTypeGroupComparer.cs](../../Model/GroupsData/NameAndTypeGroupComparer.cs) |
| `Model.GroupsData.NumberGroupComparer` | [Model-18](Model-18.mdpuml) | [NumberGroupComparer.cs](../../Model/GroupsData/NumberGroupComparer.cs) |
| `Model.GroupsData.StringNumberGroupComparer` | [Model-18](Model-18.mdpuml) | [StringNumberGroupComparer.cs](../../Model/GroupsData/StringNumberGroupComparer.cs) |
| `Model.IO.LoadModelFromASCIITextFile` | [Model-10](Model-10.mdpuml) | [LoadModelFromASCIITextFile.cs](../../Model/IO/LoadModelFromASCIITextFile.cs) |
| `Model.IO.LoadModelFromASCIITextFile_v2` | [Model-10](Model-10.mdpuml) | [LoadModelFromASCIITextFile_v2.cs](../../Model/IO/LoadModelFromASCIITextFile_v2.cs) |
| `Model.IO.LoadModelFromBPF2TextFile` | [Model-10](Model-10.mdpuml) | [LoadModelFromBPF2TextFile.cs](../../Model/IO/LoadModelFromBPF2TextFile.cs) |
| `Model.IO.LoadModelFromBPFTextFile` | [Model-10](Model-10.mdpuml) | [LoadModelFromBPFTextFile.cs](../../Model/IO/LoadModelFromBPFTextFile.cs) |
| `Model.IO.LoadModelFromCDBTextFile` | [Model-11](Model-11.mdpuml) | [LoadModelFromCDBTextFile.cs](../../Model/IO/LoadModelFromCDBTextFile.cs) |
| `Model.IO.LoadModelFromINPTextFile` | [Model-11](Model-11.mdpuml) | [LoadModelFromINPTextFile.cs](../../Model/IO/LoadModelFromINPTextFile.cs) |
| `Model.IO.LoadModelFromINPTextFile_v2` | [Model-11](Model-11.mdpuml) | [LoadModelFromINPTextFile_v2.cs](../../Model/IO/LoadModelFromINPTextFile_v2.cs) |
| `Model.IO.LoadModelFromSalomeFile` | [Model-11](Model-11.mdpuml) | [LoadModelFromSalomeFile.cs](../../Model/IO/LoadModelFromSalomeFile.cs) |
| `Model.IO.ProgressTextReader` | [Model-11](Model-11.mdpuml) | [ProgressTextReader.cs](../../Model/IO/ProgressTextReader.cs) |
| `Model.IO.SaveModelToASCIITextFile` | [Model-11](Model-11.mdpuml) | [SaveModelToASCIITextFile.cs](../../Model/IO/SaveModelToASCIITextFile.cs) |
| `Model.IO.SaveModelToBPF2TextFile` | [Model-12](Model-12.mdpuml) | [SaveModelToBPF2TextFile.cs](../../Model/IO/SaveModelToBPF2TextFile.cs) |
| `Model.IO.SaveModelToBPFTextFile` | [Model-12](Model-12.mdpuml) | [SaveModelToBPFTextFile.cs](../../Model/IO/SaveModelToBPFTextFile.cs) |
| `Model.IO.STL.LoadFromSTLFile` | [Model-12](Model-12.mdpuml) | [LoadFromSTLFile.cs](../../Model/IO/STL/LoadFromSTLFile.cs) |
| `Model.IO.STL.SaveModelToBinarySTLFile` | [Model-12](Model-12.mdpuml) | [SaveToTxtSTLFile.cs](../../Model/IO/STL/SaveToTxtSTLFile.cs) |
| `Model.IO.STL.SaveToTxtSTLFile` | [Model-12](Model-12.mdpuml) | [SaveToTxtSTLFile.cs](../../Model/IO/STL/SaveToTxtSTLFile.cs) |
| `Model.IO.STL.StlFileLoadExeption` | [Model-12](Model-12.mdpuml) | [LoadFromSTLFile.cs](../../Model/IO/STL/LoadFromSTLFile.cs) |
| `Model.IO.STL.StlFileWriteExeption` | [Model-13](Model-13.mdpuml) | [SaveToTxtSTLFile.cs](../../Model/IO/STL/SaveToTxtSTLFile.cs) |
| `Model.Interfaces.IGroup` | [Model-3](Model-3.mdpuml) | [IGroup.cs](../../Model/Interfaces/IGroup.cs) |
| `Model.Interfaces.IGroupData` | [Model-3](Model-3.mdpuml) | [IGroupData.cs](../../Model/Interfaces/IGroupData.cs) |
| `Model.Interfaces.ILineObject<T>` | [Model-3](Model-3.mdpuml) | [ILineObject.cs](../../Model/Interfaces/ILineObject.cs) |
| `Model.Interfaces.ILoader` | [Model-2](Model-2.mdpuml) | [ILoader.cs](../../Model/Interfaces/ILoader.cs) |
| `Model.Interfaces.IModelData` | [Model-3](Model-3.mdpuml) | [IModelData.cs](../../Model/Interfaces/IModelData.cs) |
| `Model.Interfaces.IModelLoader` | [Model-2](Model-2.mdpuml) | [IModelLoader.cs](../../Model/Interfaces/IModelLoader.cs) |
| `Model.Interfaces.IModelObject` | [Model-1](Model-1.mdpuml) | [IModelObject.cs](../../Model/Interfaces/IModelObject.cs) |
| `Model.Interfaces.IModelSaver` | [Model-2](Model-2.mdpuml) | [IModelSaver.cs](../../Model/Interfaces/IModelSaver.cs) |
| `Model.Interfaces.INormal` | [Model-3](Model-3.mdpuml) | [INormal.cs](../../Model/Interfaces/INormal.cs) |
| `Model.Interfaces.IObjectsData` | [Model-14](Model-14.mdpuml), [Model-15](Model-15.mdpuml) | [IObjectsData.cs](../../Model/Interfaces/IObjectsData.cs) |
| `Model.Interfaces.IPoint` | [Model-15](Model-15.mdpuml) | [IPoint.cs](../../Model/Interfaces/IPoint.cs) |
| `Model.Interfaces.ISaver` | [Model-2](Model-2.mdpuml) | [ISaver.cs](../../Model/Interfaces/ISaver.cs) |
| `Model.Interfaces.ISaverEventArgs` | [Model-2](Model-2.mdpuml) | [ISaverEventArgs.cs](../../Model/Interfaces/ISaverEventArgs.cs) |
| `Model.Interfaces.ISquare` | [Model-15](Model-15.mdpuml) | [ISquare.cs](../../Model/Interfaces/ISquare.cs) |
| `Model.Interfaces.ISurface<T>` | [Model-15](Model-15.mdpuml) | [ISurface.cs](../../Model/Interfaces/ISurface.cs) |
| `Model.Interfaces.ISurfaceObject<T,U>` | [Model-15](Model-15.mdpuml) | [ISurfaceObject.cs](../../Model/Interfaces/ISurfaceObject.cs) |
| `Model.Interfaces.IVertexObject<T>` | [Model-15](Model-15.mdpuml) | [IVertexObject.cs](../../Model/Interfaces/IVertexObject.cs) |
| `Model.Interfaces.IVolume` | [Model-16](Model-16.mdpuml) | [IVolume.cs](../../Model/Interfaces/IVolume.cs) |
| `Model.Interfaces.ObjType` | [Model-6](Model-6.mdpuml) | [IModelObject.cs](../../Model/Interfaces/IModelObject.cs) |
| `Model.Interfaces.Attributes.WarningAttribute` | [Model-18](Model-18.mdpuml) | [WarningAttribute.cs](../../Model/Interfaces/Attributes/WarningAttribute.cs) |
| `Model.Interfaces.Comparers.ElementSurfaceEqualityComparer<T>` | [Model-4](Model-4.mdpuml) | [EqualityComparerBySurface.cs](../../Model/Interfaces/ObjectsComparers/EqualityComparerBySurface.cs) |
| `Model.Interfaces.IntegralObject.IIntegralObject` | [Model-1](Model-1.mdpuml) | [IIntegralObject.cs](../../Model/Interfaces/IntegralObject/IIntegralObject.cs) |
| `Model.Interfaces.IntegralObject.IIntegralPoint` | [Model-1](Model-1.mdpuml) | [IIntegralPoint.cs](../../Model/Interfaces/IntegralObject/IIntegralPoint.cs) |
| `Model.Interfaces.MeshObjects.ElementKind` | [Model-6](Model-6.mdpuml) | [IElement.cs](../../Model/Interfaces/MeshObjects/IElement.cs) |
| `Model.Interfaces.MeshObjects.IElement` | [Model-1](Model-1.mdpuml) | [IElement.cs](../../Model/Interfaces/MeshObjects/IElement.cs) |
| `Model.Interfaces.MeshObjects.IElement1D` | [Model-1](Model-1.mdpuml) | [IElement1D.cs](../../Model/Interfaces/MeshObjects/IElement1D.cs) |
| `Model.Interfaces.MeshObjects.IElement2D` | [Model-1](Model-1.mdpuml) | [IElement2D.cs](../../Model/Interfaces/MeshObjects/IElement2D.cs) |
| `Model.Interfaces.MeshObjects.IElement3D` | [Model-5](Model-5.mdpuml) | [IElement3D.cs](../../Model/Interfaces/MeshObjects/IElement3D.cs) |
| `Model.Interfaces.MeshObjects.ISurfaceElement` | [Model-5](Model-5.mdpuml) | [ISurfaceElement.cs](../../Model/Interfaces/MeshObjects/ISurfaceElement.cs) |
| `Model.Interfaces.ObjectsCollections.IObjectsCollection<T>` | [Model-16](Model-16.mdpuml) | [IObjectsCollection.cs](../../Model/Interfaces/ObjectsCollections/IObjectsCollection.cs) |
| `Model.Interfaces.ObjectsCollections.IObjectsSet<T>` | [Model-16](Model-16.mdpuml) | [IObjectsSet.cs](../../Model/Interfaces/ObjectsCollections/IObjectsSet.cs) |
| `Model.Interfaces.ObjectsCollections.ISetInfo` | [Model-16](Model-16.mdpuml) | [ISetInfo.cs](../../Model/Interfaces/ObjectsCollections/ISetInfo.cs) |
| `Model.Interfaces.ObjectsComparers.ComparerBySqCoords` | [Model-4](Model-4.mdpuml) | [ComparerBySqCoords.cs](../../Model/Interfaces/ObjectsComparers/ComparerBySqCoords.cs) |
| `Model.Interfaces.ObjectsComparers.EqualityComparerByCentralPoint` | [Model-4](Model-4.mdpuml) | [EqualityComparerByCentralPoint.cs](../../Model/Interfaces/ObjectsComparers/EqualityComparerByCentralPoint.cs) |
| `Model.Interfaces.ObjectsComparers.EqualityComparerByExistState` | [Model-4](Model-4.mdpuml) | [EqualityComparerByExistState.cs](../../Model/Interfaces/ObjectsComparers/EqualityComparerByExistState.cs) |
| `Model.Interfaces.ObjectsComparers.EqualityComparerByNumber<T>` | [Model-4](Model-4.mdpuml) | [EqualityComparerByNumber.cs](../../Model/Interfaces/ObjectsComparers/EqualityComparerByNumber.cs) |
| `Model.Interfaces.ObjectsFinders.ByNumberFinder` | [Model-19](Model-19.mdpuml) | [ByNumberFinder.cs](../../Model/Interfaces/ObjectsFinders/ByNumberFinder.cs) |
| `Model.Interfaces.ObjectsSorters.DistanceSorter` | [Model-19](Model-19.mdpuml) | [DistanceSorter.cs](../../Model/Interfaces/ObjectsSorters/DistanceSorter.cs) |
| `Model.MeshObjects.Beam` | [Model-6](Model-6.mdpuml) | [Beam.cs](../../Model/MeshObjects/Beam.cs) |
| `Model.MeshObjects.Element` | [Model-7](Model-7.mdpuml) | [Element.cs](../../Model/MeshObjects/Element.cs) |
| `Model.MeshObjects.Element2D` | [Model-7](Model-7.mdpuml) | [Element2D.cs](../../Model/MeshObjects/Element2D.cs) |
| `Model.MeshObjects.Element3D` | [Model-7](Model-7.mdpuml) | [Element3D.cs](../../Model/MeshObjects/Element3D.cs) |
| `Model.MeshObjects.ElementSurface` | [Model-7](Model-7.mdpuml) | [ElementSurface.cs](../../Model/MeshObjects/ElementSurface.cs) |
| `Model.MeshObjects.Hexa` | [Model-7](Model-7.mdpuml) | [Hexa.cs](../../Model/MeshObjects/Hexa.cs) |
| `Model.MeshObjects.IntegralPoint` | [Model-7](Model-7.mdpuml) | [IntegralPoint.cs](../../Model/MeshObjects/IntegralPoint.cs) |
| `Model.MeshObjects.Node` | [Model-8](Model-8.mdpuml) | [Node.cs](../../Model/MeshObjects/Node.cs) |
| `Model.MeshObjects.Penta` | [Model-8](Model-8.mdpuml) | [Penta.cs](../../Model/MeshObjects/Penta.cs) |
| `Model.MeshObjects.Quad` | [Model-8](Model-8.mdpuml) | [Quad.cs](../../Model/MeshObjects/Quad.cs) |
| `Model.MeshObjects.Tetra` | [Model-8](Model-8.mdpuml) | [Tetra.cs](../../Model/MeshObjects/Tetra.cs) |
| `Model.MeshObjects.Tetra_c` | [Model-8](Model-8.mdpuml) | [Tetra_c.cs](../../Model/MeshObjects/Tetra_c.cs) |
| `Model.MeshObjects.Triangle` | [Model-8](Model-8.mdpuml) | [Triangle.cs](../../Model/MeshObjects/Triangle.cs) |
| `Model.MeshObjects.Triangle_c` | [Model-9](Model-9.mdpuml) | [Triangle_c.cs](../../Model/MeshObjects/Triangle_c.cs) |
| `Model.ObjectsCollections.ObjectsCollection<T>` | [Model-18](Model-18.mdpuml) | [ObjectsCollection.cs](../../Model/ObjectsCollections/ObjectsCollection.cs) |
| `Model.ObjectsCollections.ObjectsSet<T>` | [Model-18](Model-18.mdpuml) | [ObjectsSet.cs](../../Model/ObjectsCollections/ObjectsSet.cs) |
| `Model.ObjectsCollections.SurfacesSet` | [Model-18](Model-18.mdpuml) | [SurfacesSet .cs](../../Model/ObjectsCollections/SurfacesSet .cs) |
| `Model.Sort.MergeSort<T>` | [Model-19](Model-19.mdpuml) | [MergeSorts.cs](../../Model/Sort/MergeSorts.cs) |
| `Model.Sort.MergeSort` | [Model-19](Model-19.mdpuml) | [MergeSorts.cs](../../Model/Sort/MergeSorts.cs) |
| `Model.Sort.MergeSortWithComparer<T>` | [Model-19](Model-19.mdpuml) | [MergeSorts.cs](../../Model/Sort/MergeSorts.cs) |
| `Model.Utilities.DefaultModelObjsComparer<T>` | [Model-19](Model-19.mdpuml) | [DefaultModelObjsComparer.cs](../../Model/Utilities/DefaultModelObjsComparer.cs) |
| `Model.Utilities.DefaultSetInfoComparer` | [Model-20](Model-20.mdpuml) | [DefaultSetInfoComparer.cs](../../Model/Utilities/DefaultSetInfoComparer.cs) |
| `Model.Utilities.MarkSurfaces` | [Model-20](Model-20.mdpuml) | [MarkSurfaces.cs](../../Model/Utilities/MarkSurfaces.cs) |
| `Model.Utilities.SurfaceNodesComparer` | [Model-20](Model-20.mdpuml) | [SurfaceNodesComparer.cs](../../Model/Utilities/SurfaceNodesComparer.cs) |
| `Model.Utilities.SurfacesCreator` | [Model-20](Model-20.mdpuml) | [SurfacesCreator.cs](../../Model/Utilities/SurfacesCreator.cs) |
| `Model.Utilities.SurfacesFinder` | [Model-20](Model-20.mdpuml) | [SurfacesFinder.cs](../../Model/Utilities/SurfacesFinder.cs) |

## Регистрация изменений

`ObjectsSet.Append`, `ObjectsCollection.Append` и `ModelData.Append` добавляют данные без обратных вызовов. Коллекция возвращает имена созданных наборов, включая пустые и переименованные; модель возвращает имена в порядке, указанном в `IModelData`. После успешного добавления `ProjectController` сам публикует по одному сообщению `Created` для каждого нового набора и добавленной группы, а для изменённых постоянных наборов — по одному `Updated`. При объединении наборов получатель сообщает одно `Updated` после завершения `Append`, независимо от количества добавленных объектов. Пустое добавление в существующий набор сообщения не создаёт.

Ошибка нумерации передаётся исключением; незавершённый `Append` не вызывает уведомлений. Автоматический откат уже изменённых данных не реализован. После исправления массовых операций `Renumber` и `MarkSurfaces.Mark` также не принимают делегатов уведомления. Результат выполненной операции контроллер объединяет по наборам до публикации. Выжившие группы при очистке сохраняют экземпляр, чтобы условия продолжали ссылаться на них.

## Источники и статус

Состав типов и сигнатуры сверены с текущими `.cs` и исключениями `<Compile Remove>` в проекте. Диаграммы описывают целевое размещение событий. Старые `Classes.mdpuml`, схемы отдельных алгоритмов и их изображения имеют статус исторических материалов и не определяют актуальный контракт.
