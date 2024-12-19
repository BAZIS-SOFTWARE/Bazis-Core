using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Interfaces.Tasks
{
    public interface IFrameData : IData, IGroupDependent, ITimeDependentData
    {
        /// <summary>
        /// FrameFunction
        /// </summary>
        IFrameFunction FrameFunction { get; }
        /// <summary>
        /// MovedFrame
        /// </summary>
        IMovedFrame MovedFrame { get; }
        /// <summary>
        /// TrajectoryInfo
        /// </summary>
        string TrajectoryInfo { get; }
    }
}
