using MathNet.Numerics.LinearAlgebra;
using Model.Utilities;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.MeshObjects
{
    public class Hexa : Element3D
    {
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Гекса;

        public Hexa(int number, INode[] nodes) : base(number, nodes)
        {
            if (surfaces.Count != 6)
                throw new Exception($"Коллличество поверхностей в гексаэлементе {number} должно быть 6");
            var masterSurface = GetMasterSurface(4, surfaces);
            this.nodes = SortNodes(masterSurface.Number, surfaces).ToList();

            CreateIntegrationPoint(8, 0, 0, 0);
        }



        public Hexa(int number, INode[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)
            {
                case 1:

                    CreateIntegrationPoint(8, 0, 0, 0);
                    // вес тут 8, так как вес каждой точки 0 равен 2 (2*2*2=8)
                    break;
                case 8:
                CreateIntegrationPoint(1, 0.5774, 0.5774, 0.5774);
                CreateIntegrationPoint(1, 0.5774, 0.5774, -0.5774);
                 CreateIntegrationPoint(1, 0.5774, -0.5774, 0.5774);
                CreateIntegrationPoint(1, 0.5774, -0.5774, -0.5774);
                 CreateIntegrationPoint(1, -0.5774, 0.5774, 0.5774);
                 CreateIntegrationPoint(1, -0.5774, 0.5774, -0.5774);
                 CreateIntegrationPoint(1, -0.5774, -0.5774, 0.5774);
                CreateIntegrationPoint(1, -0.5774, -0.5774, -0.5774);
                 break;

                
                default:
                    throw new ArgumentException();
            }
        }

        /// <inheritdoc/>

        public override double CalcVolume()
        {
            var vol = 0.0;
            var counter = 0;

            foreach (var item in GetJacobiMatrixes())
            {
                vol += item.Determinant() * intPoints[counter].Weigt;
                counter++;
            }
            return Math.Abs(vol);
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
    }
}

