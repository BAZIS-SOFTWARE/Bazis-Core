using System.Collections.Generic;

namespace Model.Interfaces
{
    /// <summary>
    /// ISurfaceObject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    public interface ISurfaceObject<T,U> : IModelObject, IVertexObject<U> where T : ISurface<U> where U : IPoint
    {
        /// <summary>
        /// NumberOfSides
        /// </summary>
        int NumberOfSides { get; }
        /// <summary>
        /// GetSurface
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> GetSurfaces();
    }
}
