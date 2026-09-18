using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class ColorPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject, IModelView modelView)
        {
                var color = modelView.GetColor(pointObject.ObjType, pointObject.Number);

                vertexAtributes[stride + 0] = color.R / 255.0f;
                vertexAtributes[stride + 1] = color.G / 255.0f;
                vertexAtributes[stride + 2] = color.B / 255.0f;
                vertexAtributes[stride + 3] = color.A / 255.0f;

                stride += 4;
        }
    }
}
