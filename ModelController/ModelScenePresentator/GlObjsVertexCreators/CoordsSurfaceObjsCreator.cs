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
                        var numbOfVerts = (surface.NumberOfPoints - 2) * 3;

                        if (numbOfVerts == 3)
                        {
                            for (int i = 0; i < numbOfVerts; i++)
                            {
                                vertexAtributes[stride + 3 * i + 0] = surface[i].Position._x;
                                vertexAtributes[stride + 3 * i + 1] = surface[i].Position._y;
                                vertexAtributes[stride + 3 * i + 2] = surface[i].Position._z;
                            }
                        }
                        else
                            for (int k = 3; k < numbOfVerts; k++)
                            {
                                vertexAtributes[stride + 3 + 0] = surface[0].Position._x;
                                vertexAtributes[stride + 3 + 1] = surface[0].Position._y;
                                vertexAtributes[stride + 3 + 2] = surface[0].Position._z;

                                vertexAtributes[stride + 3 * k - 1 + 0] = surface[k - 1].Position._x;
                                vertexAtributes[stride + 3 * k - 1 + 1] = surface[k - 1].Position._y;
                                vertexAtributes[stride + 3 * k - 1 + 2] = surface[k - 1].Position._z;

                                vertexAtributes[stride + 3 * k + 0] = surface[k].Position._x;
                                vertexAtributes[stride + 3 * k + 1] = surface[k].Position._y;
                                vertexAtributes[stride + 3 * k + 2] = surface[k].Position._z;

                            }
                        stride += numbOfVerts * 3;
                    }

                }
            }
        }
    }
}