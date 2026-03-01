using Project.Tasks.Functions;
using System.Collections.Generic;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// Direction
    /// </summary>
    public enum Direction : int { None,X,Y,Z };
    /// <summary>
    /// IValuableData
    /// </summary>
    public interface ICondData : IData, IGroupDependent, ITimeDependentData
    {
        /// <summary>
        /// Direction
        /// </summary>
        Direction Direction { get; }
        /// <summary>
        /// FrameFunction
        /// </summary>
        Function Function { get; set; }
        
        /// <summary>
        /// Value
        /// </summary>
        double Value { get; set; }
    }
}
