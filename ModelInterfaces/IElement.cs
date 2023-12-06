using Geometry;

namespace ModelInterfaces
{
    /// <summary>
    /// ElementKind
    /// </summary>
    public enum ElementKind
    {
        /// <summary>
        /// Стержень
        /// </summary>
        Стержень,
        /// <summary>
        /// Треугольник
        /// </summary>
        Треугольник,
        /// <summary>
        /// Квадрат
        /// </summary>
        Квадрат,
        /// <summary>
        /// Тетра
        /// </summary>
        Тетра,
        /// <summary>
        /// Гекса
        /// </summary>
        Гекса,
        /// <summary>
        /// Пента
        /// </summary>
        Пента
    }
    /// <summary>
    /// IElement
    /// </summary>
    public interface IElement : IVertexObject<INode>, IIntegralObject
    {
        /// <summary>
        /// DetJ
        /// </summary>
        /// <param name="integralPoint"></param>
        /// Внутренняя точка в локальных координатах
        /// кси,эта,дзета если стержень,прямоугольник, гекса, пента
        /// <returns></returns>
        float DetJ(Point3D integralPoint);
        /// <summary>
        /// ElementKind
        /// </summary>
        ElementKind ElementKind { get; }

        /// <summary>
        /// this
        /// </summary>
        /// <param name="nodeInd"></param>
        /// <returns></returns>
        INode this[int nodeInd] { get; }
    }
}
