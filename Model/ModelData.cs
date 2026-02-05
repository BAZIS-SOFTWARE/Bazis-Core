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
using Model.IO.STL;
using System;
using Model.Interfaces.Attributes;

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

        public VolumeData VolumeData { get; } = new VolumeData();
        /// <inheritdoc/>

        public void Append(IModelData modelData)
        {
            ObjectData.NodesSet.Append(modelData.ObjectData.NodesSet);

            ObjectData.E1DCollection.Append(modelData.ObjectData.E1DCollection);
            ObjectData.E2DCollection.Append(modelData.ObjectData.E2DCollection);
            ObjectData.E3DCollection.Append(modelData.ObjectData.E3DCollection);

            ObjectData.PointsSet.Append(modelData.ObjectData.PointsSet);

            ObjectData.CurveSet.Append(modelData.ObjectData.CurveSet);
            ObjectData.SurfaceSet.Append(modelData.ObjectData.SurfaceSet);

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
        [Warning("По возможности не использовать. Использовать метод load,Import контроллера")]
        public void Load(string path)
        {
            //var ext = System.IO.Path.GetExtension(path);

            //IModelLoader loader = new LoadModelFromBPF2TextFile();

            //if (ext == ".bpf")
            //    loader = new LoadModelFromBPFTextFile();
            //else if (ext == ".stl" | ext == ".STL")
            //    loader = new LoadFromSTLFile();
            //else if (ext == ".inp")
            //    loader = new LoadModelFromINPTextFile_v2();
            //else if (ext == ".asc" | ext == ".ASC")
            //    loader = new LoadModelFromASCIITextFile_v2();
            //else if (ext == ".cdb")
            //    loader = new LoadModelFromCDBTextFile();
            //else if (ext == ".dat")
            //    loader = new LoadModelFromSalomeFile();
            //else
            //    throw new System.Exception("Неизвестный формат загрузки модели");

            var modelData = Loader.Load(path);

            ObjectData = modelData.ObjectData;
            GroupData = modelData.GroupData;
        }


        /// <inheritdoc/>

        public void Save(string path)
        {
            var ext = System.IO.Path.GetExtension(path);

            IModelSaver saver = new SaveModelToBPF2TextFile();

            if (ext == ".bpf")
                saver = new SaveModelToBPFTextFile();
            else if (ext == ".stl" | ext == ".STL")
                saver = new SaveToTxtSTLFile();
            else if (ext == ".asc" | ext == ".ASC")
                saver = new SaveModelToASCIITextFile();
            else
                throw new System.Exception("Неизвестный формат сохранения модели");

            saver.Save(this, path);
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
