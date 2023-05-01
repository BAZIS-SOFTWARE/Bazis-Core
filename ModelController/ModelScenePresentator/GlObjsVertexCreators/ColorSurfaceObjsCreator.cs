using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class ColorSurfaceObjsCreator : ISurfaceObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject element)
        {
            foreach (var surface in element.GetSurface())
            {
                if (surface.ViewState)
                {
                    for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                    {
                        vertexAtributes[stride + 9 * k + 0] = surface.MasterColor.R / 255.0f;
                        vertexAtributes[stride + 9 * k + 1] = surface.MasterColor.G / 255.0f;
                        vertexAtributes[stride + 9 * k + 2] = surface.MasterColor.B / 255.0f;

                        vertexAtributes[stride + 9 * k + 3] = surface.MasterColor.R / 255.0f;
                        vertexAtributes[stride + 9 * k + 4] = surface.MasterColor.G / 255.0f;
                        vertexAtributes[stride + 9 * k + 5] = surface.MasterColor.B / 255.0f;

                        vertexAtributes[stride + 9 * k + 6] = surface.MasterColor.R / 255.0f;
                        vertexAtributes[stride + 9 * k + 7] = surface.MasterColor.G / 255.0f;
                        vertexAtributes[stride + 9 * k + 8] = surface.MasterColor.B / 255.0f;
                    }

                    stride += (surface.NumberOfPoints - 2) * 3 * 3; // (кол-во точек - 2 ) * 3 атрибута и * 3 треугольника

                }

            }
         }
    }
}
