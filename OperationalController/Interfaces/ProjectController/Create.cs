using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
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
