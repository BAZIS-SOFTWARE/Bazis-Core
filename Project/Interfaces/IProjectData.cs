using Model.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using System;

namespace Project.Interfaces
{
    /// <summary>
    /// IProjectData
    /// </summary>
    public interface IProjectData
    {
        //public event Action DeleteAllGroupsEvent;
        //public event Action DeleteGroupEvent;
        //public event Action DeleteObjectsEvent;
        //public event Action DeleteSelectedObjectsEvent;




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
        string ResultDB { get; set; }
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
        /// <summary>
        /// ChangeTaskType
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        void ChangeTaskType(TaskType taskType);
    }
}
