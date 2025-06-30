using Model.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using Project.Tasks.Functions;
using Project.Tasks.FrameCreators;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Project.Tasks.IO
{
    /// <summary>
    /// LoadTaskDataTextFormat
    /// </summary>
    public class LoadTaskDataTextFormat : ITaskDataLoader
    {
        /// <inheritdoc/>
        public event Action<object, ILoadEventArgs> LoadEvent;
/// <inheritdoc/>

        public ITaskData Load(string path, IGroupData groupData)
        {

            var lines = File.ReadLines(path).SkipWhile(line => !line.Contains("Проект"));  // пропустит строки по условию
            //.TakeWhile(line => line.Contains("#Проект")); // берет строки по условию

            var taskData = new TaskData();

            foreach (var line in lines)
            {
                var splitLine = line.Split(':');

                if (splitLine.Length > 1)
                {
                    var dataName = splitLine[0].Replace(" ", "");
                    var dataAr = splitLine[1].Remove(0, 1);

                    DataKind kind;
                    if(Enum.TryParse(dataName, out kind))
                    {
                        var data = taskData.Create(kind, dataAr, groupData);
                        taskData.Add(data);
                    }
                                   
                }             
            }

            return taskData;
        }    
    }
}
