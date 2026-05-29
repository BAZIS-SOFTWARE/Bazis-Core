using Geometry;
using Model.Interfaces;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// CreateElementsGroup
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="setName"></param>
        /// <returns></returns>
        string CreateElementsGroup(ObjType objType, string setName);

        /// <summary>
        /// BoundaryNodesGroup
        /// </summary>
        /// <param name="setName"></param>
        string BoundaryNodesGroup(string setName);
    }
}
