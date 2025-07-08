using ModelControllerInterfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectController.MeshObjsCreator
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
