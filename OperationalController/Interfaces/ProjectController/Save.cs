using GmshApi;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// SaveSMF
        /// </summary>
        /// <param name="path"></param>
        void SaveSMF(string path);
    }
}
