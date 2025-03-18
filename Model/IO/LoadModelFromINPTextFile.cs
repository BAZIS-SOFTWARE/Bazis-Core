using Geometry;
using Model.Events;
using Model.GroupsData;
using Model.MeshObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Model.ObjectsCollections;
using System.Drawing;
using Group = Model.GroupsData.Group;


namespace Model.IO
{
    /// <summary>
    /// Loads ModelData from file in .inp (gmsh) format
    /// </summary>
    public class LoadModelFromINPTextFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;

        private string[] lines;

        private static Random random = new Random();
        private static string extractNumbersPattern = @"(-)?\d+(\.\d+(e(-|\+)\d+)?)?";
        private static string extractElementTypePattern = @"(?<=TYPE=)\w+";
        private static string extractGroupNamePattern = @"(?<=(EL|N)SET=)\w*";

        public IModelData Load(string path)
        {
            var indexToNode = new Dictionary<int, Node>();
            var indexToElement = new Dictionary<int, Element>();
            var groups = new List<Group>();
            var sets = new List<ObjectsSet<Element>>();
            lines = File.ReadAllLines(path);
            var i = 0;
            var offset = 0;
            while(i < lines.Length)
            {
                if (lines[i].StartsWith("*NODE", StringComparison.OrdinalIgnoreCase))
                    indexToNode = ReadNodes(i, out offset);
                else if (lines[i].StartsWith("*ELEMENT", StringComparison.OrdinalIgnoreCase))
                    indexToElement = ReadElements(i, out offset, indexToNode);
                else if (Regex.Match(lines[i], @"\*ELSET", RegexOptions.IgnoreCase).Success)
                    sets = ReadObjectsSet(i, out offset, indexToElement);
                else if (Regex.Match(lines[i], @"\*NSET", RegexOptions.IgnoreCase).Success)
                    groups = ReadGroups(i, out offset, indexToNode);
                i += (offset > 0) 
                    ? offset 
                    : 1;
                offset = 0;
            }
            var modelData = GetModelData(indexToNode.Values, indexToElement.Values, groups, sets);
            return modelData;
        }

        private IModelData GetModelData(IEnumerable<Node> nodes, IEnumerable<Element> elements, List<Group> groups, List<ObjectsSet<Element>> sets)
        {
            var modelData = new ModelData();

            foreach (var node in nodes)
            {
                modelData.ObjectData.NodesSet.Add(node.Number, node);
            }

            elements.OrderBy(e => e.Number)
                .ToList()
                .ForEach(e => AddElementToModelData(e, modelData));

            groups.ForEach(g => modelData.GroupData.Add(g));

            sets.ForEach(s => AddObjectsSetToModelData(modelData, s));

            return modelData;
        }

        private void AddObjectsSetToModelData<T>(ModelData modelData, ObjectsSet<T> s)
            where T : Element
        {
            switch(s.ObjType)
            {
                case ObjType.Элемент1D: modelData.ObjectData.E1DCollection.Add(s as ObjectsSet<Beam>); break;
                case ObjType.Элемент2D: modelData.ObjectData.E2DCollection.Add(s as ObjectsSet<Element2D>); break;
                case ObjType.Элемент3D: modelData.ObjectData.E3DCollection.Add(s as ObjectsSet<Element3D>); break;
            }
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
            var name = element.ObjType.ToString();
            switch (element.ElementKind)
            {
                case ElementKind.Стержень: modelData.ObjectData.E1DCollection.Add(name, (Beam)element); break;
                case ElementKind.Треугольник: modelData.ObjectData.E2DCollection.Add(name, (Triangle)element); break;
                case ElementKind.Квадрат: modelData.ObjectData.E2DCollection.Add(name, (Quad)element); break;
                case ElementKind.Тетра: modelData.ObjectData.E3DCollection.Add(name, (Tetra)element); break;
                case ElementKind.Пента: modelData.ObjectData.E3DCollection.Add(name, (Penta)element); break;
                case ElementKind.Гекса: modelData.ObjectData.E3DCollection.Add(name, (Hexa)element); break;
                default: throw new ArgumentException($"Неизвестный формат элемента: {element.ElementKind}");
            }
        }

        private Dictionary<int, Element> ReadElements(int start, out int offset, Dictionary<int, Node> indexToNode)
        {
            var indexToElement = new Dictionary<int,Element>();
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

        private List<GroupsData.Group> ReadGroups(int start, out int offset, Dictionary<int, Node> indexToNode)
        {
            var groups = new List<GroupsData.Group>();
            var groupName = string.Empty;
            offset = 0;
            for (var i = start; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("**"))
                    break;
                else if (Regex.Match(lines[i], @"\*NSET", RegexOptions.IgnoreCase).Success)
                    groupName = GetGroupName(lines[i]);
                else
                {
                    var group = GetGroup(lines[i], indexToNode, groupName);
                    groups.Add(group);
                }
                offset++;
            }
            return groups;
        }

        private List<ObjectsSet<T>> ReadObjectsSet<T>(int start, out int offset, Dictionary<int, T> indexToElement)
            where T : Element
        {
            var sets = new List<ObjectsSet<T>>();
            var groupName = string.Empty;
            offset = 0;
            for (var i = start; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("**") || Regex.Match(lines[i], @"\*NSET", RegexOptions.IgnoreCase).Success)
                    break;
                else if (Regex.Match(lines[i], @"\*ELSET", RegexOptions.IgnoreCase).Success)
                    groupName = GetGroupName(lines[i]);
                else
                {
                    var objectSet = GetObjectSet(lines[i], indexToElement, groupName);
                    sets.Add(objectSet);
                }
                offset++;
            }
            return sets;
        }

        private ObjectsSet<T> GetObjectSet<T>(string line, Dictionary<int, T> mapping, string groupName)
            where T : Element
        {
            var objectSet = new ObjectsSet<T>(groupName);
            ReadNumbersFromLine(line)
                .Select(int.Parse)
                .Select(index => mapping[index])
                .ToList()
                .ForEach(e => objectSet.Add(e.Number, e));
            objectSet.SetColor(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256)));
            return objectSet;
        }

        private Group GetGroup(string line, Dictionary<int, Node> mapping, string groupName)
        {
            var group = new Group(groupName, ObjType.Узел);
            ReadNumbersFromLine(line)
                .Select(int.Parse)
                .Select(index => mapping[index])
                .ToList()
                .ForEach(e => group.Add(e));
            return group;
        }

        private string GetGroupName(string line)
        {
            return Regex.Match(line, extractGroupNamePattern, RegexOptions.IgnoreCase).Value;
        }
    }
}