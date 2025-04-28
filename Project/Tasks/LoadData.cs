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
    public class LoadData : ValuableData
    {
        /// <summary>
        /// Value
        /// </summary>
        public float LoadValue { get; set; }

        public LoadData(IGroup group, string info) : base(group, info)
        {
            Kind = DataKind.Нагрузка;
        }

        public LoadKind LoadKind { get; internal set; }
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
 
            LoadValue = value;

            if (loadDataAr[4] != "*")
                LoadFunction = loadDataAr[4];

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
                if (LocalFrame != null)
                    mfInfo = LocalFrame.ToString();
                return string.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                    Group.Name, LoadKind, Direction, LoadValue, LoadFunction, StartTime, StopTime, mfInfo);
            }
        }
        /// <summary>
        /// LoadFunction
        /// </summary>
        public string LoadFunction { get; private set; } = "*";
    }
}