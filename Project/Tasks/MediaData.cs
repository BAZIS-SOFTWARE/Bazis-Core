using System;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using PropertiesCalculator.MaterialData;

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
    public class MediaData : CondData
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
        public float HeatExchangeValue { get; set; } = 0;
        /// <summary>
        /// HeatExchangeFunc
        /// </summary>
        public Property HeatExchangeFunc { get; set; }

        //private float heatExchangeValue = 0;
        //private string heatExchangeFunc = "*";
        //private Property temperatureFunc;
        //private float temperatureValue = 1;

        /// <summary>
        /// HeatExchangeValue
        /// </summary>
        public float TemperatureValue { get; set; } = 1;
        /// <summary>
        /// Temperature of media or body
        /// </summary>
        public Property TemperatureFunc { get; set; }

        /// <summary>
        /// MediaType
        /// </summary>
        public MediaType MediaType 
        { 
            get
            {
                return HeatExchangeFunc == null & HeatExchangeValue == 0 ?
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
                    if (TemperatureFunc == null)
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, "*", TemperatureValue, StartTime, StopTime, mfInfo);
                    else
                        return string.Format("{0} {1} {2} {3} {4} {5}",
Group.Name, "*", TemperatureFunc.Name, StartTime, StopTime, mfInfo);
                }
                else
                {
                    if (HeatExchangeFunc == null & TemperatureFunc == null)
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeValue, TemperatureValue, StartTime, StopTime, mfInfo);
                    else if (HeatExchangeFunc == null & TemperatureFunc != null)
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeValue, TemperatureFunc.Name, StartTime, StopTime, mfInfo);
                    else if (HeatExchangeFunc != null & TemperatureFunc == null)
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeFunc.Name, TemperatureValue, StartTime, StopTime, mfInfo);
                    else
                        return string.Format("{0} {1} {2} {3} {4} {5}",
                      Group.Name, HeatExchangeFunc.Name, TemperatureFunc.Name, StartTime, StopTime, mfInfo);
                }
            }
        }
        /// <summary>
        /// TrySetTemp
        /// </summary>
        /// <param name="v"></param>
        /// <exception cref="NotImplementedException"></exception>
        //public void SetTemp(float v)
        //{
            //if (v == "*")
            //    throw new Exception("Граничное условие тепловой задачи должно содержать сведения о температуре.");

            //var res = 0.0f;
            //if (float.TryParse(v, out res))
            //{
                //TemperatureValue = v;
                //TemperatureFunc = null;
            //}
  
            //else
            //{
            //    temperatureValue = 1;
            //    temperatureFunc = v;
            //}
    
        //}
        /// <summary>
        /// TrySetHeatExchange
        /// </summary>
        /// <param name="v"></param>
        //public void SetHeatExchange(float v)
        //{
            //if (float.TryParse(v, out var media))
            //{
               //HeatExchangeValue = v;
               //HeatExchangeFunc = null;
            //}
               
            //else 
            //{
            //    if (v != "*")
            //        heatExchangeValue = 1;
            //    else
            //        heatExchangeValue = 0;
            //    heatExchangeFunc = v;
            //}

        //}
    }
}