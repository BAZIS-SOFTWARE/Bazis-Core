using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// ConnectByBeams
        /// </summary>
        /// <param name="masterGroup"></param>
        /// <param name="slaveGroup"></param>
        /// <param name="radius"></param>
        /// <param name="maxBeams"></param>
        void ConnectByBeams(string masterGroup, string slaveGroup, double radius, int maxBeams);
    }
}
