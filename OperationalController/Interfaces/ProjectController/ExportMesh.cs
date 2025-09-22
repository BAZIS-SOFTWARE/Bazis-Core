using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IController
    {
        /// <summary>
        /// ExportMesh
        /// </summary>
        /// <param name="v"></param>
        void ExportMesh(string v);
    }
}
