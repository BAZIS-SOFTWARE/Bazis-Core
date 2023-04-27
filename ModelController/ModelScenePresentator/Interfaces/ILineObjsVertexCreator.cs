using Model.Interfaces;

namespace ModelController.ModelScenePresentator.Interfaces
{
    public interface ILineObjsVertexCreator
    {
        void Create(ref int stride, float[] vertexAtributes, INodeObject nodeObject);
    }
}