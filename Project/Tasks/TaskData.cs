using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Microsoft.Scripting.Utils;
using Model.GroupsData;
using Model.Interfaces;
using Project.Interfaces.IO;
using Project.Interfaces.Tasks;
using Project.Tasks.Functions;
using Project.Tasks.Functions.FrameFunctions;
using Project.Tasks.IO;
using Project.Tasks.LocalFrames;
using Project.Tasks.Materials;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using static IronPython.Modules.PythonRegex;
using static IronPython.Runtime.Profiler;
using static System.Net.Mime.MediaTypeNames;



namespace Project.Tasks
{

    public class TaskData : ITaskData
    {
        /// <inheritdoc/>
        public MaterialDBData Materials { get; set; }
        /// <inheritdoc/>
        public FunctionDBData Functions { get; set; }

        /// <summary>
        /// TaskData
        /// </summary>
        /// <param name="materials"></param>
        /// <param name="functions"></param>
        public TaskData(MaterialDBData materials, FunctionDBData functions) : this()
        {
            Materials = materials;
            Functions = functions;
        }

        /// <inheritdoc/>
        public TaskType TaskType { get; set; } = TaskType.Linear;
        /// <inheritdoc/>

        public TaskKind TaskKind { get; set; } = TaskKind.химическая;

        List<ICondData> taskData;

        public int Count { get { return taskData.Count; } }

        public bool IsReadOnly => throw new System.NotImplementedException();
        /// <inheritdoc/>

        public ITaskDataSaver Saver { get; set; }
        /// <inheritdoc/>
        public ITaskDataLoader Loader { get; set; } = new LoadTaskDataTextFormat();

        public ICondData this[int ind]
        {
            get
            {
                return taskData[ind];
            }
            set
            {
                taskData[ind] = value;
            }
        }
        /// <summary>
        /// Task data constructor
        /// </summary>
        public TaskData()
        {
            taskData = new List<ICondData>();
        }
        /// <inheritdoc/>

        public IEnumerable<T> Find<T>() where T : ICondData
        {
            foreach (var data in taskData)
            {
                if (data is T)
                    yield return (T)data;
            }
        }
        /// <inheritdoc/>

        public IEnumerable<ICondData> Find(DataKind kind)
        {
            foreach (var data in taskData)
            {
                if (data.Kind == kind)
                    yield return data;
            }
        }
        /// <inheritdoc/>

        public IEnumerable<ICondData> Find(string grName)
        {
            foreach (var data in taskData)
            {
                if (data.Group.Name == grName)
                    yield return data;
            }
        }
        /// <inheritdoc/>

        public void Add(ICondData data)
        {
            taskData.Add(data);
        }
        /// <inheritdoc/>

        public void RemoveRange(IEnumerable<string> dataInfo)
        {
            foreach (var data in dataInfo)
            {
                var ind = taskData.FindIndex(x => x.ToString() == data);
                taskData.RemoveAt(ind);
            }
        }
        /// <inheritdoc/>

        public int IndexOf(ICondData item)
        {
            return taskData.IndexOf(item);
        }
        /// <inheritdoc/>

        public void Insert(int index, ICondData item)
        {
            throw new System.NotImplementedException();
        }
        /// <inheritdoc/>

        public void RemoveAt(int index)
        {
            taskData.RemoveAt(index);
        }
        /// <inheritdoc/>

        public void Clear()
        {
            taskData.Clear();
        }
        /// <inheritdoc/>

        public bool Contains(ICondData item)
        {
            return taskData.Find(x => x.Kind == item.Kind & x.ToString().Equals(item.ToString())) == null ? false : true;
        }
        /// <inheritdoc/>

        public void CopyTo(ICondData[] array, int arrayIndex)
        {
            taskData.CopyTo(array, arrayIndex);
        }
        /// <inheritdoc/>

        public bool Remove(ICondData item)
        {
            return taskData.Remove(item);
        }

        public int RemoveAll(DataKind kind)
        {
            return taskData.RemoveAll(x => x.Kind == kind);
        }

