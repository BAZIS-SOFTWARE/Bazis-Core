using Geometry;

namespace OperationalController.GeomObjsCreator
{
    /// <summary>
    /// Геометрические данные линейного предпросмотра фаски.
    /// </summary>
    public sealed class ChamferPreviewData
    {
        /// <summary>
        /// Создаёт данные предпросмотра по концам ребра и двум векторам смещения.
        /// </summary>
        public ChamferPreviewData(
            Point3D edgeStart,
            Point3D edgeEnd,
            Point3D firstOffset,
            Point3D secondOffset)
        {
            EdgeStart = edgeStart;
            EdgeEnd = edgeEnd;
            FirstOffset = firstOffset;
            SecondOffset = secondOffset;
        }

        /// <summary>Начало выбранного ребра.</summary>
        public Point3D EdgeStart { get; }

        /// <summary>Конец выбранного ребра.</summary>
        public Point3D EdgeEnd { get; }

        /// <summary>Вектор смещения контура на первой соседней поверхности.</summary>
        public Point3D FirstOffset { get; }

        /// <summary>Вектор смещения контура на второй соседней поверхности.</summary>
        public Point3D SecondOffset { get; }

        /// <summary>
        /// Возвращает две продольные и две замыкающие линии контура фаски.
        /// </summary>
        public Segment3D[] CreateSegments()
        {
            var firstStart = EdgeStart.Sum(FirstOffset);
            var firstEnd = EdgeEnd.Sum(FirstOffset);
            var secondStart = EdgeStart.Sum(SecondOffset);
            var secondEnd = EdgeEnd.Sum(SecondOffset);

            return new[]
            {
                new Segment3D(firstStart, firstEnd),
                new Segment3D(secondStart, secondEnd),
                new Segment3D(firstStart, secondStart),
                new Segment3D(firstEnd, secondEnd)
            };
        }
    }
}
