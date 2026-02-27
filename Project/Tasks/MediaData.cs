using System;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using MaterialDB.MaterialData;
using Project.Tasks.FrameFunctions;
using Project.Tasks.ParametersFunctions;

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
        public float HexValue { get; set; } = 0;
        /// <summary>
        /// ChanheHeatExchangeInTemperature
        /// </summary>
        public Property HexTempFunc { get; set; }

        /// <summary>
        /// FrameFunction
        /// </summary>
        public ParametersFunction HexParsFunc { get; set; }

        //private float heatExchangeValue = 0;
        //private string heatExchangeFunc = "*";
        //private Property temperatureFunc;
        //private float temperatureValue = 1;

        /// <summary>
        /// HeatExchangeValue
        /// </summary>
        public float TempValue { get; set; } = 1;
        /// <summary>
        /// Temperature of media or body
        /// </summary>
        public Property TempTimeFunc { get; set; }

        /// <summary>
        /// MediaType
        /// </summary>
        public MediaType MediaType 
        { 
            get
            {
                return HexTempFunc == null & HexValue == 0 ?
                    MediaType.ConstantTemp : MediaType.HeatFlux;
            }
        }

/// <inheritdoc/>

        protected override string GetInfo
        {
            get
            {               
                var hexParInfo = "*";
                if (HexParsFunc != null)
                    hexParInfo = HexParsFunc.ToString();

                var hexTempInfo = "*";
                if (HexTempFunc != null)
                    hexTempInfo = HexTempFunc.ToString();

                var hexInfo = $"{HexValue};{hexTempInfo};{hexParInfo}";

                var tempTimeInfo = "*";
                if (TempTimeFunc != null)
                    tempTimeInfo = TempTimeFunc.ToString();

                var tempInfo = $"{TempValue};{tempTimeInfo}";

                return
                  $"{Group.Name} {hexInfo} {tempInfo} {StartTime} {StopTime}";
            }
        }

        internal void TrySetParamFunction(string v)
        {
            throw new NotImplementedException();
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