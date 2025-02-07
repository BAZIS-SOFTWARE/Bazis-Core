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

namespace Model.IO
{
    /// <summary>
    /// Loads ModelData from file in .inp (gmsh) format
    /// </summary>
    public class LoadModelFromINPTextFile_v2 : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;
        private Dictionary<int, Node> mapIndexToNode;
        private Dictionary<int, Element> mapIndexToElement;
        private ModelData modelData;

        /// <summary>
        /// Load a model in the .inp format from the specified path
        /// </summary>
        public IModelData Load(string path)
        {
            modelData = new ModelData();
            using (var reader = new StreamReader(path))
            {
                LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки модели"));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.StartsWith("*NODE"))
                        ReadNodes(reader);
                }
            }
            return modelData;
        }

        /// <summary>
        /// Reads a block of nodes using the reader until it encounters element's block marker. Then passes control to the ReadElements method
        /// </summary>
        private void ReadNodes(StreamReader reader)
        {
            var nodes = new List<Node>();
            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки узлов"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == "******* E L E M E N T S *************")
                {
                    mapIndexToNode = nodes.ToDictionary(n => n.Number, n => n);
                    var startLine = reader.ReadLine();
                    ReadElements(startLine, reader);
                    break;
                }
                var tokens = line.Replace(" ", "").Split(',');
                var nodeIndex = int.Parse(tokens[0]);
                var coordinates = tokens.Skip(1)
                    .Select(x => float.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture))
                    .ToArray();
                var node = new Node(nodeIndex, new Point3D(coordinates[0], coordinates[1], coordinates[2]));
                nodes.Add(node);
            }
            nodes.OrderBy(e => e.Number)
                .ToList()
                .ForEach(e => modelData.ObjectData.NodeCollection.Add(ObjType.Узел.ToString(), e));
        }


        /// <summary>
        /// Define an ElementKind by string constant from file
        /// </summary>
        private ElementKind GetElementKind(string line)
        {
            var elementType = line.Replace(" ", "").Split(',')[1].Replace("type=", "");
            switch (elementType)
            {
                case "T3D2": return ElementKind.Стержень;
                case "CPS3": return ElementKind.Треугольник;
                case "CPS4": return ElementKind.Квадрат;
                case "C3D4": return ElementKind.Тетра;
                default: throw new ArgumentException($"Неизвестный формат элемента: {elementType}");
            }
        }


        /// <summary>
        ///  Creates an element by passed index, nodes and type and returns it
        /// </summary>
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

        /// <summary>
        /// Adds an element to the corresponding ModelData collection
        /// </summary>
        private void AddElementToModelData(Element element)
        {
            switch (element.ElementKind)
            {
                case ElementKind.Стержень: modelData.ObjectData.E1DCollection.Add(ObjType.Элемент1D.ToString(),(Beam)element); break;
                case ElementKind.Треугольник: modelData.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(), (Triangle)element); break;
                case ElementKind.Квадрат: modelData.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(), (Quad)element); break;
                case ElementKind.Тетра: modelData.ObjectData.E3DCollection.Add(ObjType.Элемент3D.ToString(), (Tetra)element); break;
                default: throw new ArgumentException($"Неизвестный формат элемента: {element.ElementKind}");
            }
        }

        /// <summary>
        /// Reads an element's block from the reader until it encounters to 'ELSET' marker. Then passes control to the ReadGroups method
        /// </summary>
        private void ReadElements(string startLine, StreamReader reader)
        {
            var elements = new List<Element>();
            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки элементов"));
            var elementType = GetElementKind(startLine);
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("*ELSET"))
                {
                    mapIndexToElement = elements.ToDictionary(e => e.Number, e => e);
                    ReadGroups(line, reader);
                    break;
                }
                else if (line.StartsWith("*ELEMENT"))
                    elementType = GetElementKind(line);
                else
                {
                    var tokens = line.Replace(" ", "").Split(',');
                    var elementIndex = int.Parse(tokens[0]);
                    var elementNodes = tokens.Skip(1)
                        .Select(int.Parse)
                        .Select(i => mapIndexToNode[i])
                        .ToArray();
                    var element = GetElement(elementIndex, elementNodes, elementType);
                    elements.Add(element);
                }
            }
            elements.OrderBy(e => e.Number)
                .ToList()
                .ForEach(e => AddElementToModelData(e));
        }

        private void AddGroupToModelData(string groupName, List<Element> elements)
        {
            var group = new Group(groupName, elements.First().ObjType);
            group.AddRange(elements);
            modelData.GroupData.Add(group);
        }

        /// <summary>
        /// Reads group's block from the reader until it encounters 'NSET' marker or end of stream
        /// </summary>
        private void ReadGroups(string currentLine, StreamReader reader)
        {
            var elements = new List<Element>();
            var groupName = currentLine.Split(',')[1].Replace("ELSET=", "");
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("*NSET"))
                {
                    AddGroupToModelData(groupName, elements);
                    break;
                }
                else if (line.StartsWith("*ELSET"))
                {
                    AddGroupToModelData(groupName, elements);
                    elements = new List<Element>();
                    groupName = line.Split(',')[1].Replace("ELSET=", "");
                }
                else
                {
                    var tokens = line.Replace(", ", " ").Trim().Split(' ');
                    var indexes = tokens.Select(int.Parse);
                    elements.AddRange(indexes.Select(i => mapIndexToElement[i]));
                }
            }
        }
    }
}