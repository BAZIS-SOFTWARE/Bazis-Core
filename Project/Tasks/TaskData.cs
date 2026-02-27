using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Project.Interfaces.Tasks;
using Project.Interfaces.IO;
using Model.Interfaces;
using System;
using Project.Tasks.IO;
using Project.Tasks.FrameCreators;
using MaterialDB.FunctionData;
using MaterialDB.MaterialData;
using Project.Tasks.Materials;
using Model.GroupsData;
using static IronPython.Runtime.Profiler;


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

            if (kind == DataKind.Материал)
            {
                if (infoAr.Length < 4)
                    throw new Exception($"{info} - Строка данных имеет неверный формат!");

                var group = groupData.Find(infoAr[0]);

                var isBeamOrPlate = infoAr[1] == "стержни" || infoAr[1] == "пластины"; //что бы не дублировать проверки ниже
                var matIndex = isBeamOrPlate ? 3 : 1;
                var startIndex = isBeamOrPlate ? 4 : 2;
                var stopIndex = isBeamOrPlate ? 5 : 3;

                if (!Materials.ContainsKey(infoAr[matIndex]))
                    throw new Exception($"Материал {info} не содержится в базе данных!");

                if (!float.TryParse(infoAr[startIndex], out float start))
                    throw new Exception($"{info} - Время старта задано неверно!");

                if (!float.TryParse(infoAr[stopIndex], out float stop))
                    throw new Exception($"{info} - Время окончания задано неверно!");

                if (infoAr[1] == "стержни")
                {
                    var beamMatData = new BeamMatData(Materials[infoAr[matIndex]], group, start, stop);

                    if (!float.TryParse(infoAr[2], out float diameter))
                        throw new Exception($"{info} - Диаметр задан неверно!");
                    beamMatData.Diameter = diameter;

                    physicalData = beamMatData;
                }
                else if (infoAr[1] == "пластины")
                {
                    var plateMatData = new PlateMatData(Materials[infoAr[matIndex]], group, start, stop);

                    if (!float.TryParse(infoAr[2], out float thickness))
                        throw new Exception($"{info} - Tолщина задан неверно!");
                    plateMatData.Thickness = thickness;

                    physicalData = plateMatData;
                }
                else
                {
                    var matData = new MatData(Materials[infoAr[matIndex]], group, start, stop);
                    physicalData = matData;
                }
            }

            else if (kind == DataKind.Среда)
            {
                if (infoAr.Length < 6)
                    throw new Exception($"{info} - Строка данных имеет неверный формат.");

                float start;
                if (!float.TryParse(infoAr[3], out start))
                    throw new Exception($"{info} - Время старта задано неверно!");

                float stop;
                if (!float.TryParse(infoAr[4], out stop))
                    throw new Exception($"{info} - Время окончания задано неверно!");

                var mediaData = new MediaData(groupData.Find(infoAr[0]), start, stop);

                var medInfAr = infoAr[1].Split(";");

                if (float.TryParse(medInfAr[0], out float heatExch))
                    mediaData.HexValue = heatExch;

                if (medInfAr[1] != "*")
                    if (!Functions.ContainsKey(medInfAr[1]))
                        throw new Exception($"функция {medInfAr[1]} не содержится в базе данных!");
                    else
                        mediaData.HexTempFunc = Functions[medInfAr[1]];
                else if (medInfAr[2] != "*")
                {
                    if (medInfAr[2].Contains("HEX"))
                        mediaData.TrySetParamFunction(medInfAr[2]);
                    if (medInfAr[2].Contains("MRF"))
                        mediaData.HexParsFunc = GetMRF(medInfAr[2], groupData);

                }


                mediaData.

                var tempInfAr = infoAr[2].Split(";");

                if (float.TryParse(tempInfAr[0], out float temp))
                    mediaData.TempValue = temp;

                if (tempInfAr[1] != "*")
                    if (!Functions.ContainsKey(tempInfAr[1]))
                        throw new Exception($"функция {tempInfAr[1]} не содержится в базе данных!");
                    else
                        mediaData.TempTimeFunc = Functions[tempInfAr[1]];

                physicalData = mediaData;
            }

            else if (kind == DataKind.Нагрузка)
            {
                if (infoAr.Length < 8)
                    throw new Exception($"{info} - Строка данных имеет неверный формат!");

                float start;
                if (!float.TryParse(infoAr[5], out start))
                    throw new Exception($"{info} - Время старта задано неверно!");

                float stop;
                if (!float.TryParse(infoAr[6], out stop))
                    throw new Exception($"{info} - Время окончания задано неверно!");

                var loadData = new LoadData(groupData.Find(infoAr[0]), start, stop);

                loadData.TrySetKind(infoAr[1]);
                loadData.TrySetDirection(infoAr[2]);
                loadData.TrySetValue(infoAr[3]);


                if (infoAr[4] != "*")
                    if (!Functions.ContainsKey(infoAr[4]))
                        throw new Exception($"функция {infoAr[4]} не содержится в базе данных!");
                    else
                        loadData.TimeFunction = Functions[infoAr[4]];

                physicalData = loadData;
            }

            else if (kind == DataKind.Нагрев)
            {
                if (infoAr.Length < 5)
                    throw new Exception($"{info} - Строка данных имеет неверный формат!");

                float start;
                if (!float.TryParse(infoAr[4], out start))
                    throw new Exception($"{info} - Время старта задано неверно!");

                float stop;
                if (!float.TryParse(infoAr[5], out stop))
                    throw new Exception($"{info} - Время окончания задано неверно!");

                //dataInfo = dataInfo.Replace("*", "0");
                var heatData = new HeatData(groupData.Find(infoAr[3]), start, stop);

                heatData.TrySetFrameFunction(infoAr[2]);
                heatData.TrySetValue(infoAr[0]);

                if (infoAr[1] != "*")
                    if (!Functions.ContainsKey(infoAr[1]))
                        throw new Exception($"функция {infoAr[1]} не содержится в базе данных!");
                    else
                        heatData.TimeFunction = Functions[infoAr[1]];

                physicalData = heatData;
            }

            else
            {
                if (infoAr.Length < 6)
                    throw new Exception($"{info} - Строка данных имеет неверный формат!");

                float start;
                if (!float.TryParse(infoAr[5], out start))
                    throw new Exception($"{info} - Время старта задано неверно!");

                float stop;
                if (!float.TryParse(infoAr[6], out stop))
                    throw new Exception($"{info} - Время окончания задано неверно!");

                var clampData = new ClampData(groupData.Find(infoAr[0]), start, stop);

                clampData.TrySetKind(infoAr[1]);
                clampData.TrySetDirection(infoAr[2]);
                clampData.TrySetValue(infoAr[3]);

                if (infoAr[4] != "*")
                    if (!Functions.ContainsKey(infoAr[4]))
                        throw new Exception($"функция {infoAr[4]} не содержится в базе данных!");
                    else
                        clampData.TimeFunction = Functions[infoAr[4]];

                physicalData = clampData;
            }

            // TODO Провести для каждого ГУ

            //if(physicalData.FrameFunction != null)
            //{
            //    physicalData.FrameFunction.LocalFrame = GetLocalFrame(infoAr[2], groupData);

            //    // вводим синхронизацию времени окончания и скорости движения
            //    if (physicalData.FrameFunction.LocalFrame is MovedFrame mf)
            //        physicalData.StopTime = physicalData.StartTime + (float)Math.Round(mf.CalcMotionTime(), 4);
            //}


            return physicalData;
        }

        private LocalFrame GetMRF(string data, IGroupData groupData)
        {
            //var mrfStr = dataAr.First(x => x.Contains("MRF"));
            var stopInd = data.IndexOf("MRF") + 4;
            var mrfStr = data.Remove(0, stopInd);

            var mrfAr = mrfStr.Split(';');
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

        private LocalFrame GetSRF(string data, IGroupData groupData)
        {
            //var srfStr = dataAr.First(x => x.Contains("SRF"));
            var stopInd = data.IndexOf("SRF") + 4;
            var srfStr = data.Remove(0, stopInd);

            var srfAr = srfStr.Split(';');

            var frameGr = groupData.Find(srfAr[0]);
            var lf = new StaticFrame(frameGr);
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
