using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// ExportMesh
        /// </summary>
        /// <param name="v"></param>
        void ExportMesh(string v);
    }
}
