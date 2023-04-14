using Model.Interfaces;
using ModelController.Interfaces;
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
                        }
                        stride += surface.NumberOfPoints * 3;
                    }

                }
            }
        }
    }
}