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
        public float Value { get; set; } = 1;
        /// <summary>
        /// PowerFunction
        /// </summary>
        public string TimeFunction { get; set; } = "*";

        /// <inheritdoc/>
        protected override string GetInfo
        {
            get
            {
                var ffInfo = "*";
                if (FrameFunction != null)
                    ffInfo = FrameFunction.ToString();

                return
                  $"{Value} {TimeFunction} {ffInfo} {Group.Name} {StartTime} {StopTime}";
            }
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
