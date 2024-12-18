using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;
using System.Drawing;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class ColorLineObjsCreator<T> : ILineObjsVertexCreator<T>
        where T : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject)
        {
            //Цвет черный с alpha 0.0 - установим alpha в 1.0f
            /*var color = nodeObject.MasterColor;*/
            var color = Color.FromArgb(255, nodeObject.MasterColor.R, nodeObject.MasterColor.G,
                                                                      nodeObject.MasterColor.B);

            for (int k = 0; k < nodeObject.NumberOfPoints; k++)
            {
                vertexAtributes[stride + 4 * k + 0] = color.R / 255.0f;
                vertexAtributes[stride + 4 * k + 1] = color.G / 255.0f;
                vertexAtributes[stride + 4 * k + 2] = color.B / 255.0f;
                vertexAtributes[stride + 4 * k + 3] = color.A / 255.0f;
            }
            stride += nodeObject.NumberOfPoints * 4;
        }
    }
}
