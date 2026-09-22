using MathNet.Numerics.LinearAlgebra;
using Model.Utilities;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.MeshObjects
{
    public class Hexa : Element3D
    {
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Гекса;
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
                    var intPoint =CreateIntegrationPoint(8, 0, 0, 0);
                    intPoints.Add(intPoint);
                }
                else
                {
                    var int0 = CreateIntegrationPoint(1, 0.5774, 0.5774, 0.5774);
                    int0.Index = 0;
                    intPoints.Add(int0);

                    var int1 = CreateIntegrationPoint(1, 0.5774, 0.5774, -0.5774);
                    int1.Index = 1;
                    intPoints.Add(int1);

                    var int2 = CreateIntegrationPoint(1, 0.5774, -0.5774, 0.5774);
                    int2.Index = 2;
                    intPoints.Add(int2);

                    var int3 = CreateIntegrationPoint(1, 0.5774, -0.5774, -0.5774);
                    int3.Index = 3;
                    intPoints.Add(int3);

                    var int4 = CreateIntegrationPoint(1, -0.5774, 0.5774, 0.5774);
                    int4.Index = 4;
                    intPoints.Add(int4);

                    var int5 = CreateIntegrationPoint(1, -0.5774, 0.5774, -0.5774);
                    int5.Index = 5;
                    intPoints.Add(int5);

                    var int6 = CreateIntegrationPoint(1, -0.5774, -0.5774, 0.5774);
                    int6.Index = 6;
                    intPoints.Add(int6);

                    var int7 = CreateIntegrationPoint(1, -0.5774, -0.5774, -0.5774);
                    int7.Index = 7;
                    intPoints.Add(int7);
                }
            }
        }

        public Hexa(int number, Node[] nodes) : base(number, nodes)
        {
            if (nodes.Length < 8)
                throw new Exception($"Коллличество узлов в элементе {ElementKind} {number} должно быть не меньше 8");

            //Точки те же, но перечислены в обратном направлениии
            surfaces.Add(new ElementSurface(this, 0, new Node[] { nodes[3], nodes[2], nodes[1], nodes[0] }));
            surfaces.Add(new ElementSurface(this, 1, new Node[] { nodes[5], nodes[6], nodes[7], nodes[4] }));
            surfaces.Add(new ElementSurface(this, 2, new Node[] { nodes[1], nodes[5], nodes[4], nodes[0] }));
            surfaces.Add(new ElementSurface(this, 3, new Node[] { nodes[6], nodes[2], nodes[3], nodes[7] }));
            surfaces.Add(new ElementSurface(this, 4, new Node[] { nodes[2], nodes[6], nodes[5], nodes[1] }));
            surfaces.Add(new ElementSurface(this, 5, new Node[] { nodes[3], nodes[0], nodes[4], nodes[7] }));

            Level = 1;

            IncidentMatrix = new int[8][];

            IncidentMatrix[0] = [1, 3, 4];
            IncidentMatrix[1] = [0, 2, 5];
            IncidentMatrix[2] = [1, 3, 6];
            IncidentMatrix[3] = [0, 2, 7];
            IncidentMatrix[4] = [0, 5, 7];
            IncidentMatrix[5] = [1, 4, 6];
            IncidentMatrix[6] = [2, 5, 7];
            IncidentMatrix[7] = [3, 4, 6];
        }



        public Hexa(int number, Node[] nodes, int level) : this(number, nodes)
        {
            Level = level;
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

        public override Matrix<double> CalcJacobi(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(8, 3);

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
            var eta = intPoint["eta"];
            var ksi = intPoint["ksi"];
            var dze = intPoint["dze"];

            return new double[,]
            {
                // по ksi
                { -(1-eta)*(1-dze)/8,   (1-eta)*(1-dze) / 8,   (1+eta)*(1-dze)/8,   -(1+eta)*(1-dze)/8,
                        -(1-eta)*(1+dze)/8,   (1-eta)*(1+dze)/8,   (1+eta)*(1+dze)/8,   -(1+eta)*(1+dze)/8},
                // по eta
                { -(1-ksi)*(1-dze)/8,   -(1+ksi)*(1-dze)/8,   (1+ksi)*(1-dze)/8,   (1-ksi)*(1-dze)/8,
                    -(1-ksi)*(1+dze)/8,   -(1+ksi)*(1+dze)/8,   (1+ksi)*(1+dze)/8,   (1-ksi)*(1+dze)/8   },
                // по dze
                {  -(1-ksi)*(1-eta)/8,   -(1+ksi)*(1-eta)/8,   -(1+ksi)*(1+eta)/8,   -(1-ksi)*(1+eta)/8,
                    (1-ksi)*(1-eta)/8,   (1+ksi)*(1-eta)/8,   (1+ksi)*(1+eta)/8,   (1-ksi)*(1+eta)/8 }
};

        }
/// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var eta = intPoint["eta"];
            var ksi = intPoint["ksi"];
            var dze = intPoint["dze"];

            var formFunctions = new double[]
            {
      

                (1-ksi)*(1-eta)*(1-dze)/8,
                (1+ksi)*(1-eta)*(1-dze)/8,
                (1+ksi)*(1+eta)*(1-dze)/8,
                (1-ksi)*(1+eta)*(1-dze)/8,
                (1-ksi)*(1-eta)*(1+dze)/8,
                (1+ksi)*(1-eta)*(1+dze)/8,
                (1+ksi)*(1+eta)*(1+dze)/8,
                (1-ksi)*(1+eta)*(1+dze)/8

            };

          return formFunctions;
           
        }
/// <inheritdoc/>

        public override IEnumerable<IntegralPoint> CreateIntegrationPoints(int level)
        {
            if (level == 1)
            {
                yield return CreateIntegrationPoint(8, 0, 0, 0);
            }
            else
            {
                yield return CreateIntegrationPoint(1, 0.5774, 0.5774, 0.5774);
                yield return CreateIntegrationPoint(1, 0.5774, 0.5774, -0.5774);
                yield return CreateIntegrationPoint(1, 0.5774, -0.5774, 0.5774);
                yield return CreateIntegrationPoint(1, 0.5774, -0.5774, -0.5774);
                yield return CreateIntegrationPoint(1, -0.5774, 0.5774, 0.5774);
                yield return CreateIntegrationPoint(1, -0.5774, 0.5774, -0.5774);
                yield return CreateIntegrationPoint(1, -0.5774, -0.5774, 0.5774);
                yield return CreateIntegrationPoint(1, -0.5774, -0.5774, -0.5774);
            }
        }
    }
}

