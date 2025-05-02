using Model.Interfaces;
using Project.Interfaces.IO;
using System.Collections.Generic;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// TaskType
    /// </summary>
    public enum TaskType
    {
        /// <summary>
        /// 1D case
        /// </summary>
        Linear = 1,
        /// <summary>
        /// 2D case
        /// </summary>
        Plain = 2,
        /// <summary>
        /// 2d axi simmetry case
        /// </summary>
        AxiPlain = 3,
        /// <summary>
        /// 3d case
        /// </summary>
        Volume = 4
    }
    public interface ITaskData : IList<IPhysicalData>
    {
        /// <summary>
        /// Find all data according to name
        /// </summary>
        /// <param name="kind"></param>
        /// <returns></returns>
        IEnumerable<IPhysicalData> Find(DataKind kind);

        /// <summary>
        /// Find all data as T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> Find<T>() where T : IPhysicalData;
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
        void AddRange(IEnumerable<IPhysicalData> datas);
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
        /// <param name="name"></param>
        /// <param name="dataInfo"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        IPhysicalData Create(string name, string dataInfo, IGroup group);
    }
}
