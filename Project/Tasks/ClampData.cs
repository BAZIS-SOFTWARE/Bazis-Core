using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Globalization;

namespace Project.Tasks
{
    public enum ClampKind
    {
        Жесткое,
        Упругое,
        Симметрия,
        Контакт
    }

    public enum ClampType
    {
        Динамическое,
        Статическое
    }

    public enum ClampFrame
    {
        Глобальная,
        Локальная
    }

    public class ClampData : PhysicalData
    {
        public ClampData(IGroup group, string info) : base(group, info)
        {
            Kind = DataKind.Закрепление;
        }
        public ClampData(IGroup group,float start,float stop) : base (group,start,stop)
        {
            Kind = DataKind.Закрепление;
        }

        public ClampKind ClampKind { get; private set; }

        public ClampType ClampType { get; private set; } = ClampType.Статическое;

        public ClampFrame ClampFrame { get; private set; }
/// <inheritdoc/>

        protected override void SetInfo(string info)
        {
            var clampDataAr = info.Split(' ');

            if (clampDataAr.Length < 6)
                throw new Exception("Строка данных имеет неверный формат!");

            var clmKind = clampDataAr[1];

            ClampKind clampKind;
            Enum.TryParse(clampDataAr[1], out clampKind);
            ClampKind = clampKind;

            Direction direction;
            Enum.TryParse(clampDataAr[2], out direction);
            Direction = direction;

            if (clampDataAr[3] != "*")
                ClampFunction = clampDataAr[3];

            float startTime;
            if (!float.TryParse(clampDataAr[4], out startTime))
                throw new Exception("Время старта задано неверно!");

            StartTime = startTime;

            float stopTime;
            if (!float.TryParse(clampDataAr[5], out stopTime))
                throw new Exception("Время окончания задано неверно!");
            
            StopTime = stopTime;
        }
        /// <inheritdoc/>

        protected override string GetInfo
        {
            get { return string.Format("{0} {1} {2} {3} {4} {5} {6}", 
                Group.Name, ClampKind, Direction, ClampFunction, StartTime, StopTime,"*"); }
        }
        /// <summary>
        /// ClampFunction
        /// </summary>
        public string ClampFunction { get; private set; } = "*";
    }
}