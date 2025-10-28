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
        /// LoadSMF
        /// </summary>
        /// <param name="path"></param>
        void LoadSMF(string path);



    }
}
