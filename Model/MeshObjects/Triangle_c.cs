using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model.MeshObjects
{
    public class Triangle_c : Element2D
    {
        /// <inheritdoc/>
        public override int Level
        {
            get
            {
                if (intPoints.Count == 1)
                    return 1;
                else return 2;
            }
            set
            {
                intPoints.Clear();
                if (value == 1)
                {
                    var intPoint = CreateIntegrationPoint(1.0 / 2, 1.0 / 3, 1.0 / 3);
                    intPoints.Add(intPoint);
                }
                else
                {

                    var int0 = CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 0);
                    intPoints.Add(int0);
                    var int1 = CreateIntegrationPoint(1.0 / 6, 0, 1.0 / 2);
                    intPoints.Add(int1);
                    var int2 = CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 1.0 / 2);
                    intPoints.Add(int2);
                }
            }
        }
        /// <summary>
        /// Triangle_c
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        public Triangle_c(int number, Node[] nodes) : base(number, nodes)
        {
            if (nodes.Length < 3)
                throw new Exception($"Коллличество узлов в элементе {ElementKind} {number} должно быть не меньше 3");

            Level = 1;
        }
        /// <summary>
        /// Triangle_c
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <param name="level"></param>
        /// <exception cref="ArgumentException"></exception>
        public Triangle_c(int number, Node[] nodes, int level) : this(number, nodes)
        {
            Level = level;
        }

        /// <summary>
        /// CreateIntegrationPoint
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public override IEnumerable<IntegralPoint> CreateIntegrationPoints(int level)
        {
            if (level == 1)
            {
                yield return CreateIntegrationPoint(1.0 / 2, 1.0 / 3, 1.0 / 3);
            }
            else
            {
                yield return CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 0);
                yield return CreateIntegrationPoint(1.0 / 6, 0, 1.0 / 2);
                yield return CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 1.0 / 2);
            }
        }
        /// <inheritdoc/>

        public override ElementKind ElementKind => ElementKind.Треугольник;
/// <inheritdoc/>

        public override Matrix<double> CalcJacobi(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(3, 2);

            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
                matrixPoints[i, 1] = nodes[i].Position._y;
            }

            var result = matrixDFF.Multiply(matrixPoints);
            return result;
        }
/// <inheritdoc/>

        public override double CalcSquare()
        {
            var sqr = 0.0;
            var counter = 0;

            foreach (var item in GetIntegralPoints())
            {
                sqr += Math.Abs(item.Jacobi.Determinant()) * intPoints[counter].Weigt;
                counter++;
            }
            return sqr;
        }
/// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return new double[,]
{
                { -1, 1, 0},// i,j,k to ksi axis
                { -1, 0, 1} // i,j,k to eta axis
};
        }
/// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var ksi = intPoint["ksi"];
            var eta = intPoint["eta"];

            var formFunctions = new double[]
            {
                1 - ksi - eta,
                ksi,
                eta
            };
            return formFunctions;
        }
    }
}
