using ModelInterfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class NormalsPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject)
        {
            if(pointObject.ViewState)
            {
                vertexAtributes[stride + 0] = 0;
                vertexAtributes[stride + 1] = 0;
                vertexAtributes[stride + 2] = 0;

                stride += 3;
            }
        }
    }
}