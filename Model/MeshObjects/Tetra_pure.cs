using MathNet.Numerics.LinearAlgebra;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.MeshObjects
{
    public class Tetra_pure : Element3D
    {
        public Tetra_pure(int number, INode[] nodes) : base(number, nodes)
        {
            CreateIntegrationPoint(1.0/6, 1.0 / 4, 1.0 / 4, 1.0 / 4);
        }

        public override ElementKind ElementKind => throw new NotImplementedException();

        public override Matrix<double> CalcJacobi(IIntegralPoint integralPoint)
        {
            throw new NotImplementedException();
        }

        public override double CalcVolume()
        {
            throw new NotImplementedException();
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
                { -1, 0, 0, -1 }, 
                { 0, 1, 0, -1 }, 
                { 0, 0, 1, -1 } 
            };
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            return new double[] { intPoint["ksi"], intPoint["eta"], intPoint["dze"] };
        }
    }
}
