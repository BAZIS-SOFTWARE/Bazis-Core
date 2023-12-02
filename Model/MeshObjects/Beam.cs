using Geometry;
using ModelInterfaces;
using System;
using System.Drawing;

namespace Model.MeshObjects
{
    public class Beam : Element, ILineObject
    {
        public Beam(int number, Node[] nodes) : base(number, nodes)
        {
            InitialColor = Color.FromArgb(255, 192, 203);
            MasterColor = Color.FromArgb(255, 192, 203);
            SlaveColor = Color.FromArgb(255, 192, 203);
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

        public override float DetJ(Point3D point3D)
        {
            throw new NotImplementedException();
        }

        //public override float[] GetDerivativesFormFunctions(Point3D point3D)
        //{
        //    throw new NotImplementedException();
        //}

        //public override float[] GetFormFunctions(Point3D point3D)
        //{
        //    var answer = new float[2];
        //    answer[0] = (nodes[1].Position._x - point3D._x) / CalcLength();
        //    answer[1] = (point3D._x - nodes[0].Position._x) / CalcLength();
        //    return answer;
        //}
    }
}
