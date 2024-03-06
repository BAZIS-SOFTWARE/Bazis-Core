using ModelInterfaces;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces.Utilities
{
    /// <summary>
    /// Substract one mesh from another and get result
    /// </summary>
    public interface IMesh2DRefiner
    {
        /// <summary>
        /// Refine
        /// </summary>
        /// <param name="elements"></param>
        /// <param name="elementSize"></param>
        void Refine(IEnumerable<IElement2D> elements, float elementSize);
    }
}
