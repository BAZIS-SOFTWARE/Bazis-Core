using Geometry;
using ModelInterfaces;
using ModelInterfaces.IntegralObject;
using System;
using System.Drawing;

namespace Model.MeshObjects
{
    public class Beam : Element, ILineObject<INode>
    {
        public Beam(int number, Node[] nodes) : base(number, nodes)
        {
            InitialColor = Color.FromArgb(255, 192, 203);
            MasterColor = Color.FromArgb(255, 192, 203);
            SlaveColor = Color.FromArgb(255, 192, 203);

            var intPoint = new IntegralPoint();
            intPoint.AddDirection("ksi", 0.0f);
            intPoint.Weigt = CalcLength();
            intPoints.Add(intPoint);
            //jacobians.Add(DetJ(intPoint));
        }


        public Beam(int number, Node[] nodes, int intNumber) : base(number, nodes)
        {
            IntegralPoint intPoint;
            switch (intNumber)
            {
                case 1:
                    intPoint = new IntegralPoint();
                    intPoint.AddDirection("ksi", 0.0f);
                    intPoint.Weigt = CalcLength();
                    AddIntegralPoint(intPoint);
                    break;
                case 2:
                    intPoint = new IntegralPoint();
                    intPoint.AddDirection("ksi", -(float)Math.Sqrt(3) / 3);
                    intPoint.Weigt = 1;
                    AddIntegralPoint(intPoint);

                    intPoint = new IntegralPoint();
                    intPoint.AddDirection("ksi", (float)Math.Sqrt(3) / 3);
                    intPoint.Weigt = 1;
                    AddIntegralPoint(intPoint);
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

        public override float DetJ(IIntegralPoint point3D)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>

        public override float[] GetDerivativesFormFunctions(IIntegralPoint intPoint)
        {
            var answer = new float[2];
            answer[0] = -1 / CalcLength();
            answer[1] = 1 / CalcLength();
            return answer;
        }
        /// <inheritdoc/>

        public override float[] GetFormFunctions(IIntegralPoint intPoint)
        {
            var answer = new float[2];
            answer[0] = (nodes[1].Position._x - intPoint["ksi"]) / CalcLength();
            answer[1] = (intPoint["ksi"] - nodes[0].Position._x) / CalcLength();
            return answer;
        }
    }
}
