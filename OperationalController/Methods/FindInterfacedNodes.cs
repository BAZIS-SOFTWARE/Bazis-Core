using OperationalController;
using OperationalController.MeshObjsUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace OperationalController
{
    public partial class Controller
    {
        IInterfacedNodesFinder interfacedNodesFinder { get; } = new InterfacedNodesFinder();
        /// <inheritdoc/>

        public Dictionary<int, List<int>> FindInterfacedNodes(List<int> nodes, int elemDim)
        {
            if (elemDim > 3 | elemDim < 0)
                throw new Exception("Неправильная размерность элементов");

            if (elemDim == 0)
                return interfacedNodesFinder.Find(nodes, ModelData.ObjectData.GetAllElements());
            else if (elemDim == 1)
                return interfacedNodesFinder.Find(nodes, ModelData.ObjectData.E1DCollection.GetObjects());
            else if (elemDim == 2)
                return interfacedNodesFinder.Find(nodes, ModelData.ObjectData.E2DCollection.GetObjects());
            else
                return interfacedNodesFinder.Find(nodes, ModelData.ObjectData.E3DCollection.GetObjects());
        }
        /// <inheritdoc/>

        public Dictionary<int, List<int>> FindInterfacedNodes(int elemDim)
        {
            if (elemDim > 3 | elemDim < 0)
                throw new Exception("Неправильная размерность элементов");

            if (elemDim == 0)
                return interfacedNodesFinder.Find(ModelData.ObjectData.GetAllElements());
            else if (elemDim == 1)
                return interfacedNodesFinder.Find(ModelData.ObjectData.E1DCollection.GetObjects());
            else if (elemDim == 2)
                return interfacedNodesFinder.Find(ModelData.ObjectData.E2DCollection.GetObjects());
            else
                return interfacedNodesFinder.Find(ModelData.ObjectData.E3DCollection.GetObjects());
        }
    }
}
