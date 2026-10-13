using Model.Interfaces.ObjectsCollections;
using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Накапливает наборы, которые нужно перерисовать, до конца пакета обновлений.
    /// </summary>
    /// <remarks>
    /// Изменение одного набора может прийти несколько раз за пакет — накопитель
    /// объединяет их по ИЛИ, поэтому подписчик получает каждый набор один раз
    /// со всеми накопленными видами изменений.
    /// </remarks>
    internal sealed class ChangedSets
    {
        readonly Dictionary<ISetInfo, ModelViewChange> sets = new Dictionary<ISetInfo, ModelViewChange>(ReferenceEqualityComparer.Instance);

        /// <summary>
        /// Есть ли накопленные наборы.
        /// </summary>
        public bool IsEmpty => sets.Count == 0;

        /// <summary>
        /// Добавляет изменение заданному набору.
        /// </summary>
        public void Add(ISetInfo setInfo, ModelViewChange change)
        {
            if (setInfo == null)
                return;

            sets.TryGetValue(setInfo, out var existing);
            sets[setInfo] = existing | change;
        }

        /// <summary>
        /// Создаёт неизменяемые аргументы события из накопленных наборов.
        /// </summary>
        public ModelViewChangedEventArgs CreateEventArgs()
        {
            return new ModelViewChangedEventArgs(sets);
        }

        /// <summary>
        /// Очищает накопитель.
        /// </summary>
        public void Clear()
        {
            sets.Clear();
        }
    }
}
