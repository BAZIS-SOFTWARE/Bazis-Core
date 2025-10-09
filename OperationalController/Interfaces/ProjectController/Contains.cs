using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial interface IProjectController
    {
        /// <summary>
        /// ContainsModelObject
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="objNumb"></param>
        /// <returns></returns>
        bool ContainsModelObject(ObjType objType, int objNumb);


    }
}
