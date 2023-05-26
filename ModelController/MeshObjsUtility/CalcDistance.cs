using System;
using Geometry;
using Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsUtility
{
    /// <summary>
    /// CalcDistance
    /// </summary>
    public class CalcDistance
    {
        /// <summary>
        /// DistanceBetweenPlaneAndNode
        /// </summary>
        /// <param name="plane"></param>
        /// Плоскость проецирования
        /// <param name="node"></param>
        /// Узел проецирования
        /// <returns></returns>
        public Line DistanceBetweenPlaneAndNode(Plane plane, Node node)
        {
            float x1 = plane.P0._x;
            float x2 = plane.P1._x;
            float x3 = plane.P2._x;

            float y1 = plane.P0._y;
            float y2 = plane.P1._y;
            float y3 = plane.P2._y;

            float z1 = plane.P0._z;
            float z2 = plane.P1._z;
            float z3 = plane.P2._z;

            Point3D v = new Point3D(x2 - x1, y2 - y1, z2 - z1);
            Point3D v2 = new Point3D(x3 - x1, y3 - y1, z3 - z1);
            var vector = Vector.CrossProd(v, v2);

            var point = plane.P0.GetPointProectionOnPlane(node.Position, vector);
            return new Line(plane.P0, point);
        }
    }
}
