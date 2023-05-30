using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
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
                        for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                        {
                            vertexAtributes[stride + 9 * k + 0] = surface[0].Position._x;
                            vertexAtributes[stride + 9 * k + 1] = surface[0].Position._y;
                            vertexAtributes[stride + 9 * k + 2] = surface[0].Position._z;

                            vertexAtributes[stride + 9 * k + 3] = surface[k + 1].Position._x;
                            vertexAtributes[stride + 9 * k + 4] = surface[k + 1].Position._y;
                            vertexAtributes[stride + 9 * k + 5] = surface[k + 1].Position._z;

                            vertexAtributes[stride + 9 * k + 6] = surface[k + 2].Position._x;
                            vertexAtributes[stride + 9 * k + 7] = surface[k + 2].Position._y;
                            vertexAtributes[stride + 9 * k + 8] = surface[k + 2].Position._z;

                        }
                        stride += (surface.NumberOfPoints - 2) * 3 * 3; // (кол-во точек - 2 ) * 3 атрибута и * 3 треугольника
                    }
                }
            }
        }
    }
}