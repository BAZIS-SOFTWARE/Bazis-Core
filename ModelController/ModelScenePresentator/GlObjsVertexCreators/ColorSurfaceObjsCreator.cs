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
                    //for (int k = 0; k < surface.NumberOfPoints; k++)
                    //{
                    //    vertexAtributes[stride + 3 * k + 0] = element.MasterColor.R / 255.0f;
                    //    vertexAtributes[stride + 3 * k + 1] = element.MasterColor.G / 255.0f;
                    //    vertexAtributes[stride + 3 * k + 2] = element.MasterColor.B / 255.0f;
                    //}

                    var numbOfVerts = (surface.NumberOfPoints - 2) * 3;

                    if (numbOfVerts == 3)
                    {
                        for (int i = 0; i < numbOfVerts; i++)
                        {
                            vertexAtributes[stride + 3 * i + 0] = element.MasterColor.R / 255.0f;
                            vertexAtributes[stride + 3 * i + 1] = element.MasterColor.G / 255.0f;
                            vertexAtributes[stride + 3 * i + 2] = element.MasterColor.B / 255.0f;
                        }
                    }
                    else
                        for (int k = 3; k < numbOfVerts; k++)
                        {
                            vertexAtributes[stride + 3 + 0] = element.MasterColor.R / 255.0f;
                            vertexAtributes[stride + 3 + 1] = element.MasterColor.G / 255.0f;
                            vertexAtributes[stride + 3 + 2] = element.MasterColor.B / 255.0f;

                            vertexAtributes[stride + 3 * k - 1 + 0] = element.MasterColor.R / 255.0f;
                            vertexAtributes[stride + 3 * k - 1 + 1] = element.MasterColor.G / 255.0f;
                            vertexAtributes[stride + 3 * k - 1 + 2] = element.MasterColor.B / 255.0f;

                            vertexAtributes[stride + 3 * k + 0] = element.MasterColor.R / 255.0f;
                            vertexAtributes[stride + 3 * k + 1] = element.MasterColor.G / 255.0f;
                            vertexAtributes[stride + 3 * k + 2] = element.MasterColor.B / 255.0f;

                        }
                    stride += surface.NumberOfPoints * 3;
                }
                
            }
         }
    }
}
