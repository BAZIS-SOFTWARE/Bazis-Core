using System;
using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Передаёт согласованный пакет изменений состояния проекта.
    /// </summary>
    public sealed class ProjectMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Создаёт аргументы уведомления об изменениях.
        /// </summary>
        public ProjectMessageEventArgs(IReadOnlyList<ChangeMessage> changes)
        {
            if (changes == null)
                throw new ArgumentNullException(nameof(changes));
            if (changes.Count == 0)
                throw new ArgumentException("At least one change is required.", nameof(changes));
            Changes = new List<ChangeMessage>(changes).AsReadOnly();
        }

        public IReadOnlyList<ChangeMessage> Changes { get; }
    }
}
