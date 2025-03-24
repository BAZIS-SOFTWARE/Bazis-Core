using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class LayoutSurfaceObjsCreator<T, U> : ISurfaceObjsVertexCreator<T, U>
        where T : ISurface<U> where U : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject<T, U> surfaceElement)
        {
            if (surfaceElement.ViewState)
            {
                foreach (var surface in surfaceElement.GetSurfaces())
                {
                    if (surface.ViewState)
                    {
                        for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                        {
                            vertexAtributes[stride + k + 0] = surfaceElement.Number;
                            vertexAtributes[stride + k + 1] = surfaceElement.Number;
                            vertexAtributes[stride + k + 2] = surfaceElement.Number;
                        }
                        stride += (surface.NumberOfPoints - 2) * 3; // (кол-во точек - 2 ) * 1 атрибут и * 3 точки
                    }
                }
            }
        }
    }
}
