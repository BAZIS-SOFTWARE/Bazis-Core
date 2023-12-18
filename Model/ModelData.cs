using Functions.Sort;
using Model.GroupsData;
using Model.IO;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public class ModelData  : IModelData
    {
        public IObjectsData ObjectData { get; private set; } = new ObjectsData();
        public IGroupData GroupData { get; private set; } = new GroupData();
        /// <inheritdoc/>
        public IModelSaver Saver { get; set; } = new SaveModelToProjectTextFile();
        /// <inheritdoc/>
        public IModelLoader Loader { get; set; } = new LoadModelFromProjectTextFile();
/// <inheritdoc/>

        public void Clear()
        {
            ObjectData.Clear();
            GroupData.Clear();
        }      
/// <inheritdoc/>

        public void Load(string path)
        {
            var modelData = Loader.Load(path);

            ObjectData = modelData.ObjectData;
            GroupData = modelData.GroupData;

            foreach (var item in ObjectData)
                MergeSort<IModelObject>.MergeSortFunction(item.Value);
        }
/// <inheritdoc/>

        public bool Remove(ObjType objType)
        {
            if (ObjectData.ContainsKey(objType))
            {
                foreach (var obj in ObjectData[objType])
                    obj.ExistState = false;

                ObjectData.ClearNotExisted();

                var existGroups = GroupData.Where(x => x.ObjType != objType).ToList();

                GroupData.Clear();

                foreach (var existGroup in existGroups)
                    GroupData.Add(existGroup);
                return true;
            }
            else return false;
        }

        /// <inheritdoc/>

        public void Save(string path)
        {
            Saver.Save(this, path);
        }
/// <inheritdoc/>

        public void SetGroupData(IGroupData groupData)
        {
            GroupData = groupData;
        }

        /// <inheritdoc/>

        public void SetObjectData(IObjectsData objectData)
        {
            ObjectData = objectData;
        }
    }
}
