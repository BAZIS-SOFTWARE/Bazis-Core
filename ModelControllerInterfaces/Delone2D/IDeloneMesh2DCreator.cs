using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces.Delone2D
{
    /// <summary>
    /// IDeloneMesh2DCreator
    /// </summary>
    public interface IDeloneMesh2DCreator
    {
        /// <summary>
        /// GetElements
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElement2D> GetElements();
        /// <summary>
        /// MakeTriangulation
        /// </summary>
        /// <param name="nodes"></param>
        void MakeTriangulation(List<INode> nodes);
    }
}
