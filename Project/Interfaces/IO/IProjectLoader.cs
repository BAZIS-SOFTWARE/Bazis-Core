using System;

namespace Project.Interfaces.IO
{
    /// <summary>
    /// IProjectLoader
    /// </summary>
    public interface IProjectLoader
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        IProjectData Load(string fullPath);
        /// <summary>
        /// LoadEvent
        /// </summary>
        event Action<object, ILoadEventArgs> LoadEvent;
    }
}