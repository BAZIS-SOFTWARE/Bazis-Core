using Functions.Sort;
using Model.GroupsData;
using Model.IO;
using ModelInterfaces;
using System.Linq;

namespace Model
{
    public class ModelData  : IModelData
    {
        public IObjectsData ObjectData { get; set; } = new ObjectsData();
        public IGroupData GroupData { get; set; } = new GroupData();
        /// <inheritdoc/>
        public IModelSaver Saver { get; set; } = new SaveModelToProjectTextFile();
        /// <inheritdoc/>
        public IModelLoader Loader { get; set; } = new LoadModelFromProjectTextFile();

        public void Clear()
        {
            ObjectData.Clear();
            GroupData.Clear();
        }      

        public void Load(IModelData modelData)
        {
            ObjectData = modelData.ObjectData;
            GroupData = modelData.GroupData;

            var list = ObjectData.ToList();
            MergeSort<IModelObject>.MergeSortFunction(list);

            ObjectData.Clear();
            ObjectData.AddRange(list);
        }

        public void SetObjectData(IObjectsData objectData)
        {
            ObjectData = objectData;
        }
    }
}
