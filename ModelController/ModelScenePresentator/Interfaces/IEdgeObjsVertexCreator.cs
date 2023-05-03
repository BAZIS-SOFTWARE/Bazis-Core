using Model.Interfaces;

namespace ModelController.ModelScenePresentator.Interfaces
{
    public interface IEdgeObjsVertexCreator
    {
        void Create(ref int stride, bool[] vertexAtributes, ISurfaceObject surfaceElement);
    }
}
