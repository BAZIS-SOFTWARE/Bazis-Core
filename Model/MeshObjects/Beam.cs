using Geometry;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using Model.Interfaces;
using Model.Interfaces.IntegralObject;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;

namespace Model.MeshObjects
{
    public class Beam : Element, ILineObject<Node>
    {
        public Beam(int number, Node[] nodes) : base(number, nodes)
        {
            Dim = 1;
            if (nodes.Length < 2)
                throw new Exception($"Коллличество узлов в элементе {number} должно быть не меньше 2");
            else if (nodes.Length > 2)
                    throw new Exception($"Коллличество узлов в элементе {number} должно быть не больше 2");
            
            Level = 1;

            IncidentMatrix = new int[2][];

            IncidentMatrix[0] = [1];
            IncidentMatrix[1] = [0];
        }



        public Beam(int number, Node[] nodes, int level) : this(number, nodes)
        {
            Level = level;
        }
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Стержень;

        /// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Элемент1D;
        /// <inheritdoc/>
        public override int Level 
        
        {
            get => intPoints.Count; 
            set
            {
                intPoints.Clear();
                if (value == 1)
                {
                    CreateIntegrationPoint(2, 0.0);
                }
                else
                {
                    CreateIntegrationPoint(1, -Math.Sqrt(3) / 3);
                    CreateIntegrationPoint(1, Math.Sqrt(3) / 3);
                }
            }
        }

        public void CreateIntegrationPoint(double w, double ksi)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("ksi", ksi);
            intPoint.Weigt = w;
            intPoint.Jacobi = CalcJacobi(intPoint);

            intPoints.Add(intPoint);
        }

        /// <inheritdoc/>

        public override Point3D CalcCentr()
        {
            var center = new Point3D();

            foreach (var node in nodes)
            {
                center = center.Sum(node.Position);
            }
            return center.Div(NumberOfPoints);
        }
        /// <inheritdoc/>

        public float CalcLength()
        {
            return Vector.GetVectorLength(nodes[1].Position.Sub(nodes[0].Position));
        }
        /// <inheritdoc/>

        public override Matrix<double> CalcJacobi(IIntegralPoint intPoint)
        {
            var derFormFunctions = GetDerivativesFormFunctions(intPoint);
            var matrixDFF = Matrix<double>.Build.DenseOfArray(derFormFunctions);
            var matrixPoints = Matrix<double>.Build.Dense(2, 1);

            for (var i = 0; i < nodes.Count; i++)
            {
                matrixPoints[i, 0] = nodes[i].Position._x;
            }

            var result = matrixDFF.Multiply(matrixPoints);
            return result;
        }
        /// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            return new double[,]
            {
                {-0.5,0.5}
            };
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var answer = new double[2];
            answer[0] = (1 - intPoint["ksi"]) / 2;
            answer[1] = (1 + intPoint["ksi"]) / 2;
            return answer;
        }

        public override IEnumerable<IntegralPoint> CreateIntegrationPoints(int level)
        {
            throw new NotImplementedException();
        }
    }
}
