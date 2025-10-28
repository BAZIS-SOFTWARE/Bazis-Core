using System;
using System.IO;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void ChangeMeshSetOrder(int elemDim, string name, int order)
        {
            if (elemDim > 3 | elemDim < 1)
                throw new Exception("Неправильная размерность элементов");

            if (elemDim == 1)
                foreach (var item in ModelData.ObjectData.E1DCollection[name].Values)
                    item.Level = order;

            else if (elemDim == 2)
                foreach (var item in ModelData.ObjectData.E2DCollection[name].Values)
                    item.Level = order;
            else
                foreach (var item in ModelData.ObjectData.E3DCollection[name].Values)
                    item.Level = order;
        }
    }
}
