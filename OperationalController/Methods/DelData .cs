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

        public void ClearAllData()
        {
            TaskData?.Clear();
            ModelData?.ClearAll();
        }

        /// <inheritdoc/>

        public void ClearModelCollection(ObjType objType)
        {
            ModelData.ObjectData.Clear(objType);
            ModelData.GroupData.ClearNotExisted();

            TaskData.ClearNotExisted(ModelData.GroupData);
        }
        /// <inheritdoc/>

        public void DeleteModelGroup(string grName)
        {
            var gr = ModelData.GroupData.Find(grName);
            if (gr != null)
            {
                ModelData.GroupData.Remove(gr);
                TaskData.ClearNotExisted(gr);
            }

        }
        /// <inheritdoc/>

        public void DeleteModelSet(ObjType objType, string name)
        {
            ModelData.ObjectData.Remove(objType, name);
            ModelData.GroupData.ClearNotExisted();

            TaskData.ClearNotExisted(ModelData.GroupData);
        }
        /// <inheritdoc/>

        public void DeleteModelObjects(ObjType objType, IEnumerable<int> objNumbs)
        {

            foreach (var numb in objNumbs)
            {
                var obj = ModelData.ObjectData.Find(objType, numb);
                if (obj != null)
                    obj.ExistState = false;
            }

            ModelData.ObjectData.ClearNotExisted();
            ModelData.GroupData.ClearNotExisted();
            TaskData.ClearNotExisted(ModelData.GroupData);
        }
    }
}
