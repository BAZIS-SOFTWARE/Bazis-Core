using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class ColorLineObjsCreator : ILineObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, INodeObject nodeObject)
        {
            var color = nodeObject.MasterColor;

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
