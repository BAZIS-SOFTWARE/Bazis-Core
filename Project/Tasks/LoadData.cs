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
        public float Value { get; set; }

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

        protected override void SetInfo(string info)
        {
            var loadDataAr = info.Split(' ');

            if (loadDataAr.Length < 7)
                throw new Exception("Строка данных имеет неверный формат!");

            LoadKind loadKind;
            Enum.TryParse(loadDataAr[1], out loadKind);
            LoadKind = loadKind;

            Direction direction;
            Enum.TryParse(loadDataAr[2], out direction);
            Direction = direction;

            float value;
            if (!float.TryParse(loadDataAr[3], out value))
                throw new Exception("Значение усилия должно быть числом!");
 
            Value = value;

            if (loadDataAr[4] != "*")
                TimeFunction = loadDataAr[4];

            float startTime;
            if (!float.TryParse(loadDataAr[5], out startTime))
                throw new Exception("Время старта задано неверно!");

            StartTime = startTime;

            float stopTime;
            if (!float.TryParse(loadDataAr[6], out stopTime))
                throw new Exception("Время окончания задано неверно!");

            StopTime = stopTime;
        }

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
    }
}