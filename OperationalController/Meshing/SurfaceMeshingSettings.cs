using GmshApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OperationalController
{
    /// <summary>
    /// Хранит параметры разбиения поверхности.
    /// </summary>
    public sealed class SurfaceMeshingSettings
    {
        /// <summary>
        /// Возвращает признак заданного трансфинитного разбиения.
        /// </summary>
        public bool IsTransfinite { get; }

        /// <summary>
        /// Возвращает номера угловых точек.
        /// </summary>
        public IReadOnlyList<int> CornerPointNumbers { get; }

        /// <summary>
        /// Возвращает направление разбиения.
        /// </summary>
        public Arrangement Arrangement { get; }

        /// <summary>
        /// Возвращает признак рекомбинации элементов поверхности.
        /// </summary>
        public bool IsRecombined { get; }

        /// <summary>
        /// Возвращает номера встроенных кривых.
        /// </summary>
        public IReadOnlyList<int> EmbeddedCurveNumbers { get; }

        /// <summary>
        /// Возвращает номера граничных точек поверхности.
        /// </summary>
        public IReadOnlyList<int> BoundaryPointNumbers { get; }

        /// <summary>
        /// Инициализирует параметры разбиения поверхности.
        /// </summary>
        public SurfaceMeshingSettings(bool isTransfinite, IEnumerable<int> cornerPointNumbers, Arrangement arrangement, bool isRecombined, IEnumerable<int> embeddedCurveNumbers, IEnumerable<int> boundaryPointNumbers)
        {
            IsTransfinite = isTransfinite;
            CornerPointNumbers = cornerPointNumbers?.ToArray() ?? Array.Empty<int>();
            Arrangement = arrangement;
            IsRecombined = isRecombined;
            EmbeddedCurveNumbers = embeddedCurveNumbers?.ToArray() ?? Array.Empty<int>();
            BoundaryPointNumbers = boundaryPointNumbers?.ToArray() ?? Array.Empty<int>();
        }
    }
}
