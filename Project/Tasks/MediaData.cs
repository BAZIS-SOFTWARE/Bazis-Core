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

        public MediaData(IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Среда;
        }


        /// <summary>
        /// HeatExchangeValue
        /// </summary>
        public float HeatExchangeValue 
        {
            get { return heatExchangeValue; }
        } 
        /// <summary>
        /// HeatExchangeFunc
        /// </summary>
        public string HeatExchangeFunc 
        {
            get { return heatExchangeFunc; }
        }

        private float heatExchangeValue = 0;
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
        }
        /// <summary>
        /// Temperature of media or body
        /// </summary>
        public string TemperatureFunc
        {
            get { return temperatureFunc; }
        }

        /// <summary>
        /// MediaType
        /// </summary>
        public MediaType MediaType 
        { 
            get
            {
                return heatExchangeFunc == "*" & heatExchangeValue == 0 ?
                    MediaType.ConstantTemp : MediaType.HeatFlux;
            }
        }

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
        /// <summary>
        /// TrySetTemp
        /// </summary>
        /// <param name="v"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void TrySetTemp(string v)
        {
            if (v == "*")
                throw new Exception("Граничное условие тепловой задачи должно содержать сведения о температуре.");

            var res = 0.0f;
            if (float.TryParse(v, out res))
            {
                temperatureValue = res;
                temperatureFunc = "*";
            }
  
            else
            {
                temperatureValue = 1;
                temperatureFunc = v;
            }
    
        }
        /// <summary>
        /// TrySetHeatExchange
        /// </summary>
        /// <param name="v"></param>
        public void TrySetHeatExchange(string v)
        {
            if (float.TryParse(v, out var media))
            {
                heatExchangeValue = media;
                heatExchangeFunc = "*";
            }
               
            else 
            {
                if (v != "*")
                    heatExchangeValue = 1;
                else
                    heatExchangeValue = 0;
                heatExchangeFunc = v;
            }

        }
    }
}