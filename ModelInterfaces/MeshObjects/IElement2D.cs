using Geometry.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace ModelInterfaces.MeshObjects
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

    }
}
