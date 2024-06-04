using ModelInterfaces;
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
        IEnumerable<(INode, INode)> FindOpenSegments(IElement2D[] elements);
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="StartNumber"></param>
        /// <returns></returns>
        IEnumerable<ILineObject<INode>> Create(int StartNumber, IElement2D[] elements);
    }
}
