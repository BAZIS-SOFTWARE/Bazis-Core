namespace GmshApi.Api
{
    /// <summary>
    /// Алгоритмы распределения точек на кривых\поверхностях
    /// </summary>
    public enum TransfiniteAlgorithm
    {
        /// <summary>
        /// Распределение по закнам геометрической прогрессия
        /// </summary>
        Progressive,
        /// <summary>
        /// Равномерное распределение точек с обоих концов кривой
        /// </summary>
        Bump,
        /// <summary>
        /// Beta
        /// </summary>
        Beta
    }
}
