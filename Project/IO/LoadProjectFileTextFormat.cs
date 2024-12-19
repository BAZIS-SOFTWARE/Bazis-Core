using Project.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using System;
using System.IO;
using System.Threading;

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

        public void Load(IProjectData project, string path)
        {
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();

                    if (line == "Проект")
                    {
                        LoadEvent?.Invoke(this, new LoadEventArgs("Загрузка данных проекта..."));
                        Thread.Sleep(100);
                        LoadProjectData(sr, project);
                    }
                }
            }


            Thread.Sleep(100);
            //modelData loading
            if (project.ModelData != null)
            {
                project.ModelData.Loader.LoadEvent += (ar1, ar2) =>
                { LoadEvent?.Invoke(this, new LoadEventArgs(ar2.Message)); };

                project.ModelData.Load(path);
            }

            //taskData loading
            if(project.TaskData != null)
            {
                project.TaskData.Loader.LoadEvent += (ar1, ar2) =>
                { LoadEvent?.Invoke(this, new LoadEventArgs(ar2.Message)); };
                
                project.TaskData.Load(path, project.ModelData.GroupData);
            }
        }


        private static void LoadProjectData(StreamReader sr, IProjectData project)
        {
            var line = String.Empty;
            line = sr.ReadLine();

            while (line != "#Проект")
            {
                var splitLine = line.Split(':');

                var dataName = splitLine[0].Replace(" ", "");
                if (dataName == "Название")
                    project.GeneralData.Name = splitLine[1].Split(' ')[1];
                if (dataName == "Сведения")
                    project.GeneralData.Comments = splitLine[1].Split(' ')[1];
                if (dataName == "БазаМатериалов")
                    project.GeneralData.Materials = splitLine[1].Split(' ')[1];
                if (dataName == "БазаФункций")
                    project.GeneralData.Functions = splitLine[1].Split(' ')[1];
                if (dataName == "Вид")
                    if (splitLine[1].Split(' ')[1] == "Plain")
                        project.GeneralData.TaskType = TaskType.Plain;
                    else if (splitLine[1].Split(' ')[1] == "AxiPlain")
                        project.GeneralData.TaskType = TaskType.AxiPlain;
                    else { project.GeneralData.TaskType = TaskType.Volume; }

                line = sr.ReadLine();
            }
        }      

        
    }
}
