using Model.Interfaces;

namespace OperationalController.ModelScenePresentator.Interfaces
{
    public interface ILineObjsVertexCreator<T> where T : IPoint
    {
        void Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject, IModelView modelView);
    }
}
