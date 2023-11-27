using Functions.Sort;
using Model.GroupsData;
using Model.IO;
using ModelInterfaces;
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

            var list = ObjectData.ToList();
            MergeSort<IModelObject>.MergeSortFunction(list);

            ObjectData.Clear();
            ObjectData.AddRange(list);
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
