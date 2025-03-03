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
        private Dictionary<int, IElement> mapIndexToElement = new Dictionary<int, IElement>();
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
            int elementIndex = 0;
            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки элементов"));
            var elementType = GetElementKind(startLine);
            var elementName = startLine.Replace(" ", "").Split(',')[2].Replace("ELSET=", "");

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line.StartsWith("*ELSET"))
                {
                    CreateSet(elementName, mapIndexToElement[elementIndex].ObjType);
                    mapIndexToElement.Clear();
                    ReadGroups(line, reader);
                }
                else if (line.StartsWith("*ELEMENT"))
                {
                    CreateSet(elementName, mapIndexToElement[elementIndex].ObjType);
                    mapIndexToElement.Clear();
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
                    mapIndexToElement.Add(elementIndex, element);
                    allElements.Add(elementIndex, element);
                }
            }
        }

        private void AddGroupToModelData(string groupName, ObjType objsType, IEnumerable<int> numbers)
        {
            var group = new GroupsData.Group(groupName, objsType);

            foreach (var number in numbers)
            {
                if (objsType == ObjType.Узел)
                {
                    var obj = modelData.ObjectData.NodesSet[number];
                    group.Add(obj);
                }
                else
                {
                    var obj = allElements[number];
                    group.Add(obj);
                }
            }
            modelData.GroupData.Add(group);
        }

        /// <summary>
        /// Reads group's block from the reader until it encounters 'NSET' marker or end of stream
        /// </summary>
        private void ReadGroups(string currentLine, StreamReader reader)
        {
            var groupName = currentLine.Split(',')[1].Replace("ELSET=", "");

            var line = reader.ReadLine();
            var tokens = line.Replace(", ", " ").Trim().Split(' ');
            var numbers = tokens.Select(int.Parse);
            var objsType = allElements[numbers.First()].ObjType;
            SaveElements(numbers);

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                if (line.StartsWith("*NSET"))
                {
                    if (mapIndexToElement.Count != 0)
                    {
                        CreateSet(groupName, objsType);
                        mapIndexToElement.Clear();
                    }
                    else AddGroupToModelData(groupName, objsType, numbers);
                    groupName = line.Split(',')[1].Replace("NSET=", "");
                    objsType = ObjType.Узел;
                }
                else if (line.StartsWith("*ELSET"))
                {
                    CreateSet(groupName, objsType);
                    groupName = line.Split(',')[1].Replace("ELSET=", "");
                    mapIndexToElement.Clear();
                }
                else
                {
                    tokens = line.Replace(", ", " ").Trim().Split(' ');
                    numbers = tokens.Select(int.Parse);
                    if (objsType != ObjType.Узел)
                    {
                        objsType = allElements[numbers.First()].ObjType;
                        SaveElements(numbers);
                    }
                }
            }
            AddGroupToModelData(groupName, objsType, numbers);
        }

        private void SaveElements(IEnumerable<int> numbers)
        {
            mapIndexToElement.Clear();
            foreach (int number in numbers)
            {
                mapIndexToElement.Add(number, allElements[number]);
            }
        }

        private void CreateSet(string setName, ObjType objsType)
        {           
            if (objsType == ObjType.Элемент1D)
            {
                modelData.ObjectData.E1DCollection.Add(setName);
                var set1D = modelData.ObjectData.E1DCollection[setName];
                FillSet_v2(set1D, ViewMode.Line);              
            }

            else if (objsType == ObjType.Элемент2D)
            {
                modelData.ObjectData.E2DCollection.Add(setName);
                var set2D = modelData.ObjectData.E2DCollection[setName];
                FillSet_v2(set2D, ViewMode.LineSurface);                
            }

            else
            {
                modelData.ObjectData.E3DCollection.Add(setName);
                var set3D = modelData.ObjectData.E3DCollection[setName];
                FillSet_v2(set3D, ViewMode.LineSurface);               
            }
        }
        private void FillSet_v2<T>(ObjectsSet<T> set, ViewMode mode)
    where T : IElement
        {
            var rnd = new Random();
            var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));

            foreach (var element in mapIndexToElement)
            {
                var obj = element.Value;
                set.Add(element.Key, (T)obj);
            }            
            set.SetViewMode(mode);
            set.SetColor(color);
        }
    }
}