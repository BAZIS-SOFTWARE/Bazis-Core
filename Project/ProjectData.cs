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
        public string ResultDB { get; set; } = "";

/// <inheritdoc/>

        public void ClearAllData()
        {
            TaskData?.Clear();
            ResultDB = "";
            ModelData?.ClearAll();
        }
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

        public void ChangeTaskType(TaskType taskType)
        {
            GeneralData.TaskType = taskType;
            taskData.Clear();
        }
/// <inheritdoc/>

        public void ClearMeshCollection(ObjType objType)
        {
            modelData.ObjectData.Clear(objType);
            modelData.GroupData.ClearNotExisted();

            taskData.ClearNotExisted(ModelData.GroupData);
        }
        /// <inheritdoc/>

        public void DeleteMeshGroup(string grName)
        {
            var gr = modelData.GroupData.Find(grName);
            if (gr != null)
            {
                modelData.GroupData.Remove(gr);
                taskData.ClearNotExisted(gr);
            }

        }
/// <inheritdoc/>

        public void DeleteMeshSet(ObjType objType, string name)
        {
            modelData.ObjectData.Remove(objType, name);
            modelData.GroupData.ClearNotExisted();

            taskData.ClearNotExisted(ModelData.GroupData);
        }
/// <inheritdoc/>

        public void DeleteMeshObjects(ObjType objType, IEnumerable<int> objNumbs)
        {

            foreach (var numb in objNumbs)
            {
                var obj = modelData.ObjectData.Find(objType, numb);
                if (obj != null)
                    obj.ExistState = false;
            }

            modelData.ObjectData.ClearNotExisted();
            modelData.GroupData.ClearNotExisted();
            taskData.ClearNotExisted(ModelData.GroupData);
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
