using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class NormalsPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject, IModelView modelView, System.Drawing.Color? setColor)
        {
            vertexAtributes[stride + 0] = 0;
            vertexAtributes[stride + 1] = 0;
            vertexAtributes[stride + 2] = 0;

            stride += 3;
        }
    }
}
