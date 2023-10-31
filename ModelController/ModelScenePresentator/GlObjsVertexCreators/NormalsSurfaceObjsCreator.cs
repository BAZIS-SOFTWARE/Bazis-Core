using Geometry;
using Model;
using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class NormalsSurfaceObjsCreator : ISurfaceObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject surfaceElement)
        {
            if (surfaceElement.ViewState)
            {
                var centr = CalculateCenter(surfaceElement);
                foreach (var surface in surfaceElement.GetSurface())
                {
                    if (surface.ViewState)
                    {
                        var normal = surface.CalcNormal();
                        var plane = new Plane(surface[0].Position, normal);
                        var proj = centr.GetPointProectionOnPlane(plane);
                        normal = proj.Sub(centr);
                        for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                        {
                            vertexAtributes[stride + 9 * k + 0] = normal._x;
                            vertexAtributes[stride + 9 * k + 1] = normal._y;
                            vertexAtributes[stride + 9 * k + 2] = normal._z;

                            vertexAtributes[stride + 9 * k + 3] = normal._x;
                            vertexAtributes[stride + 9 * k + 4] = normal._y;
                            vertexAtributes[stride + 9 * k + 5] = normal._z;

                            vertexAtributes[stride + 9 * k + 6] = normal._x;
                            vertexAtributes[stride + 9 * k + 7] = normal._y;
                            vertexAtributes[stride + 9 * k + 8] = normal._z;
                        }

                        stride += (surface.NumberOfPoints - 2) * 3 * 3; // (кол-во точек - 2 ) * 3 атрибута и * 3 треугольника*/
                    }
                }
            }
        }
        protected virtual Point3D CalculateCenter(ISurfaceObject surfaceElement)
        {
            return surfaceElement.CalcCentralPoint();
        }
    }

    public class NormalsSurface2DObjsCreator : NormalsSurfaceObjsCreator
    {
        private Point3D Center { get; set; }
        protected override Point3D CalculateCenter(ISurfaceObject surfaceElement)
        {
            return Center;
        }

        public void SetCenter(IEnumerable<ISurfaceObject> obj)
        {
            var points = new HashSet<Node>();
            var modelCenter = new Point3D();
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
                                if (!points.Contains(node))
                                {
                                    points.Add(node);
                                    modelCenter = modelCenter.Sum(node.Position);
                                }
                            }
                        }
                    }
                }
            }
            Center = modelCenter.Div(points.Count);
        }
    }
}