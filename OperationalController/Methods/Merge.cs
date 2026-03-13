using Model;
using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
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

        public void MergeElements(ObjType objType, string setMaster, string setSlave)
        {
            if (objType == ObjType.Элемент1D)
            {
                var slave = ModelData.ObjectData.E1DCollection[setSlave];
                ModelData.ObjectData.E1DCollection[setMaster].Append(slave);
                ModelData.ObjectData.E1DCollection.Remove(setSlave);
            }
            else if (objType == ObjType.Элемент2D)
            {
                var slave = ModelData.ObjectData.E2DCollection[setSlave];
                ModelData.ObjectData.E2DCollection[setMaster].Append(slave);
                ModelData.ObjectData.E2DCollection.Remove(setSlave);
            }
            else if (objType == ObjType.Элемент3D)
            {
                var slave = ModelData.ObjectData.E3DCollection[setSlave];
                ModelData.ObjectData.E3DCollection[setMaster].Append(slave);
                ModelData.ObjectData.E3DCollection.Remove(setSlave);
            }
            else
                throw new Exception($"Операция не поддерживается для объектов {objType}");           
        }

    }
}
