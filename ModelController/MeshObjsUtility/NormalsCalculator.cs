using Geometry;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsUtility
{
    public class NormalsCalculator
    {
        SurfaceElement[] elements;
        public NormalsCalculator(SurfaceElement[] elements)
        {
            this.elements = elements;
        }

        public void CalcNodeNormals()
        {
            foreach (var element in elements)
            {
                var centre = element.CalcCentralPoint();
                foreach (var surface in element.GetSurface())
                {
                    var normal = surface.CalcNormal();
                    var proj = centre.GetPointProectionOnPlane(new Plane(surface[0].Position, normal));
                    var normalV = proj.Sub(centre);
                    foreach (var node in surface.GetNodes())
                    {
                        if (node.ViewState)
                            node.Normal = node.Normal.Sum(normalV);
                    }
                }
            }
        }

        public List<Line> CalcElemsNormals()
        {
            var lines = new List<Line>();

            foreach (var element in elements)
            {
                var centr = element.CalcCentralPoint();
                foreach (var surface in element.GetSurface())
                {
                    var normal = surface.CalcNormal();
                    var nodeOnSide = surface[0];

                    var p0 = centr.GetPointProectionOnPlane(new Plane(nodeOnSide.Position, normal));
                    var p1 = p0.Sum(normal);

                    var line = new Line(p0, p1);
                    lines.Add(line);
                }
            }
            return lines;
        }
    }
}
