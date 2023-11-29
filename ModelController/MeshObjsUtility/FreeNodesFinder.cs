using Model.MeshObjects;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsUtility
{
    public class FreeNodesFinder
    {
        IObjectsData objsData;
        public FreeNodesFinder(IObjectsData objsData)
        {
            if (objsData.FindMany<Node>() == null) throw new Exception("Модель не содержит узлов!");
            this.objsData = objsData;
        }
        public IEnumerable<int> Find<T>() where T : Element
        {
            var elements = objsData.FindMany<T>().ToArray();
            var finder = new InterfacedNodesFinder();
            var interfaceNodes = finder.Find(elements);

            var nodesNumbersAll = objsData.FindMany<Node>().Select(x => x.Number);
            var nodesNumbersElements = interfaceNodes.Keys;
            var distNodesNumbersElements = nodesNumbersElements.Distinct();
            var freeNodesNumbers = nodesNumbersAll.Except(distNodesNumbersElements);

            return freeNodesNumbers;
        }
    }
}
