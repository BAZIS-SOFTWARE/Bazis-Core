using Model.Interfaces;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// Merge
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="setMaster"></param>
        /// <param name="setSlave"></param>
        void MergeElements(ObjType objType, string setMaster, string setSlave);
    }
}
