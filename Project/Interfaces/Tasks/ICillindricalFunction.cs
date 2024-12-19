namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// ICillindricalFunction
    /// </summary>
    public interface ICillindricalFunction : IFrameFunction
    {
        /// <summary>
        /// BottomDiam
        /// </summary>
        float BottomDiam { get; }
        /// <summary>
        /// UpperDiam
        /// </summary>
        float UpperDiam { get; }
        /// <summary>
        /// Length
        /// </summary>
        float Length { get; }
    }
}