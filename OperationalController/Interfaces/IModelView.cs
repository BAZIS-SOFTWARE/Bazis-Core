using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет состояние представления объектов расчётной модели.
    /// </summary>
    public interface IModelView
    {
        /// <summary>
        /// Модель, для объектов которой хранится состояние представления.
        /// </summary>
        IModelData Model { get; }

        /// <summary>
        /// Цвет выделенных объектов.
        /// </summary>
        Color SelectionColor { get; set; }

        /// <summary>
        /// Значение альфа-канала цвета объектов.
        /// </summary>
        byte Transparency { get; set; }

        /// <summary>
        /// Скрывать ли грани объёмных элементов, закрытые соседними элементами.
        /// Признак общемодельный: грань может быть общей для элементов из разных наборов,
        /// и пер-сетовый флаг дал бы для одного куска геометрии два противоречивых ответа.
        /// </summary>
        bool HideInsideSurfaces { get; set; }

        /// <summary>
        /// Возвращает итоговый цвет объекта.
        /// </summary>
        Color GetColor(ObjType objType, int number);

        /// <summary>
        /// Возвращает итоговый цвет объекта заданного набора.
        /// </summary>
        Color GetColor(ISetInfo setInfo, int number);

        /// <summary>
        /// Возвращает цвет заданного набора.
        /// </summary>
        Color GetColor(ISetInfo setInfo);

        /// <summary>
        /// Устанавливает цвет заданного набора.
        /// </summary>
        void SetColor(ISetInfo setInfo, Color color);

        /// <summary>
        /// Устанавливает индивидуальный цвет объектов.
        /// </summary>
        void SetColor(ObjType objType, IEnumerable<int> numbers, Color color);

        /// <summary>
        /// Снимает индивидуальную окраску объектов.
        /// </summary>
        void ClearColor(ObjType objType, IEnumerable<int> numbers);

        /// <summary>
        /// Возвращает режим отображения заданного набора.
        /// </summary>
        ViewMode GetViewMode(ISetInfo setInfo);

        /// <summary>
        /// Устанавливает режим отображения заданного набора.
        /// </summary>
        void SetViewMode(ISetInfo setInfo, ViewMode viewMode);

        /// <summary>
        /// Возвращает признак видимости объекта.
        /// </summary>
        bool GetVisible(ObjType objType, int number);

        /// <summary>
        /// Устанавливает видимость объектов.
        /// </summary>
        void SetVisible(ObjType objType, IEnumerable<int> numbers, bool visible);

        /// <summary>
        /// Скрывает все выделенные объекты и снимает с них выделение.
        /// </summary>
        void HideSelected();

        /// <summary>
        /// Делает видимыми все объекты.
        /// </summary>
        void ShowAll();

        /// <summary>
        /// Возвращает признак выделения объекта.
        /// </summary>
        bool IsSelected(ObjType objType, int number);

        /// <summary>
        /// Возвращает номера выделенных объектов заданного типа.
        /// </summary>
        IEnumerable<int> GetSelected(ObjType objType);

        /// <summary>
        /// Возвращает выделенные объекты модели.
        /// </summary>
        IEnumerable<IModelObject> GetSelection();

        /// <summary>
        /// Возвращает типы, в которых есть выделенные объекты.
        /// </summary>
        IEnumerable<ObjType> SelectedTypes { get; }

        /// <summary>
        /// Возвращает число выделенных объектов.
        /// </summary>
        int SelectedCount { get; }

        /// <summary>
        /// Пытается получить единственный тип выделенных объектов.
        /// </summary>
        bool TryGetSingleSelectedType(out ObjType objType);

        /// <summary>
        /// Добавляет объекты к выделению.
        /// </summary>
        void Select(ObjType objType, IEnumerable<int> numbers);

        /// <summary>
        /// Исключает объекты из выделения.
        /// </summary>
        void Deselect(ObjType objType, IEnumerable<int> numbers);

        /// <summary>
        /// Переключает состояние выделения объектов.
        /// </summary>
        void ToggleSelection(ObjType objType, IEnumerable<int> numbers);

        /// <summary>
        /// Заменяет выделение заданными объектами одного типа.
        /// </summary>
        void SetSelection(ObjType objType, IEnumerable<int> numbers);

        /// <summary>
        /// Снимает выделение объектов заданного типа.
        /// </summary>
        void ClearSelection(ObjType objType);

        /// <summary>
        /// Снимает выделение всех объектов.
        /// </summary>
        void ClearSelection();

        /// <summary>
        /// Удаляет из состояния представления номера отсутствующих объектов модели.
        /// </summary>
        void Prune();

        /// <summary>
        /// Очищает состояние представления.
        /// </summary>
        void Reset();

        /// <summary>
        /// Открывает пакет изменений представления.
        /// </summary>
        IDisposable BeginUpdate();

        /// <summary>
        /// Возникает после изменения состояния представления. Аргументы перечисляют
        /// наборы, которые нужно перерисовать, и сообщают, что в каждом изменилось
        /// (<see cref="ModelViewChange"/>). Разворачивание изменения в наборы делает
        /// само представление, поэтому подписчику не нужно обходить модель:
        /// см. <see cref="ModelViewChangedEventArgs.GetChangedSets"/>.
        ///
        /// Изменения, заданные номерами объектов, помечают только те наборы, которым
        /// эти номера принадлежат. Операции над типом целиком (снятие выделения,
        /// показ всех, подрезка, сброс) помечают все наборы типа, а прозрачность,
        /// цвет выделения и скрытие внутренних граней — все наборы модели.
        /// </summary>
        event EventHandler<ModelViewChangedEventArgs> Changed;
    }
}
