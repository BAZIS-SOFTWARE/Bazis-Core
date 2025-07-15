using Model.Interfaces.ObjectsCollections;
using Model.Interfaces;
using OperationalController;
using OperationalController.Extensions;
using OperationalController.MeshObjsCreator;
using OperationalController.MeshObjsUtility;
using OperationalController.ObjsCreator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Interfaces.MeshObjects;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>

        public IEnumerable<ISetInfo> GetModelSetsInfo(string objects)
        {
            var objType = objects.ToEnum<ObjType>();
            foreach (var item in ModelData.ObjectData.GetSetsInfo(objType))
                yield return item;

        }
        /// <inheritdoc/>
        public IEnumerable<IModelObject> GetModelObjects(string objects)
        {
            var objType = objects.ToEnum<ObjType>();
            return ModelData.ObjectData.GetObjects(objType);
        }
        /// <inheritdoc/>

        public ISetInfo GetModelSetInfo(string objects, string setName)
        {
            var objType = objects.ToEnum<ObjType>();
            return ModelData.ObjectData.GetSetInfo(objType, setName);
        }

        /// <inheritdoc/>

        public IEnumerable<IModelObject> GetAllModelObjects()
        {
            return ModelData.ObjectData.GetAllObjects();
        }

        /// <inheritdoc/>

        public IEnumerable<IElement> GetModelElements()
        {
            return ModelData.ObjectData.GetAllElements();
        }
        /// <inheritdoc/>
        public IGroup GetModelGroup(int grIndex)
        {
            return ModelData.GroupData[grIndex];
        }
    }
}
