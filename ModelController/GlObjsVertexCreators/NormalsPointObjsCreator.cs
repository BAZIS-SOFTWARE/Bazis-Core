using Model.Interfaces;
using ModelController.Interfaces;

namespace ModelController.GlObjsVertexCreators
{
    public class NormalsPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject)
        {
            vertexAtributes[stride + 0] = 0;
            vertexAtributes[stride + 1] = 0;
            vertexAtributes[stride + 2] = 0;

            stride += 3;
        }
    }
}