        public IEnumerator<ICondData> GetEnumerator()
        {
            foreach (var data in taskData)
            {
                yield return data;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <inheritdoc/>

        public void Load(ITaskData taskData)
        {
            this.taskData.Clear();
            this.taskData.AddRange(taskData);
        }
        /// <inheritdoc/>

        public void AddRange(IEnumerable<ICondData> datas)
        {
            taskData.AddRange(datas);
        }
        /// <inheritdoc/>

        public void Save(string projectFile)
        {
            Saver.Save(this, projectFile);
        }
        /// <inheritdoc/>

        public void Load(string projectFile, IGroupData gropData)
        {
            var taskData = Loader.Load(projectFile, gropData);

            this.taskData.Clear();
            this.taskData.AddRange(taskData);

            Materials = taskData.Materials;
            Functions = taskData.Functions;

            TaskType = taskData.TaskType;
            TaskKind = taskData.TaskKind;
        }
        /// <inheritdoc/>
        public void ClearNotExisted(IGroup group)
        {
            var delData = taskData.Where(x => x.Group == group).ToList();

            foreach (var item in delData)
                taskData.Remove(item);
        }
        /// <inheritdoc/>

        public void ClearNotExisted(IGroupData gropData)
        {
            //var valData = taskData.Select(x => (IPhysicalData)x);

            var existData = taskData.Where(x => gropData.Contains(x.Group)).ToList();

            taskData.Clear();
            taskData.AddRange(existData);
        }
        /// <inheritdoc/>

        public ICondData Create(DataKind kind, string info, IGroupData groupData)
        {
            if (groupData == null)
                throw new ArgumentNullException(nameof(groupData));

            CondData physicalData;

            var infoAr = info.Split(' ');

            var group = groupData.Find(infoAr[4]);

            if (!float.TryParse(infoAr[5], out float start))
                throw new Exception($"{info} - Время старта задано неверно!");

            if (!float.TryParse(infoAr[6], out float stop))
                throw new Exception($"{info} - Время окончания задано неверно!");

            if (kind == DataKind.Материал)
            {
                if (infoAr.Length < 4)
                    throw new Exception($"{info} - Строка данных имеет неверный формат!");

                if (!Materials.ContainsKey(infoAr[7]))
                    throw new Exception($"Материал {info} не содержится в базе данных!");
                var mat = Materials[infoAr[7]];
                if (info.Contains("стержни"))
                {
                    var beamMatData = new BeamMatData(mat, group, start, stop);

                    if (!float.TryParse(infoAr[9], out float diameter))
                        throw new Exception($"{info} - Диаметр задан неверно!");
                    beamMatData.Diameter = diameter;

                    physicalData = beamMatData;
                }
                else if (info.Contains("пластины"))
                {
                    var plateMatData = new PlateMatData(mat, group, start, stop);

                    if (!float.TryParse(infoAr[9], out float thickness))
                        throw new Exception($"{info} - Tолщина задан неверно!");
                    plateMatData.Thickness = thickness;

                    physicalData = plateMatData;
                }
                else
                {
                    var matData = new MatData(mat, group, start, stop);
                    physicalData = matData;
                }
            }

            else if (kind == DataKind.Среда)
            {
                if (infoAr.Length < 6)
                    throw new Exception($"{info} - Строка данных имеет неверный формат.");

                var mediaData = new MediaData(group, start, stop);
                physicalData = mediaData;
            }

            else if (kind == DataKind.Нагрузка)
            {
                if (infoAr.Length < 8)
                    throw new Exception($"{info} - Строка данных имеет неверный формат!");

                var loadData = new LoadData(group, start, stop);

                loadData.TrySetKind(infoAr[7]);
                physicalData = loadData;
            }

            else if (kind == DataKind.Нагрев)
            {
                if (infoAr.Length < 5)
                    throw new Exception($"{info} - Строка данных имеет неверный формат!");

                var heatData = new HeatData(group, start, stop);
                physicalData = heatData;
            }

            else
            {
                if (infoAr.Length < 6)
                    throw new Exception($"{info} - Строка данных имеет неверный формат!");

                var clampData = new ClampData(group, start, stop);

                clampData.TrySetKind(infoAr[7]);

                physicalData = clampData;
            }

            // задаем значение
            if (float.TryParse(infoAr[0], out float value))
                physicalData.Value = value;

            // задаем направление
            physicalData.TrySetDirection(infoAr[3]);

            // задаем функцию
            if (infoAr[1] != "*")
                physicalData.Function = TryCreateFunction(infoAr[1]);

            if (infoAr[2].Contains("MRF"))
            {
                physicalData.LocalFrame = GetMRF(infoAr[2], groupData);
                // это не обязательное условие
                //physicalData.Function.AddParameter(new Parameter("TIME"));
            }

            if (infoAr[2].Contains("SRF"))
                physicalData.LocalFrame = GetSRF(infoAr[2], groupData);

            return physicalData;
        }

        private Function TryCreateFunction(string v)
        {
            Function function;

            string[] parsInfo = v.Split(',');

            if (v.Contains(FuncName.SPH.ToString()))
                function = new SPH();
            else if (v.Contains(FuncName.CIL.ToString()))
                function = new CIL();
            else if (v.Contains(FuncName.DHEX.ToString()))
                function = new DHEX();
            else if (v.Contains(FuncName.FHEX.ToString()))
                function = new FHEX();
            else if (v.Contains(FuncName.TT.ToString()))
                function = new TT();
            else if (v.Contains(FuncName.PT.ToString()))
                function = new PT();
            else if (v.Contains(".py"))
            {
                var cf = new CustomFrameFunction();
                cf.CreateEngine(parsInfo[0]);
                function = cf;
            }

            else
                throw new Exception($"Неизвестная функция {v}");

            TrySetParams(parsInfo,function);

            return function;
            
        }

        private void TrySetParams(string[] funInfo, Function function)
        {
            foreach (var item in funInfo.Skip(1))
            {
                var parInfo = item.Split("=");
                if(parInfo.Length != 2)
                    throw new Exception($"Параметр {item} в функции {funInfo[0]} задан неверно");

                Parameter funcParam;

                double digit;
                if (Double.TryParse(parInfo[1], out digit))
                    funcParam = new Parameter(parInfo[0], ParameterType.Constant, digit);

                else
                {
                    // использовать шаблон string pattern = @"^T\(.*\)$"; // @ - буквальная строка
                    if (parInfo[1].Contains("Table"))
                    {
                        var match = Regex.Match(parInfo[1], @"Table\((.*)\)");
                        var results = match.Groups[1].Value.Split(';');

                        var table = Functions[results[0]];
                        var param = function[results[1]];

                        funcParam = new TableParameter(table, parInfo[0], param);
                    }
                    else
                        funcParam = new Parameter(parInfo[0]);
                }

                if (function.ContainsParameter(funcParam.Name) &&
                    function[funcParam.Name].ParameterType == funcParam.ParameterType)
                        function[funcParam.Name] = funcParam;
                else
                    // попробуем строго ограничить прием параметров
                    throw new Exception($"Параметр {funcParam.Name} задан неверно или не существует");
                    //function.AddParameter(funcParam);
            }
        }

        private MovedFrame GetMRF(string data, IGroupData groupData)
        {
            string pattern = "[\\(\\)]|MRF";
            var mrfAr = Regex.Replace(data, pattern, "", RegexOptions.IgnoreCase).Split(',');
            
            //var mrfAr = Regex.Replace(data, @"^MRF\(.*\)$", "").Split(";");

            if (mrfAr.Length == 3)
            {
                var baseLineGrName = mrfAr[0].Split('|')[0];
                var refLineGrName = mrfAr[0].Split('|')[1];

                var baseLine = groupData.Find(baseLineGrName);
                var refLine = groupData.Find(refLineGrName);
                var vel = float.Parse(mrfAr[1]);

                var lf = new MovedFrame(baseLine, refLine, vel);
                lf.SetShiftingAndRotation(mrfAr[2]);

                return lf;
            }
            else
            {
                throw new Exception("Неправильный ввод данных");
            }
        }

        private StaticFrame GetSRF(string data, IGroupData groupData)
        {
            string pattern = "[\\(\\)]|SRF";
            var srfAr = Regex.Replace(data, pattern, "", RegexOptions.IgnoreCase).Split(',');

            var frameGr = groupData.Find(srfAr[0]);

            StaticFrame lf;
            if(frameGr != null)
                lf = new StaticFrame(frameGr);
            else
                lf = new StaticFrame();

            lf.SetShiftingAndRotation(srfAr[1]);
            return lf;
        }

        /// <inheritdoc/>

        public bool Contains(string grName)
        {
            return taskData.FirstOrDefault(x => x.Group.Name == grName)
                == null ? false : true;
        }
    }
}
