using ModelControllerInterfaces;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectController.MeshObjsUtility
{
    public class FreeNodesFinder : IFreeNodesFinder
    {
        /// <inheritdoc/>
        public IEnumerable<int> Find(IObjectsData objsData)
        {
            if (objsData.NodesSet.Count == 0) throw new Exception("Модель не содержит узлов!");
            var elements = objsData.GetAllElements();
            var finder = new InterfacedNodesFinder();
            var interfaceNodes = finder.Find(elements);

            var nodesNumbersAll = objsData.NodesSet.Values.Select(x => x.Number);
            var nodesNumbersElements = interfaceNodes.Keys;
            var distNodesNumbersElements = nodesNumbersElements.Distinct();
            var freeNodesNumbers = nodesNumbersAll.Except(distNodesNumbersElements);

            return freeNodesNumbers;
        }
    }
}
