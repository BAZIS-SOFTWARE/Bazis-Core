using Project.Results;
using System;
using System.Collections.Generic;

namespace Project.Interfaces.IO
{
    /// <summary>
    /// IResultsLoader
    /// </summary>
    public interface IResultsLoader
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IEnumerable<Result> Load(string path);
        /// <summary>
        /// LoadEvent
        /// </summary>
        event Action<object, ILoadEventArgs> LoadEvent;
    }
}
