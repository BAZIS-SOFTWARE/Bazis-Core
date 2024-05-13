using ModelInterfaces.MeshObjects;
using System.Collections.Generic;

namespace ModelControllerInterfaces
{
    public interface IVolumeMeshCreator
    {
        /// <summary>
        /// Sew3Dfrom2D
        /// </summary>
        /// <param name="firstSet"></param>
        /// <param name="secSet"></param>
        /// <returns></returns>
        List<IElement3D> Sew3Dfrom2D(List<IElement2D> firstSet, List<IElement2D> secSet);
    }
}