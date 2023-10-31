using Functions.Sort;
using Model.GroupsData;
using Model.Interfaces;
using System.Linq;

namespace Model
{
    public class ModelData 
    {
        public ObjectsData ObjectData { get; internal set; }
        public GroupData GroupData{ get; internal set; }

        public ModelData()
        {
            ObjectData = new ObjectsData();
            GroupData = new GroupData();
        }

        public void Clear()
        {
            ObjectData.Clear();
            GroupData.Clear();
        }      

        public void Load(ModelData modelData)
        {
            ObjectData = modelData.ObjectData;
            GroupData = modelData.GroupData;

            var list = ObjectData.ToList();
            MergeSort<IModelObject>.MergeSortFunction(list);

            ObjectData.Clear();
            ObjectData.AddRange(list);
        }

        public void SetObjectData(ObjectsData objectData)
        {
            ObjectData = objectData;
        }
    }
}
