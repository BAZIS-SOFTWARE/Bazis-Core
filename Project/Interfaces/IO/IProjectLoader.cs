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
        /// <param name="path"></param>
        /// <returns></returns>
        void Load(IProjectData projectData, string path);
        /// <summary>
        /// LoadEvent
        /// </summary>
        event Action<object, ILoadEventArgs> LoadEvent;
    }
}