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
        /// <summary>
        /// GetResultFromDb
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="tableName"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Result GetResultFromDb(string fullPath, string tableName);
        /// <summary>
        /// GetValuesFromDb. Возвращает только уникальные значения.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        IEnumerable<float> GetValuesFromDb(string fullPath, string tableName, string column);
    }
}
