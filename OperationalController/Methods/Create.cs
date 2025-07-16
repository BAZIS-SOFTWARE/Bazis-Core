using Model.IO;
using Model.IO.STL;
using Project;
using System;
using System.IO;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>

        public void Create(string projectFile)
        {
            project = new ProjectData();

            var ext = System.IO.Path.GetExtension(projectFile);

            if (ext == ".bpf2")
            {
                ModelData.Saver = new SaveModelToBPF2TextFile();
                ModelData.Loader = new LoadModelFromBPF2TextFile();
            }

            else if (ext == ".bpf")
            {
                ModelData.Saver = new SaveModelToBPFTextFile();
                ModelData.Loader = new LoadModelFromBPFTextFile();
            }

            else if (ext == ".inp")
                ModelData.Loader = new LoadModelFromINPTextFile();
            else if (ext == ".inp_v2")
                ModelData.Loader = new LoadModelFromINPTextFile_v2();
            else if (ext == ".ASC")
                ModelData.Loader = new LoadModelFromASCIITextFile_v2();
            else if (ext == ".dat")
                ModelData.Loader = new LoadModelFromSalomeFile();
            else if (ext == ".STL" | ext == ".stl")
                ModelData.Loader = new LoadFromSTLFile();
            else if(ext == ".cdb")
                ModelData.Loader = new LoadModelFromCDBTextFile();

            else
                throw new Exception("неправильно задано расширение файла");


            GeneralData.Name = System.IO.Path.GetFileName(projectFile);
            GeneralData.Path = System.IO.Path.GetDirectoryName(projectFile);
        }
    }
}
