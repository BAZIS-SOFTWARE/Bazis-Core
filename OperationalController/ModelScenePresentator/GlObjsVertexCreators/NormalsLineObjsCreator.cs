using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class NormalsLineObjsCreator<T> : ILineObjsVertexCreator<T>
        where T : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject, IModelView modelView, System.Drawing.Color? setColor)
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
