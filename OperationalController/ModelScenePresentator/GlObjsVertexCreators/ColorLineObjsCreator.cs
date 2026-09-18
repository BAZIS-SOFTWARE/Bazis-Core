using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class ColorLineObjsCreator<T> : ILineObjsVertexCreator<T>
        where T : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject, IModelView modelView)
        {
            var color = modelView.GetColor(nodeObject.ObjType, nodeObject.Number);

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
