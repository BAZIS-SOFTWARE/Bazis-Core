using ModelInterfaces;

namespace ModelController.ModelScenePresentator.Interfaces
{
    public interface ISurfaceObjsVertexCreator<T,U> where T : ISurface<U> where U : IPoint
    {
        void Create(ref int stride, float[] vertexAtributes, ISurfaceObject<T,U> surfaceElement);
    }
}