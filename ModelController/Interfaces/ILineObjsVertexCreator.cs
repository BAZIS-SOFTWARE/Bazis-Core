using Model.Interfaces;

namespace ModelController.Interfaces
{
    public interface ILineObjsVertexCreator
    {
        void Create(ref int stride, float[] vertexAtributes, INodeObject nodeObject);
    }
}