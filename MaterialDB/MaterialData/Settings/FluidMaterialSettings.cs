namespace MaterialDB.MaterialData.Settings
{
    /// <summary>
    /// Настройки жидкости. Базовые для настроек газа.
    /// </summary>
    public class FluidMaterialSettings : MaterialSettings
    {
        /// <summary>
        /// «Опорное давление», МПа.
        /// </summary>
        public double ReferencePressure { get; set; } = 1;
    }
}
