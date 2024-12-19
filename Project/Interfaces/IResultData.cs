using Project.Interfaces.IO;
using System.Collections.Generic;
using System.Data;

namespace Project.Interfaces
{
    /// <summary>
    /// IResultData
    /// </summary>
    public interface IResultData : IList<IResult>
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
        void AddRange(IEnumerable<IResult> results);
        /// <summary>
        /// AddRange
        /// </summary>
        /// <param name="results"></param>
        /// <param name="comparer"></param>
        void AddRange(IEnumerable<IResult> results, IEqualityComparer<IResult> comparer);
        /// <summary>
        /// FindByTimeInterval
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="firstTime"></param>
        /// <param name="lastTime"></param>
        /// <returns></returns>
        IEnumerable<IResult> FindByTimeInterval(string taskKind, float firstTime, float lastTime);
        /// <summary>
        /// FindByTime
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        IResult FindByTime(TaskKind taskKind, float time);
        /// <summary>
        /// FindByTime
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        IResult FindByTime(TaskKind taskKind, float time, float delta);
        /// <summary>
        /// FindByTime
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        IResult FindByTime(string taskKind, float time);
        /// <summary>
        /// FindByTime
        /// </summary>
        /// <param name="taskKind"></param>
        /// <param name="time"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        IResult FindByTime(string taskKind, float time, float delta);
        /// <summary>
        /// FindByTaskKind
        /// </summary>
        /// <param name="taskKind"></param>
        /// <returns></returns>
        IEnumerable<IResult> FindByTaskKind(string taskKind);
        /// <summary>
        /// FindByTaskKind
        /// </summary>
        /// <param name="taskKind"></param>
        /// <returns></returns>
        IEnumerable<IResult> FindByTaskKind(TaskKind taskKind);

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
