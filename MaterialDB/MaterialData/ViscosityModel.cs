namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Способ задания вязкости («Модель вязкости»).
    /// </summary>
    public enum ViscosityModel
    {
        /// <summary>
        /// Таблица «Динамическая вязкость».
        /// </summary>
        Table = 0,
        /// <summary>
        /// Формула Сазерленда.
        /// </summary>
        Sutherland = 1
    }
}
