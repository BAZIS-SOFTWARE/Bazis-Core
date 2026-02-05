using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Project.Interfaces.Tasks;
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
        /// <summary>
        /// ContainsConditions
        /// </summary>
        /// <param name="grName"></param>
        /// <returns></returns>
        bool ContainsConditions(string grName);
    }
}
