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
    public class LoadData : CondData
    {
        /// <summary>
        /// Value
        /// </summary>
        public double Value { get; set; }

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
                    Group.Name, LoadKind, Direction, Value, TimeFunction, StartTime, StopTime, mfInfo);
            }
        }
        /// <summary>
        /// LoadFunction
        /// </summary>
        public string TimeFunction { get; set; } = "*";

        public void TrySetKind(string v)
        {
            LoadKind loadKind;
            if(Enum.TryParse(v, out loadKind))
                LoadKind = loadKind;
        }

        public void TrySetDirection(string v)
        {
            Direction direction;
            if (Enum.TryParse(v, out direction))
                Direction = direction;
        }

        public void TrySetValue(string v)
        {

            if (double.TryParse(v, out var value))
                Value = value;
            else
                throw new Exception("Значение усилия должно быть числом!");
        }

        public void TrySetTimeFunction(string v)
        {
            if (v != "*")
                TimeFunction = v;
        }
    }
}