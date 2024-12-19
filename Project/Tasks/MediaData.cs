using System;
using Model.Interfaces;
using Project.Interfaces.Tasks;

namespace Project.Tasks
{
    /// <summary>
    /// MediaType
    /// </summary>
    public enum MediaType
    {
        /// <summary>
        /// HeatFlux
        /// </summary>
        HeatFlux,
        /// <summary>
        /// ConstantTemp
        /// </summary>
        ConstantTemp
    }
    public class MediaData : ValuableData
    {
        /// <summary>
        /// MediaProcess
        /// </summary>
        //public string MediaProcess { get; internal set; } 

        public MediaData(IGroup group, string info) : base(group, info)
        {
            Kind = DataKind.Med;
            Name = "Среда";
        }
/// <inheritdoc/>


        public override string ToString()
        {
                return string.Format("{0} : {1}",Name, GetInfo);           
        }
/// <inheritdoc/>

        public override void SetInfo(string info)
        {
            var medDataAr = info.Split(' ');

            if (medDataAr.Length < 6)
                throw new Exception("Строка данных имеет неверный формат.");

            if (medDataAr[1] != "*")
            {
                MediaType = MediaType.HeatFlux;
                var resEx = 0.0f;
                if (float.TryParse(medDataAr[1], out resEx))
                    HeatExchangeValue = resEx;
                else
                    HeatExchangeFunc = medDataAr[1];
            }

            if (medDataAr[2] == "*")
                throw new Exception("Граничное условие тепловой задачи должно содержать сведения о температуре.");

            var res = 0.0f;
                if (float.TryParse(medDataAr[2], out res))
                    TemperatureValue = res;
                else
                    TemperatureFunc = medDataAr[2];
            

            float startTime;
            if (!float.TryParse(medDataAr[3], out startTime))
                throw new Exception("Время старта задано неверно!");

            StartTime = startTime;

            float stopTime;
            if (!float.TryParse(medDataAr[4], out stopTime))
                throw new Exception("Время окончания задано неверно!");

            StopTime = stopTime;
        }
        /// <summary>
        /// HeatExchangeValue
        /// </summary>
        public float HeatExchangeValue { get; private set; } = 1;
        /// <summary>
        /// HeatExchangeFunc
        /// </summary>
        public string HeatExchangeFunc { get; private set; } = "*";
        /// <summary>
        /// HeatExchangeValue
        /// </summary>
        public float TemperatureValue { get; private set; } = 1;
        /// <summary>
        /// Temperature of media or body
        /// </summary>
        public string TemperatureFunc { get; private set; } = "*";
        /// <summary>
        /// MediaType
        /// </summary>
        public MediaType MediaType { get; set; } = MediaType.ConstantTemp;

/// <inheritdoc/>

        public override string GetInfo
        {
            get
            {
                if (MediaType == MediaType.ConstantTemp)
                {
                    if (TemperatureFunc == "*")
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, "*", TemperatureValue, StartTime, StopTime, TrajectoryInfo);
                    else
                        return string.Format("{0} {1} {2} {3} {4} {5}",
Group.Name, "*", TemperatureFunc, StartTime, StopTime, TrajectoryInfo);
                }
                else
                {
                    if (HeatExchangeFunc == "*" & TemperatureFunc == "*")
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeValue, TemperatureValue, StartTime, StopTime, TrajectoryInfo);
                    else if (HeatExchangeFunc == "*" & TemperatureFunc != "*")
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeValue, TemperatureFunc, StartTime, StopTime, TrajectoryInfo);
                    else if (HeatExchangeFunc != "*" & TemperatureFunc == "*")
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeFunc, TemperatureValue, StartTime, StopTime, TrajectoryInfo);
                    else
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeFunc, TemperatureFunc, StartTime, StopTime, TrajectoryInfo);
                }
            }
        }
    }
}