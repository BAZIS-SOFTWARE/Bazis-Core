using Geometry;
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

        private string[] lines;

        private static string extractNumbersPattern = @"(-)?\d+(\.\d+(e(-|\+)\d+)?)?";
        private static string extractElementTypePattern = @"(?<=TYPE=)\w+";
        private static string extractGroupNamePattern = @"(?<=(EL|N)SET=)\w*";

        public IModelData Load(string path)
        {
            var indexToNode = new Dictionary<int, Node>();
            var indexToElement = new Dictionary<int, Element>();
            var groups = new List<GroupsData.Group>();
            lines = File.ReadAllLines(path);
            var i = 0;
            var offset = 0;
            while(i < lines.Length)
            {
                if (lines[i].StartsWith("*NODE", StringComparison.OrdinalIgnoreCase))
                    indexToNode = ReadNodes(i, out offset);
                else if (lines[i].StartsWith("*ELEMENT", StringComparison.OrdinalIgnoreCase))
                    indexToElement = ReadElements(i, out offset, indexToNode);
                else if (Regex.Match(lines[i], @"\*(EL|N)SET", RegexOptions.IgnoreCase).Success)
                    groups = ReadGroups(i, out offset, indexToElement, indexToNode);
                i += (offset > 0) 
                    ? offset 
                    : 1;
                offset = 0;
            }
            var modelData = GetModelData(indexToNode.Values, indexToElement.Values, groups);
            return modelData;
        }

        private IModelData GetModelData(IEnumerable<Node> nodes, IEnumerable<Element> elements, List<GroupsData.Group> groups)
        {
            var modelData = new ModelData();
            nodes.OrderBy(n => n.Number)
                .ToList()
                .ForEach(n => modelData.ObjectData.NodeCollection.Add(n));
            elements.OrderBy(e => e.Number)
                .ToList()
                .ForEach(e => AddElementToModelData(e, modelData));
            groups.ForEach(g => modelData.GroupData.Add(g));
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

        private Dictionary<int, Node> ReadNodes(int start, out int offset)
        {
            var indexToNode = new Dictionary<int, Node>();
            offset = 0;
            for(var i = start; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("**"))
                    break;
                var tokens = ReadNumbersFromLine(lines[i]).ToArray();
                if (tokens.Length == 4)
                {
                    var node = ParseNodeFromLine(tokens);
                    indexToNode.Add(node.Number, node);
                }
                offset++;
            }
            return indexToNode;
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
                case ElementKind.Пента: modelData.ObjectData.E3DCollection.Add((Penta)element); break;
                case ElementKind.Гекса: modelData.ObjectData.E3DCollection.Add((Hexa)element); break;
                default: throw new ArgumentException($"Неизвестный формат элемента: {element.ElementKind}");
            }
        }

        private Dictionary<int, Element> ReadElements(int start, out int offset, Dictionary<int, Node> indexToNode)
        {
            var indexToElement = new Dictionary<int, Element>();
            var elementType = string.Empty;
            offset = 0;
            for(var i = start; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("*ELSET", StringComparison.OrdinalIgnoreCase) || lines[i].StartsWith("**"))
                    break;
                else if (lines[i].StartsWith("*ELEMENT", StringComparison.OrdinalIgnoreCase))
                    elementType = Regex.Match(lines[i], extractElementTypePattern, RegexOptions.IgnoreCase).Value;
                else
                {
                    var tokens = ReadNumbersFromLine(lines[i]).ToArray();
                    if (tokens.Length > 0)
                    {
                        var element = ParseElementFromLine(tokens, elementType, indexToNode);
                        indexToElement.Add(element.Number, element);
                    }
                }
                offset++;
            }
            return indexToElement;
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
                case "C3D6": return new Penta(elementIndex, elementNodes);
                case "C3D8": return new Hexa(elementIndex, elementNodes);
                default: throw new ArgumentException($"Неизвестный формат элемента: {elementType}");
            }
        }

        private List<GroupsData.Group> ReadGroups(int start, out int offset, Dictionary<int, Element> indexToElement, 
            Dictionary<int, Node> indexToNode)
        {
            var groups = new List<GroupsData.Group>();
            var groupName = string.Empty;
            offset = 0;
            var isReadNodes = false;
            for(var i = start; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("**"))
                    break;
                else if (Regex.Match(lines[i], @"\*(EL|N)SET", RegexOptions.IgnoreCase).Success)
                    groupName = GetGroupName(lines[i], out isReadNodes);
                else
                {
                    var group = isReadNodes
                        ? ReadOneGroup(lines[i], indexToNode, groupName)
                        : ReadOneGroup(lines[i], indexToElement, groupName);
                    groups.Add(group);
                }
                offset++;
            }
            return groups;
        }

        private GroupsData.Group ReadOneGroup<T>(string line, Dictionary<int, T> mapping, string groupName)
            where T : ModelObject
        {
            var elements = ReadNumbersFromLine(line)
                        .Select(int.Parse)
                        .Select(idx => mapping[idx]);
            var group = new GroupsData.Group(groupName, elements.First().ObjType);
            group.AddRange(elements);
            return group;
        }

        private string GetGroupName(string line, out bool isReadNodes)
        {
            isReadNodes = false;
            if (line.StartsWith("*NSET", StringComparison.OrdinalIgnoreCase))
                isReadNodes = true;
            return Regex.Match(line, extractGroupNamePattern, RegexOptions.IgnoreCase).Value;
        }
    }
}