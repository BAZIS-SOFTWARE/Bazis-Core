using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces.IntegralObject;
using Model.MeshObjects;
using System.Collections.Generic;

namespace Model.Interfaces.MeshObjects
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
    public interface IElement : IVertexObject<Node>, IIntegralObject
    {
        /// <summary>
        /// GetNodeIndex
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        int GetNodeIndex(Node node);
        /// <summary>
        /// Уровень точности (расчетов) интегрирования внутри элемента.
        /// 1 - одна точка интегрирования. Этот уровень лучше использовать для расчетов граничных условиий.
        /// 2 - две или более. Зависит от формы элемента. Этот уровень лучше использовать для расчетов среды.
        /// </summary>
        int Level { get; set; }
        /// <summary>
        /// Calc jacobian for some intPoint
        /// </summary>
        /// <param name="integralPoint"></param>
        /// Внутренняя точка в локальных координатах
        /// кси,эта,дзета если гекса или пента. кси,эта если прямоугольник. кси если стержень.
        /// l координаты если треугольник или тетраэдр
        /// <returns></returns>
        Matrix<double> CalcJacobi(IIntegralPoint integralPoint);
        /// <summary>
        /// ElementKind
        /// </summary>
        ElementKind ElementKind { get; }

        /// <summary>
        /// this
        /// </summary>
        /// <param name="nodeInd"></param>
        /// <returns></returns>
        Node this[int nodeInd] { get; set; }

        /// <summary>
        /// GetDerivativesFormFunctions
        /// </summary>
        /// <param name="intPoint"></param>
        /// <returns></returns>
        double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint);
        /// <summary>
        /// GetFormFunctions
        /// </summary>
        /// <param name="intPoint"></param>
        /// <returns></returns>
        double[] GetFormFunctions(IIntegralPoint intPoint);
        /// <summary>
        /// CreateIntegrationPoint
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        IEnumerable<IntegralPoint> CreateIntegrationPoints(int level);
    }
}
