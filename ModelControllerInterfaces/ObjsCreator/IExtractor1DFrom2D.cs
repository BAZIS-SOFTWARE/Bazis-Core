using Model.MeshObjects;
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
        /// Create
        /// </summary>
        /// <param name="StartNumber"></param>
        /// <returns></returns>
        IEnumerable<Beam> Create(int startNumber, IElement2D[] elements);
    }
}
