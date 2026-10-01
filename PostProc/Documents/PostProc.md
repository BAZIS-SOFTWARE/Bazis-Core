# PostProc

[PostProc.mdpuml](PostProc.mdpuml) показывает `PostProcController`, цветовые интервалы
`ItemRange` и предоставляемые им службы. [PostProc-1.mdpuml](PostProc-1.mdpuml) раскрывает
экспорт результатов в TXT/CSV и поверхностной сетки в BPF/STL.
[PostProc-2.mdpuml](PostProc-2.mdpuml) показывает построение цветных фигур из значений
`Result` и геометрии сетки. Обе области отмечены на основной диаграмме.

В исходниках есть `IPostProcController`, но `PostProcController` сейчас не указывает этот
интерфейс в объявлении класса; диаграмма не изображает реализацию. `ResultsFieldsCreator`
использует `SurfaceGradientsCreator` для поверхностей. Ветка создания линейных объектов
пока выбрасывает `NotImplementedException`; схема показывает зависимость, а не готовую
функцию. Тип `Result` принадлежит проекту `ResultDB`.

Исходники: [PostProcController.cs](../PostProcController.cs),
[IPostProcController.cs](../IPostProcController.cs),
[ResultsFieldsCreator.cs](../ScenePresenter/ResultsFieldsCreator.cs).
