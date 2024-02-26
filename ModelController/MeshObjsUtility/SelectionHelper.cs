using Geometry;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Model.MeshObjects;
using ModelInterfaces.MeshObjects;
using Model;

namespace ModelController.MeshObjsUtility
{
    /// <summary>
    /// SelectionHelper
    /// </summary>
    public class SelectionHelper
    {
        private readonly IObjectsData objectsData;
        /// <summary>
        /// SelectionHelper
        /// </summary>
        /// <param name="objectsData"></param>
        public SelectionHelper(IObjectsData objectsData)
        {
            this.objectsData = objectsData;
        }

        /// <summary>
        /// Выбор последовательности узлов по заданному напрвлению. Направление задаётся номерами двух смежных узлов.
        /// </summary>
        /// <param name="angleInGradus">Максимальный угол в градусах, под которым будет выделение</param>
        /// <param name="numbFirst">Номер первого узла</param>
        /// <param name="numbSecond">Номер второго узла</param>
        /// <param name="color">Цвет выделения</param>
        /// <returns>
        /// Номера выделенных узлов
        /// </returns>
        public List<int> SelectNodeInDirection(float angleInGradus, int numbFirst, int numbSecond, Color color)     
        {
            var elements = objectsData.GetAllElements().ToList();

            var interfacedElements = new InterfacedNodesFinder().Find(elements);
            
            var nodes = objectsData.NodeCollection.ToArray();
            
            var firstNode = nodes.Find(numbFirst);
            var secondNode = nodes.Find(numbSecond);

            var previousDirection = secondNode.Position.Sub(firstNode.Position);
            var result = new HashSet<int> { numbFirst, numbSecond };
            var currentNode = secondNode;
            var isMoveNext = true;
            while (isMoveNext)
            {
                isMoveNext = false;
                foreach(var elementNumber in interfacedElements[currentNode.Number])
                {           
                    var element = elements.Find(elementNumber);
                    foreach(var node in element.GetVertexes())
                    {
                        var currentDirection = node.Position.Sub(currentNode.Position);
                        var angle = Math.Acos(Vector.GetAngleVectors(currentDirection, previousDirection));
                        if(angle <= ConvertGradusToRadian(angleInGradus) && !result.Contains(node.Number))
                        {
                            currentNode = node;
                            previousDirection = currentDirection;
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
        /// <summary>
        /// SelectNodeInPlane
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="color"></param>
        public void SelectNodeInPlane(Plane plane, Color color)
                    
        {
            var nodes = objectsData.NodeCollection.ToArray();
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
        public List<int> SelectE2DInPlane(float angleInGradus, int startNumber, Color color)         
        {
            var maxAngleRadians = ConvertGradusToRadian(angleInGradus);
            var elements = objectsData.E2DCollection.ToArray();
            var elementsAdjacency = GetAdjacentElements<IElement2D>(elements);
            var queue = new Queue<int>();
            queue.Enqueue(startNumber);
            var used = new HashSet<int>();
            while(queue.Count > 0)
            {
                var currentNumber = queue.Dequeue();
                var currentElement = elements.Find(currentNumber);
                foreach (var neighbourNumber in elementsAdjacency[currentNumber])
                {
                    if(!used.Contains(neighbourNumber))
                    {
                        var neighbourElement = elements.Find(neighbourNumber);
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
        private Dictionary<int, List<int>> GetAdjacentElements<T>(IEnumerable<IElement> elements)
            where T: IElement
        {
            var result = new Dictionary<int, List<int>>();
            foreach (var element in elements)
            {
                var neighbourElementsNumber = new HashSet<int>();
                foreach (var node in element.GetVertexes())
                {
                    foreach (var elementAdjacent in node.GetElements().Where(x => x is T))
                    {
                        neighbourElementsNumber.Add(elementAdjacent.Number);
                    }
                }
                result.Add(element.Number, neighbourElementsNumber.ToList());
            }
            return result;
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
