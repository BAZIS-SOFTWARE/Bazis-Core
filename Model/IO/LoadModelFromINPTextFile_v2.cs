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
        private Dictionary<int, IElement> allElements = new Dictionary<int, IElement>();
        private ModelData modelData;
        private Random rnd = new Random();
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
                    else if (line.Contains("*ELSET") || line.Contains("*NSET"))
                    {
                        ReadGroups(reader);
                    }
                    else
                        line = reader.ReadLine();
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
            SetParametersElements(reader);
        }

        private void SetParametersElements(StreamReader reader)
        {
            var setName = GetName(line);
            var elementType = GetElementKind(line);
            var objsType = GetType(elementType);

            if (objsType == ObjType.Элемент1D)
            {
                modelData.ObjectData.E1DCollection.Add(setName);
                var set1D = modelData.ObjectData.E1DCollection[setName];
                FillSet(set1D, ViewMode.Line, reader, elementType);
            }

            else if (objsType == ObjType.Элемент2D)
            {
                modelData.ObjectData.E2DCollection.Add(setName);
                var set2D = modelData.ObjectData.E2DCollection[setName];
                FillSet(set2D, ViewMode.LineSurface, reader, elementType);
            }
            else
            {
                modelData.ObjectData.E3DCollection.Add(setName);
                var set3D = modelData.ObjectData.E3DCollection[setName];
                FillSet(set3D, ViewMode.LineSurface, reader, elementType);
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
            else if (line.StartsWith("*NSET"))
            {
                return line.Replace(" ", "").Split(',')[1].Replace("NSET=", "");
            }
            else
            {
                throw new ArgumentException($"Неизвестный формат строки: {line}");
            }
        }

        private void FillSet<T>(ObjectsSet<T> set, ViewMode mode, StreamReader reader, ElementKind elementKind)
    where T : IElement
        {
            var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
            set.SetViewMode(mode);
            set.SetColor(color);

            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                if (line.StartsWith("*ELEMENT"))
                {
                    SetParametersElements(reader);
                    break;
                }
                else if (line.StartsWith("*ELSET") || line.StartsWith("*NSET"))
                {
                    break;
                }
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
        private void ReadGroups(StreamReader reader)
        {
            while (!reader.EndOfStream)
            {
                var previousLine = line;
                var groupName = GetName(line);
                line = reader.ReadLine();
                var tokens = line.Replace(", ", " ").Trim().Split(' ');
                var numbers = tokens.Select(int.Parse);

                ObjType objsType;
                if (previousLine.StartsWith("*ELSET"))
                    objsType = allElements[numbers.First()].ObjType;
                else objsType = ObjType.Узел;

                AddGroupToModelData(groupName, numbers, objsType);
                line = reader.ReadLine();
            }
        }

        private void AddGroupToModelData(string groupName, IEnumerable<int> numbers, ObjType objsType)
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
    }
}