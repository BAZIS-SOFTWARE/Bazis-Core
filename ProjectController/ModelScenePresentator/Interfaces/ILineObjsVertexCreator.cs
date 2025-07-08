using Model.Interfaces;

namespace ProjectController.ModelScenePresentator.Interfaces
{
    public interface ILineObjsVertexCreator<T> where T : IPoint
    {
        void Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject);
    }
}