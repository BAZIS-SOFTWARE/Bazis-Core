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

        protected override string GetInfo
        {
            get {
                var mff = "*";
                if (FrameFunction?.LocalFrame != null)
                    mff = FrameFunction.LocalFrame.ToString();

                return $"{Group.Name} {ClampKind} {Direction} {Value} {ValueFunction} " +
                    $"{StartTime} {StopTime} {mff}"; 
            }
        }
        /// <summary>
        /// Function of one argument
        /// </summary>
        public string ValueFunction { get; set; } = "*";

        /// <summary>
        /// PowerValue
        /// </summary>
        public float Value { get; set; } = 1;
    }
}