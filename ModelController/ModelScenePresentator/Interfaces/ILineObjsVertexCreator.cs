using ModelInterfaces;

namespace ModelController.ModelScenePresentator.Interfaces
{
    public interface ILineObjsVertexCreator<T> where T : IPoint
    {
        void Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject);
    }
}