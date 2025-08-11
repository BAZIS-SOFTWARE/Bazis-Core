using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;

namespace Project.Tasks
{
    public class MatData : CondData
    {
        public MatData(string matName, IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Материал;
            MatName = matName;
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
    }
}