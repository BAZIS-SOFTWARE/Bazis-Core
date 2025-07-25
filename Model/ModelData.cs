using Model.GroupsData;
using Model.IO;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.ObjectsCollections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Model.Interfaces.ObjectsCollections;
using Model.GeometryObjects;

namespace Model
{
    public class ModelData  : IModelData
    {
        public IObjectsData ObjectData { get; private set; } = new ObjectsData();
        public IGroupData GroupData { get; private set; } = new GroupData();
        /// <inheritdoc/>
        public IModelSaver Saver { get; set; } = new SaveModelToBPFTextFile();
        /// <inheritdoc/>
        public IModelLoader Loader { get; set; } = new LoadModelFromBPFTextFile();

        /// <inheritdoc/>

        public void Append(string path)
        {
            var modelData = Loader.Load(path);

            ObjectData.NodesSet.Append(modelData.ObjectData.NodesSet);

            ObjectData.E1DCollection.Append(modelData.ObjectData.E1DCollection);
            ObjectData.E2DCollection.Append(modelData.ObjectData.E2DCollection);
            ObjectData.E3DCollection.Append(modelData.ObjectData.E3DCollection);

            ObjectData.PointsSet.Append(modelData.ObjectData.PointsSet);

            ObjectData.CurveSet.Append(modelData.ObjectData.CurveSet);
            ObjectData.SurfaceCollection.Append(modelData.ObjectData.SurfaceCollection);

            foreach (var item in modelData.GroupData)
                GroupData.Add(item);
        }



        /// <inheritdoc/>

        public void ClearAll()
        {
            ObjectData.ClearAll();
            GroupData.Clear();
        }

        /// <inheritdoc/>

        public void Load(string path)
        {
            var modelData = Loader.Load(path);

            ObjectData = modelData.ObjectData;
            GroupData = modelData.GroupData;
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
