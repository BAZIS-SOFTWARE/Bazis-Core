using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// Import
        /// </summary>
        /// <param name="projectFile"></param>
        /// <param name="gmshController"></param>
        void ImportCAD(string projectFile, IGmshController gmshController);
    }
}
