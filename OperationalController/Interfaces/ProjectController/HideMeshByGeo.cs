using Geometry;
using Model.Interfaces;
using OperationalController.GmshController;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// HideMesh
        /// </summary>
        /// <param name="elemDim"></param>
        void HideMesh(int elemDim);
    }
}
