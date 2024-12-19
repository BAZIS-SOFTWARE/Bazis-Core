using System;
using Project.Interfaces.Tasks;

namespace Project.Interfaces.IO
{
    /// <summary>
    /// ITaskDataSaver
    /// </summary>
    public interface ITaskDataSaver
    {
        /// <summary>
        /// SaveEvent
        /// </summary>
        event Action<object, ISaveEventArgs> SaveEvent;
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="project"></param>
        void Save(ITaskData project, string path);
    }
}
