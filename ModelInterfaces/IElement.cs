using Geometry;

namespace ModelInterfaces
{
    /// <summary>
    /// ElementKind
    /// </summary>
    public enum ElementKind
    {
        /// <summary>
        /// Пента
        /// </summary>
        Пента,
        /// <summary>
        /// Тетра
        /// </summary>
        Тетра,
        /// <summary>
        /// Гекса
        /// </summary>
        Гекса,
        /// <summary>
        /// Треугольник
        /// </summary>
        Треугольник,
        /// <summary>
        /// Квадрат
        /// </summary>
        Квадрат,
        /// <summary>
        /// Стержень
        /// </summary>
        Стержень
    }
    /// <summary>
    /// IElement
    /// </summary>
    public interface IElement : INodeObject
    {
        /// <summary>
        /// DetJ
        /// </summary>
        /// <param name="point3D"></param>
        /// <returns></returns>
        float DetJ(Point3D point3D);
        /// <summary>
        /// ElementKind
        /// </summary>
        ElementKind ElementKind { get; }
    }
}
