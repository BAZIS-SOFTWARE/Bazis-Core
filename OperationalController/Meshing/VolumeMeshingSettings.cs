namespace OperationalController
{
    /// <summary>
    /// Перечисляет типы разбиения объёма.
    /// </summary>
    public enum VolumeMeshingType
    {
        /// <summary>
        /// Параметры разбиения не заданы.
        /// </summary>
        Undefined,

        /// <summary>
        /// Регулярное разбиение.
        /// </summary>
        Regular,

        /// <summary>
        /// Градиентное разбиение.
        /// </summary>
        Gradient
    }

    /// <summary>
    /// Хранит параметры градиентного разбиения объёма.
    /// </summary>
    public sealed class GradientVolumeMeshingSettings
    {
        /// <summary>
        /// Возвращает степень изменения размера элементов.
        /// </summary>
        public double Power { get; }

        /// <summary>
        /// Возвращает максимальную длину перехода.
        /// </summary>
        public double DistanceMaximum { get; }

        /// <summary>
        /// Возвращает размер элементов на поверхности.
        /// </summary>
        public double SurfaceElementSize { get; }

        /// <summary>
        /// Возвращает размер элементов в центре.
        /// </summary>
        public double CoreElementSize { get; }

        /// <summary>
        /// Инициализирует параметры градиентного разбиения объёма.
        /// </summary>
        public GradientVolumeMeshingSettings(double power, double distanceMaximum, double surfaceElementSize, double coreElementSize)
        {
            Power = power;
            DistanceMaximum = distanceMaximum;
            SurfaceElementSize = surfaceElementSize;
            CoreElementSize = coreElementSize;
        }
    }

    /// <summary>
    /// Хранит параметры разбиения объёма.
    /// </summary>
    public sealed class VolumeMeshingSettings
    {
        /// <summary>
        /// Возвращает тип разбиения объёма.
        /// </summary>
        public VolumeMeshingType Type { get; }

        /// <summary>
        /// Возвращает параметры градиентного разбиения.
        /// </summary>
        public GradientVolumeMeshingSettings GradientSettings { get; }

        /// <summary>
        /// Инициализирует параметры разбиения объёма.
        /// </summary>
        public VolumeMeshingSettings(VolumeMeshingType type, GradientVolumeMeshingSettings gradientSettings = null)
        {
            Type = type;
            GradientSettings = gradientSettings;
        }
    }
}
