using Model.Interfaces;
using System;
using System.Collections.Generic;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void ClearNotExistedTaskData(IGroupData groupData)
        {
            TaskData?.ClearNotExisted(groupData);
        }
        /// <inheritdoc/>
        public void ClearNotExistedModelData()
        {
            ModelData?.ObjectData.ClearNotExisted();
        }
        /// <inheritdoc/>
        public void ClearNotExistedGroupData()
        {
            ModelData?.GroupData.ClearNotExisted();
        }
/// <inheritdoc/>

        public void ClearEmptySet()
        {
            ModelData?.ObjectData.ClearEmptySet();
        }
        /// <inheritdoc/>

        public void ClearTaskData()
        {
            TaskData?.Clear();
        }
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
        public bool DeleteCond(int index)
        {
            var data = TaskData[index];
            return TaskData.Remove(data);
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
            ModelData.ObjectData.ClearEmptySet();
            ModelData.GroupData.ClearNotExisted();
            TaskData.ClearNotExisted(ModelData.GroupData);
        }
    }
}
