using System.Collections.Generic;

namespace Model.Utilities
{
    /// <summary>
    /// Сравнивает поверхности по набору номеров их узлов.
    /// Ключи приходят отсортированными, поэтому порядок обхода узлов роли не играет.
    /// </summary>
    /// <remarks>
    /// Нужен вместо <see cref="Model.Interfaces.Comparers.ElementSurfaceEqualityComparer{T}"/>:
    /// тот делегирует в GetHashCode поверхности, а ElementSurface его не переопределяет,
    /// поэтому равные поверхности получают разные хеши и в словаре не находятся.
    /// </remarks>
    public class SurfaceNodesComparer : IEqualityComparer<int[]>
    {
        /// <inheritdoc/>
        public bool Equals(int[] first, int[] second)
        {
            if (ReferenceEquals(first, second))
                return true;

            if (first == null || second == null)
                return false;

            if (first.Length != second.Length)
                return false;

            for (var index = 0; index < first.Length; index++)
            {
                if (first[index] != second[index])
                    return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public int GetHashCode(int[] nodeNumbers)
        {
            if (nodeNumbers == null)
                return 0;

            var hash = 17;
            foreach (var number in nodeNumbers)
                hash = hash * 31 + number;

            return hash;
        }
    }
}
