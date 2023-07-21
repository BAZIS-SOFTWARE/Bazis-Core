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

        /// <summary>
        /// Выбор последовательности узлов по заданному напрвлению. Направление задаётся номерами двух смежных узлов.
        /// </summary>
        /// <param name="angleInGradus">Максимальный угол в градусах, под которым будет выделение</param>
        /// <param name="indexFirst">Номер первого узла</param>
        /// <param name="indexSecond">Номер второго узла</param>
        /// <param name="color">Цвет выделения</param>
        /// <returns>
        /// Номера выделенных узлов
        /// </returns>
        public List<int> SelectInDirection<T>(float angleInGradus, int indexFirst, int indexSecond, Color color)
            where T : SurfaceElement
        {
            var elements = objectsData.FindMany<T>().ToArray();
            var interfacedElements = new FindInterfacedNodes(elements).Find();
            var firstNode = (Node)objectsData.Find(indexFirst);
            var secondNode = (Node)objectsData.Find(indexSecond);
            var initialDirection = secondNode.Position.Sub(firstNode.Position);
            var result = new HashSet<int> { indexFirst, indexSecond };
            var currentNode = secondNode;
            var isMoveNext = true;
            while (isMoveNext)
            {
                isMoveNext = false;
                foreach(var elementNumber in interfacedElements[currentNode.Number])
                {
                    var element = (T)objectsData.Find(elementNumber);
                    foreach(var node in element.GetNodes())
                    {
                        var currentDirection = node.Position.Sub(currentNode.Position);
                        var angle = Math.Acos(Vector.GetAngleVectors(currentDirection, initialDirection));
                        if(angle <= ConvertGradusToRadian(angleInGradus) && !result.Contains(node.Number))
                        {
                            currentNode = node;
                            node.MasterColor = color;
                            result.Add(node.Number);
                            isMoveNext = true;
                            break;
                        }
                    }
                    if (isMoveNext)
                        break;
                }
            }
            return result.ToList();
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
        /// Выбор всех элементов, лежащих в одной плоскости с заданным элементом, 
        /// и углом между плоскостями, не большим чем заданный угол
        /// </summary>
        /// <returns>
        /// Список номеров выбранных элементов
        /// </returns>
        public List<int> SelectInPlane<T>(float angleInGradus, int startNumber, Color color)
            where T : Element2D
        {
            var maxAngleRadians = ConvertGradusToRadian(angleInGradus);
            var elements = objectsData.FindMany<Element2D>().ToArray();
            var graph = GetAdjacentElements(elements);
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
                        var angleRadian = Math.Acos(Math.Abs(cos));
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
        /// Словарь, где ключ - номер элемента (2D, 3D), значение - список номеров элементов, 
        /// имеющих хотя бы одну общую точку с ключом.
        /// </returns>
        private Dictionary<int, List<int>> GetAdjacentElements(SurfaceElement[] elements)
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

        /// <summary>
        /// Перевести угол из градусов в радианы
        /// </summary>
        private float ConvertGradusToRadian(float angleInGradus)
        {
            return angleInGradus * (float)Math.PI / 180f;
        }
    }
}
