using Model.Interfaces;
using Project.Interfaces.IO;
using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using System;
using System.Collections.Generic;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// TaskType
    /// </summary>
    //[Flags]
    public enum TaskType
    {
        /// <summary>
        /// 1D case
        /// </summary>
        Linear = 2,
        /// <summary>
        /// 2D case
        /// </summary>
        Plain = 4,
        /// <summary>
        /// 2d axi simmetry case
        /// </summary>
        AxiPlain = 8,
        /// <summary>
        /// 3d case
        /// </summary>
        Volume = 16,
        /// <summary>
        /// 3d case with plates and beams
        /// </summary>
        Volume_mixed = 32,
    }

    /// <summary>
    /// Задачи для препроцессора для создания пользовательских задач
    /// </summary>
    [Flags]
    public enum TaskKind : int
    {
        /// <summary>
        /// ХИМИЧЕСКАЯ задача с одиночным процессом.
        /// Один тип файлов задач
        /// </summary>
        химическая = 2,
        /// <summary>
        /// ТЕРМИЧЕСКАЯ задача с одиночным процессом.
        /// Один тип файлов задач
        /// </summary>
        термическая = 4,
        /// <summary>
        /// МЕХАНИЧЕСКАЯ задача с одиночным процессом.
        /// Один тип файлов задач
        /// </summary>
        механическая = 8
    };
    public interface ITaskData : IList<ICondData>
    {
        /// <summary>
        /// Materials
        /// </summary>
        MaterialDBData Materials { get; set; }
        /// <summary>
        /// Functions
        /// </summary>
        FunctionDBData Functions { get; set; }
        /// <summary>
        /// TaskType
        /// </summary>
        TaskType TaskType { get; set; }
        /// <summary>
        /// TaskKind
        /// </summary>
        TaskKind TaskKind { get; set; }
        /// <summary>
        /// Find by group name
        /// </summary>
        /// <param name="grName"></param>
        /// <returns></returns>
        IEnumerable<ICondData> Find(string grName);
        /// <summary>
        /// Find all data according to name
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        IEnumerable<ICondData> Find(DataKind kind);

        /// <summary>
        /// Find all data as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> Find<T>() where T : ICondData;
        /// <summary>
        /// Remove range
        /// </summary>
        /// <param name="dataInfo"></param>
        void RemoveRange(IEnumerable<string> dataInfo);
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="projectFile"></param>
        void Save(string projectFile);
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="projectFile"></param>
        void Load(string projectFile, IGroupData groupData);
        /// <summary>
        /// Name
        /// </summary>
        void AddRange(IEnumerable<ICondData> datas);
        /// <summary>
        /// Save task data
        /// </summary>
        ITaskDataSaver Saver { get; set; }
        /// <summary>
        /// Load task data
        /// </summary>
        ITaskDataLoader Loader { get; set; }
        /// <summary>
        /// ClearNotExisted
        /// </summary>
        /// <param name="gropData"></param>
        void ClearNotExisted(IGroupData gropData);
        /// <summary>
        /// Create data by name (dataType)
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>           
        void ClearNotExisted(IGroup group);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="kind"></param>
        /// <param name="dataInfo"></param>
        /// <param name="groupData"></param>
        /// <returns></returns>
        ICondData Create(DataKind kind, string dataInfo, IGroupData groupData);
        /// <summary>
        /// Contains
        /// </summary>
        /// <param name="grName"></param>
        /// <returns></returns>
        bool Contains(string grName);
    }
}
