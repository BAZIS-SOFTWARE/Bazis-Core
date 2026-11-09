namespace MaterialDB.MaterialData.Settings
{
    /// <summary>
    /// Настройки газа.
    /// </summary>
    public class GasMaterialSettings : FluidMaterialSettings
    {
        /// <summary>
        /// «Модель плотности». При IdealGas таблица «Плотность» не создаётся и нужна молярная масса.
        /// </summary>
        public DensityModel DensityModel { get; set; } = DensityModel.Table;

        /// <summary>
        /// «Модель вязкости». При Sutherland таблица вязкости не создаётся и нужны коэффициенты Сазерленда.
        /// </summary>
        public ViscosityModel ViscosityModel { get; set; } = ViscosityModel.Table;

        /// <summary>
        /// Молярная масса, кг/моль. Обязательна при модели идеального газа; записывается строкой «M» в «Модель плотности».
        /// При табличной плотности не сохраняется.
        /// </summary>
        public double? MolarMass { get; set; }

        /// <summary>
        /// Коэффициенты Сазерленда. Обязательны при модели вязкости Сазерленда; записываются строками μ0, T0, S
        /// в «Модель вязкости». При табличной вязкости не сохраняются.
        /// </summary>
        public SutherlandCoefficients? Sutherland { get; set; }
    }
}
