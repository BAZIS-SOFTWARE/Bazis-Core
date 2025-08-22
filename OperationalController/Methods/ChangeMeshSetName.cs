using Model.ObjectsCollections;
using System;
using System.IO;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>
        public void ChangeMeshSetName(int elemDim, string oldName, string newName)
        {
            if (elemDim > 3 | elemDim < 1)
                throw new Exception("Неправильная размерность элементов");

            if (elemDim == 1)
                ModelData.ObjectData.E1DCollection.ChangeName(oldName, newName);
            else if (elemDim == 2)
                ModelData.ObjectData.E2DCollection.ChangeName(oldName, newName);
            else
                ModelData.ObjectData.E3DCollection.ChangeName(oldName, newName);
        }
    }
}
