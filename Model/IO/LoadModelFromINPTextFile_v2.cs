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
using System.Drawing;
using Model.Interfaces.ObjectsCollections;
using Model.ObjectsCollections;


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
        private Dictionary<int, IElement> allElements = new Dictionary<int, IElement>();
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
            //var nodes = new List<Node>();
            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки узлов"));
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == "******* E L E M E N T S *************")
                {
                    //mapIndexToNode = nodes.ToDictionary(n => n.Number, n => n);
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
                modelData.ObjectData.NodesSet.Add(node.Number, node);
            }
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
        /// Reads an element's block from the reader until it encounters to 'ELSET' marker. Then passes control to the ReadGroups method
        /// </summary>
        private void ReadElements(string startLine, StreamReader reader)
        {
            var elements = new List<Element>();
            var rnd = new Random();
            int elementIndex = 0;
            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки элементов"));
            var elementType = GetElementKind(startLine);

            var elementName = startLine.Replace(" ", "").Split(',')[2].Replace("ELSET=", "");

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
                {
                    var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    CreateSet(modelData, elementName, allElements[elementIndex].ObjType, color);
                    allElements.Clear();
                    elementName = line.Replace(" ", "").Split(',')[2].Replace("ELSET=", "");
                    elementType = GetElementKind(line);
                }

                else
                {
                    var tokens = line.Replace(" ", "").Split(',');
                    elementIndex = int.Parse(tokens[0]);
                    var elementNodes = tokens.Skip(1)
                        .Select(int.Parse)
                        .Select(i => modelData.ObjectData.NodesSet[i])
                        .ToArray();
                    var element = GetElement(elementIndex, elementNodes, elementType);
                    allElements.Add(elementIndex, element);
                }

            }
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
        private void CreateSet(ModelData model, string setName, ObjType objsType, Color color)
        {
            if (objsType == ObjType.Элемент1D)
            {
                model.ObjectData.E1DCollection.Add(setName);
                var set1D = model.ObjectData.E1DCollection[setName];
                FillSet_v2(set1D);
                set1D.SetViewMode(ViewMode.Line);
                set1D.SetColor(color);
            }

            else if (objsType == ObjType.Элемент2D)
            {
                model.ObjectData.E2DCollection.Add(setName);
                var set2D = model.ObjectData.E2DCollection[setName];
                FillSet_v2(set2D);
                set2D.SetViewMode(ViewMode.LineSurface);
                set2D.SetColor(color);
            }

            else
            {
                model.ObjectData.E3DCollection.Add(setName);
                var set3D = model.ObjectData.E3DCollection[setName];
                FillSet_v2(set3D);
                set3D.SetViewMode(ViewMode.LineSurface);
                set3D.SetColor(color);
            }
        }
        private void FillSet_v2<T>(ObjectsSet<T> set)
    where T : IElement
        {
            foreach (var element in allElements)
            {
                var obj = element.Value;
                set.Add(element.Key, (T)obj);
            }
        }
    }
}