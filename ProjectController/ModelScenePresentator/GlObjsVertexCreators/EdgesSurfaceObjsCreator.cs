using Model.Interfaces;
using System.Collections.Generic;

namespace ProjectController.ModelScenePresentator.GlObjsVertexCreators
{
    public class EdgesSurfaceObjsCreator<T,U>
        where T : ISurface<U> where U : IPoint
    {
        public void Create(ref int stride, bool[] vertexAtributes, IEnumerable<ISurfaceObject<T,U>> surfaceElements)
        {
            foreach (var surfaceElement in surfaceElements)
            {
                if (surfaceElement.ViewState)
                {
                    foreach (var surface in surfaceElement.GetSurfaces())
                    {
                        if (surface.ViewState)
                        {
                            for (int i = 0; i < surface.NumberOfPoints - 2; i++)
                            {
                                if (i == 0)
                                {
                                    vertexAtributes[stride + 3 * i] = true;
                                    vertexAtributes[stride + 3 * i + 1] = true;
                                    vertexAtributes[stride + 3 * i + 2] = true;
                                }
 
                                else
                                {
                                    vertexAtributes[stride + 3 * i - 1] = false;
                                    vertexAtributes[stride + 3 * i] = false;
                                    vertexAtributes[stride + 3 * i + 1] = true;
                                    vertexAtributes[stride + 3 * i + 2] = true;
                                }
                            }
                            stride += (surface.NumberOfPoints - 2) * 3;
                        }
                    }
                }
            }
        }
    }
}
