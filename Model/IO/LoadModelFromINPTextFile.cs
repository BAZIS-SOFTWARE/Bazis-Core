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
using Group = Model.GroupsData.Group;
using Model.ObjectsCollections;
using Model.Interfaces.ObjectsCollections;


namespace Model.IO
{
    /// <summary>
    /// Loads ModelData from file in .inp (gmsh) format
    /// </summary>
    public class LoadModelFromINPTextFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;

        private string[] lines;

        private static string extractNumbersPattern = @"(-)?\d+(\.\d+(e(-|\+)\d+)?)?";
        private static string extractElementTypePattern = @"(?<=TYPE=)\w+";
        private static string extractGroupNamePattern = @"(?<=(EL|N)SET=)\w*";

        public IModelData Load(string path)
        {
            var modelData = new ModelData();
            var indexToElement = new Dictionary<int, Element>();
            lines = File.ReadAllLines(path);
            var i = 0;
            var offset = 0;
            while(i < lines.Length)
            {
                if (lines[i].StartsWith("*NODE", StringComparison.OrdinalIgnoreCase))
                    ReadNodes(i, out offset, modelData);
                // по нашему наборы
                else if (lines[i].StartsWith("*ELEMENT", StringComparison.OrdinalIgnoreCase))
                    ReadElements(i, out offset, indexToElement, modelData);
                // по нашему  group
                else if (Regex.Match(lines[i], @"\*(EL|N)SET", RegexOptions.IgnoreCase).Success)
                    ReadGroups(i, out offset, indexToElement, modelData);
                i += (offset > 0) ? offset : 1;
                offset = 0;
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

        private void ReadNodes(int start, out int offset, ModelData modelData)
        {
            offset = 0;
            for(var i = start; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("**"))
                    break;
                var tokens = ReadNumbersFromLine(lines[i]).ToArray();
                if (tokens.Length == 4)
                {
                    var node = ParseNodeFromLine(tokens);
                    modelData.ObjectData.NodesSet.Add(node.Number, node);
                }
                offset++;
            }
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

        private void AddElementToModelData(Element element, string setName, ModelData modelData)
        {
            if (element is Element3D element3D)
                modelData.ObjectData.E3DCollection.Add(setName, element3D);
            else if (element is Element2D element2D)
                modelData.ObjectData.E2DCollection.Add(setName, element2D);
            else if (element is Beam beam)
                modelData.ObjectData.E1DCollection.Add(setName, beam);
            else
                throw new ArgumentException($"Неизвестный тип элемента: {element}");
        }

        private void ReadElements(int start, out int offset, Dictionary<int, Element> indexToElement, ModelData modelData)
        {
            var elementType = string.Empty;
            var setName = string.Empty;

            offset = 0;
            for(var i = start; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("*ELSET", StringComparison.OrdinalIgnoreCase) || lines[i].StartsWith("**"))
                    break;
                else if (lines[i].StartsWith("*ELEMENT", StringComparison.OrdinalIgnoreCase))
                {
                    elementType = Regex.Match(lines[i], extractElementTypePattern, RegexOptions.IgnoreCase).Value;
                    setName = GetGroupName(lines[i]);
                }
                else
                {
                    var tokens = ReadNumbersFromLine(lines[i]).ToArray();
                    if (tokens.Length > 0)
                    {
                        var element = CreateElement(tokens, elementType, modelData);
                        indexToElement.Add(element.Number, element);
                        AddElementToModelData(element, setName, modelData);
                    }
                }
                offset++;
            }
        }

        private Element CreateElement(string[] tokens, string elementType, ModelData modelData)
        {
            var elementIndex = int.Parse(tokens[0]);
            var elementNodes = tokens.Skip(1)
                .Select(int.Parse)
                .Select(index => modelData.ObjectData.NodesSet[index])
                .ToArray();

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

        private void ReadGroups(int start, out int offset, Dictionary<int, Element> indexToElement, ModelData modelData)
        {
            var groupName = string.Empty;
            var isReadNodes = false;
            offset = 0;
            for (var i = start; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("**"))
                    break;
                else if (Regex.Match(lines[i], @"\*(EL|N)SET", RegexOptions.IgnoreCase).Success)
                {
                    groupName = GetGroupName(lines[i]);
                    isReadNodes = Regex.Match(lines[i], @"\*NSET", RegexOptions.IgnoreCase).Success;
                }
                else
                {
                    var group = isReadNodes
                        ? GetGroup(lines[i], index => modelData.ObjectData.NodesSet[index], groupName)
                        : GetGroup(lines[i], index => indexToElement[index], groupName);
                    modelData.GroupData.Add(group);
                }
                offset++;
            }
        }

        private Group GetGroup<T>(string line, Func<int, T> mapping, string groupName)
            where T : ModelObject
        {
            var elements = ReadNumbersFromLine(line)
                                                    .Select(int.Parse)
                                                    .Select(index => mapping(index))
                                                    .ToList();
            var objType = elements[0].ObjType;
            var group = new Group(groupName, objType);
            elements.ForEach(e => group.Add(e));
            return group;
        }

        private string GetGroupName(string line)
        {
            return Regex.Match(line, extractGroupNamePattern, RegexOptions.IgnoreCase).Value;
        }
    }
}
