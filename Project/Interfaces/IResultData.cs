using Project.Interfaces.IO;
using Project.Results;
using System.Collections.Generic;
using System.Data;

namespace Project.Interfaces
{
    /// <summary>
    /// IResultData
    /// </summary>
    public interface IResultData : IList<Result>
    {
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="path"></param>
        void Load(string path);

        List<TaskKind> GetResultKinds();
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="time"></param>
        /// <param name="data"></param>
        /// <param name="taskKind"></param>
        void Add(float time, DataSet data, TaskKind taskKind);
        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="results"></param>
        void AddRange(IEnumerable<Result> results);
        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="results"></param>
        /// <param name="comparer"></param>
        void AddRange(IEnumerable<Result> results, IEqualityComparer<Result> comparer);
        /// <summary>
        /// FindByTimeInterval
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="firstTime"></param>
        /// <param name="lastTime"></param>
        /// <returns></returns>
        IEnumerable<Result> FindByTimeInterval(string taskKind, float firstTime, float lastTime);
        /// <summary>
        /// FindByTime
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Result FindByTime(TaskKind taskKind, float time);
        /// <summary>
        /// FindByTime
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        Result FindByTime(TaskKind taskKind, float time, float delta);
        /// <summary>
        /// FindByTime
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        Result FindByTime(string taskKind, float time);
        /// <summary>
        /// FindByTime
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        Result FindByTime(string taskKind, float time, float delta);
        /// <summary>
        /// FindByTaskKind
        /// </summary>
        /// <param name="taskKind"></param>
        /// <returns></returns>
        IEnumerable<Result> FindByTaskKind(string taskKind);
        /// <summary>
        /// FindByTaskKind
        /// </summary>
        /// <param name="taskKind"></param>
        /// <returns></returns>
        IEnumerable<Result> FindByTaskKind(TaskKind taskKind);

        /// <summary>
        /// Save task data
        /// </summary>
        IResultsSaver Saver { get; set; }
        /// <summary>
        /// Load task data
        /// </summary>
        IResultsLoader Loader { get; set; }
    }
}
