using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

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
        /// GeneralData
        /// </summary>
        IGeneralData GeneralData { get;  }
        /// <summary>
        /// Save
        /// </summary>
        /// <param name="fullPath"></param>
        void Save(string fullPath);
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="fullPath"></param>
        void Load(string fullPath);
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="projectFile"></param>
        //void Load(string projectFile);
    }
}
