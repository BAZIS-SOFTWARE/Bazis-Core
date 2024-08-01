using Geometry;
using ModelInterfaces;
using ModelInterfaces.IntegralObject;
using ModelInterfaces.MeshObjects;
using System;
using System.Drawing;

namespace Model.MeshObjects
{
    public class Beam : Element, ILineObject<INode>
    {
        public Beam(int number, INode[] nodes) : base(number, nodes)
        {
            InitialColor = Color.FromArgb(255, 192, 203);
            MasterColor = Color.FromArgb(255, 192, 203);
            SlaveColor = Color.FromArgb(255, 192, 203);

            var intPoint = new IntegralPoint();
            intPoint.AddCoordinate("ksi", 0.0f);
            intPoint.Weigt = CalcLength();
            intPoints.Add(intPoint);
            //jacobians.Add(DetJ(intPoint));
        }



        public Beam(int number, INode[] nodes, int intpNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intpNumber)
            {
                case 1:
                    intPoint = new IntegralPoint();
                    intPoint.AddCoordinate("ksi", 0.0f);
                    intPoint.Weigt = CalcLength();
                    intPoints.Add(intPoint);
                    break;
                case 2:
                    intPoint = new IntegralPoint();
                    intPoint.AddCoordinate("ksi", -(float)Math.Sqrt(3) / 3);
                    intPoint.Weigt = 1;
                    intPoints.Add(intPoint);

                    intPoint = new IntegralPoint();
                    intPoint.AddCoordinate("ksi", (float)Math.Sqrt(3) / 3);
                    intPoint.Weigt = 1;
                    intPoints.Add(intPoint);
                    break;
                default:
                    throw new ArgumentException();
            }
        }
        /// <inheritdoc/>

        public override ElementKind ElementKind { get; } = ElementKind.Стержень;

        /// <inheritdoc/>

        public override ObjType ObjType { get; } = ObjType.Элемент1D;
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
            return Vector.GetVectorLenght(nodes[1].Position.Sub(nodes[0].Position));
        }
        /// <inheritdoc/>

        public override double DetJ(IIntegralPoint point3D)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>

        public override double[,] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            var answer = new double[1,2];
            answer[0,0] = -1 / CalcLength();
            answer[0,1] = 1 / CalcLength();
            return answer;
        }
        /// <inheritdoc/>

        public override double[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var answer = new double[2];
            answer[0] = (nodes[1].Position._x - intPoint["ksi"]) / CalcLength();
            answer[1] = (intPoint["ksi"] - nodes[0].Position._x) / CalcLength();
            return answer;
        }
    }
}
