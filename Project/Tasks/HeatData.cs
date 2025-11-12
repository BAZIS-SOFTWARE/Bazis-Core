using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.Tasks.Functions;
using PropertiesCalculator.MaterialData;
using System;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;

namespace Project.Tasks
{
    public enum FunctionType
    {
        SPH,
        CIL,
        DEL,
        Custom
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
        public Property TimeFunction { get; set; }

        /// <inheritdoc/>
        protected override string GetInfo
        {
            get
            {
                var ffInfo = "*";
                if (FrameFunction != null)
                    ffInfo = FrameFunction.ToString();

                var tfInfo = "*";
                if (TimeFunction != null)
                    tfInfo = TimeFunction.Name;

                return
                  $"{Heat} {tfInfo} {ffInfo} {Group.Name} {StartTime} {StopTime}";
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

        public void TrySetTimeFunction(Property v)
        {
            if (v != null)
                TimeFunction = v;
        }
    }
}
