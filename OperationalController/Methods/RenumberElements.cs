using Model;
using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using Model.IO;
using Model.IO.STL;
using Model.MeshObjects;
using Model.ObjectsCollections;
using OperationalController.GmshController;
using Project;
using System;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace OperationalController
{
    public partial class ProjectController
    {
        //public string Message { get; private set; }
        /// <inheritdoc/>

        public void RenumberElements()
        {
            var number = 0;
            foreach (var item in ModelData.ObjectData.E1DCollection)
                item.Value.Renumber(ref number);

            foreach (var item in ModelData.ObjectData.E2DCollection)
                item.Value.Renumber(ref number);

            foreach (var item in ModelData.ObjectData.E3DCollection)
                item.Value.Renumber(ref number);

            foreach (var objType in new[] { ObjType.Элемент1D, ObjType.Элемент2D, ObjType.Элемент3D })
                foreach (var setInfo in ModelData.ObjectData.GetSetsInfo(objType))
                    PublishSetChange(setInfo, ChangeAction.Updated);

        }
    }
}
