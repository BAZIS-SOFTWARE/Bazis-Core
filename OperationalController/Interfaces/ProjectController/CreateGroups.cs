using Geometry;
using Model.Interfaces;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// CreateGroupByMeshObjs
        /// </summary>
        /// <param name="dim"></param>
        /// <param name="setName"></param>
        /// <returns></returns>
        string CreateGroupByMeshObjs(int dim, string setName);

        /// <summary>
        /// CreateGroupByGeoObjs
        /// </summary>
        /// <param name="meshDim"></param>
        /// <param name="geoDim"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        string CreateGroupByGeoObjs(int meshDim, int geoDim, int tag);
        /// <summary>
        /// CreateSurfaceNodesGroup
        /// </summary>
        /// <param name="setName"></param>
        /// <returns></returns>
        string CreateOpenSurfaceNodesGroup(string setName);
    }
}
