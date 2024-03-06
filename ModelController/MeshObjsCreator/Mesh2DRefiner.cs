using ModelControllerInterfaces.Utilities;
using ModelInterfaces.MeshObjects;
using System.Collections.Generic;

namespace ModelController.MeshObjsCreator
{
    public class Mesh2DRefiner : IMesh2DRefiner
    {
        /// <inheritdoc/>
        public void Refine(IEnumerable<IElement2D> elements, float elementSize)
        {
            throw new System.NotImplementedException();
        }
    }
}