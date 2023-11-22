using ModelInterfaces;

namespace ModelController.ModelScenePresentator.Interfaces
{
    public interface ILineObjsVertexCreator
    {
        void Create(ref int stride, float[] vertexAtributes, INodeObject nodeObject);
    }
}