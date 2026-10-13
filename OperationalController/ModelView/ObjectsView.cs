using System;
using System.Collections.Generic;
using System.Drawing;
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
        readonly Dictionary<int, Color> objectColors = new Dictionary<int, Color>();
        readonly Dictionary<ISetInfo, SetStyle> setStyles = new Dictionary<ISetInfo, SetStyle>(ReferenceEqualityComparer.Instance);
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
        public bool TryGetObjectColor(int number, out Color color)
        {
            return objectColors.TryGetValue(number, out color);
        }

        /// <inheritdoc/>
        public bool SetObjectColor(IEnumerable<int> numbers, Color color)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            var changed = false;
            foreach (var number in numbers)
            {
                if (objectColors.TryGetValue(number, out var current) && current == color)
                    continue;

                objectColors[number] = color;
                changed = true;
            }

            return changed;
        }

        /// <inheritdoc/>
        public bool ClearObjectColor(IEnumerable<int> numbers)
        {
            if (numbers == null)
                throw new ArgumentNullException(nameof(numbers));

            var changed = false;
            foreach (var number in numbers)
                changed = objectColors.Remove(number) || changed;

            return changed;
        }

        /// <inheritdoc/>
        public bool ClearObjectColors()
        {
            if (objectColors.Count == 0)
                return false;

            objectColors.Clear();
            return true;
        }

        /// <inheritdoc/>
        public bool TryGetSetColor(ISetInfo setInfo, out Color color)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            if (setStyles.TryGetValue(setInfo, out var style) && style.Color.HasValue)
            {
                color = style.Color.Value;
                return true;
            }

            color = default;
            return false;
        }

        /// <inheritdoc/>
        public bool SetSetColor(ISetInfo setInfo, Color color)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            if (!setStyles.TryGetValue(setInfo, out var style))
                style = AddSetStyle(setInfo);

            if (style.Color.HasValue && style.Color.Value == color)
                return false;

            style.Color = color;
            return true;
        }

        /// <inheritdoc/>
        public bool TryGetViewMode(ISetInfo setInfo, out ViewMode viewMode)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            if (setStyles.TryGetValue(setInfo, out var style) && style.ViewMode.HasValue)
            {
                viewMode = style.ViewMode.Value;
                return true;
            }

            viewMode = default;
            return false;
        }

        /// <inheritdoc/>
        public bool SetViewMode(ISetInfo setInfo, ViewMode viewMode)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            if (!setStyles.TryGetValue(setInfo, out var style))
                style = AddSetStyle(setInfo);

            if (style.ViewMode.HasValue && style.ViewMode.Value == viewMode)
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
            changed = RemoveMissing(hidden, exists) || changed;

            var missingColors = new List<int>();
            foreach (var number in objectColors.Keys)
                if (!exists(number))
                    missingColors.Add(number);

            foreach (var number in missingColors)
                objectColors.Remove(number);

            return missingColors.Count > 0 || changed;
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
        /// Создаёт хранилище переопределений стиля набора.
        /// </summary>
        private SetStyle AddSetStyle(ISetInfo setInfo)
        {
            var style = new SetStyle();
            setStyles.Add(setInfo, style);
            return style;
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
