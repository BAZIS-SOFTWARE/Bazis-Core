using System;
using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Хранит разреженное состояние представления объектов одного типа.
    /// </summary>
    public class ObjectsView : IObjectsView
    {
        readonly HashSet<int> hidden = new HashSet<int>();
        readonly HashSet<int> selected = new HashSet<int>();

        /// <inheritdoc/>
        public bool IsEmpty => hidden.Count == 0 && selected.Count == 0;

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
        public bool Prune(Predicate<int> exists)
        {
            if (exists == null)
                throw new ArgumentNullException(nameof(exists));

            var changed = RemoveMissing(selected, exists);
            return RemoveMissing(hidden, exists) || changed;
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
