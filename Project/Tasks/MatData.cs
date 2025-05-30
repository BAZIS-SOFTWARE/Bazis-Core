using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;

namespace Project.Tasks
{
    public class MatData : PhysicalData
    {
        public MatData(IGroup group, string info) : base(group, info)
        {
            Kind = DataKind.Материал;
        }

        public MatData(IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Материал;
        }
        /// <summary>
        /// MatName
        /// </summary>
        public string MatName { get; set; }
/// <inheritdoc/>

        protected override string GetInfo
        {
            get
            {
                var mfInfo = "*";
                if (FrameFunction?.LocalFrame != null)
                    mfInfo = FrameFunction.LocalFrame.ToString();
                return string.Format("{0} {1} {2} {3} {4}", Group.Name, MatName, StartTime, StopTime, mfInfo);
            }
        }
/// <inheritdoc/>

        protected override void SetInfo(string info)
        {
            if (info.Split(' ').Length < 4)
                throw new Exception("Строка данных имеет неверный формат!");

            var infoAr = info.Split(' ');

            MatName = infoAr[1];
            float startTime;
            if (!float.TryParse(infoAr[2], out startTime))
                throw new Exception("Время старта задано неверно!");

            StartTime = startTime;

            float stopTime;
            if (!float.TryParse(infoAr[3], out stopTime))
                throw new Exception("Время окончания задано неверно!");

            StopTime = stopTime;
        }
    }
}