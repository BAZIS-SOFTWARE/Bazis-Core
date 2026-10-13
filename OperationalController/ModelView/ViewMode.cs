namespace OperationalController
{
    /// <summary>
    /// Определяет способ отображения набора объектов.
    /// </summary>
    public enum ViewMode
    {
        /// <summary>
        /// Отображение точками.
        /// </summary>
        Point,

        /// <summary>
        /// Отображение линиями.
        /// </summary>
        Line,

        /// <summary>
        /// Отображение поверхностью.
        /// </summary>
        Surface,

        /// <summary>
        /// Отображение поверхностью с линиями.
        /// </summary>
        LineSurface
    }
}
