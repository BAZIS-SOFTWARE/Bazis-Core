using Model.Interfaces;
using Project.Interfaces;
using Project.IO;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;

namespace Project
{
    /// <summary>
    /// ProjectData
    /// </summary>
    public class ProjectData : IProjectData
    {
        /// <inheritdoc/>
        public IModelData ModelData { get; set; }
        /// <inheritdoc/>

        public ITaskData TaskData { get; set; }
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

        public IProjectSaver Saver { get; set; } = new SaveProjectTextFormat();
        /// <inheritdoc/>

        public IProjectLoader Loader { get; set; } = new LoadProjectFromTextFormat();
        /// <inheritdoc/>
        public IGeneralData GeneralData { get; set; } = new GeneralData();

        /// <summary>
        /// ProjectData
        /// </summary>
        /// <param name="_name"></param>
        /// <param name="_path"></param>
        public ProjectData(string _name, string _path)
        {
            GeneralData.Name = _name;
            GeneralData.Path = _path;
        }
        /// <summary>
        /// ProjectData
        /// </summary>
        public ProjectData() {}
    }
}
