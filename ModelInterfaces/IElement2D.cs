using Geometry;

namespace ModelInterfaces
{
    /// <summary>
    /// ElementKind 3D
    /// </summary>
    public enum ElementKind2D
    {
        /// <summary>
        /// Треугольник
        /// </summary>
        Треугольник,
        /// <summary>
        /// Квадрат
        /// </summary>
        Квадрат
    }
    public interface IElement2D : ISurfaceElement, INormal,ISquare
    {

    }
}
