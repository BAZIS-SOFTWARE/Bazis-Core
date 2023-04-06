using Model.Interfaces;

namespace ModelController.Interfaces
{
    public interface ISurfaceObjsVertexCreator
    {
        void Create(ref int stride, float[] vertexAtributes, ISurfaceObject surfaceElement);
    }
}