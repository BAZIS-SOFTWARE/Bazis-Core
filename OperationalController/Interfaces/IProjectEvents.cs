using System;

namespace OperationalController
{
    /// <summary>
    /// Предоставляет уведомления об изменениях проекта и его представления.
    /// </summary>
    public interface IProjectEvents
    {
        event EventHandler<ProjectMessageEventArgs> Message;
    }
}
