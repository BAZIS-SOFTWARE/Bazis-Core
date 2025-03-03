using Geometry.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Model.MeshObjects;

namespace Model.Interfaces.MeshObjects
{
    /// <summary>
    /// IElement2D
    /// </summary>
    public interface IElement2D : ISurfaceElement, ISquare
    {
        /// <summary>
        /// TransitionMatrix. Матрица перехода к плоскости XOY
        /// </summary>
        Matrix<double> TransitionMatrix { get; set; }
        /// <summary>
        /// GetTransitionMatrix
        /// </summary>
        /// <returns></returns>
        double[,] CalcTransitionMatrix( IPoint3D[] points);
        /// <summary>
        /// CreateIntegrationPoint
        /// </summary>
        /// <param name="w"></param>
        /// <param name="ksi"></param>
        /// <param name="eta"></param>
        /// <returns></returns>
        IntegralPoint CreateIntegrationPoint(double w, double ksi, double eta);

    }
}
