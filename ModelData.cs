using Functions.Sort;
using Geometry;
using Model.GroupsData;
using Model.Interfaces;
using ModelInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ModelData 
    {
        public event Action<object, LoaderEventArgs> LoadEvent;
        public event Action<object, SaverEventArgs> SaveEvent;

        //IModelLoader loader;
        //IModelSaver saver;

        public ObjectsData ObjectData { get; internal set; }
        public GroupData GroupData{ get; internal set; }

        public void SetLoader(IModelLoader loader)
        {
            this.loader = loader;
            this.loader.LoadEvent += LoadModel_EventHandler;
        }

        public void SetSaver(IModelSaver saver)
        {
            this.saver = saver;
            saver.SaveEvent += SaveModel_SaveEvent;
        }

        private void SaveModel_SaveEvent(object arg1, SaverEventArgs arg2)
        {
            SaveEvent(this, arg2);
        }

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

        private void LoadModel_EventHandler(object arg1, LoaderEventArgs arg2)
        {
            LoadEvent(this, arg2);
        }      


        public void Load(string path)
        {
            var newModel = loader.Load(path);
            ObjectData = newModel.ObjectData;
            GroupData = newModel.GroupData;

            var list = ObjectData.ToList();
            MergeSort<ModelObject>.MergeSortFunction(list);

            ObjectData.Clear();
            ObjectData.AddRange(list);
        }

        public void Save(string path)
        {
            saver.Save(this,path);
        }

        
    }
}
