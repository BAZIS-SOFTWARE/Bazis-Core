using ModelControllerInterfaces.ObjsCreator;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsCreator
{
    public class Extract1DFrom2D : IExtractor1DFrom2D
    {
        /// <inheritdoc/>
        public IEnumerable<IElement2D> Create(int StartNumber, IElement2D[] elements)
        {
            throw new NotImplementedException();
        }
/// <inheritdoc/>

        public IEnumerable<IElementSurface> FindOpenSurfaces(IElement2D[] elements)
        {
            throw new NotImplementedException();
        }
    }
}
