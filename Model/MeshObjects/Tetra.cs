using Geometry;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces.Attributes;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;

namespace Model.MeshObjects
{
    [Warning("Не использовать в решателе. Вместо него применять Tetra_c")]
    public class Tetra : Element3D
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
                    var intPoint = CreateIntegrationPoint(0,1.0/6, 1.0 / 4, 1.0 / 4, 1.0 / 4, 1.0 / 4);
                    intPoints.Add(intPoint);
                }
                else
                {
                    var a = 0.58541020;
                    var b = 0.13819660;
                    var int0 = CreateIntegrationPoint(0,1.0 / 4, a, b, b, b);
                    intPoints.Add(int0);

                    var int1 = CreateIntegrationPoint(1,1.0 / 4, b, a, b, b);
                    intPoints.Add(int1);

                    var int2 = CreateIntegrationPoint(2,1.0 / 4, b, b, a, b);
                    intPoints.Add(int2);

                    var int3 = CreateIntegrationPoint(3,1.0 / 4, b, b, b, a);
                    intPoints.Add(int3);
                }
            }
        }
        public Tetra(int number, Node[] nodes) : base(number, nodes)
        {
            if (nodes.Length < 4)
                throw new Exception($"Коллличество узлов в элементе {ElementKind} {number} должно быть не меньше 4");
            //Точки те же, но перечислены в обратном направлениии
            surfaces.Add(new ElementSurface(number, 0, new Node[] { nodes[2], nodes[1], nodes[0] }));
            surfaces.Add(new ElementSurface(number, 1, new Node[] { nodes[1], nodes[3], nodes[0] }));
            surfaces.Add(new ElementSurface(number, 1, new Node[] { nodes[2], nodes[3], nodes[1] }));
            surfaces.Add(new ElementSurface(number, 1, new Node[] { nodes[3], nodes[2], nodes[0] }));

            Level = 1;
        }



        public Tetra(int number, Node[] nodes, int level) : this(number, nodes)
        {
            Level = level;
        }
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Тетра;

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

        public IntegralPoint CreateIntegrationPoint(int index,double w, double l1, double l2, double l3,double l4)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("l1", l1);
            intPoint.AddCoordinate("l2", l2);
            intPoint.AddCoordinate("l3", l3);
            intPoint.AddCoordinate("l4", l4);
            intPoint.Weigt = w;
            intPoint.Index = index;

            intPoint.Jacobi = CalcJacobi(intPoint);

            return intPoint;
        }

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

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return new double[,] { { 1, 0,0,-1 }, { 0, 1,0,-1 }, { 0, 0, 1,-1 } };
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            return new double[] { intPoint["l1"], intPoint["l2"], intPoint["l3"], intPoint["l4"] };
        }

        public override IEnumerable<IntegralPoint> CreateIntegrationPoints(int level)
        {
            if (level == 1)
            {
                yield return CreateIntegrationPoint(0,1, 1.0 / 4, 1.0 / 4, 1.0 / 4, 1.0 / 4);
            }
            else
            {
                var a = 0.58541020;
                var b = 0.13819660;
                yield return CreateIntegrationPoint(0,1.0 / 4, a, b, b, b);
                yield return CreateIntegrationPoint(1,1.0 / 4, b, a, b, b);
                yield return CreateIntegrationPoint(2,1.0 / 4, b, b, a, b);
                yield return CreateIntegrationPoint(3,1.0 / 4, b, b, b, a);
            }
        }
    }
}
