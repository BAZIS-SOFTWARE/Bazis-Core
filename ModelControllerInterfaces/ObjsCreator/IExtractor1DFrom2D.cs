using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces.ObjsCreator
{
    public interface IExtractor1DFrom2D
    {
        /// <summary>
        /// FindOpenSurfaces
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElementSurface> FindOpenSurfaces(IElement2D[] elements);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="StartNumber"></param>
        /// <returns></returns>
        IEnumerable<IElement2D> Create(int StartNumber, IElement2D[] elements);
    }
}
