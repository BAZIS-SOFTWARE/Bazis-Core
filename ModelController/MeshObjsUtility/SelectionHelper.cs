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
        ObjectsData objectsData;
        public SelectionHelper(ObjectsData objectsData)
        {
            this.objectsData = objectsData;
        }

        public List<int> SelectInDirecion<T>(float angleToSearch, int firstNumb, int secondNumb, Color color)
            where T : SurfaceElement
        {
            var elements = objectsData.FindMany<T>().ToArray();
            var finder = new FindInterfacedNodes(elements);
            var interfacesNodes = finder.Find();

            var cos = Math.Cos(angleToSearch * (float)Math.PI / 180);
            var isFind = true;
            var visitNodes = new List<int>() { firstNumb, secondNumb };

            while (isFind)
            {
                isFind = false;
                var firstNode = (Node)objectsData.Find(firstNumb);
                var secondNode = (Node)objectsData.Find(secondNumb);
                var dir = secondNode.Position.Sub(firstNode.Position);

                foreach (var elNumber in interfacesNodes[secondNumb])
                {
                    var elem = objectsData.Find(elements, elNumber);

                    foreach (var elNode in elem.GetNodes())
                    {
                        var tempDir = elNode.Position.Sub(secondNode.Position);
                        var angle = Math.Abs(Vector.GetAngleVectors(dir, tempDir));
                        if (angle > cos && !visitNodes.Contains(elNode.Number))
                        {
                            elNode.MasterColor = color;

                            firstNumb = visitNodes.Last();
                            secondNumb = elNode.Number;

                            visitNodes.Add(elNode.Number);
                            isFind = true;
                            break;
                        }

                    }
                    if (isFind) break;
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


        public List<int> SelectInPlane<T>(float angleToSearch, int startNumber, Color color)
                        where T : Element2D
        {
            var elements = objectsData.FindMany<T>().ToArray();

            var finder = new FindInterfacedNodes(elements);
            var interfacesNodes = finder.Find();

            var cos = Math.Cos(angleToSearch * (float)Math.PI / 180);

            var visitElements = new List<int>() { startNumber };
            var isFind = true;
            while (isFind)
            {
                isFind = false;
                var elNumb = visitElements.Last();

                var element = objectsData.Find(elements, elNumb);
                foreach (var node in element.GetNodes())
                {
                    var normal = element.CalcNormal();
                    foreach (var neibElNumb in interfacesNodes[node.Number])
                    {
                        var neibElement = objectsData.Find(elements, neibElNumb);
                        //neibElement.CalcSideNormals();
                        var neibNormal = neibElement.CalcNormal();

                        var angle = Vector.GetAngleVectors(normal, neibNormal);

                        if (angle > cos && !visitElements.Contains(neibElement.Number))
                        {
                            neibElement.MasterColor = color;
                            visitElements.Add(neibElNumb);
                            isFind = true;

                        }
                    }
                    if (isFind) break;
                }
            }
            return visitElements;
        }
    }
}
