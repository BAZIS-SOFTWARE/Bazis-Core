using Geometry;
using Model.GeometryObjects;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Octree;
using System;
using System.Collections.Generic;

namespace OperationalController.MeshObjsUtility
{
    public class FindCornerNodes
    {
        /// <summary>
        /// Ищет угловые точки на ограниченном контуром сетке
        /// </summary>
        /// <param name="surfaceElements">Элементы</param>
        /// <param name="corner">Пороговый угол для добавления точки как угловой</param>
        /// <returns>Список угловых точек</returns>
        public List<Node> Find(IEnumerable<ISurfaceElement> surfaceElements, float corner = 30)
        {
            var bounds = new FindBoundaryEdges();
            var nodes = bounds.GetSortedBoundaryEdges(surfaceElements);
            if(nodes.Count < 4)
                return nodes;

            var corners = new List<Node>();

            var rad = corner * MathF.PI / 180;
            var first = nodes[0];
            var last = nodes[1];
            var lastVec = Vector.GetVectorNorm(last.Position.Sub(first.Position));

            for(var i = 2; i < nodes.Count; ++i)
            {
                var curVec = Vector.GetVectorNorm(nodes[i].Position.Sub(last.Position));
                var value = GetAngle(lastVec, curVec);
                if(value >= rad)
                    corners.Add(last);
                last = nodes[i];
                lastVec = curVec;
            }

            lastVec = Vector.GetVectorNorm(nodes[0].Position.Sub(last.Position));
            var currentVec = Vector.GetVectorNorm(nodes[1].Position.Sub(nodes[0].Position));
            var angle = GetAngle(lastVec, currentVec);
            if (angle >= rad)
                corners.Add(last);

            return corners;
        }

        private float GetAngle(Point3D lastVec, Point3D curVec)
        {
            var cos = Vector.DotProd(lastVec, curVec);
            var value = MathF.Acos(cos);
            return value;
        }
    }
}
