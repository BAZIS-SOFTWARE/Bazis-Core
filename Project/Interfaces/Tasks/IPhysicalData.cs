namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// Direction
    /// </summary>
    public enum Direction : int { None,X,Y,Z };
    /// <summary>
    /// IValuableData
    /// </summary>
    public interface IPhysicalData : IData, IGroupDependent, ITimeDependentData
    {
        /// <summary>
        /// Direction
        /// </summary>
        Direction Direction { get; }
        /// <summary>
        /// FrameFunction
        /// </summary>
        IFrameFunction FrameFunction { get; }
    }
}
