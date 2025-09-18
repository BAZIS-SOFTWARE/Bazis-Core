using Model;
using Model.Interfaces;
using Model.IO.STL;
using Model.IO;
using Newtonsoft.Json;
using Project.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using Project.Tasks;
using PropertiesCalculator.FunctionData;
using PropertiesCalculator.MaterialData;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Project.Tasks.IO;

namespace Project.IO
{
    /// <summary>
    /// LoadProjectFromTextFormat
    /// </summary>
    public class LoadProjectFromTextFormat : IProjectLoader
    {
        /// <inheritdoc/>
        public event Action<object, ILoadEventArgs> LoadEvent;
        /// <inheritdoc/>

        public IProjectData Load(string fullPath)
        {
            //Thread.Sleep(100);
            //modelData loading

            var project = new ProjectData(fullPath);

            var ext = System.IO.Path.GetExtension(fullPath);

            var name = System.IO.Path.GetFileNameWithoutExtension(fullPath);
            project.GeneralData.Name = name;

            IModelLoader modelLoader;

            if (ext == ".bpf")
                modelLoader = new LoadModelFromBPFTextFile();
            else if(ext == ".bpf2")
                modelLoader = new LoadModelFromBPF2TextFile();
            //else if (ext == ".stl" | ext == ".STL")
            //    modelLoader = new LoadFromSTLFile();
            //else if (ext == ".inp")
            //    modelLoader = new LoadModelFromINPTextFile_v2();
            //else if (ext == ".asc" | ext == ".ASC")
            //    modelLoader = new LoadModelFromASCIITextFile_v2();
            //else if (ext == ".cdb")
            //    modelLoader = new LoadModelFromCDBTextFile();
            //else if (ext == ".dat")
            //    modelLoader = new LoadModelFromSalomeFile();
            else
                throw new System.Exception("Неизвестный формат загрузки проекта");

            modelLoader.LoadEvent += (ar1, ar2) =>
            { LoadEvent?.Invoke(this, new LoadEventArgs(ar2.Message)); };

            var modelData = modelLoader.Load(fullPath);

            project.ModelData = modelData;

            //taskData loading


            var taskLoader = new LoadTaskDataTextFormat();

            taskLoader.LoadEvent += (ar1, ar2) =>
            { LoadEvent?.Invoke(this, new LoadEventArgs(ar2.Message)); };

            var taskData = taskLoader.Load(fullPath, project.ModelData.GroupData);

            project.TaskData = taskData;

            return project;
        }
    }
}
