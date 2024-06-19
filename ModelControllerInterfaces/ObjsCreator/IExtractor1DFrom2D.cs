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
    /// <summary>
    /// IExtractor1DFrom2D
    /// </summary>
    public interface IExtractor1DFrom2D
    {
        /// <summary>
        /// Create
        /// </summary>
        /// <param name="startNumber"></param>
        /// /// <param name="elements"></param>
        /// <returns></returns>
        IEnumerable<Beam> Create(int startNumber, IElement2D[] elements);
    }
}
