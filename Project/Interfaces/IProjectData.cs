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
        /// DeleteGroup
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumbs"></param>
        void DeleteMeshObjects(ObjType objType, IEnumerable<int> objNumbs);

        /// <summary>
        /// DeleteMeshGroup
        /// </summary>
        /// <param name="grName"></param>
        void DeleteMeshGroup(string grName);
        /// <summary>
        /// DeleteMeshSet
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="name"></param>
        void DeleteMeshSet(ObjType objType, string name);

        /// <summary>
        /// ClearMeshCollection
        /// </summary>
        /// <param name="objType"></param>
        void ClearMeshCollection(ObjType objType);
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
        IModelData ModelData { get;}
        /// <summary>
        /// TaskData
        /// </summary>
        ITaskData TaskData { get; }
        /// <summary>
        /// GeneralData
        /// </summary>
        IGeneralData GeneralData { get;  }
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
