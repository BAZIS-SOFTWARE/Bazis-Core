using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет пустое представление для операций, которым состояние вида не требуется.
    /// </summary>
    public class NullModelView : IModelView
    {
        /// <summary>
        /// Создаёт пустое представление заданной модели.
        /// </summary>
        public NullModelView(IModelData model)
        {
            Model = model;
            SelectionColor = Color.Red;
            Transparency = byte.MaxValue;
        }

        /// <inheritdoc/>
        public IModelData Model { get; }

        /// <inheritdoc/>
        public Color SelectionColor { get; set; }

        /// <inheritdoc/>
        public byte Transparency { get; set; }

        /// <inheritdoc/>
        public int SelectedCount => 0;

        /// <inheritdoc/>
        public IEnumerable<ObjType> SelectedTypes { get { yield break; } }

        /// <inheritdoc/>
        public event EventHandler Changed
        {
            add { }
            remove { }
        }

        /// <inheritdoc/>
        public Color GetColor(ObjType objType, int number)
        {
            var setInfo = Model?.ObjectData.GetSetInfo(objType, number);
            if (setInfo == null)
                return Color.Empty;

            var color = GetSetColor(setInfo);
            var alpha = color.A * Transparency / byte.MaxValue;
            return Color.FromArgb(alpha, color);
        }

        /// <inheritdoc/>
        public Color GetSetColor(ISetInfo setInfo)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            return setInfo.ObjType == ObjType.Узел ? Color.Blue : Color.DarkGray;
        }

        /// <inheritdoc/>
        public void SetSetColor(ISetInfo setInfo, Color color) { }

        /// <inheritdoc/>
        public ViewMode GetViewMode(ISetInfo setInfo)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            if (setInfo.ObjType == ObjType.Узел || setInfo.ObjType == ObjType.Точка)
                return ViewMode.Point;

            if (setInfo.ObjType == ObjType.Кривая || setInfo.ObjType == ObjType.Элемент1D)
                return ViewMode.Line;

            return ViewMode.LineSurface;
        }

        /// <inheritdoc/>
        public void SetViewMode(ISetInfo setInfo, ViewMode viewMode) { }

        /// <inheritdoc/>
        public bool GetVisible(ObjType objType, int number)
        {
            return true;
        }

        /// <inheritdoc/>
        public void SetVisible(ObjType objType, IEnumerable<int> numbers, bool visible) { }

        /// <inheritdoc/>
        public void HideSelected() { }

        /// <inheritdoc/>
        public void ShowAll() { }

        /// <inheritdoc/>
        public bool IsSelected(ObjType objType, int number)
        {
            return false;
        }

        /// <inheritdoc/>
        public IEnumerable<int> GetSelected(ObjType objType)
        {
            yield break;
        }

        /// <inheritdoc/>
        public IEnumerable<IModelObject> GetSelection()
        {
            yield break;
        }

        /// <inheritdoc/>
        public bool TryGetSingleSelectedType(out ObjType objType)
        {
            objType = default;
            return false;
        }

        /// <inheritdoc/>
        public void Select(ObjType objType, IEnumerable<int> numbers) { }

        /// <inheritdoc/>
        public void Deselect(ObjType objType, IEnumerable<int> numbers) { }

        /// <inheritdoc/>
        public void ToggleSelection(ObjType objType, IEnumerable<int> numbers) { }

        /// <inheritdoc/>
        public void SetSelection(ObjType objType, IEnumerable<int> numbers) { }

        /// <inheritdoc/>
        public void ClearSelection(ObjType objType) { }

        /// <inheritdoc/>
        public void ClearSelection() { }

        /// <inheritdoc/>
        public void Prune() { }

        /// <inheritdoc/>
        public void Reset() { }

        /// <inheritdoc/>
        public IDisposable BeginUpdate()
        {
            return new EmptyUpdateScope();
        }

        /// <summary>
        /// Представляет пустой пакет изменений.
        /// </summary>
        private sealed class EmptyUpdateScope : IDisposable
        {
            /// <inheritdoc/>
            public void Dispose() { }
        }
    }
}
