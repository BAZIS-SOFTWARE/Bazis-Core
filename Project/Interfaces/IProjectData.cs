using Model.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;

namespace Project.Interfaces
{
    /// <summary>
    /// IProjectData
    /// </summary>
    public interface IProjectData
    {
        /// <summary>
        /// Saver
        /// </summary>
        IProjectSaver Saver { get; set; }
        /// <summary>
        /// Loader
        /// </summary>
        IProjectLoader Loader { get; set; }
        /// <summary>
        /// Model
        /// </summary>
        IModelData ModelData { get; set; }
        /// <summary>
        /// TaskData
        /// </summary>
        ITaskData TaskData { get; set; }
        /// <summary>
        /// ResultData
        /// </summary>
        IResultData ResultData { get; set; }
        /// <summary>
        /// GeneralData
        /// </summary>
        IGeneralData GeneralData { get; set; }
        /// <summary>
        /// ClearAllData
        /// </summary>
        void ClearAllData();

        /// <summary>
        /// Save
        /// </summary>
        void Save();
        /// <summary>
        /// Load
        /// </summary>
        void Load();
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="projectFile"></param>
        void Load(string projectFile);
    }
}
