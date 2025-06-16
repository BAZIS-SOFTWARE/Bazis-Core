using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Globalization;

namespace Project.Tasks
{
    public enum LoadKind
    {
        Сила,
        Давление
    }
    public class LoadData : PhysicalData
    {
        /// <summary>
        /// Value
        /// </summary>
        public double Value { get; set; }

        public LoadData(IGroup group, string info) : base(group, info)
        {
            Kind = DataKind.Нагрузка;
        }

        public LoadData(IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Нагрузка;
        }     


        public LoadKind LoadKind { get; set; }

/// <inheritdoc/>

        protected override string GetInfo
        {
            get
            {
                var mfInfo = "*";
                if (FrameFunction?.LocalFrame != null)
                    mfInfo = FrameFunction.LocalFrame.ToString();
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                    Group.Name, LoadKind, Direction, Value, ValueFunction, StartTime, StopTime, mfInfo);
            }
        }
        /// <summary>
        /// LoadFunction
        /// </summary>
        public string ValueFunction { get; set; } = "*";
    }
}