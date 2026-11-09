namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Способ задания плотности («Модель плотности»).
    /// </summary>
    public enum DensityModel
    {
        /// <summary>
        /// Таблица «Плотность».
        /// </summary>
        Table = 0,
        /// <summary>
        /// Уравнение состояния идеального газа.
        /// </summary>
        IdealGas = 1
    }
}
