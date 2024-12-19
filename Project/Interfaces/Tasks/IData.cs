namespace Project.Interfaces.Tasks
{
    public enum DataKind
    {
        Mat,
        Med,
        Heat,
        Clamp,
        Load
    }
    /// <summary>
    /// IData
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Kind
        /// </summary>
        DataKind Kind { get; }
        /// <summary>
        /// GetInfo
        /// </summary>
        string GetInfo { get; }
        /// <summary>
        /// SetInfo
        /// </summary>
        /// <param name="info"></param>
        void SetInfo(string info);
    }
}
