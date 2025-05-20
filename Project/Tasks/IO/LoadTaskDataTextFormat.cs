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
                        var group = groupData.Find(subAr[3]);
                        var data = new HeatData(group, dataAr);

                        if(subAr[2] != "*")
                        {
                            var builder = new FrameFunctionBuilder(subAr[2].Split(';'));
                            data.FrameFunction = (FrameFunction)builder;
                        }

                        if(subAr[6] != "*")
                        {
                            var trajAr = subAr[6].Split(';');
                            if (trajAr.Length == 3)
                            {
                                var baseLineGrName = trajAr[0].Split('|')[0];
                                var refLineGrName = trajAr[0].Split('|')[1];

                                var baseLine = groupData.Find(baseLineGrName);
                                var refLine = groupData.Find(refLineGrName);
                                var vel = float.Parse(trajAr[1]);

                                data.LocalFrame = new MovedFrame(baseLine, refLine, vel);
                            }
                            else
                            {
                                var frameGr = groupData.Find(trajAr[0]);
                                data.LocalFrame = new StaticFrame(frameGr);
                            }
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

        //private IPowerFunction CreateFrameFunction(string[] heatSourceAr)
        //{
        //    HeatSources processType;
        //    if (!Enum.TryParse(heatSourceAr[0], out processType))
        //        throw new Exception("Источник задан неверно!");

        //    if (processType == HeatSources.ARC)
        //    {
        //        return (Arc)new ArcBuilder().SetCurrent(heatSourceAr[2]).SetVoltage(heatSourceAr[3]).
        //            SetWidth(heatSourceAr[1]);
        //    }

        //    else if (processType == HeatSources.FSWPin)
        //    {
        //        return (FSWPin)new FSWPinBuilder().SetRotSpeed(heatSourceAr[1]).SetLength(heatSourceAr[2]).
        //            SetBottomDiam(heatSourceAr[3]).SetUpperDiam(heatSourceAr[4]).SetYieldFunc(heatSourceAr[5]);

        //    }
        //    else if (processType == HeatSources.FSWShoulder)
        //    {
        //        return (FSWShoulder)new FSWShoulderBuilder().SetAxisForce(heatSourceAr[1]).
        //            SetRotSpeed(heatSourceAr[2]).SetLength(heatSourceAr[3]).
        //            SetBottomDiam(heatSourceAr[4]).SetUpperDiam(heatSourceAr[5]).
        //            SetFricModuleFunc(heatSourceAr[6]);
        //    }
        //    else
        //    {
        //        return (Lazer)new LazerBuilder().SetPower(heatSourceAr[1]).SetLength(heatSourceAr[2]).SetUpperDiam(heatSourceAr[3]).SetBottomDiam(heatSourceAr[4]);
        //    }
        //}
    }
}
