using Model;
using Model.Interfaces;
using Model.IO.STL;
using Model.IO;
using Newtonsoft.Json;
using Project.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using Project.Tasks;
using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
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
        /// <summary>
        /// Читает проект и сообщает процент двух проходов по файлу.
        /// </summary>
        public IProjectData Load(string fullPath, IProgress<int> progress = null)
        {
            //Thread.Sleep(100);
            //modelData loading

            var project = new ProjectData(fullPath);

            var ext = System.IO.Path.GetExtension(fullPath).ToLowerInvariant();

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

            var modelProgress = progress == null ? null : new ProgressRange(progress, 0, 50);
            var modelData = modelLoader.Load(fullPath, modelProgress);

            project.ModelData = modelData;

            //taskData loading


            var taskLoader = new LoadTaskDataTextFormat();

            var taskProgress = progress == null ? null : new ProgressRange(progress, 50, 50);
            var taskData = taskLoader.Load(fullPath, project.ModelData.GroupData, taskProgress);

            project.TaskData = taskData;

            return project;
        }
    }
}
