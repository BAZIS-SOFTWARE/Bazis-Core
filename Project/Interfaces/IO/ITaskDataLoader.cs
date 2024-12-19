using System;
using Model.Interfaces;
using Project.Interfaces.Tasks;

namespace Project.Interfaces.IO
{
    /// <summary>
    /// ITaskDataLoader
    /// </summary>
    public interface ITaskDataLoader
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="path"></param>
        /// <param name="groupData"></param>
        /// <returns></returns>
        ITaskData Load(string path, IGroupData groupData);
        /// <summary>
        /// LoadEvent
        /// </summary>
        event Action<object, ILoadEventArgs> LoadEvent;
    }
}
