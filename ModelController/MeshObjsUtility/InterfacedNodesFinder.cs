using Model;
using ModelControllerInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsUtility
{

    [Warning("Старайтесь использовать метод GetElements() в классе Node если нужно перебрать смежные элементы у узлов!")]
    public class InterfacedNodesFinder : IInterfacedNodesFinder
    {
        public Dictionary<int, List<int>> Find(List<int> nodes, IEnumerable<Element> elements)
        {
            var interfaceNodes = new Dictionary<int, List<int>>();
            foreach (var element in elements)
            {
                if (element.ViewState)
                {
                    foreach (var node in element.GetNodes())
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
        public Dictionary<int, List<int>> Find(IEnumerable<Element> elements)
        {
            var interfaceNodes = new Dictionary<int, List<int>>();

            foreach (var element in elements)
            {
                if (element.ViewState)
                {
                    foreach (var node in element.GetNodes())
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
