# Model

[Model.mdpuml](Model.mdpuml) показывает агрегат `ModelData`: данные объектов, группы и объёмы.
`ObjectsData` хранит отдельные наборы геометрических объектов и узлов, а также коллекции
наборов элементов 1D, 2D и 3D. Иерархия конкретных объектов вынесена в
[Model-1.mdpuml](Model-1.mdpuml).

[Model-2.mdpuml](Model-2.mdpuml) показывает интерфейсы чтения и сохранения и основные
реализации форматов. Параметр `IProgress<int>` в `IModelLoader.Load` передаёт процент чтения;
`ProgressTextReader` используется текстовыми загрузчиками. На основной схеме обе вынесенные
области помечены именами поддиаграмм.

Старый файл [Classes.mdpuml](Classes.mdpuml) оставлен как историческая схема; для текущего
состава классов следует использовать `Model.mdpuml` и поддиаграммы.

Исходники: [ModelData.cs](../ModelData.cs), [ObjectsData.cs](../ObjectsData.cs),
[IModelLoader.cs](../Interfaces/IModelLoader.cs).
