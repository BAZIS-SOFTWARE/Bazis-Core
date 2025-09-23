using Model.Interfaces.ObjectsCollections;
using Model.Interfaces;
using System.Collections.Generic;
using Model.Interfaces.MeshObjects;
using Project.Interfaces.Tasks;
using Model.GeometryObjects;
using PropertiesCalculator.MaterialData;
using PropertiesCalculator.FunctionData;
using Model.ObjectsCollections;
using Model.MeshObjects;
using OperationalController.GmshController;
using System;
using System.Data;
using Model.IO;
using Project.IO;
using Project.Interfaces.IO;
using Model.IO.STL;

namespace OperationalController
{
    public partial class Controller
    {
        public event Action<string> MessageEvent;
      
        public void UnsubMessasge()
        {
            MessageEvent = null;
        }

        public void Load(string path)
        {
            var loader = new LoadProjectFromTextFormat();
            loader.LoadEvent += (ar1, ar2) => { MessageEvent?.Invoke(ar2.Message); };
            project = loader.Load(path);
        }
       
        /// <inheritdoc/>

        public void Save(string path)
        {
            if (project == null)
                throw new Exception("Проект не создан");

            var saver = new SaveProjectTextFormat();
            saver.Save(project, path);
        }
        /// <inheritdoc/>

        public void Append(string path)
        {
            if (project == null)
                throw new Exception("Проект не создан");

            var ext = System.IO.Path.GetExtension(path);

            IModelLoader loader;
            if (ext == ".inp")
                loader = new LoadModelFromINPTextFile();
            else if (ext == ".inp_v2")
                loader = new LoadModelFromINPTextFile_v2();
            else if (ext == ".ASC" | ext == ".asc")
                loader = new LoadModelFromASCIITextFile_v2();
            else if (ext == ".dat")
                loader = new LoadModelFromSalomeFile();
            else if (ext == ".STL" | ext == ".stl")
                loader = new LoadFromSTLFile();
            else if (ext == ".cdb")
                loader = new LoadModelFromCDBTextFile();

            else
                throw new Exception("неправильно задано расширение файла");

            loader.LoadEvent += (ar1, ar2) =>
            {
                MessageEvent?.Invoke(ar2.Message);
            };
            var modelData = loader.Load(path);

            project.ModelData.Append(modelData);
        }
    }
}
