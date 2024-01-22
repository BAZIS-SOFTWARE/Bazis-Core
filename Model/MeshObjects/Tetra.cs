using Geometry;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.MeshObjects
{
    public class Tetra : Element3D
    {
        public Tetra(int number, INode[] nodes) : base(number, nodes)
        {
            var intPoint = new IntegralPoint();
            intPoint.AddDirection("l1", 1 / 4);
            intPoint.AddDirection("l2", 1 / 4);
            intPoint.AddDirection("l3", 1 / 4);
            intPoint.AddDirection("l4", 1 / 4);
            intPoint.Weigt = CalcVolume();
            //jacobians.Add(DetJ(intPoint));
            intPoints.Add(intPoint);
        }



        public Tetra(int number, INode[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)

            {
                case 1:
                    intPoint = new IntegralPoint();
                    intPoint.AddDirection("l1", 1 / 4);
                    intPoint.AddDirection("l2", 1 / 4);
                    intPoint.AddDirection("l3", 1 / 4);
                    intPoint.AddDirection("l4", 1 / 4);
                    intPoint.Weigt = CalcVolume();
                    intPoints.Add(intPoint);

                    //jacobians.Add(DetJ(intPoint));
                    break;
                case 4:
                    float alpha = 0.58541020f;
                    float beta = 0.13819660f;
                    for (int i = 0; i < 4; i++)
                    {
                        intPoint = new IntegralPoint();
                        for (int j = 0; j < i; j++)
                            intPoint.AddDirection($"l{j + 1}", beta);
                        intPoint.AddDirection($"l{i + 1}", alpha);
                        for (int j = i + 1; j < 4; j++)
                            intPoint.AddDirection($"l{j + 1}", beta);

                        intPoint.Weigt = 1 / 4f;
                        intPoints.Add(intPoint);

                        //jacobians.Add(DetJ(intPoint));
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Тетра;

        /// <inheritdoc/>

        public override float CalcVolume()
        {
            var mCoord = new float[4, 4];
            var p = new Point3D[] { nodes[0].Position, nodes[1].Position, nodes[2].Position, nodes[3].Position };
            for (int j = 0; j < p.Length; j++)
            {
                mCoord[j, 0] = 1;
                mCoord[j, 1] = p[j]._x;
                mCoord[j, 2] = p[j]._y;
                mCoord[j, 3] = p[j]._z;
            }

            var cMatr = Math.Abs(Matrix<float>.Build.DenseOfArray(mCoord).Determinant() / 6);
            //var str = cMatr.ToString(4, 4);
            return cMatr;
        }
        /// <inheritdoc/>

        public override float DetJ(IIntegralPoint point3D)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>

        public override float[] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            // Расчитано для линейного элемента с одной т.интегрирования в центре
            // по формулам 14.11а и б
            return new float[] { 1,0,0, 0,1,0, 0,0,1, -1,-1,-1 };
        }
        /// <inheritdoc/>

        public override float[] GetFormFunctions(IIntegralPoint intPoint)
        {
            return new float[] { intPoint["l1"], intPoint["l2"], intPoint["l3"], intPoint["l4"] };
        }
    }
}
