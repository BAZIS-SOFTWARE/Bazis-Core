using System;
using System.IO;
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

        public void Save(IProjectData project)
        {
            var fullName = $@"{project.GeneralData.Path}\{project.GeneralData.Name}";

            //сохранение сетки
            project.ModelData.Save(fullName);

            using (var sw = new StreamWriter(fullName, true, System.Text.Encoding.Unicode) { AutoFlush = true })
            {
                sw.WriteLine("");
                sw.WriteLine("Проект");
                sw.WriteLine("Название : {0}", project.GeneralData.Name);
                sw.WriteLine("Сведения : {0}", project.GeneralData.Comments);
                sw.WriteLine("Вид : {0}", project.GeneralData.TaskType);
                sw.WriteLine("Тип : {0}", project.GeneralData.TaskKind);
                sw.WriteLine("БазаМатериалов : {0}", project.GeneralData.Materials);
                sw.WriteLine("БазаФункций : {0}", project.GeneralData.Functions);

                    //сохранение данных задачи
                    if (project.TaskData != null)
                    foreach (var item in project.TaskData)
                        sw.WriteLine(item.ToString());
                sw.WriteLine("#Проект");
            }
        }
    }
}
