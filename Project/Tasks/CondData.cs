using Model.Interfaces;
using Model.Interfaces.Attributes;
using Project.Interfaces.Tasks;
using System;

namespace Project.Tasks
{
    /// <summary>
    /// ValuableData
    /// </summary>
    public abstract class CondData : ICondData
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
        [Obsolete("Не использовать этот конструктор! Указывать время начала и конца по-умолчанию")]
        protected CondData(IGroup group, string info)
        {
            if (info == null)
                throw new ArgumentNullException(nameof(info));
            else if (info.Length == 0)
                throw new Exception("Строка данных пустая!");

            if (group == null)
                throw new NullReferenceException($"Группа {group} не существует. Строка {info}");
            Group = group;

            //SetInfo(info);
        }

        protected CondData(IGroup group, float startTime, float stopTime)
        {
            if (group == null)
                throw new NullReferenceException($"Группа {group} не существует");
            Group = group;

            if(startTime.CompareTo(stopTime) >= 0)
                throw new Exception("Время окончания не может быть меньше или равно времени начала!");

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

        public Direction Direction { get; set; }

    }
}
