using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;
using System.Collections.Generic;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{

    public class ColorSurfaceObjsCreator : ISurfaceObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject surfObject)
        {
            foreach (var surface in surfObject.GetSurface())
            {
                if (surface.ViewState)
                {
                    for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                    {
                        vertexAtributes[stride + 12 * k + 0] = surfObject.MasterColor.R / 255.0f;
                        vertexAtributes[stride + 12 * k + 1] = surfObject.MasterColor.G / 255.0f;
                        vertexAtributes[stride + 12 * k + 2] = surfObject.MasterColor.B / 255.0f;
                        vertexAtributes[stride + 12 * k + 3] = surfObject.MasterColor.A / 255.0f;

                        vertexAtributes[stride + 12 * k + 4] = surfObject.MasterColor.R / 255.0f;
                        vertexAtributes[stride + 12 * k + 5] = surfObject.MasterColor.G / 255.0f;
                        vertexAtributes[stride + 12 * k + 6] = surfObject.MasterColor.B / 255.0f;
                        vertexAtributes[stride + 12 * k + 7] = surfObject.MasterColor.A / 255.0f;

                        vertexAtributes[stride + 12 * k + 8] = surfObject.MasterColor.R / 255.0f;
                        vertexAtributes[stride + 12 * k + 9] = surfObject.MasterColor.G / 255.0f;
                        vertexAtributes[stride + 12 * k + 10] = surfObject.MasterColor.B / 255.0f;
                        vertexAtributes[stride + 12 * k + 11] = surfObject.MasterColor.A / 255.0f;
                    }

                    stride += (surface.NumberOfPoints - 2) * 4 * 3; // (кол-во точек - 2 ) * 4 атрибута и * 3 треугольника

                }

            }
         }
    }
}
