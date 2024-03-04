using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IBoundaryEdgesFinder
    /// </summary>
    public interface IBoundaryEdgesFinder
    {
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="modelData"></param>
        /// <returns></returns>
        List<string> Find(IModelData modelData);
    }
}
