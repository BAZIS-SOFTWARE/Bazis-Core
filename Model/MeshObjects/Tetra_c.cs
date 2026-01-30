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
                    var intPoint = CreateIntegrationPoint(1.0 / 6, 1.0 / 4, 1.0 / 4, 1.0 / 4);
                    intPoints.Add(intPoint);
                }
                else
                {
                    var a = 0.58541020;
                    var b = 0.13819660;
                    var int0 = CreateIntegrationPoint(1.0 / 24, a, b, b);
                    intPoints.Add(int0);

                    var int1 = CreateIntegrationPoint(1.0 / 24, b, a, b);
                    intPoints.Add(int1);

                    var int2 = CreateIntegrationPoint(1.0 / 24, b, b, a);
                    intPoints.Add(int2);

                    var int3 = CreateIntegrationPoint(1.0 / 24, b, b, b);
                    intPoints.Add(int3);
                }
            }
        }
        /// <summary>
        /// Tetra_pure
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        public Tetra_c(int number, Node[] nodes) : base(number, nodes)
        {
            if (nodes.Length < 4)
                throw new Exception($"Коллличество узлов в элементе {ElementKind} {number} должно быть не меньше 4");

            surfaces.Add(new ElementSurface(number, 0, new Node[] { nodes[0], nodes[1], nodes[2] }));
            surfaces.Add(new ElementSurface(number, 1, new Node[] { nodes[0], nodes[3], nodes[1] }));
            surfaces.Add(new ElementSurface(number, 1, new Node[] { nodes[1], nodes[3], nodes[2] }));
            surfaces.Add(new ElementSurface(number, 1, new Node[] { nodes[0], nodes[2], nodes[3] }));

            Level = 2;

            IncidentMatrix = new int[4][];

            IncidentMatrix[0] = [1, 2, 3];
            IncidentMatrix[1] = [0, 2, 3];
            IncidentMatrix[2] = [0, 1, 3];
            IncidentMatrix[3] = [0, 1, 2];
        }
        /// <summary>
        /// Tetra_c
        /// </summary>
        /// <param name="number"></param>
        /// <param name="nodes"></param>
        /// <param name="level"></param>
        /// <exception cref="ArgumentException"></exception>
        public Tetra_c(int number, Node[] nodes, int level) : this(number, nodes)
        {
            Level = level;
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

            foreach (var item in GetIntegralPoints())
            {
                vol += Math.Abs(item.Jacobi.Determinant()) * intPoints[counter].Weigt;
                counter++;
            }
            return vol;
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

        public override IEnumerable<IntegralPoint> CreateIntegrationPoints(int level)
        {
            if (level == 1)
            {
                yield return CreateIntegrationPoint(1.0 / 6, 1.0 / 4, 1.0 / 4, 1.0 / 4);
            }
            else
            {
                var a = 0.58541020;
                var b = 0.13819660;
                yield return CreateIntegrationPoint(1.0 / 24, a, b, b);
                yield return CreateIntegrationPoint(1.0 / 24, b, a, b);
                yield return CreateIntegrationPoint(1.0 / 24, b, b, a);
                yield return CreateIntegrationPoint(1.0 / 24, b, b, b);
            }
        }
    }
}
