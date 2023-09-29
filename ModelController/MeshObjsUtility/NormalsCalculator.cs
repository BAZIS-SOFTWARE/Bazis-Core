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
                    var plane = new Plane(surface[0].Position, normal);
                    var proj = centre.GetPointProectionOnPlane(plane);
                    var normalV = proj.Sub(centre);
                    foreach (var node in surface.GetNodes())
                    {
                        if (node.ViewState)
                            node.Normal = node.Normal.Sum(normalV);
                    }
                }
            }
        }

        public List<Segment3D> CalcElemsNormals()
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
