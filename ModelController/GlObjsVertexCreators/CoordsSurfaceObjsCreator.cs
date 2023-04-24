using Model;
using Model.Interfaces;
using ModelController.Interfaces;
using System.Drawing;
using System.Linq;

namespace ModelController.GlObjsVertexCreators
{
    public class CoordsSurfaceObjsCreator : ISurfaceObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject surfaceElement)
        {
            if (surfaceElement.ViewState)
            {
                foreach (var surface in surfaceElement.GetSurface())
                {
                    if (surface.ViewState)
                    {
                        var points = surface.GetPoints().ToArray();

                        for (int k = 0; k < points.Length; k++)
                        {
                            vertexAtributes[stride + 3 * k + 0] = points[k]._x;
                            vertexAtributes[stride + 3 * k + 1] = points[k]._y;
                            vertexAtributes[stride + 3 * k + 2] = points[k]._z;


                            var n0 = new Node(0, surfPoints[0]);
                            var n1 = new Node(1, surfPoints[1]);
                            var n2 = new Node(2, surfPoints[2]);

                            subSurfs.Add(new Surface(0, new Node[] { n0, n1, n2 }, ObjKind.ElementSurface, color));

                            for (int i = 3; i < surfPoints.Count(); i++)
                            {
                                n0 = new Node(0, surfPoints[0]);
                                n1 = new Node(i - 1, surfPoints[i - 1]);
                                n2 = new Node(i, surfPoints[i]);

                                subSurfs.Add(new Surface(0, new Node[] { n0, n1, n2 }, ObjKind.ElementSurface, color));
                            }

                        }
                        stride += surface.NumberOfPoints * 3;
                    }

                }
            }
        }
    }
}