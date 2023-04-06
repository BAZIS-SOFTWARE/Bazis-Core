using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelController.MeshObjsUtility
{
    public class FindInterfacedNodes
    {
        Element[] elements;
        public FindInterfacedNodes(Element[] elements)
        {
            this.elements = elements;
        }
        public Dictionary<int, List<int>> Find(List<int> nodes)
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
                            {
                                interfaceNodes.Add(node.Number, new List<int>() { element.Number });
                                //break;
                            }
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

        public Dictionary<int, List<int>> Find()
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
                        {
                            interfaceNodes.Add(nodeNumber, new List<int>() { element.Number });
                            //break;
                        }
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
