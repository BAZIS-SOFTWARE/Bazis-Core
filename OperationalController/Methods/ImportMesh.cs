using Model;
using Model.Interfaces;
using Model.IO;
using Model.IO.STL;
using OperationalController.GmshController;
using Project;
using System;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace OperationalController
{
    public partial class ProjectController
    {
        //public string Message { get; private set; }
        /// <inheritdoc/>

        public void ImportMesh(string filePath)
        {
            var path = System.IO.Path.GetDirectoryName(filePath);
            var name = "новый_проект.bpf2";
            project = new ProjectData($"{path}//{name}");


            var ext = System.IO.Path.GetExtension(filePath);

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
            var modelData = loader.Load(filePath);

            project.ModelData.SetObjectData(modelData.ObjectData);
            project.ModelData.SetGroupData(modelData.GroupData);

            MarkModelSurfaces();
        }

    }
}
