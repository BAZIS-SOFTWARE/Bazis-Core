using Model.Interfaces;
using ModelController.Interfaces;

namespace ModelController.GlObjsVertexCreators
{
    public class NormalsSurfaceObjsCreator : ISurfaceObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject surfaceElement)
        {
            if (surfaceElement.ViewState)
            {
                foreach (var surface in surfaceElement.GetSurface())
                {
                    if (surface.ViewState)
                    {
                        var normal = surface.CalcNormal();
                        for (int k = 0; k < surface.NumberOfPoints; k++)
                        {
                            vertexAtributes[stride + 3 * k + 0] = normal._x;
                            vertexAtributes[stride + 3 * k + 1] = normal._y;
                            vertexAtributes[stride + 3 * k + 2] = normal._z;
                        }
                        stride += surface.NumberOfPoints * 3;
                    }
                }
            }
        }
    }
}