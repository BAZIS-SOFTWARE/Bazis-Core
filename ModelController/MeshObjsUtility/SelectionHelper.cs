using Geometry;
using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace ModelController.MeshObjsUtility
{
    public class SelectionHelper
    {
        private readonly ObjectsData objectsData;

        public SelectionHelper(ObjectsData objectsData)
        {
            this.objectsData = objectsData;
        }

        public List<int> SelectInDirection<T>(float angleInGradus, int firstNumber, int secondNumber, Color color)
            where T : SurfaceElement
        {
            var elements = objectsData.FindMany<T>().ToArray();
            var finder = new FindInterfacedNodes(elements);
            var interfacesNodes = finder.Find();
            var cos = Math.Cos(ConvertGradusToRadian(angleInGradus));
            var isFind = true;
            var visitNodes = new List<int>() { firstNumber, secondNumber };
            while (isFind)
            {
                isFind = false;
                var firstNode = (Node)objectsData.Find(firstNumber);
                var secondNode = (Node)objectsData.Find(secondNumber);
                var dir = secondNode.Position.Sub(firstNode.Position);
                foreach (var elementNumber in interfacesNodes[secondNumber])
                {
                    var elem = objectsData.Find(elements, elementNumber);
                    foreach (var elementNode in elem.GetNodes())
                    {
                        var tempDirection = elementNode.Position.Sub(secondNode.Position);
                        var angle = Math.Abs(Vector.GetAngleVectors(dir, tempDirection));
                        if (angle > cos && !visitNodes.Contains(elementNode.Number))
                        {
                            elementNode.MasterColor = color;
                            firstNumber = visitNodes.Last();
                            secondNumber = elementNode.Number;
                            visitNodes.Add(elementNode.Number);
                            isFind = true;
                            break;
                        }
                    }
                    if (isFind) 
                        break;
                }
            }
            return visitNodes;
        }
        
        public void SelectInPlane<T>(Plane plane, Color color)
                    where T : Node
        {
            var nodes = objectsData.FindMany<T>().ToArray();
            foreach (var node in nodes)
            {
                var dist = plane.PointPosition(node.Position);
                if (Math.Abs(dist) < 1e-2)
                    node.MasterColor = color;
            }
        }

        /// <summary>
        /// Выбор всех элементов, лежащих в одной плоскости с startNumber элементом, и углом между плоскостями, меньшим чем angleInGradus
        /// </summary>
        /// <returns>
        /// Список выбранных номеров элементов
        /// </returns>
        public List<int> SelectInPlane<T>(float angleInGradus, int startNumber, Color color)
            where T : Element2D
        {
            var maxAngleRadians = ConvertGradusToRadian(angleInGradus);
            var elements = objectsData.FindMany<Element2D>().ToArray();
            var graph = CreateElementsGraph(elements);
            var queue = new Queue<int>();
            queue.Enqueue(startNumber);
            var used = new HashSet<int>();
            while(queue.Count > 0)
            {
                var currentNumber = queue.Dequeue();
                var currentElement = objectsData.Find(elements, currentNumber);
                foreach (var neighbourNumber in graph[currentNumber])
                {
                    if(!used.Contains(neighbourNumber))
                    {
                        var neighbourElement = objectsData.Find(elements, neighbourNumber);
                        var cos = Vector.GetAngleVectors(currentElement.CalcNormal(), neighbourElement.CalcNormal());
                        if (cos < 0)
                            cos *= -1;
                        var angleRadian = Math.Acos(cos);
                        if (angleRadian <= maxAngleRadians)
                        {
                            used.Add(neighbourNumber);
                            queue.Enqueue(neighbourNumber);                            
                            neighbourElement.MasterColor = color;
                        }
                    }
                }
            }
            return used.ToList();
        }

        /// <returns>
        /// Словарь, где ключ - номер элемента, значение - список номеров элементов, имеющих хотя бы одну общую точку с ключом
        /// </returns>
        private Dictionary<int, List<int>> CreateElementsGraph(Element2D[] elements)
        {
            var graph = new Dictionary<int, List<int>>();
            var finder = new FindInterfacedNodes(elements);
            var interfacedNodes = finder.Find();
            foreach (var element in elements)
            {
                var nodes = element.GetNodes();
                var neighbourElementsNumber = new HashSet<int>();
                foreach (var node in nodes)
                {
                    foreach (var interfacedElementNumber in interfacedNodes[node.Number])
                    {
                        neighbourElementsNumber.Add(interfacedElementNumber);
                    }
                }
                graph.Add(element.Number, neighbourElementsNumber.ToList());
            }
            return graph;
        }

        private float ConvertGradusToRadian(float angleInGradus)
        {
            return angleInGradus * (float)Math.PI / 180f;
        }
    }
}
