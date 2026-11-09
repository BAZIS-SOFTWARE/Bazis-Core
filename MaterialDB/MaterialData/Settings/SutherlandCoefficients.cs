namespace MaterialDB.MaterialData.Settings
{
    /// <summary>
    /// Коэффициенты формулы Сазерленда.
    /// </summary>
    public sealed class SutherlandCoefficients
    {
        /// <summary>
        /// Создаёт набор коэффициентов.
        /// </summary>
        public SutherlandCoefficients(double mu0, double t0, double s)
        {
            Mu0 = mu0;
            T0 = t0;
            S = s;
        }

        /// <summary>
        /// Опорная вязкость μ0, МПа·с.
        /// </summary>
        public double Mu0 { get; }

        /// <summary>
        /// Опорная температура T0, К.
        /// </summary>
        public double T0 { get; }

        /// <summary>
        /// Постоянная Сазерленда S, К.
        /// </summary>
        public double S { get; }
    }
}
