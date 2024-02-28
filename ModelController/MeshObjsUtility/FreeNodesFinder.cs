using Model.MeshObjects;
using ModelInterfaces;
using ModelInterfaces.MeshObjects;
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
            if (objsData.NodeCollection.Count == 0) throw new Exception("Модель не содержит узлов!");
            this.objsData = objsData;
        }
        public IEnumerable<int> Find()
        {
            var elements = objsData.GetObjects(ObjType.Элемент).Select(x => (IElement)x);
            var finder = new InterfacedNodesFinder();
            var interfaceNodes = finder.Find(elements);

            var nodesNumbersAll = objsData.NodeCollection.Select(x => x.Number);
            var nodesNumbersElements = interfaceNodes.Keys;
            var distNodesNumbersElements = nodesNumbersElements.Distinct();
            var freeNodesNumbers = nodesNumbersAll.Except(distNodesNumbersElements);

            return freeNodesNumbers;
        }
    }
}
