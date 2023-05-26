using System;
using Geometry;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsUtility
{
    public class CalcDistance
    {
        public Line DistanceBetweenPlaneAndNode(Plane plane, Node[] nodes, Node checkNode)
        {
            float x1 = nodes[0].Position._x;
            float x2 = nodes[1].Position._x;
            float x3 = nodes[2].Position._x;

            float y1 = nodes[0].Position._y;
            float y2 = nodes[1].Position._y;
            float y3 = nodes[2].Position._y;

            float z1 = nodes[0].Position._z;
            float z2 = nodes[1].Position._z;
            float z3 = nodes[2].Position._z;

            Point3D v = new Point3D(x2 - x1, y2 - y1, z2 - z1);
            Point3D v2 = new Point3D(x3 - x1, y3 - y1, z3 - z1);
            var vector = Vector.GetVectorsProd(v, v2);

            var point = nodes[0].Position.GetPointProectionOnPlane(checkNode.Position, vector);
            return new Line(nodes[0].Position, point);
        }
    }
}
