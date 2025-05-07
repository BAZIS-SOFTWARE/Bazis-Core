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
    public class HeatData : PhysicalData
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
        protected override string GetInfo
        {
            get
            {
                var mfInfo = "*";
                if (LocalFrame != null)
                    mfInfo = LocalFrame.ToString();

                var ffInfo = "*";
                if (FrameFunction != null)
                    ffInfo = FrameFunction.ToString();

                return
                  $"{PowerValue} {PowerFunction} {ffInfo} {Group.Name} {StartTime} {StopTime} {mfInfo}";
            }
        }


        /// <inheritdoc/>

        protected override void SetInfo(string info)
        {
            var heatDataAr = info.Split(' ');

            if (heatDataAr.Length < 5)
                throw new Exception("Строка данных имеет неверный формат!");

            float value;
            if (float.TryParse(heatDataAr[0], out value))
                PowerValue = value;


            PowerFunction = heatDataAr[1];

            float startTime;
            if (!float.TryParse(heatDataAr[4], out startTime))
                throw new Exception("Время старта задано неверно!");

            StartTime = startTime;

            float stopTime;
            if (!float.TryParse(heatDataAr[5], out stopTime))
                throw new Exception("Время окончания задано неверно!");

            StopTime = stopTime;

        }     

        public HeatData(IGroup group, string info) : base(group, info)
        {
            Kind = DataKind.Нагрев;     
        }

        public HeatData(IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Нагрев;
        }
    }
}
