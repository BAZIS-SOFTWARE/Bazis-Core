using Project.Results;
using System;
using System.Collections.Generic;
using System.Data;

namespace Project.Interfaces.IO
{
    /// <summary>
    /// IResultsLoader
    /// </summary>
    public interface IResultsLoader
    {
        /// <summary>
        /// LoadEvent
        /// </summary>
        event Action<object, ILoadEventArgs> LoadEvent;

        /// <summary>
        /// GetResultsFromDb
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="tables"></param>
        /// <returns></returns>
        IEnumerable<Result> GetResults(string fullPath, List<string> tables);
        /// <summary>
        /// GetResultFromDb
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="tables"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Result GetResult(string fullPath, List<string> tables, float time);
        /// <summary>
        /// GetValuesFromDb. Возвращает только уникальные значения.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        IEnumerable<float> GetValues(string fullPath, string tableName, string column);
        /// <summary>
        /// GetTablesNames
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        IEnumerable<string> GetTablesNames(string fullPath);
        /// <summary>
        /// GetTableSchema
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, List<string>>> GetTablesSchemes(string fullPath);
    }
}
