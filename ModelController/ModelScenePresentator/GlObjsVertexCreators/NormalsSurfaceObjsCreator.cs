using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
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
                        //for (int k = 0; k < surface.NumberOfPoints; k++)
                        //{
                        //    vertexAtributes[stride + 3 * k + 0] = normal._x;
                        //    vertexAtributes[stride + 3 * k + 1] = normal._y;
                        //    vertexAtributes[stride + 3 * k + 2] = normal._z;
                        //}

                        var numbOfVerts = (surface.NumberOfPoints - 2) * 3;

                        if (numbOfVerts == 3)
                        {
                            for (int i = 0; i < numbOfVerts; i++)
                            {
                                vertexAtributes[stride + 3 * i + 0] = normal._x;
                                vertexAtributes[stride + 3 * i + 1] = normal._y;
                                vertexAtributes[stride + 3 * i + 2] = normal._z;
                            }
                        }
                        else
                            for (int k = 3; k < numbOfVerts; k++)
                            {
                                vertexAtributes[stride + 3 + 0] = normal._x;
                                vertexAtributes[stride + 3 + 1] = normal._y;
                                vertexAtributes[stride + 3 + 2] = normal._z;

                                vertexAtributes[stride + 3 * k - 1 + 0] = normal._x;
                                vertexAtributes[stride + 3 * k - 1 + 1] = normal._y;
                                vertexAtributes[stride + 3 * k - 1 + 2] = normal._z;

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