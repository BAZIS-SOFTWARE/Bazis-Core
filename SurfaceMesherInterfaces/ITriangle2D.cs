using System.Collections.Generic;

namespace SurfaceMesherInterfaces
{
    public interface ITriangle2D
    {
        INode2D A { get; }

        INode2D B { get; }

        INode2D C { get; }

        int Index { get; }

        /// <summary>
        /// Получить площадь треугольника
        /// </summary>
        /// <returns></returns>
        double GetArea();

        double GetQuality();

        IEnumerable<INode2D> GetNodes();

        IPoint2D GetCentroid();
    }
}