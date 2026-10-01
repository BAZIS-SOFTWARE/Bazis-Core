using System;

namespace OperationalController
{
    /// <summary>
    /// Передаёт одно изменение состояния проекта.
    /// </summary>
    public sealed class ProjectMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Создаёт аргументы уведомления об изменении.
        /// </summary>
        public ProjectMessageEventArgs(ChangeMessage change)
        {
            Change = change ?? throw new ArgumentNullException(nameof(change));
        }

        public ChangeMessage Change { get; }
    }
}
