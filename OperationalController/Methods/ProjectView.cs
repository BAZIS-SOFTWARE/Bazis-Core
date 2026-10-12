using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController
{
    public partial class ProjectController
    {
        public Color SelectionColor => modelView.SelectionColor;
        public byte Transparency => modelView.Transparency;
        public bool HideInsideSurfaces => modelView.HideInsideSurfaces;
        public int SelectedCount => modelView.SelectedCount;
        public IEnumerable<ObjType> SelectedTypes => modelView.SelectedTypes;

        /// <summary>Возвращает итоговый цвет объекта.</summary>
        public Color GetColor(ObjType objType, int number) => modelView.GetColor(objType, number);

        /// <summary>Возвращает итоговый цвет объекта заданного набора.</summary>
        public Color GetColor(ISetInfo setInfo, int number) => modelView.GetColor(setInfo, number);

        /// <summary>Возвращает цвет набора.</summary>
        public Color GetColor(ISetInfo setInfo) => modelView.GetColor(setInfo);

        /// <summary>Возвращает режим отображения набора.</summary>
        public ViewMode GetViewMode(ISetInfo setInfo) => modelView.GetViewMode(setInfo);

        /// <summary>Возвращает признак видимости объекта.</summary>
        public bool GetVisible(ObjType objType, int number) => modelView.GetVisible(objType, number);

        /// <summary>Возвращает признак выделения объекта.</summary>
        public bool IsSelected(ObjType objType, int number) => modelView.IsSelected(objType, number);

        /// <summary>Перечисляет номера выделенных объектов заданного типа.</summary>
        public IEnumerable<int> GetSelected(ObjType objType)
        {
            foreach (var number in modelView.GetSelected(objType))
                yield return number;
        }

        /// <summary>Перечисляет выделенные объекты модели.</summary>
        public IEnumerable<IModelObject> GetSelection()
        {
            foreach (var modelObject in modelView.GetSelection())
                yield return modelObject;
        }

        /// <summary>Определяет единственный тип выделенных объектов.</summary>
        public bool TryGetSingleSelectedType(out ObjType objType) => modelView.TryGetSingleSelectedType(out objType);

        /// <summary>Устанавливает цвет выделения.</summary>
        public void SetSelectionColor(Color color) => modelView.SelectionColor = color;

        /// <summary>Устанавливает прозрачность отображения.</summary>
        public void SetTransparency(byte transparency) => modelView.Transparency = transparency;

        /// <summary>Устанавливает режим скрытия внутренних поверхностей.</summary>
        public void SetHideInsideSurfaces(bool hide) => modelView.HideInsideSurfaces = hide;

        /// <summary>Устанавливает цвет набора.</summary>
        public void SetColor(ISetInfo setInfo, Color color) => modelView.SetColor(setInfo, color);

        /// <summary>Устанавливает цвет отдельных объектов.</summary>
        public void SetColor(ObjType objType, IEnumerable<int> numbers, Color color) => modelView.SetColor(objType, numbers, color);

        /// <summary>Удаляет цвет отдельных объектов.</summary>
        public void ClearColor(ObjType objType, IEnumerable<int> numbers) => modelView.ClearColor(objType, numbers);

        /// <summary>Удаляет индивидуальные цвета объектов.</summary>
        public void ClearColor() => modelView.ClearColor();

        /// <summary>Устанавливает режим отображения набора.</summary>
        public void SetViewMode(ISetInfo setInfo, ViewMode viewMode) => modelView.SetViewMode(setInfo, viewMode);

        /// <summary>Устанавливает видимость объектов.</summary>
        public void SetVisible(ObjType objType, IEnumerable<int> numbers, bool visible) => modelView.SetVisible(objType, numbers, visible);

        /// <summary>Скрывает выделенные объекты.</summary>
        public void HideSelected() => modelView.HideSelected();

        /// <summary>Показывает все объекты.</summary>
        public void ShowAll() => modelView.ShowAll();

        /// <summary>Добавляет объекты к выделению.</summary>
        public void Select(ObjType objType, IEnumerable<int> numbers) => modelView.Select(objType, numbers);

        /// <summary>Исключает объекты из выделения.</summary>
        public void Deselect(ObjType objType, IEnumerable<int> numbers) => modelView.Deselect(objType, numbers);

        /// <summary>Переключает выделение объектов.</summary>
        public void ToggleSelection(ObjType objType, IEnumerable<int> numbers) => modelView.ToggleSelection(objType, numbers);

        /// <summary>Заменяет выделение объектами заданного типа.</summary>
        public void SetSelection(ObjType objType, IEnumerable<int> numbers) => modelView.SetSelection(objType, numbers);

        /// <summary>Очищает выделение объектов заданного типа.</summary>
        public void ClearSelection(ObjType objType) => modelView.ClearSelection(objType);

        /// <summary>Очищает выделение всех объектов.</summary>
        public void ClearSelection() => modelView.ClearSelection();

        /// <summary>Объединяет уведомления о виде до завершения области.</summary>
        public IDisposable BeginViewUpdate() => modelView.BeginUpdate();
    }
}
