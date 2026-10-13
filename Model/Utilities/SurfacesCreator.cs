using Geometry;
using Model.MeshObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Utilities
{
    /// <summary>
    /// SurfacesCreator
    /// </summary>
    public class SurfacesCreator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static List<ElementSurface> CreateSurfacesFromSetOfNodes(IElement owner, Node[] nodes)
        {
            var surfaces = new List<ElementSurface>();
            var centr = CalcCentr(nodes);
            var surfNumber = 0;

            foreach (var surfNodes in SurfacesFinder.FindSurfaceNodes(nodes))
            {
                surfNodes.RoundSort(); //Anticlockwise(nodes);
                var surface = new ElementSurface(owner, surfNumber, surfNodes);

                var normal = surface.CalcNormal();
                var plane = new Plane(surface[0].Position, normal);
                var proj = centr.GetPointProectionOnPlane(plane);
                var vec = proj.Sub(centr);
                var res = Geometry.Vector.DotProd(normal, vec);

                if (res < 0)
                    surface.ReverseVertexes();

                surfaces.Add(surface);
                surfNumber++;
            }

            return surfaces;
        }
        /// <summary>
        /// Вычисление центра области узлов
        /// </summary>
        /// <returns></returns>
        private static Point3D CalcCentr(Node[] nodes)
        {
            var center = new Point3D();

            foreach (var node in nodes)
            {
                center = center.Sum(node.Position);
            }
            return center.Div(nodes.Length);
        }       
    }
}
