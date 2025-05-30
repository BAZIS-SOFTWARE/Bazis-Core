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
    public class MediaData : PhysicalData
    {
        /// <summary>
        /// MediaProcess
        /// </summary>
        //public string MediaProcess { get; internal set; } 

        public MediaData(IGroup group, string info) : base(group, info)
        {
            Kind = DataKind.Среда;
        }

        public MediaData(IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Среда;
        }


        /// <inheritdoc/>

        protected override void SetInfo(string info)
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
        public float HeatExchangeValue 
        {
            get { return heatExchangeValue; }
            set
            {
                heatExchangeFunc = "*";
                heatExchangeValue = value;
            }
        } 
        /// <summary>
        /// HeatExchangeFunc
        /// </summary>
        public string HeatExchangeFunc 
        {
            get { return heatExchangeFunc; }
            set
            {
                heatExchangeFunc = value;
                heatExchangeValue = 1;
            }
        }

        private float heatExchangeValue = 1;
        private string heatExchangeFunc = "*";
        private string temperatureFunc = "*";
        private float temperatureValue = 1;

        /// <summary>
        /// HeatExchangeValue
        /// </summary>
        public float TemperatureValue 
        { 
            get
            {
                return temperatureValue;
            }  
            set
            {
                temperatureFunc = "*";
                temperatureValue = value;
            }
        }
        /// <summary>
        /// Temperature of media or body
        /// </summary>
        public string TemperatureFunc
        {
            get { return temperatureFunc; }
            set
            {
                temperatureFunc = value;
                temperatureValue = 1;
            }
        }

        /// <summary>
        /// MediaType
        /// </summary>
        public MediaType MediaType { get; set; } = MediaType.ConstantTemp;

/// <inheritdoc/>

        protected override string GetInfo
        {
            get
            {
                var mfInfo = "*";
                if (FrameFunction?.LocalFrame != null)
                    mfInfo = FrameFunction.LocalFrame.ToString();

                if (MediaType == MediaType.ConstantTemp)
                {
                    if (TemperatureFunc == "*")
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, "*", TemperatureValue, StartTime, StopTime, mfInfo);
                    else
                        return string.Format("{0} {1} {2} {3} {4} {5}",
Group.Name, "*", TemperatureFunc, StartTime, StopTime, mfInfo);
                }
                else
                {
                    if (HeatExchangeFunc == "*" & TemperatureFunc == "*")
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeValue, TemperatureValue, StartTime, StopTime, mfInfo);
                    else if (HeatExchangeFunc == "*" & TemperatureFunc != "*")
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeValue, TemperatureFunc, StartTime, StopTime, mfInfo);
                    else if (HeatExchangeFunc != "*" & TemperatureFunc == "*")
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeFunc, TemperatureValue, StartTime, StopTime, mfInfo);
                    else
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeFunc, TemperatureFunc, StartTime, StopTime, mfInfo);
                }
            }
        }
    }
}