using GmshApi;

namespace OperationalController
{
    /// <summary>
    /// Хранит параметры разбиения кривой.
    /// </summary>
    public sealed class CurveMeshingSettings
    {
        /// <summary>
        /// Возвращает признак заданного трансфинитного разбиения.
        /// </summary>
        public bool IsTransfinite { get; }

        /// <summary>
        /// Возвращает число узлов на кривой.
        /// </summary>
        public int NodesCount { get; }

        /// <summary>
        /// Возвращает тип распределения узлов.
        /// </summary>
        public MeshType MeshType { get; }

        /// <summary>
        /// Возвращает коэффициент распределения узлов.
        /// </summary>
        public double Coefficient { get; }

        /// <summary>
        /// Инициализирует параметры разбиения кривой.
        /// </summary>
        public CurveMeshingSettings(bool isTransfinite, int nodesCount, MeshType meshType, double coefficient)
        {
            IsTransfinite = isTransfinite;
            NodesCount = nodesCount;
            MeshType = meshType;
            Coefficient = coefficient;
        }
    }
}
