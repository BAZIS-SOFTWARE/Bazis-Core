using Model.Interfaces.ObjectsCollections;
using System;
using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Описывает изменение состояния представления: какие наборы затронуты и что в них изменилось.
    /// </summary>
    /// <remarks>
    /// Разворачивание изменения в наборы делает <see cref="ModelView"/>, поэтому подписчику
    /// не нужно обходить всю модель: он идёт только по <see cref="GetChangedSets"/>.
    /// </remarks>
    public class ModelViewChangedEventArgs : EventArgs
    {
        readonly Dictionary<ISetInfo, ModelViewChange> sets;

        /// <summary>
        /// Создаёт аргументы события, копируя переданные изменения.
        /// </summary>
        public ModelViewChangedEventArgs(IReadOnlyDictionary<ISetInfo, ModelViewChange> sets)
        {
            if (sets == null)
                throw new ArgumentNullException(nameof(sets));

            this.sets = new Dictionary<ISetInfo, ModelViewChange>(sets, ReferenceEqualityComparer.Instance);
        }

        /// <summary>
        /// Число затронутых наборов.
        /// </summary>
        public int Count => sets.Count;

        /// <summary>
        /// Перечисляет наборы, которые нужно перерисовать.
        /// </summary>
        public IEnumerable<ISetInfo> GetChangedSets()
        {
            foreach (var pair in sets)
                yield return pair.Key;
        }

        /// <summary>
        /// Возвращает изменения заданного набора.
        /// </summary>
        public ModelViewChange GetChanges(ISetInfo setInfo)
        {
            if (setInfo == null)
                throw new ArgumentNullException(nameof(setInfo));

            sets.TryGetValue(setInfo, out var changes);
            return changes;
        }

        /// <summary>
        /// Есть ли у набора хотя бы одно из указанных изменений.
        /// </summary>
        public bool HasAny(ISetInfo setInfo, ModelViewChange changes)
        {
            var setChanges = GetChanges(setInfo);
            return (setChanges & changes) != ModelViewChange.None;
        }
    }
}
