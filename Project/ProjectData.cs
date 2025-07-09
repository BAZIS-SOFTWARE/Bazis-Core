using Model.Interfaces;
using Project.Interfaces;
using Project.IO;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using Model;
using Model.Interfaces.ObjectsCollections;
using System.Collections.Generic;
using Project.Tasks;
using System.Data.SqlTypes;
using Project.Tasks.FrameCreators;
using System.Drawing;
using System;
using Project.Tasks.Functions;
using Model.GroupsData;

namespace Project
{
    /// <summary>
    /// ProjectData
    /// </summary>
    public class ProjectData : IProjectData
    {
        IModelData modelData = new ModelData();
        ITaskData taskData = new TaskData();
        IGeneralData generalData = new GeneralData();
        /// <inheritdoc/>
        public IModelData ModelData { get { return modelData; } } 
        /// <inheritdoc/>

        public ITaskData TaskData { get { return taskData; } } 

        /// <inheritdoc/>

        
/// <inheritdoc/>

        public void Load(string projectFile)
        {
            GeneralData.Name = System.IO.Path.GetFileName(projectFile);
            GeneralData.Path = System.IO.Path.GetDirectoryName(projectFile); 

            Loader.Load(this,projectFile);
        }
/// <inheritdoc/>

        public void Save()
        {
            Saver.Save(this);
        }
/// <inheritdoc/>

        public void Load()
        {
            Loader.Load(this, $@"{GeneralData.Path}\{GeneralData.Name}");
        }

        /// <inheritdoc/>

        public IProjectSaver Saver { get; set; } = new SaveProjectTextFormat();
        /// <inheritdoc/>

        public IProjectLoader Loader { get; set; } = new LoadProjectFromTextFormat();
        /// <inheritdoc/>
        public IGeneralData GeneralData 
        {
            get { return generalData; }
            
        } 

        /// <summary>
        /// ProjectData
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_path"></param>
        public ProjectData(string _name, string _path)
        {
            generalData.Name = _name;
            generalData.Path = _path;
        }
        /// <summary>
        /// ProjectData
        /// </summary>
        public ProjectData() {}
    }
}
