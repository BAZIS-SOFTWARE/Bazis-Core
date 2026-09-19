using System;
using System.Collections.Generic;
using System.Drawing;
using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;

namespace OperationalController
{
    /// <summary>
    /// Хранит разреженное состояние представления объектов одного типа.
    /// </summary>
    public class ObjectsView : IObjectsView
    {
        readonly HashSet<int> hidden = new HashSet<int>();
        readonly HashSet<int> selected = new HashSet<int>();
        readonly Dictionary<ISetInfo, SetStyle> setStyles = new Dictionary<ISetInfo, SetStyle>(ReferenceEqualityComparer.Instance);
        readonly Color[] setColors = [Color.SteelBlue, Color.Orange, Color.MediumSeaGreen, Color.MediumOrchid, Color.Goldenrod, Color.IndianRed];
        int nextSetColor;

        /// <inheritdoc/>
        public bool IsEmpty => hidden.Count == 0 && selected.Count == 0 && setStyles.Count == 0;

        /// <inheritdoc/>
        public int SelectedCount => selected.Count;

        /// <inheritdoc/>
        public int HiddenCount => hidden.Count;

        /// <inheritdoc/>
        public bool IsSelected(int number)
        {
            return selected.Contains(number);
        }

        /// <inheritdoc/>
        public bool IsHidden(int number)
        {
            return hidden.Contains(number);
        }

        /// <inheritdoc/>
        public IEnumerable<int> GetSelected()
        {
            foreach (var number in selected)
                yield return number;
        }

        /// <inheritdoc/>
        public bool Select(IEnumerable<int> numbers)
        {
            return Add(selected, numbers);
        }

        /// <inheritdoc/>
        public bool Deselect(IEnumerable<int> numbers)
        {
            return Remove(selected, numbers);
        }

        /// <inheritdoc/>
        public bool ToggleSelection(IEnumerable<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            var changed = false;
            var processed = new HashSet<int>();
            foreach (var number in numbers)
            {
                if (!processed.Add(number))
                    continue;

                changed = selected.Remove(number) || selected.Add(number) || changed;
            }

            return changed;
        }

        /// <inheritdoc/>
        public bool SetSelection(IEnumerable<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            var next = new HashSet<int>(numbers);
            if (selected.SetEquals(next))
                return false;

            selected.Clear();
            selected.UnionWith(next);
            return true;
        }

        /// <inheritdoc/>
        public bool ClearSelection()
        {
            if (selected.Count == 0)
                return false;

            selected.Clear();
            return true;
        }

        /// <inheritdoc/>
        public bool SetHidden(IEnumerable<int> numbers, bool isHidden)
        {
            return isHidden ? Add(hidden, numbers) : Remove(hidden, numbers);
        }

        /// <inheritdoc/>
        public bool ShowAll()
        {
            if (hidden.Count == 0)
                return false;

            hidden.Clear();
            return true;
        }

        /// <inheritdoc/>
        public Color GetSetColor(ISetInfo setInfo)
        {
            return GetOrCreateSetStyle(setInfo).Color;
        }

        /// <inheritdoc/>
        public bool SetSetColor(ISetInfo setInfo, Color color)
        {
            var style = GetOrCreateSetStyle(setInfo);
            if (style.Color == color)
                return false;

            style.Color = color;
            return true;
        }

        /// <inheritdoc/>
        public ViewMode GetViewMode(ISetInfo setInfo)
        {
            return GetOrCreateSetStyle(setInfo).ViewMode;
        }

        /// <inheritdoc/>
        public bool SetViewMode(ISetInfo setInfo, ViewMode viewMode)
        {
            var style = GetOrCreateSetStyle(setInfo);
            if (style.ViewMode == viewMode)
                return false;

            style.ViewMode = viewMode;
            return true;
        }

        /// <inheritdoc/>
        public bool ClearSelectionAndHidden()
        {
            var changed = ClearSelection();
            return ShowAll() || changed;
        }

        /// <inheritdoc/>
        public bool Prune(Predicate<int> exists)
        {
            if (exists == null)
                throw new ArgumentNullException(nameof(exists));

            var changed = RemoveMissing(selected, exists);
            return RemoveMissing(hidden, exists) || changed;
        }

        /// <inheritdoc/>
        public bool PruneSetStyles(IEnumerable<ISetInfo> existingSets)
        {
            if (existingSets == null)
                throw new ArgumentNullException(nameof(existingSets));

            var existing = new HashSet<ISetInfo>(existingSets, ReferenceEqualityComparer.Instance);
            var removed = new List<ISetInfo>();
            foreach (var setInfo in setStyles.Keys)
                if (!existing.Contains(setInfo))
                    removed.Add(setInfo);

            foreach (var setInfo in removed)
                setStyles.Remove(setInfo);

            return removed.Count > 0;
        }

        /// <summary>
        /// Возвращает существующий или создаёт начальный стиль набора.
        /// </summary>
        private SetStyle GetOrCreateSetStyle(ISetInfo setInfo)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            if (setStyles.TryGetValue(setInfo, out var style))
                return style;

            style = new SetStyle(GetDefaultColor(setInfo.ObjType), GetDefaultViewMode(setInfo.ObjType));
            setStyles.Add(setInfo, style);
            return style;
        }

        /// <summary>
        /// Возвращает начальный цвет набора объектов.
        /// </summary>
        private Color GetDefaultColor(ObjType objType)
        {
            if (objType == ObjType.Узел)
                return Color.Blue;

            if (objType == ObjType.Точка)
                return Color.FromArgb(255, 207, 64);

            var color = setColors[nextSetColor % setColors.Length];
            nextSetColor++;
            return color;
        }

        /// <summary>
        /// Возвращает начальный режим отображения набора объектов.
        /// </summary>
        private ViewMode GetDefaultViewMode(ObjType objType)
        {
            if (objType == ObjType.Узел || objType == ObjType.Точка)
                return ViewMode.Point;

            if (objType == ObjType.Кривая || objType == ObjType.Элемент1D)
                return ViewMode.Line;

            return ViewMode.LineSurface;
        }

        /// <summary>
        /// Добавляет номера в множество.
        /// </summary>
        private bool Add(HashSet<int> numbers, IEnumerable<int> added)
        {
            if (added == null)
                throw new ArgumentNullException(nameof(added));

            var changed = false;
            foreach (var number in added)
                changed = numbers.Add(number) || changed;

            return changed;
        }

        /// <summary>
        /// Удаляет номера из множества.
        /// </summary>
        private bool Remove(HashSet<int> numbers, IEnumerable<int> removed)
        {
            if (removed == null)
                throw new ArgumentNullException(nameof(removed));

            var changed = false;
            foreach (var number in removed)
                changed = numbers.Remove(number) || changed;

            return changed;
        }

        /// <summary>
        /// Удаляет несуществующие номера из множества.
        /// </summary>
        private bool RemoveMissing(HashSet<int> numbers, Predicate<int> exists)
        {
            var missing = new List<int>();
            foreach (var number in numbers)
                if (!exists(number))
                    missing.Add(number);

            return Remove(numbers, missing);
        }
    }
}
