using Project.Tasks.LocalFrame;
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
        /// LocalFrame
        /// </summary>
        ILocalFrame LocalFrame { get; set; }
    }
}
