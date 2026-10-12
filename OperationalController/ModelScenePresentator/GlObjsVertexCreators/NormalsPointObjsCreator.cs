using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class NormalsPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject, System.Drawing.Color color)
        {
            vertexAtributes[stride + 0] = 0;
            vertexAtributes[stride + 1] = 0;
            vertexAtributes[stride + 2] = 0;

            stride += 3;
        }
    }
}
