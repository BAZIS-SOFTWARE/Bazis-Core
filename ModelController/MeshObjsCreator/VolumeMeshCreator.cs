using ModelControllerInterfaces;
using ModelInterfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsCreator
{
    public class VolumeMeshCreator : IVolumeMeshCreator
    {
        /// <inheritdoc/>
        public List<IElement3D> Sew3Dfrom2D(List<IElement2D> firstSet, List<IElement2D> secSet)
        {
            throw new NotImplementedException();
        }
    }
}
