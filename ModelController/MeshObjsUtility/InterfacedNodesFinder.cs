using ModelControllerInterfaces;
using Model.Interfaces.Attributes;
using Model.Interfaces.MeshObjects;
using System.Collections.Generic;

namespace ModelController.MeshObjsUtility
{

    [Warning("Старайтесь использовать метод GetElements() в классе Node если нужно перебрать смежные элементы у узлов!")]
    public class InterfacedNodesFinder : IInterfacedNodesFinder
    {
        public Dictionary<int, List<int>> Find(List<int> nodes, IEnumerable<IElement> elements)
        {
            var interfaceNodes = new Dictionary<int, List<int>>();
            foreach (var element in elements)
            {
                if (element.ViewState)
                {
                    foreach (var node in element.GetVertexes())
                    {
                        if (nodes.Contains(node.Number))
                        {
                            if (!interfaceNodes.ContainsKey(node.Number))
                                interfaceNodes.Add(node.Number, new List<int>() { element.Number });
                            else
                            {
                                if (!interfaceNodes[node.Number].Contains(element.Number))
                                    interfaceNodes[node.Number].Add(element.Number);
                            }
                        }
                    }
                }
            }
            return interfaceNodes;
        }
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="elements"></param>
        /// Список элементов
        /// <returns></returns>
        public Dictionary<int, List<int>> Find(IEnumerable<IElement> elements)
        {
            var interfaceNodes = new Dictionary<int, List<int>>();

            foreach (var element in elements)
            {
                if (element.ViewState)
                {
                    foreach (var node in element.GetVertexes())
                    {
                        var nodeNumber = node.Number;
                        if (!interfaceNodes.ContainsKey(nodeNumber))
                            interfaceNodes.Add(nodeNumber, new List<int>() { element.Number });
                        else
                        {
                            if (!interfaceNodes[nodeNumber].Contains(element.Number))
                                interfaceNodes[nodeNumber].Add(element.Number);
                        }
                    }
                }
            }

            return interfaceNodes;
        }
    }
}
