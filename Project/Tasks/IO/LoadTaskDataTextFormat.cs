using Model.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using System;
using System.IO;
using System.Linq;
using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Newtonsoft.Json;
using Project.Tasks.Functions;

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

        public ITaskData Load(string fullPath, IGroupData groupData)
        {

            var lines = File.ReadLines(fullPath).SkipWhile(line => !line.Contains("Проект"));  // пропустит строки по условию                                                                                       //.TakeWhile(line => line.Contains("#Проект")); // берет строки по условию
            var path = Path.GetDirectoryName(fullPath);
            
            var taskData = new TaskData();

            foreach (var line in lines)
            {
                var splitLine = line.Split(':');

                if (splitLine.Length > 1)
                {
                    if (splitLine.Length > 2)
                        splitLine[1] = string.Join(":", splitLine.Skip(1));
                    var dataName = splitLine[0].Replace(" ", "");
                    var dataAr = splitLine[1].Remove(0, 1);

                    if (dataName == "БазаМатериалов")
                    {
                        var matDBName = splitLine[1].Split(' ')[1];
        //                taskData.Materials =
        //GetDataBase<MaterialDBData>(matDBName, path);
        //                taskData.Materials.Name = matDBName;
                        taskData.Materials = new MaterialDBData(matDBName, path);
                    }

                    if (dataName == "БазаФункций")
                    {
                        var funDBName = splitLine[1].Split(' ')[1];
         //               taskData.Functions =
         //GetDataBase<FunctionDBData>(funDBName, path);
         //               taskData.Functions.Name = funDBName;

                        taskData.Functions = new FunctionDBData(funDBName, path);
                    }

                    if (dataName == "Тип")
                    {
                        if (Enum.TryParse(splitLine[1].Replace(" ",""), out TaskKind taskKind))
                            taskData.TaskKind = taskKind;
                    }
                    if (dataName == "Вид")
                        if (splitLine[1].Split(' ')[1] == "Plain")
                            taskData.TaskType = TaskType.Plain;
                        else if (splitLine[1].Split(' ')[1] == "AxiPlain")
                            taskData.TaskType = TaskType.AxiPlain;
                        else if (splitLine[1].Split(' ')[1] == "Volume")
                            taskData.TaskType = TaskType.Volume;
                        else { taskData.TaskType = TaskType.Volume_mixed; }

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

    //    public T GetDataBase<T>(string dbName, string dbPath)
    //    {
    //        var filePath = FindFileByPath(dbPath, dbName);
    //        if (filePath == null)
    //        {
    //            throw new Exception($"Не найдена база {dbName} в папке {dbPath}");
    //        }

    //        else
    //        {
    //            var settingsSerializer = new JsonSerializerSettings
    //            {
    //                TypeNameHandling = TypeNameHandling.Auto,
    //                Formatting = Formatting.Indented,
    //            };

    //            return JsonConvert.DeserializeObject<T>
    //(File.ReadAllText($@"{dbPath}\{dbName}"), settingsSerializer);
    //        }
    //    }

    //    private string FindFileByPath(string path, string fileName)
    //    {
    //        var projFiles = Directory.GetFiles(path, fileName, SearchOption.AllDirectories);
    //        if (projFiles.Count() > 0)
    //        {
    //            return Path.GetDirectoryName(projFiles[0]);
    //        }

    //        return null;
    //    }
    }
}
