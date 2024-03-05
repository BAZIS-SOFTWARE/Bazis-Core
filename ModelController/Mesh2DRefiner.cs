using ModelControllerInterfaces.Utilities;
using ModelInterfaces.MeshObjects;
using System.Collections.Generic;

namespace ModelController
{
    public class Mesh2DRefiner : IMesh2DRefiner
    {
        /// <inheritdoc/>
        public void Refine(IEnumerable<IElement2D> elements)
        {
            throw new System.NotImplementedException();
        }
    }
}