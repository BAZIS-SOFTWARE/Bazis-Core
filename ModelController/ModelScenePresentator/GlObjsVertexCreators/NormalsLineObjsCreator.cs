using Model.Interfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class NormalsLineObjsCreator : ILineObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, INodeObject nodeObject)
        {
            if (nodeObject.ViewState)
            {
                for (int k = 0; k < nodeObject.NumberOfPoints; k++)
                {
                    vertexAtributes[stride + 3 * k + 0] = 0;
                    vertexAtributes[stride + 3 * k + 1] = 0;
                    vertexAtributes[stride + 3 * k + 2] = 0;
                }
                stride += nodeObject.NumberOfPoints * 3;
            }
        }
    }
}