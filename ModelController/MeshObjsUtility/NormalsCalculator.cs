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
            //var interfaceNodesFinder = new FindInterfacedNodes(elements);
            //var interfaceNodes = interfaceNodesFinder.Find();
            //var dict = new Dictionary<int, Point3D>();

            //foreach (var interfaceNode in interfaceNodes)
            //    dict.Add(interfaceNode.Key, new Point3D());

            foreach (var element in elements)
            {
                foreach (var surface in element.GetSurface())
                {
                    var normal = surface.CalcNormal();
                    foreach (var node in surface.GetNodes())
                    {
                        if (node.ViewState)
                            node.Normal = node.Normal.Sum(normal);
                    }
                }
            }

            //return dict;
        }

        public List<Geometry.Line> CalcElemsNormals()
        {

            var lines = new List<Geometry.Line>();

            foreach (var element in elements)
            {
                var centr = element.CalcCentralPoint();
                foreach (var surface in element.GetSurface())
                {
                    var normal = surface.CalcNormal();
                    var nodeOnSide = surface[0];

                    var p0 = centr.GetPointProectionOnPlane(normal, nodeOnSide.Position);
                    var p1 = p0.Sum(normal);

                    var line = new Geometry.Line(p0, p1);
                    lines.Add(line);
                }
            }
            return lines;
        }
    }
}
