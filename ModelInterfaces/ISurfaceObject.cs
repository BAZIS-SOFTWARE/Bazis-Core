using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    public interface ISurfaceObject<T,U> : IModelObject where T : ISurface<U> where U : IPoint
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
