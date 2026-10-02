using Model.ObjectsCollections;
using System;
using System.IO;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void ChangeMeshSetName(int elemDim, string oldName, string newName)
        {
            if (elemDim > 3 | elemDim < 1)
                throw new Exception("Неправильная размерность элементов");

            var objType = elemDim == 1 ? Model.Interfaces.ObjType.Элемент1D : elemDim == 2 ? Model.Interfaces.ObjType.Элемент2D : Model.Interfaces.ObjType.Элемент3D;
            var setInfo = ModelData.ObjectData.GetSetInfo(objType, oldName);
            if (setInfo == null || oldName == newName)
                return;

            if (elemDim == 1)
                ModelData.ObjectData.E1DCollection.ChangeName(oldName, newName);
            else if (elemDim == 2)
                ModelData.ObjectData.E2DCollection.ChangeName(oldName, newName);
            else
                ModelData.ObjectData.E3DCollection.ChangeName(oldName, newName);

            PublishSetChange(setInfo, ChangeAction.Updated);
        }
    }
}
