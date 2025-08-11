using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.Tasks.Functions;
using System;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;

namespace Project.Tasks
{
    public enum HeatSources
    {
        SPH,
        CIL
    }
    /// <summary>
    /// HeatData
    /// </summary>
    public class HeatData : CondData
    {
        /// <summary>
        /// Heat.J/mm3
        /// </summary>
        public float Heat { get; set; } = 1;

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
                  $"{Heat} {TimeFunction} {ffInfo} {Group.Name} {StartTime} {StopTime}";
            }
        }

        public HeatData(IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Нагрев;
        }

        public void TrySetFrameFunction(string v)
        {
            if (v != "*")
                FrameFunction = (FrameFunction)new FrameFunctionBuilder(v.Split(';'));
        }

        public bool TrySetValue(string v)
        {
            if (!float.TryParse(v, out var heat))
                return false;

            Heat = heat;
            return true;

        }

        public void TrySetTimeFunction(string v)
        {
            if (v != "*")
                TimeFunction = v;
        }
    }
}
