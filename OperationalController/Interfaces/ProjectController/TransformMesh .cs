using Geometry;
using Model.Interfaces;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// MoveMesh
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="vector"></param>
        void MoveMesh(ObjType objType, Point3D vector);

        /// <summary>
        /// RotateMesh
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="axis"></param>
        /// <param name="angle"></param>
        void RotateMesh(ObjType objType, Point3D axis, float angle);
    }
}
