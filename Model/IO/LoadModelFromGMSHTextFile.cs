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

        private static string extractNumbersPattern = @"\d+(\.\d+(e-\d+)?)?";
        private static string extractElementTypePattern = @"(?<=TYPE=)\w+";
        private static string extractGroupNamePattern = @"(?<=ELSET=)\w*";


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

        private IEnumerable<string> ReadNumbersFromLine(string line)
        {
            var matches = Regex.Matches(line, extractNumbersPattern);
            for(var i = 0; i < matches.Count; i++)
            {
                yield return matches[i].Value;
            }
        }

        private void ReadNodes(StreamReader reader, ModelData modelData)
        {
            var indexToNode = new Dictionary<int, Node>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("**")) // end of block
                {
                    while(!line.StartsWith("*ELEMENT") && !reader.EndOfStream)
                        line = reader.ReadLine();
                    if(line.StartsWith("*ELEMENT"))
                        ReadElements(line, reader, indexToNode, modelData);
                    break;
                }
                var tokens = ReadNumbersFromLine(line).ToArray();
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
            var elementType = Regex.Match(startLine.ToUpper(), extractElementTypePattern).Value;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("*ELSET") || line.StartsWith("**"))
                {
                    while (!line.StartsWith("*ELSET") && !reader.EndOfStream)
                        line = reader.ReadLine();
                    if (line.StartsWith("*ELSET"))
                        ReadGroups(line, reader, indexToElement, modelData);
                    break;
                }
                else if (line.StartsWith("*ELEMENT"))
                    elementType = Regex.Match(line.ToUpper(), extractElementTypePattern).Value;
                else
                {
                    var tokens = ReadNumbersFromLine(line).ToArray();
                    if (tokens.Length > 0)
                    {
                        var element = ParseElementFromLine(tokens, elementType, indexToNode);
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

        private Element ParseElementFromLine(string[] tokens, string elementType, Dictionary<int, Node> indexToNode)
        {
            var elementIndex = int.Parse(tokens[0]);
            var elementNodes = tokens.Skip(1)
                .Select(int.Parse)
                .Select(i => indexToNode[i])
                .ToArray();
            var element = GetElement(elementIndex, elementNodes, elementType);
            return element;
        }

        private Element GetElement(int elementIndex, Node[] elementNodes, string elementType)
        {
            switch (elementType)
            {
                case "T3D2": return new Beam(elementIndex, elementNodes);
                case "CPS3": return new Triangle(elementIndex, elementNodes);
                case "CPS4": return new Quad(elementIndex, elementNodes);
                case "C3D4": return new Tetra(elementIndex, elementNodes);
                default: throw new ArgumentException($"Неизвестный формат элемента: {elementType}");
            }
        }

        private void ReadGroups(string currentLine, StreamReader reader, Dictionary<int, Element> indexToElement, ModelData modelData)
        {
            var groupName = Regex.Match(currentLine, extractGroupNamePattern).Value;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("*ELSET"))
                    groupName = Regex.Match(line, extractGroupNamePattern).Value;
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