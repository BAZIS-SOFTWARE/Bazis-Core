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
using System.Reflection;
using System.Data;


namespace Model.IO
{
    /// <summary>
    /// Loads ModelData from file in .inp (gmsh) format
    /// </summary>
    public class LoadModelFromINPTextFile_v2 : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;        
        private Dictionary<int, IElement> allElements = new Dictionary<int, IElement>();
        private ModelData modelData;
        private string line;

        /// <summary>
        /// Load a model in the .inp format from the specified path
        /// </summary>
        public IModelData Load(string path)
        {
            modelData = new ModelData();
            using (var reader = new StreamReader(path))
            {
                LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки модели"));
                line = reader.ReadLine();
                while (!reader.EndOfStream)
                {                                      
                    if (line.Contains("*NODE"))
                    {
                        foreach (var item in ReadNodes(reader))
                            modelData.ObjectData.NodesSet.Add(item.Number, item);
                    }
                    else if (line.Contains("*NSET"))
                    {
                        ReadGroups(line, reader);
                    }
                    else line = reader.ReadLine();
                }
            }
            return modelData;
        }

        /// <summary>
        /// Reads a block of nodes using the reader until it encounters element's block marker.
        /// </summary>
        private IEnumerable<Node> ReadNodes(StreamReader reader)
        {
            LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка узлов..."));

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                if (line == "******* E L E M E N T S *************")
                {
                    ReadElements(reader);
                    yield break;
                }
                var tokens = line.Replace(" ", "").Split(',');
                var nodeIndex = int.Parse(tokens[0]);
                var coordinates = tokens.Skip(1)
                    .Select(x => float.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture))
                    .ToArray();
                var node = new Node(nodeIndex, new Point3D(coordinates[0], coordinates[1], coordinates[2]));
                yield return node;
            }
        }

        /// <summary>
        /// Reads an element's block from the reader until it encounters to 'NSET' marker. Then passes control to the ReadGroups method
        /// </summary>
        private void ReadElements(StreamReader reader)
        {
            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки элементов"));
            line = reader.ReadLine();
            var rnd = new Random();
            SetParametersElementOrElset(reader, rnd);
        }

        private void SetParametersElementOrElset(StreamReader reader, Random rnd)
        {
            string setName = "";
            ElementKind elementType = ElementKind.Стержень;
            var objsType = ObjType.Элемент1D;

            if (line.StartsWith("*ELSET"))
            {
                setName = GetName(line);
                line = reader.ReadLine();
                var elements = line.Replace(", ", " ").Trim().Split(' ');
                var elementIndex = int.Parse(elements[0]);
                elementType = allElements[elementIndex].ElementKind;
                objsType = allElements[elementIndex].ObjType;
            }
            else
            {
                elementType = GetElementKind(line);
                setName = GetName(line);
                objsType = GetType(elementType);
                line = reader.ReadLine();
            }
            CreateSet(reader, rnd, setName, elementType, objsType);
        }

        private void CreateSet(StreamReader reader, Random rnd, string setName, ElementKind elementType, ObjType objsType)
        {
            var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));

            if (objsType == ObjType.Элемент1D)
            {
                modelData.ObjectData.E1DCollection.Add(setName);
                var set1D = modelData.ObjectData.E1DCollection[setName];
                FillSet(set1D, ViewMode.Line, color, reader, elementType, rnd);
            }

            else if (objsType == ObjType.Элемент2D)
            {
                modelData.ObjectData.E2DCollection.Add(setName);
                var set2D = modelData.ObjectData.E2DCollection[setName];
                FillSet(set2D, ViewMode.LineSurface, color, reader, elementType, rnd);
            }
            else
            {
                modelData.ObjectData.E3DCollection.Add(setName);
                var set3D = modelData.ObjectData.E3DCollection[setName];
                FillSet(set3D, ViewMode.LineSurface, color, reader, elementType, rnd);
            }
        }
        private string GetName(string line)
        {
            if (line.StartsWith("*ELEMENT"))
            {
                return line.Replace(" ", "").Split(',')[2].Replace("ELSET=", "");
            }
            else if (line.StartsWith("*ELSET"))
            {
                return line.Replace(" ", "").Split(',')[1].Replace("ELSET=", "");
            }
            else
            {
                return line.Replace(" ", "").Split(',')[1].Replace("NSET=", "");
            }
        }
        private void FillSet<T>(ObjectsSet<T> set, ViewMode mode, Color color, StreamReader reader, ElementKind elementKind, Random rnd)
    where T : IElement
        {
            set.SetViewMode(mode);
            set.SetColor(color);
            while (!reader.EndOfStream)
            {
                if (line.StartsWith("*ELEMENT") || line.StartsWith("*ELSET"))
                {
                    SetParametersElementOrElset(reader, rnd);
                    break;
                }
                else if (line.StartsWith("*NSET"))
                {
                    break;
                }
                var elements = line.Trim().Split(',');
                if (elements.Contains(""))
                {
                    var tokens = line.Replace(", ", " ").Trim().Split(' ');
                    var numbers = tokens.Select(int.Parse);
                    foreach (var number in numbers)
                        set.Add(number, (T)allElements[number]);
                }
                else
                {
                    var tokens = line.Replace(" ", "").Split(',');
                    var elementIndex = int.Parse(tokens[0]);
                    var elementNodes = tokens.Skip(1)
                        .Select(int.Parse)
                        .Select(i => modelData.ObjectData.NodesSet[i])
                        .ToArray();
                    var element = GetElement(elementIndex, elementNodes, elementKind);
                    allElements.Add(elementIndex, element);
                    set.Add(elementIndex, (T)allElements[elementIndex]);
                }
                line = reader.ReadLine();
            }
        }
        private ObjType GetType(ElementKind elementKind)
        {
            switch (elementKind)
            {
                case ElementKind.Стержень: return ObjType.Элемент1D;
                case ElementKind.Треугольник: return ObjType.Элемент2D;
                case ElementKind.Квадрат: return ObjType.Элемент2D;
                case ElementKind.Тетра: return ObjType.Элемент3D;
                default: throw new ArgumentException($"Неизвестный формат элемента: {elementKind}");
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
                //case "": return; 
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
        /// Reads group's block from the reader until it encounters 'NSET' marker or end of stream
        /// </summary>
        private void ReadGroups(string currentLine, StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var groupName = GetName(line);
                line = reader.ReadLine();
                var tokens = line.Replace(", ", " ").Trim().Split(' ');
                var numbers = tokens.Select(int.Parse);
                AddGroupToModelData(groupName, numbers);
                line = reader.ReadLine();
            }
        }

        private void AddGroupToModelData(string groupName, IEnumerable<int> numbers)
        {
            var group = new GroupsData.Group(groupName, ObjType.Узел);

            foreach (var number in numbers)
            {
                var obj = modelData.ObjectData.NodesSet[number];
                group.Add(obj);
            }
            modelData.GroupData.Add(group);            
        }
    }
}