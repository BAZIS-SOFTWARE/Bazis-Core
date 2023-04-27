using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class ColorPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject)
        {
            var color = pointObject.MasterColor;

            vertexAtributes[stride + 0] = color.R / 255.0f;
            vertexAtributes[stride + 1] = color.G / 255.0f;
            vertexAtributes[stride + 2] = color.B / 255.0f;

            stride += 3;
        }
    }
}
