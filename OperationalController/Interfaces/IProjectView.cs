using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет запросы и команды представления текущей модели.
    /// </summary>
    public interface IProjectView
    {
        Color SelectionColor { get; }
        byte Transparency { get; }
        bool HideInsideSurfaces { get; }
        int SelectedCount { get; }
        IEnumerable<ObjType> SelectedTypes { get; }
        /// <summary>Возвращает итоговый цвет объекта.</summary>
        Color GetColor(ObjType objType, int number);
        /// <summary>Возвращает итоговый цвет объекта заданного набора.</summary>
        Color GetColor(ISetInfo setInfo, int number);
        /// <summary>Возвращает цвет набора.</summary>
        Color GetColor(ISetInfo setInfo);
        /// <summary>Возвращает режим отображения набора.</summary>
        ViewMode GetViewMode(ISetInfo setInfo);
        /// <summary>Возвращает признак видимости объекта.</summary>
        bool GetVisible(ObjType objType, int number);
        /// <summary>Возвращает признак выделения объекта.</summary>
        bool IsSelected(ObjType objType, int number);
        /// <summary>Перечисляет номера выделенных объектов заданного типа.</summary>
        IEnumerable<int> GetSelected(ObjType objType);
        /// <summary>Перечисляет выделенные объекты модели.</summary>
        IEnumerable<IModelObject> GetSelection();
        /// <summary>Определяет единственный тип выделенных объектов.</summary>
        bool TryGetSingleSelectedType(out ObjType objType);
        /// <summary>Устанавливает цвет выделения.</summary>
        void SetSelectionColor(Color color);
        /// <summary>Устанавливает прозрачность отображения.</summary>
        void SetTransparency(byte transparency);
        /// <summary>Устанавливает режим скрытия внутренних поверхностей.</summary>
        void SetHideInsideSurfaces(bool hide);
        /// <summary>Устанавливает цвет набора.</summary>
        void SetColor(ISetInfo setInfo, Color color);
        /// <summary>Устанавливает цвет отдельных объектов.</summary>
        void SetColor(ObjType objType, IEnumerable<int> numbers, Color color);
        /// <summary>Удаляет цвет отдельных объектов.</summary>
        void ClearColor(ObjType objType, IEnumerable<int> numbers);
        /// <summary>Удаляет индивидуальные цвета объектов.</summary>
        void ClearColor();
        /// <summary>Устанавливает режим отображения набора.</summary>
        void SetViewMode(ISetInfo setInfo, ViewMode viewMode);
        /// <summary>Устанавливает видимость объектов.</summary>
        void SetVisible(ObjType objType, IEnumerable<int> numbers, bool visible);
        /// <summary>Скрывает выделенные объекты.</summary>
        void HideSelected();
        /// <summary>Показывает все объекты.</summary>
        void ShowAll();
        /// <summary>Добавляет объекты к выделению.</summary>
        void Select(ObjType objType, IEnumerable<int> numbers);
        /// <summary>Исключает объекты из выделения.</summary>
        void Deselect(ObjType objType, IEnumerable<int> numbers);
        /// <summary>Переключает выделение объектов.</summary>
        void ToggleSelection(ObjType objType, IEnumerable<int> numbers);
        /// <summary>Заменяет выделение объектами заданного типа.</summary>
        void SetSelection(ObjType objType, IEnumerable<int> numbers);
        /// <summary>Очищает выделение объектов заданного типа.</summary>
        void ClearSelection(ObjType objType);
        /// <summary>Очищает выделение всех объектов.</summary>
        void ClearSelection();
        /// <summary>Объединяет уведомления о виде до завершения области.</summary>
        IDisposable BeginViewUpdate();
    }
}
