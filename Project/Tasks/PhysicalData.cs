using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;

namespace Project.Tasks
{
    /// <summary>
    /// ValuableData
    /// </summary>
    public abstract class PhysicalData : IPhysicalData
    {
        /// <inheritdoc/>
        public DataKind Kind { get; internal set; }
        /// <inheritdoc/>

        protected abstract string GetInfo { get; }
        /// <summary>
        /// PhysicalData
        /// </summary>
        /// <param name="group"></param>
        /// <param name="info"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        /// <exception cref="NullReferenceException"></exception>

        protected PhysicalData(IGroup group, string info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            else if (info.Length == 0)
                throw new Exception("Строка данных пустая!");

            if (group == null)
                throw new NullReferenceException($"Группа {group} не существует. Строка {info}");
            Group = group;

            SetInfo(info);
        }
        /// <inheritdoc/>

        protected abstract void SetInfo(string info);
        protected PhysicalData(IGroup group, float startTime, float stopTime)
        {
            if (group == null)
                throw new NullReferenceException($"Группа {group} не существует");
            Group = group;

            if(startTime.CompareTo(stopTime) == -1)
                throw new Exception("Время окончания не может быть меньше времени начала!");

            StartTime = startTime;
            StopTime = stopTime;
        }

        //public string GroupName { get; internal set; }
        /// <inheritdoc/>

        public float StartTime { get; set; }
        /// <inheritdoc/>

        public float StopTime { get; set; }
        /// <inheritdoc/>

        public IGroup Group { get; set; }


        /// <inheritdoc/>
        public IFrameFunction FrameFunction { get; set; }
        /// <inheritdoc/>


        public override string ToString()
        {
            return $"{Kind} : {GetInfo}";
        }

        /// <inheritdoc/>

        public ILocalFrame LocalFrame { get; set; }

        /// <inheritdoc/>

        public Direction Direction { get; internal set; }

    }
}
