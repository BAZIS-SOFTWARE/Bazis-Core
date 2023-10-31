using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Model;
using Geometry;
using ModelController.MeshObjsUtility;
using System;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class SmoothNormalsElementObjsCreator : ISurfaceObjsVertexCreator
    {
        Dictionary<Node, List<Surface>> map;
        Point3D modelCentre;
        float minCos;
        public SmoothNormalsElementObjsCreator(IEnumerable<ISurfaceObject> objects, string vertexType)
        {
            //map = NormalsCalculator.BuildNeighborsMap(objects, out modelCentre);
            var tokens = vertexType.Split(' ');
            var outAngle = 0.0f;
            var angle = tokens.Length > 2 && float.TryParse(tokens[2], out outAngle) ? outAngle : 60;
            var radAngle = angle / 180f * (float)Math.PI;
            minCos = (float)Math.Cos(radAngle);
        }

        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject surfaceElement)
        {
            if (surfaceElement.ViewState)
            {
                var centre = CalcCentralPoint(surfaceElement);
                foreach (var surface in surfaceElement.GetSurface())
                {
                    if (surface.ViewState)
                    {
                        Node first = null, last = null, current = null;
                        foreach (var node in surface.GetNodes().Select((n, i) => (n, i)))
                        {
                            last = current;
                            current = node.n;
                            if (node.i > 2)
                            {
                                WriteNormal(vertexAtributes, ref stride, surface, first, centre);
                                WriteNormal(vertexAtributes, ref stride, surface, last, centre);
                            }
                            else if (node.i == 0) first = node.n;
                            WriteNormal(vertexAtributes, ref stride, surface, current, centre);
                        }
                    }
                }
            }
        }

        protected virtual Point3D CalcCentralPoint(ISurfaceObject surfaceElement)
        {
            return modelCentre;
        }

        private void WriteNormal(float[] vertexAttributes, ref int stride, Surface surface, Node node, Point3D centre)
        {
            var values = map[node];
            var normal = GetNormalOfElement(surface, centre);
            node.Normal = new Point3D(normal._x, normal._y, normal._z);
            foreach (var surf in values)
            {
                if (!surface.Equals(surf))
                {
                    var otherNormal = GetNormalOfElement(surf, centre);
                    if (Vector.GetAngleVectors(normal, otherNormal) >= minCos)
                        node.Normal = node.Normal.Sum(otherNormal);
                }
            }
            vertexAttributes[stride] = node.Normal._x;
            vertexAttributes[stride + 1] = node.Normal._y;
            vertexAttributes[stride + 2] = node.Normal._z;
            stride += 3;
        }

        private Point3D GetNormalOfElement(Surface surface, Point3D centre)
        {
            var normal = surface.CalcNormal();
            var pointDir = surface[0].Position.Sub(centre);
            var dot = Vector.DotProd(normal, pointDir);
            var sign = Math.Sign(dot);
            normal = normal.Mult(sign);
            return normal;
        }
    }

    public class SmoothNormalsElement3DObjsCreator : SmoothNormalsElementObjsCreator
    {
        public SmoothNormalsElement3DObjsCreator(IEnumerable<ISurfaceObject> objects, string vertexType) : base(objects, vertexType)
        {
        }

        protected override Point3D CalcCentralPoint(ISurfaceObject surfaceElement)
        {
            return surfaceElement.CalcCentralPoint();
        }
    }
}
