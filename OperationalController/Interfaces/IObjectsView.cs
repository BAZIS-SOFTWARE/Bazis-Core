using System;
using System.Collections.Generic;
using System.Drawing;
using Model.Interfaces.ObjectsCollections;

namespace OperationalController
{
    /// <summary>
    /// Хранит состояние представления объектов одного типа.
    /// </summary>
    public interface IObjectsView
    {
        /// <summary>
        /// Возвращает признак отсутствия сохранённого состояния.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Возвращает число выделенных объектов.
        /// </summary>
        int SelectedCount { get; }

        /// <summary>
        /// Возвращает число скрытых объектов.
        /// </summary>
        int HiddenCount { get; }

        /// <summary>
        /// Возвращает признак выделения объекта.
        /// </summary>
        bool IsSelected(int number);

        /// <summary>
        /// Возвращает признак скрытости объекта.
        /// </summary>
        bool IsHidden(int number);

        /// <summary>
        /// Возвращает номера выделенных объектов.
        /// </summary>
        IEnumerable<int> GetSelected();

        /// <summary>
        /// Добавляет номера к выделению.
        /// </summary>
        bool Select(IEnumerable<int> numbers);

        /// <summary>
        /// Удаляет номера из выделения.
        /// </summary>
        bool Deselect(IEnumerable<int> numbers);

        /// <summary>
        /// Переключает состояние выделения номеров.
        /// </summary>
        bool ToggleSelection(IEnumerable<int> numbers);

        /// <summary>
        /// Заменяет выделение указанными номерами.
        /// </summary>
        bool SetSelection(IEnumerable<int> numbers);

        /// <summary>
        /// Очищает выделение.
        /// </summary>
        bool ClearSelection();

        /// <summary>
        /// Устанавливает скрытость объектов.
        /// </summary>
        bool SetHidden(IEnumerable<int> numbers, bool hidden);

        /// <summary>
        /// Делает видимыми все объекты.
        /// </summary>
        bool ShowAll();

        /// <summary>
        /// Возвращает цвет заданного набора.
        /// </summary>
        Color GetSetColor(ISetInfo setInfo);

        /// <summary>
        /// Устанавливает цвет заданного набора.
        /// </summary>
        bool SetSetColor(ISetInfo setInfo, Color color);

        /// <summary>
        /// Возвращает режим отображения заданного набора.
        /// </summary>
        ViewMode GetViewMode(ISetInfo setInfo);

        /// <summary>
        /// Устанавливает режим отображения заданного набора.
        /// </summary>
        bool SetViewMode(ISetInfo setInfo, ViewMode viewMode);

        /// <summary>
        /// Очищает выделение и скрытость, сохраняя стили наборов.
        /// </summary>
        bool ClearSelectionAndHidden();

        /// <summary>
        /// Удаляет номера, для которых предикат существования возвращает <c>false</c>.
        /// </summary>
        bool Prune(Predicate<int> exists);

        /// <summary>
        /// Удаляет стили наборов, отсутствующих в модели.
        /// </summary>
        bool PruneSetStyles(IEnumerable<ISetInfo> existingSets);
    }
}
