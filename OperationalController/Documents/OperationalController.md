# OperationalController

`IProjectController` объединяет управление сеансом, операции с моделью, настройки вида и
уведомления об изменениях. Клиент хранит ссылку на контроллер и подписывается на
`IProjectEvents.Message`. Формат изменений подробно описан в [ProjectEvents.mdpuml](ProjectEvents.mdpuml),
операции контроллера — в [ProjectController.md](ProjectController.md).
`IGeometryQueries` также предоставляет номера смежных геометрических сущностей, которые
нужны инструментам построения и предварительного просмотра в GUI.

`IProjectSession.Name` доступно только для чтения. Для смены имени применяется
`ChangeProjectName(name)`. Если имя действительно изменилось, контроллер отправляет
`ProjectChange(PropertyChanged, Name)` через `Message`.

Внутренний `ModelView` не передаётся клиенту. Команды представления доступны через
`IProjectView`; при загрузке и создании проекта контроллер переносит общие настройки вида
в новый экземпляр `ModelView`. Процент чтения файла передаётся отдельно через
`IProgress<int>` для `Open` и `Append`.

[OperationalController.mdpuml](OperationalController.mdpuml) показывает основной контракт и
службы проекта. Контракты сетки, геометрии, групп и условий сведены в подписанные блоки,
чтобы основная схема оставалась читаемой. Вынесенные области отмечены на ней:

- [OperationalController-1.mdpuml](OperationalController-1.mdpuml) — внутренний вид и
  типизированные сообщения; подробные схемы — [ModelView.mdpuml](ModelView.mdpuml) и
  [ProjectEvents.mdpuml](ProjectEvents.mdpuml).
- [OperationalController-2.mdpuml](OperationalController-2.mdpuml) — извлечение границ
  сетки, экструзия и создание презентаторов для клиентов.

`ProjectController` создаёт и хранит `ModelView`, загрузчик модели и контроллер Gmsh.
`PresentersCreator` выбирает реализацию `IObjsPresenter` для типа отображаемого объекта.

Исходники: [ProjectController.cs](../ProjectController.cs),
[IProjectController.cs](../Interfaces/IProjectController.cs),
[ModelView.cs](../ModelView/ModelView.cs).
