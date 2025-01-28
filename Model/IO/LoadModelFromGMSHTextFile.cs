using Geometry;
using Model.Events;
using Model.MeshObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model.IO
{
    public class LoadModelFromGMSHTextFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;
        private static Regex extractNumbers;
        private static Regex extractElementType;
        private static Regex extractGroupName;

        static LoadModelFromGMSHTextFile()
        {
            extractNumbers = new Regex(@"\d+(\.\d+(e-\d+)?)?");
            extractElementType = new Regex(@"(?<=TYPE=)\w+");
            extractGroupName = new Regex(@"(?<=ELSET=)\w*");
        }

        public IModelData Load(string path)
        {
            var modelData = new ModelData();
            using (var reader = new StreamReader(path))
            {
                LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки модели"));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.StartsWith("*NODE"))
                        ReadNodes(reader, modelData);
                }
            }
            return modelData;
        }

        private string[] ReadNumbersFromLine(string line)
        {
            return extractNumbers
                    .Matches(line)
                    .Cast<Match>()
                    .Select(x => x.Value)
                    .ToArray();
        }

        private void ReadNodes(StreamReader reader, ModelData modelData)
        {
            var indexToNode = new Dictionary<int, Node>();
            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки узлов"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("**")) // end of nodes block
                {
                    while(!line.StartsWith("*ELEMENT") && !reader.EndOfStream)
                        line = reader.ReadLine();
                    if(line.StartsWith("*ELEMENT"))
                        ReadElements(line, reader, indexToNode, modelData);
                    break;
                }
                var tokens = ReadNumbersFromLine(line);
                if (tokens.Length == 4) // parse coordinates
                {
                    var node = ParseNodeFromLine(tokens);
                    indexToNode.Add(node.Number, node);
                }
                else
                    continue;
            }
            indexToNode.OrderBy(e => e.Key)
                .ToList()
                .ForEach(e => modelData.ObjectData.NodeCollection.Add(e.Value));
        }

        private Node ParseNodeFromLine(string[] tokens)
        {
            var nodeIndex = int.Parse(tokens[0]);
            var coordinates = tokens.Skip(1)
                .Select(x => float.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture))
                .ToArray();
            var node = new Node(nodeIndex, new Point3D(coordinates[0], coordinates[1], coordinates[2]));
            return node;
        }

        private ElementKind GetElementKind(string line)
        {
            var elementKind = extractElementType.Match(line.ToUpper()).Value;
            switch (elementKind)
            {
                case "T3D2": return ElementKind.Стержень;
                case "CPS3": return ElementKind.Треугольник;
                case "CPS4": return ElementKind.Квадрат;
                case "C3D4": return ElementKind.Тетра;
                default: throw new ArgumentException($"Неизвестный формат элемента: {extractElementType}");
            }
        }

        private Element GetElement(int elementIndex, Node[] elementNodes, ElementKind elementKind)
        {
            switch (elementKind)
            {
                case ElementKind.Стержень: return new Beam(elementIndex, elementNodes); 
                case ElementKind.Треугольник: return new Triangle(elementIndex, elementNodes);
                case ElementKind.Квадрат: return new Quad(elementIndex, elementNodes);
                case ElementKind.Тетра: return new Tetra(elementIndex, elementNodes);
                default: throw new ArgumentException($"Неизвестный формат элемента: {elementKind}");
            }
        }

        private void AddElementToModelData(Element element, ModelData modelData)
        {
            switch (element.ElementKind)
            {
                case ElementKind.Стержень: modelData.ObjectData.E1DCollection.Add((Beam)element); break;
                case ElementKind.Треугольник: modelData.ObjectData.E2DCollection.Add((Triangle)element); break;
                case ElementKind.Квадрат: modelData.ObjectData.E2DCollection.Add((Quad)element); break;
                case ElementKind.Тетра: modelData.ObjectData.E3DCollection.Add((Tetra)element); break;
                default: throw new ArgumentException($"Неизвестный формат элемента: {element.ElementKind}");
            }
        }

        private void ReadElements(string startLine, StreamReader reader, Dictionary<int, Node> indexToNode, ModelData modelData)
        {
            var indexToElement = new Dictionary<int, Element>();
            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки элементов"));
            var elementKind = GetElementKind(startLine);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("*ELSET") || line.StartsWith("**"))
                {
                    while (!line.StartsWith("*ELSET") && !reader.EndOfStream)
                        line = reader.ReadLine();
                    if(!reader.EndOfStream)
                        ReadGroups(line, reader, indexToElement, modelData);
                    break;
                }
                else if (line.StartsWith("*ELEMENT"))
                    elementKind = GetElementKind(line);
                else
                {
                    var tokens = ReadNumbersFromLine(line);
                    if (tokens.Length > 0)
                    {
                        var element = ParseElementFromLine(tokens, elementKind, indexToNode);
                        indexToElement.Add(element.Number, element);
                    }
                    else
                        continue;
                }
            }
            indexToElement.OrderBy(e => e.Key)
                .ToList()
                .ForEach(e => AddElementToModelData(e.Value, modelData));
        }

        private Element ParseElementFromLine(string[] tokens, ElementKind elementKind, Dictionary<int, Node> indexToNode)
        {
            var elementIndex = int.Parse(tokens[0]);
            var elementNodes = tokens.Skip(1)
                .Select(int.Parse)
                .Select(i => indexToNode[i])
                .ToArray();
            var element = GetElement(elementIndex, elementNodes, elementKind);
            return element;
        }

        private void ReadGroups(string currentLine, StreamReader reader, Dictionary<int, Element> indexToElement, ModelData modelData)
        {
            var groupName = extractGroupName.Match(currentLine).Value;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("*ELSET"))
                    groupName = extractGroupName.Match(line).Value;
                else
                {
                    var elements = ReadNumbersFromLine(line)
                        .Select(int.Parse)
                        .Select(i => indexToElement[i]);
                    var group = new GroupsData.Group(groupName, elements.First().ObjType);
                    group.AddRange(elements);
                    modelData.GroupData.Add(group);
                }
            }
        }
    }
}