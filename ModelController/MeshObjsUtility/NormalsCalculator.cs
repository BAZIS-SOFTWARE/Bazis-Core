using Geometry;
using ModelInterfaces;
using System.Collections.Generic;

namespace ModelController.MeshObjsUtility
{
    public class NormalsCalculator
    {
        public Dictionary<int, Point3D> CalcNodeNormals(IEnumerable<IElement3D> elements)
        {
            var normals = new Dictionary<int, Point3D>();
            foreach (var element in elements)
            {
                var centre = element.CalcCentr();
                foreach (var surface in element.GetSurfaces())
                {
                    var normal = surface.CalcNormal();
                    var plane = new Plane(surface[0].Position, normal);
                    var proj = centre.GetPointProectionOnPlane(plane);
                    var normalV = proj.Sub(centre);
                    foreach (var node in surface.GetVertexes())
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

        public List<Segment3D> CalcElemsNormals(IEnumerable<IElement3D> elements)
        {
            var lines = new List<Segment3D>();

            foreach (var element in elements)
            {
                var centr = element.CalcCentr();
                foreach (var surface in element.GetSurfaces())
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