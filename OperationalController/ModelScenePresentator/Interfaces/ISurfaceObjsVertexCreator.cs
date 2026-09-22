using Model.Interfaces;
using System.Collections.Generic;

namespace OperationalController.ModelScenePresentator.Interfaces
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
        /// <param name="surfaces">Отобранные к отрисовке грани одного объекта</param>
        /// <param name="color"></param>
        void Create(ref int stride, float[] vertexAtributes, IEnumerable<T> surfaces, System.Drawing.Color color);
    }
}
