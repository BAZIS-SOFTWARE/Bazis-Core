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
using System.IO;
using Model.IO.STL;

namespace OperationalController
{
    public partial class Controller
    {
        public void ExportMesh(string v)
        {
            if (project == null)
                throw new Exception("Проект не создан");

            var ext = System.IO.Path.GetExtension(v);

            IModelSaver saver;

            if (ext == ".stl" | ext == ".STL")
                saver = new SaveToTxtSTLFile();
            else if (ext == ".asc" | ext == ".ASC")
                saver = new SaveModelToASCIITextFile();
            else
                throw new Exception("Неизвестный формат сетки");

            saver.Save(project.ModelData, v);
        }
    }
}
