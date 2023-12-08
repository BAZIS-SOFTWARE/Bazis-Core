using Geometry;
using ModelInterfaces.IntegralObject;
using System.Collections.Generic;

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
        /// Calc jacobian for some intPoint
        /// </summary>
        /// <param name="integralPoint"></param>
        /// Внутренняя точка в локальных координатах
        /// кси,эта,дзета если гекса или пента. кси,эта если прямоугольник. кси если стержень.
        /// l координаты если треугольник или тетраэдр
        /// <returns></returns>
        float DetJ(IIntegralPoint integralPoint);
        /// <summary>
        /// Get Jacobians of all intPoints
        /// </summary>
        /// <returns></returns>
        IEnumerable<float> GetDetsJ();
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
        /// <summary>
        /// GetDerivativesFormFunctions
        /// </summary>
        /// <param name="intPoint"></param>
        /// <returns></returns>
        float[] GetDerivativesFormFunctions(IIntegralPoint intPoint);
        /// <summary>
        /// GetFormFunctions
        /// </summary>
        /// <param name="intPoint"></param>
        /// <returns></returns>
        float[] GetFormFunctions(IIntegralPoint intPoint);
    }
}
