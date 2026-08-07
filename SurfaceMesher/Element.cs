using System.Collections.Generic;

namespace SurfaceMesher
{
    public abstract class Element
    {
        internal Node2D[] nodes;
        /// <summary>
        /// Возвращает все узлы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Node2D> GetNodes()
        {
            foreach (var item in nodes)
            {
                yield return item;
            }
        }

    }
}