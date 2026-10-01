using Model;
using Model.GroupsData;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void ClearGroupData()
        {
            var before = CaptureData();
            ModelData.GroupData?.Clear();
            TaskData.Clear();
            PublishDataDifference(before);
        }
        /// <summary>
        /// ClearNotExistedTaskData
        /// </summary>
        /// <param name="group"></param>
        public void DeleteCond(IGroup group)
        {
            var before = CaptureData();
            TaskData.ClearNotExisted(group);
            PublishDataDifference(before);
        }
        /// <inheritdoc/>
        public void ClearNotExistedCondData()
        {
            var before = CaptureData();
            TaskData.ClearNotExisted(ModelData.GroupData);
            PublishDataDifference(before);
        }
        /// <inheritdoc/>
        public void ClearNotExistedModelData()
        {
            var before = CaptureData();
            ModelData.ObjectData?.ClearNotExisted();
            ModelData.ObjectData?.ClearEmptySet();
            ModelData.GroupData?.ClearNotExisted();
            TaskData.ClearNotExisted(ModelData.GroupData);
            PublishDataDifference(before);
            ModelView.Prune();

            //project.ClearEmptySet();
            //project.ClearNotExistedGroupData();
            //project.ClearNotExistedCondData();

        }
        /// <inheritdoc/>
        public void ClearNotExistedGroupData()
        {
            var before = CaptureData();
            ModelData.GroupData?.ClearNotExisted();
            TaskData.ClearNotExisted(ModelData.GroupData);
            PublishDataDifference(before);
        }
/// <inheritdoc/>

        public void ClearEmptySet()
        {
            var before = CaptureData();
            ModelData.ObjectData?.ClearEmptySet();
            PublishDataDifference(before);
        }
        /// <inheritdoc/>

        public void ClearTaskData()
        {
            var before = CaptureData();
            TaskData.Clear();
            PublishDataDifference(before);
        }
        /// <inheritdoc/>

        public void ClearAllData()
        {
            var before = CaptureData();
            TaskData.Clear();
            ModelData.ClearAll();
            PublishDataDifference(before);
            ModelView.Prune();
        }

        /// <inheritdoc/>

        public void ClearModelCollection(ObjType objType)
        {
            var before = CaptureData();
            ModelData.ObjectData.Clear(objType);
            ModelData.GroupData.ClearNotExisted();

            TaskData.ClearNotExisted(ModelData.GroupData);
            PublishDataDifference(before);
            ModelView.Prune();
        }
        /// <inheritdoc/>

        public void DeleteModelGroup(string grName)
        {
            var before = CaptureData();
            var gr = ModelData.GroupData.Find(grName);
            if (gr != null)
            {
                ModelData.GroupData.Remove(gr);
                TaskData.ClearNotExisted(gr);
            }
            PublishDataDifference(before);
        }

        /// <inheritdoc/>
        public bool DeleteCond(int index)
        {
            var data = TaskData[index];
            var removed = TaskData.Remove(data);
            if (removed)
                PublishConditionChange(data.Kind, ChangeAction.Deleted, index);
            return removed;
        }
        /// <inheritdoc/>

        public void DeleteModelSet(ObjType objType, string name)
        {
            var before = CaptureData();
            ModelData.ObjectData.Remove(objType, name);
            ModelData.GroupData.ClearNotExisted();

            TaskData.ClearNotExisted(ModelData.GroupData);
            PublishDataDifference(before);
            ModelView.Prune();
            ModelView.Prune();
        }
        /// <inheritdoc/>

        public void DeleteModelObjects(ObjType objType, IEnumerable<int> objNumbs)
        {
            // TODO если геометрия, то удалить по типу и номеру объекта

            foreach (var numb in objNumbs)
            {
                var obj = ModelData.ObjectData.Find(objType, numb);
                if (obj != null)
                    obj.ExistState = false;
            }
 
            ClearNotExistedModelData();
        }
    }
}
