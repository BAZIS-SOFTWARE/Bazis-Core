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
            OpenModel(filePath, null);
        }

    }
}
