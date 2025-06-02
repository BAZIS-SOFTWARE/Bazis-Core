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

        public ClampKind ClampKind { get; set; }
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

            float clampValue;
            if (!float.TryParse(clampDataAr[3], out clampValue))
                throw new Exception("Величина закрепления задана неверно!");

            ClampValue = clampValue;

            if (clampDataAr[4] != "*")
                ClampFunction = clampDataAr[4];

            float startTime;
            if (!float.TryParse(clampDataAr[5], out startTime))
                throw new Exception("Время старта задано неверно!");

            StartTime = startTime;

            float stopTime;
            if (!float.TryParse(clampDataAr[6], out stopTime))
                throw new Exception("Время окончания задано неверно!");
            
            StopTime = stopTime;
        }
        /// <inheritdoc/>

        protected override string GetInfo
        {
            get {
                var mff = "*";
                if (FrameFunction?.LocalFrame != null)
                    mff = FrameFunction.LocalFrame.ToString();

                return $"{Group.Name} {ClampKind} {Direction} {ClampValue} {ClampFunction} " +
                    $"{StartTime} {StopTime} {mff}"; 
            }
        }
        /// <summary>
        /// ClampFunction
        /// </summary>
        public string ClampFunction { get; set; } = "*";

        /// <summary>
        /// PowerValue
        /// </summary>
        public float ClampValue { get; set; } = 1;
    }
}