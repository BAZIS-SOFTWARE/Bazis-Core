using Geometry;
using Model;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using ModelController.ModelScenePresentator.GlObjsVertexCreators;

namespace ModelController.MeshObjsUtility
{
    public class NormalsCalculator
    {
        public Dictionary<int, Point3D> CalcNodeNormals(IEnumerable<Element3D> elements)
        {
            var normals = new Dictionary<int, Point3D>();
            foreach (var element in elements)
            {
                var centre = element.CalcCentralPoint();
                foreach (var surface in element.GetSurface())
                {
                    var normal = surface.CalcNormal();
                    var plane = new Plane(surface[0].Position, normal);
                    var proj = centre.GetPointProectionOnPlane(plane);
                    var normalV = proj.Sub(centre);
                    foreach (var node in surface.GetNodes())
                    {
                        if (node.ViewState)
                            if (normals.ContainsKey(node.Number))
                                normals[node.Number] = normals[node.Number].Sum(normalV);
                            else normals.Add(node.Number, normalV);
                    }
                }
            }

            return normals;
        }
        /*/// <summary>
        /// Строит карту соседей для всех точек, определяет центр модели.
        /// </summary>
        /// <param name="obj">Список элементов</param>
        /// <param name="modelCenter">Центр модели</param>
        /// <return>Карта соседей</return>
        public static Dictionary<Node, List<Surface>> BuildNeighborsMap(IEnumerable<ISurfaceObject> obj, out Point3D modelCenter)
        {
            var neighbors = new Dictionary<Node, List<Surface>>();
            modelCenter = new Point3D();
            foreach (var element in obj)
            {
                if (element.ViewState)
                {
                    foreach (var surface in element.GetSurface())
                    {
                        if (surface.ViewState)
                        {
                            foreach (var node in surface.GetNodes())
                            {
                                if (!neighbors.ContainsKey(node))
                                    neighbors.Add(node, new List<Surface>());
                                neighbors[node].Add(surface);
                                modelCenter = modelCenter.Sum(node.Position);
                            }
                        }
                    }
                }
            }
            modelCenter = modelCenter.Div(neighbors.Count);
            return neighbors;
        }*/

        public List<Segment3D> CalcElemsNormals(IEnumerable<Element3D> elements)
        {
            var lines = new List<Segment3D>();

            foreach (var element in elements)
            {
                var centr = element.CalcCentralPoint();
                foreach (var surface in element.GetSurface())
                {
                    var normal = surface.CalcNormal();

                    var plane = new Plane(surface[0].Position, normal);
                    var proj = centr.GetPointProectionOnPlane(plane);

                    var p1 = proj.Sum(normal);

                    var line = new Segment3D(proj, p1);
                    lines.Add(line);
                }
            }
            return lines;
        }
    }
}