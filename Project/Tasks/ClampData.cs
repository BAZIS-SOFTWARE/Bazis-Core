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

    public class ClampData : CondData
    {
        public ClampData(IGroup group,float start,float stop) : base (group,start,stop)
        {
            Kind = DataKind.Закрепление;
        }

        public ClampKind ClampKind { get; private set; }

        /// <inheritdoc/>

        protected override string GetInfo
        {
            get {
                var mff = "*";
                if (FrameFunction?.LocalFrame != null)
                    mff = FrameFunction.LocalFrame.ToString();

                return $"{Group.Name} {ClampKind} {Direction} {Value} {TimeFunction} " +
                    $"{StartTime} {StopTime} {mff}"; 
            }
        }
        /// <summary>
        /// Function of one argument
        /// </summary>
        public string TimeFunction { get; private set; } = "*";

        /// <summary>
        /// PowerValue
        /// </summary>
        public float Value { get; private set; } = 1;

        public void TrySetKind(string v)
        {
            ClampKind clampKind;
            Enum.TryParse(v, out clampKind);
            ClampKind = clampKind;
        }

        public void TrySetDirection(string v)
        {
            Direction direction;
            Enum.TryParse(v, out direction);
            Direction = direction;
        }

        public void TrySetValue(string v)
        {
 
            if (!float.TryParse(v, out var clampValue))
                throw new Exception("Величина закрепления задана неверно!");

            Value = clampValue;
        }

        public void TrySetTimeFunction(string v)
        {
            if (v != "*")
                TimeFunction = v;
        }
    }
}