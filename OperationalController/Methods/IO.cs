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

namespace OperationalController
{
    public partial class Controller
    {
        public event Action<string> MessageEvent;

        /// <inheritdoc/>

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
    }
}
