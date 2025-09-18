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
using Model.IO.STL;
using Model.IO;

namespace Project
{
    /// <summary>
    /// ProjectData
    /// </summary>
    public class ProjectData : IProjectData
    {

        /// <inheritdoc/>
        public IModelData ModelData { get; set; } = new ModelData();
        /// <inheritdoc/>

        public ITaskData TaskData { get; set; } = new TaskData();

        /// <inheritdoc/>
        public IGeneralData GeneralData { get; set; } = new GeneralData();


        /// <inheritdoc/>

        //public void Load(string projectFile)
        //{
        //    GeneralData.Name = System.IO.Path.GetFileName(projectFile);
        //    GeneralData.Path = System.IO.Path.GetDirectoryName(projectFile); 

        //    Loader.Load(this,projectFile);
        //}
        /// <inheritdoc/>

        public void Save(string fullPath)
        {
            Saver.Save(this,fullPath);
        }
/// <inheritdoc/>

        public void Load(string fullPath)
        {
            Loader.Load(fullPath);
        }

        /// <inheritdoc/>

        public IProjectSaver Saver { get; set; } = new SaveProjectTextFormat();
        /// <inheritdoc/>

        public IProjectLoader Loader { get; set; } = new LoadProjectFromTextFormat();



        /// <summary>
        /// ProjectData
        /// </summary>
        public ProjectData(string projectFile) 
        {
            GeneralData.Name = System.IO.Path.GetFileName(projectFile);
            //GeneralData.Path = System.IO.Path.GetDirectoryName(projectFile);

            var ext = System.IO.Path.GetExtension(projectFile);

            if (ext != ".bpf2" & ext != ".bpf")
            //{
            //    ModelData.Saver = new SaveModelToBPF2TextFile();
            //    ModelData.Loader = new LoadModelFromBPF2TextFile();
            //}

            //else if (ext == ".bpf")
            //{
            //    ModelData.Saver = new SaveModelToBPFTextFile();
            //    ModelData.Loader = new LoadModelFromBPFTextFile();
            //}
            //else
                throw new Exception("неправильно задано расширение файла");
        }
    }
}
