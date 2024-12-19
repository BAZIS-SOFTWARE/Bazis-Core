using Model.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
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

                    if (dataName == "Материал" | dataName == "Материалы")
                    {
                        var group = groupData.Find(dataAr.Split(' ')[0]);
                        var data = new MatData(group, dataAr);
                        taskData.Add(data);
                    }
  
                    else if (dataName == "Среда")
                    {
                        var group = groupData.Find(dataAr.Split(' ')[0]);
                        var data = new MediaData(group, dataAr);
                        taskData.Add(data);
                    }

                    else if (dataName == "Нагрузка")
                    {
                        var group = groupData.Find(dataAr.Split(' ')[0]);
                        var data = new LoadData(group, dataAr);
                        taskData.Add(data);
                    }

                    else if (dataName == "Нагрев")
                    {
                        var subAr = dataAr.Split(' ');
                        var group = groupData.Find(subAr[1]);
                        var data = new HeatData(group, dataAr);

                        var trajAr = subAr[4].Split(';');
                        if (trajAr.Length > 1)
                        {
                            var baseLineGrName = subAr[4].Split(';')[0].Split('|')[0];
                            var refLineGrName = subAr[4].Split(';')[0].Split('|')[1];

                            var startNodesGrName = subAr[4].Split(';')[2];

                            var baseLineGr = groupData.Find(baseLineGrName);
                            var refLineGr = groupData.Find(refLineGrName);
                            var startNodesGr = groupData.Find(startNodesGrName);
                            data.MovedFrame.BaseLine = baseLineGr;
                            data.MovedFrame.RefLine = refLineGr;
                            data.MovedFrame.StartPoints = startNodesGr;
                            data.MovedFrame.StopPoints = startNodesGr;
                        }

                        taskData.Add(data);
                    }
  
                    else if (dataName == "Закрепления" | dataName == "Закрепление")
                    {
                        var group = groupData.Find(dataAr.Split(' ')[0]);
                        var data = new ClampData(group, dataAr);
                        taskData.Add(data);
                    }                      
                }             
            }

            return taskData;
        }      
    }
}
