using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.MeshObjects
{
    /// <summary>
    /// Tetra_pure. Tetra using pure decart local coordinate system. 
    /// </summary>
    public class Tetra_c : Element3D
    {
        /// <summary>
        /// Tetra_pure
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        public Tetra_c(int number, Node[] nodes) : base(number, nodes)
        {
            CreateIntegrationPoint(1.0/6, 1.0 / 4, 1.0 / 4, 1.0 / 4);
        }
        /// <summary>
        /// Tetra_c
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <param name="intpNumber"></param>
        /// <exception cref="ArgumentException"></exception>
        public Tetra_c(int number, Node[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)

            {
                case 1:
                    CreateIntegrationPoint(1.0 / 6, 1.0 / 4, 1.0 / 4, 1.0 / 4);
                    break;
                case 4:
                    var a = 0.58541020;
                    var b = 0.13819660;
                    CreateIntegrationPoint(1.0 / 24, a, b, b );
                    CreateIntegrationPoint(1.0 / 24, b, a, b );
                    CreateIntegrationPoint(1.0 / 24, b, b, a );
                    CreateIntegrationPoint(1.0 / 24, b, b, b );
                    break;
                default:
                    throw new ArgumentException("Введите другое кол-во точек интегрирования");
            }
        }
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Тетра;
/// <inheritdoc/>

        public override Matrix<double> CalcJacobi(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(4, 3);

            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
                matrixPoints[i, 2] = nodes[i].Position._z;
            }

            var result = matrixDFF.Multiply(matrixPoints);
            return result;
        }
        /// <inheritdoc/>

        public override double CalcVolume()
        {
            var vol = 0.0;
            var counter = 0;

            foreach (var item in GetJacobiMatrixes())
            {
                vol += Math.Abs(item.Determinant()) * intPoints[counter].Weigt;
                counter++;
            }
            return Math.Abs(vol);
        }

        /// <summary>
        /// CreateIntegrationPoint
        /// </summary>
        /// <param name="w"></param>
        /// <param name="ksi"></param>
        /// <param name="eta"></param>
        /// <param name="dze"></param>
        public void CreateIntegrationPoint(double w, double ksi, double eta, double dze)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("ksi", ksi);
            intPoint.AddCoordinate("eta", eta);
            intPoint.AddCoordinate("dze", dze);
            intPoint.Weigt = w;

            intPoints.Add(intPoint);
            jacobiMatrixes.Add(CalcJacobi(intPoint));
        }

        /// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return new double[,] 
            { 
                { -1, 1, 0, 0 }, 
                { -1, 0, 1, 0 }, 
                { -1, 0, 0, 1 } 
            };
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var ksi = intPoint["ksi"];
            var eta = intPoint["eta"];
            var dze = intPoint["dze"];

            var formFunctions = new double[]
            {
                1 - ksi - eta - dze,
                ksi,
                eta,
                dze
            };

            return formFunctions;
        }
    }
}
