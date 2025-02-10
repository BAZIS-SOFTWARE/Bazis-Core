using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{

    public class ColorSurfaceObjsCreator<T,U> : ISurfaceObjsVertexCreator<T,U> 
        where T : ISurface<U> where U : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject<T,U> surfObject)
        {
            foreach (var surface in surfObject.GetSurfaces())
            {
                if (surface.ViewState)
                {
                    for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                    {
                        vertexAtributes[stride + 12 * k + 0] = surfObject.Color.R / 255.0f;
                        vertexAtributes[stride + 12 * k + 1] = surfObject.Color.G / 255.0f;
                        vertexAtributes[stride + 12 * k + 2] = surfObject.Color.B / 255.0f;
                        vertexAtributes[stride + 12 * k + 3] = surfObject.Color.A / 255.0f;

                        vertexAtributes[stride + 12 * k + 4] = surfObject.Color.R / 255.0f;
                        vertexAtributes[stride + 12 * k + 5] = surfObject.Color.G / 255.0f;
                        vertexAtributes[stride + 12 * k + 6] = surfObject.Color.B / 255.0f;
                        vertexAtributes[stride + 12 * k + 7] = surfObject.Color.A / 255.0f;

                        vertexAtributes[stride + 12 * k + 8] = surfObject.Color.R / 255.0f;
                        vertexAtributes[stride + 12 * k + 9] = surfObject.Color.G / 255.0f;
                        vertexAtributes[stride + 12 * k + 10] = surfObject.Color.B / 255.0f;
                        vertexAtributes[stride + 12 * k + 11] = surfObject.Color.A / 255.0f;
                    }

                    stride += (surface.NumberOfPoints - 2) * 4 * 3; // (кол-во точек - 2 ) * 4 атрибута и * 3 треугольника

                }

            }
         }
    }
}
