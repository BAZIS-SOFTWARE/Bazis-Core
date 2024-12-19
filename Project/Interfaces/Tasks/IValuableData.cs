namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// Direction
    /// </summary>
    public enum Direction : int { None,X,Y,Z };
    /// <summary>
    /// IValuableData
    /// </summary>
    public interface IValuableData : IFrameData
    {
        /// <summary>
        /// Direction
        /// </summary>
        Direction Direction { get; }
    }
}
