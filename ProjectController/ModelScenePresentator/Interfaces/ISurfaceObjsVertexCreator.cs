using Model.Interfaces;

namespace ProjectController.ModelScenePresentator.Interfaces
{
    /// <summary>
    /// ISurfaceObjsVertexCreator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public interface ISurfaceObjsVertexCreator<T,U> where T : ISurface<U> where U : IPoint
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="stride"></param>
        /// <param name="vertexAtributes"></param>
        /// <param name="surfaceElement"></param>
        void Create(ref int stride, float[] vertexAtributes, ISurfaceObject<T,U> surfaceElement);
    }
}