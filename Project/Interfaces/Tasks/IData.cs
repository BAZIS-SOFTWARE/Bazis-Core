namespace Project.Interfaces.Tasks
{
    public enum DataKind
    {
        Материал,
        Среда,
        Нагрев,
        Закрепление,
        Нагрузка
    }
    /// <summary>
    /// IData
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// Kind
        /// </summary>
        DataKind Kind { get; }
    }
}
