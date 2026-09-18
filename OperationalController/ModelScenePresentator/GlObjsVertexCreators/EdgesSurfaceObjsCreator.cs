using Model.Interfaces;
using System.Collections.Generic;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class EdgesSurfaceObjsCreator<T, U>
        where T : ISurface<U> where U : IPoint
    {
        public void Create(ref int stride, bool[] vertexAtributes, IEnumerable<ISurfaceObject<T, U>> surfaceElements, IModelView modelView)
        {
            foreach (var surfaceElement in surfaceElements)
            {
                if (!modelView.GetVisible(surfaceElement.ObjType, surfaceElement.Number))
                    continue;

                foreach (var surface in surfaceElement.GetSurfaces())
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
