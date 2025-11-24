using Geometry;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// Extrude3DForm2D
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="layers"></param>
        /// <param name="elements"></param>
        void Extrude3DForm2D(Point3D vector, int layers, IEnumerable<IElement2D> elements);
        /// <summary>
        /// CreateElementsSet
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="dim"></param>
        void CreateElementsSet(string setName, int dim);

        /// <summary>
        /// CreateElement
        /// </summary>
        /// <param name="setName"></param>
        /// <param name="dim"></param>
        /// <param name="nodeNumbers"></param>
        void CreateElement(string setName, int dim, int[] nodeNumbers);
        /// <summary>
        /// CreateNode
        /// </summary>
        /// <param name="number"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        void CreateNode(int number, float x, float y, float z);
    }
}
