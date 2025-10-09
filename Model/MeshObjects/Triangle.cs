using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Geometry.Attribute;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;

namespace Model.MeshObjects
{
    /// <summary>
    /// Triangle. Треугольник в L координатах
    /// </summary>
    [Warning("Вместо этого класса лучше использовать Triangle_c")]
    public class Triangle : Element2D
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
                    var intPoint = CreateIntegrationPoint(1.0 / 2, 1.0 / 3, 1.0 / 3, 1.0 / 3);
                    intPoints.Add(intPoint);
                }
                else
                {
                    var int0 = CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 0, 1.0 / 2);
                    intPoints.Add(int0);

                    var int1 = CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 1.0 / 2, 0);
                    intPoints.Add(int1);

                    var int2 = CreateIntegrationPoint(1.0 / 6, 0, 1.0 / 2, 1.0 / 2);
                    intPoints.Add(int2);
                }
            }
        }
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Треугольник;

        public Triangle(int number,Node[] nodes) : base(number, nodes)
        {
            if (nodes.Length < 3)
                throw new Exception($"Коллличество узлов в элементе {ElementKind} {number} должно быть не меньше 3");

            Level = 1;
        }

        public Triangle(int number, Node[] nodes, int level) : this(number, nodes)
        {
            Level = level;
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

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return new double[,] { { 1, 0,-1 }, { 0, 1,-1 } };
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            return new double[] { intPoint["l1"], intPoint["l2"], intPoint["l3"] };
        }
/// <inheritdoc/>

        public IntegralPoint CreateIntegrationPoint(double w, double l1, double l2, double l3)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("l1", l1);
            intPoint.AddCoordinate("l2", l2);
            intPoint.AddCoordinate("l3", l3);
            intPoint.Weigt = w;
            intPoint.Jacobi = CalcJacobi(intPoint);
            return intPoint;
        }
/// <inheritdoc/>

        public override IEnumerable<IntegralPoint> CreateIntegrationPoints(int level)
        {
            if (level == 1)
            {
                yield return CreateIntegrationPoint(1.0 / 2, 1.0 / 3, 1.0 / 3, 1.0 / 3);
            }
            else
            {
                yield return CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 0, 1.0 / 2);
                yield return CreateIntegrationPoint(1.0 / 6, 1.0 / 2, 1.0 / 2, 0);
                yield return CreateIntegrationPoint(1.0 / 6, 0, 1.0 / 2, 1.0 / 2);
            }
        }
    }
}
