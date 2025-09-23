using System;
using System.IO;
using Model.Interfaces;
using Model.IO.STL;
using Model.IO;
using Project.Interfaces;
using Project.Interfaces.IO;

namespace Project.IO
{
    /// <summary>
    /// SaveProjectTextFormat
    /// </summary>
    public class SaveProjectTextFormat : IProjectSaver
    {
        /// <inheritdoc/>
        public event Action<object, ISaveEventArgs> SaveEvent;
/// <inheritdoc/>

        public void Save(IProjectData project, string fullPath)
        {
            //var fullPath = $@"{project.GeneralData.Path}\{project.GeneralData.Name}";

            var name = System.IO.Path.GetFileNameWithoutExtension(fullPath);
            project.GeneralData.Name = name;

            var ext = System.IO.Path.GetExtension(fullPath);

            IModelSaver saver;

            if (ext == ".bpf")
                saver = new SaveModelToBPFTextFile();
            else if (ext == ".bpf2")
                saver = new SaveModelToBPF2TextFile();
            //else if (ext == ".asc" | ext == ".ASC")
            //    saver = new SaveModelToASCIITextFile();
            else
                throw new System.Exception("Неизвестный формат сохранения проекта");


            //сохранение сетки
            saver.Save(project.ModelData,fullPath);

            //сохранение задачи

            using (var sw = new StreamWriter(fullPath, true, System.Text.Encoding.Unicode) { AutoFlush = true })
            {
                sw.WriteLine("");
                sw.WriteLine("Проект");

                sw.WriteLine("Сведения : {0}", project.GeneralData.Comments);
                sw.WriteLine("Вид : {0}", project.TaskData.TaskType);
                sw.WriteLine("Тип : {0}", project.TaskData.TaskKind);

                //при создании задачи материалы и функции обязаны быть загружены!!!
                if (project.TaskData.Materials != null & project.TaskData.Functions != null)
                {
                    sw.WriteLine("БазаМатериалов : {0}", project.TaskData.Materials.Name);
                    sw.WriteLine("БазаФункций : {0}", project.TaskData.Functions.Name);

                    //сохранение данных задачи
                    foreach (var item in project.TaskData)
                        sw.WriteLine(item.ToString());
                }

                sw.WriteLine("#Проект");
            }
        }
    }
}
