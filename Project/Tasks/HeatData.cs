using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.Tasks.Functions;
using System;
using System.Globalization;

namespace Project.Tasks
{
    public enum HeatSources
    {
        ARC,
        FSWPin,
        FSWShoulder,
        LW
    }
    /// <summary>
    /// HeatData
    /// </summary>
    public class HeatData : ValuableData
    {
        /// <summary>
        /// PowerValue
        /// </summary>
        public float PowerValue { get; private set; } = 1;
        /// <summary>
        /// PowerFunctionValue
        /// </summary>
        public string PowerFunction{ get; private set; } = "*";

        /// <inheritdoc/>

        public override string GetInfo
        {
            get
            {
                return FrameFunction == null ?
                  string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4}", PowerFunction
                    , Group.Name, StartTime, StopTime, TrajectoryInfo)
                                    :
                  string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3} {4}", FrameFunction.ToString()
                    ,Group.Name, StartTime, StopTime, TrajectoryInfo);
            }
        }

/// <inheritdoc/>

        public override string ToString()
        {
            return string.Format("{0} : {1}",Name,GetInfo);
        }
/// <inheritdoc/>

        public override void SetInfo(string info)
        {
            var heatDataAr = info.Split(' ');

            if (heatDataAr.Length < 5)
                throw new Exception("Строка данных имеет неверный формат!");


            var heatSourceAr = heatDataAr[0].Split(';');

            if(heatSourceAr.Length == 1)
            {
                PowerFunction = heatDataAr[0];
            }
            else
            {
                HeatSources processType;
                if(!Enum.TryParse(heatSourceAr[0], out processType))
                    throw new Exception("Источник задан неверно!");

                if (processType == HeatSources.ARC)
                {
                    FrameFunction = (Arc)new ArcBuilder().SetCurrent(heatSourceAr[2]).SetVoltage(heatSourceAr[3]).
                        SetWidth(heatSourceAr[1]);
                }

                else if (processType == HeatSources.FSWPin)
                {
                    FrameFunction = (FSWPin)new FSWPinBuilder().SetRotSpeed(heatSourceAr[1]).SetLength(heatSourceAr[2]).
                        SetBottomDiam(heatSourceAr[3]).SetUpperDiam(heatSourceAr[4]).SetYieldFunc(heatSourceAr[5]);

                }
                else if (processType == HeatSources.FSWShoulder)
                {
                    FrameFunction = (FSWShoulder)new FSWShoulderBuilder().SetAxisForce(heatSourceAr[1]).
                        SetRotSpeed(heatSourceAr[2]).SetLength(heatSourceAr[3]).
                        SetBottomDiam(heatSourceAr[4]).SetUpperDiam(heatSourceAr[5]).
                        SetFricModuleFunc(heatSourceAr[6]);
                }
                else
                {
                    FrameFunction = (Lazer)new LazerBuilder().SetPower(heatSourceAr[1]).SetLength(heatSourceAr[2]).SetUpperDiam(heatSourceAr[3]).SetBottomDiam(heatSourceAr[4]);
                }
            }

            var mffData = heatDataAr[4].Split(';');
            
            if (mffData.Length > 1)
            {
                var velosity = float.Parse(mffData[1], NumberStyles.Float);
                MovedFrame = new MovedFrame(velosity);

                if (mffData.Length == 4)
                    MovedFrame.SetShiftingAndRotation("*");
                else
                    MovedFrame.SetShiftingAndRotation(mffData[4]);
            }              
 

            float startTime;
            if (!float.TryParse(heatDataAr[2], out startTime))
                throw new Exception("Время старта задано неверно!");

            StartTime = startTime;

            float stopTime;
            if (!float.TryParse(heatDataAr[3], out stopTime))
                throw new Exception("Время окончания задано неверно!");

            StopTime = stopTime;
        }

        public HeatData(IGroup group, string info) : base(group, info)
        {
            Kind = DataKind.Heat;
            Name = "Нагрев";       
        }
    }
}
