using Geometry;
using Model.Events;
using Model.GroupsData;
using Model.MeshObjects;
using Model.Interfaces;
using Model.Interfaces.ObjectsFinders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Model.ObjectsCollections;

namespace Model.IO
{
    /// <summary>
    /// LoadModelFromProjectTextFile
    /// </summary>
    public class LoadModelFromBPF2TextFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;

        public IModelData Load(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var newModel = new ModelData();

                string[] token = new string[1];
                while (!sr.EndOfStream)
                {
                    if (token[0] == "Узлы")
                    {
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка узлов..."));
                        Thread.Sleep(100);
                        LoadNodes(sr, newModel);
                    }
                    if (token[0] == "Элементы 3004" || token[0] == "Элементы 2003" ||
                        token[0] == "Элементы3D" || token[0] == "Элементы2D" ||
                        token[0] == "Элементы1D" || token[0] == "Элемент3D" || token[0] == "Элемент2D" ||
                        token[0] == "Элемент1D")
                    {
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка элементов..."));
                        Thread.Sleep(100);
                        LoadElement(sr, token, newModel);
                    }

                    if (token[0] == "Группы узлов и элементов")
                    {
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка групп..."));
                        Thread.Sleep(100);
                        LoadGroup(sr, newModel);
                    }
                    token = sr.ReadLine().Split(' ');
                }

                LoadEvent?.Invoke(this, new LoaderEventArgs("Модель загружена"));
                Thread.Sleep(100);
                return newModel;
            }


        }

        private static void LoadNodes(StreamReader reader, ModelData newMesh)
        {
            var line = reader.ReadLine();
            //var nodes = new List<Node>();
            while (line != "#Узлы")
            {
                var tokens = line.Split()
                    .Where(m => float.TryParse(m, NumberStyles.Float, CultureInfo.InvariantCulture, out var val))
                    .Select(m => float.Parse(m, CultureInfo.InvariantCulture));
                if (tokens.Count() != 0)
                {
                    var nodeIndex = (int)tokens.First();
                    var coordinates = tokens.Skip(1).ToArray();
                    var position = new Point3D(coordinates[0], coordinates[1], coordinates[2]);
                    var node = new Node(nodeIndex, position);
                    newMesh.ObjectData.NodesSet.Add(node.Number,node);
                }
                line = reader.ReadLine();
            }
            //newMesh.ObjectData.NodeCollection.AddRange(ObjType.Узел.ToString(), nodes.OrderBy(n => n.Number));
        }

        private void LoadElement(StreamReader reader, string[] token, ModelData newMesh)
        {
            var setName = token[1];
            var elType = token[0];

            var line = reader.ReadLine();

            while (line != $"#{elType}")
            {
                var tokens = line.Split()
.Where(m => int.TryParse(m, out var val))
.Select(int.Parse);
                var elementNumber = tokens.First();
                var level = tokens.Skip(1).First();
                var nodes = GetNodes(newMesh, tokens.Skip(2)).ToArray();
                if (elType == "Элементы3D" | elType == "Элемент3D" | elType == "Элементы 3004")
                {
                    var element = GetElement3D(nodes, elementNumber, level);
                    newMesh.ObjectData.E3DCollection.Add(setName, element);
                }
                if (elType == "Элементы2D" | elType == "Элемент2D" | elType == "Элементы 2003")
                {
                    var element = GetElement2D(nodes, elementNumber, level);
                    newMesh.ObjectData.E2DCollection.Add(setName, element);
                }
                if (elType == "Элементы1D" | elType == "Элемент1D" | elType == "Элементы 1002")
                {
                    var el2DObj = new Beam(elementNumber, nodes.ToArray())
                    { Level = level };
                    newMesh.ObjectData.E1DCollection.Add(setName, el2DObj);
                }
                line = reader.ReadLine();
            }
        }

        private IEnumerable<Node> GetNodes(ModelData modelData, IEnumerable<int> indeces)
        {
            foreach (var nodeNumber in indeces)
            {
                yield return modelData.ObjectData.NodesSet[nodeNumber];
            }
        }

        private Element3D GetElement3D(Node[] nodes, int elementNumber, int level)
        {
            switch (nodes.Length)
            {
                case 4: return new Tetra(elementNumber, nodes) { Level = level };
                case 6: return new Penta(elementNumber, nodes) { Level = level };
                case 8: return new Hexa(elementNumber, nodes) { Level = level };
                default: throw new ArgumentException($"Undefined element {elementNumber}");
            }
        }

        private Element2D GetElement2D(Node[] nodes, int elementNumber, int level)
        {
            switch (nodes.Length)
            {
                case 3: return new Triangle(elementNumber, nodes) { Level = level};
                case 4: return new Quad(elementNumber, nodes) { Level = level };
                default: throw new ArgumentException($"Undefined element {elementNumber}");
            }
        }

        private static void LoadGroup(StreamReader sr, ModelData newMesh)
        {
            var line = sr.ReadLine();
            while (line != "#Группы узлов и элементов")
            {
                var splitLine = line.Split(':');
                if (splitLine[0] == "Группа узлов ")
                {
                    var name = splitLine[1].Split(' ')[1];
                    var nodeGroup = new Group(name, ObjType.Узел);
                    var indexLine = splitLine[1].Remove(0, name.Length + 2);
                    var indexes = indexLine.Split(' ').Select(x => int.Parse(x)).ToList();

                    foreach (var index in indexes)
                    {
                        var obj = newMesh.ObjectData.Find(ObjType.Узел, index);
                        nodeGroup.Add(obj);
                    }
                    newMesh.GroupData.Add(nodeGroup);
                }
                if (splitLine[0] == "Группа элементов 3D ")
                {
                    LoadElemGroup(newMesh, ObjType.Элемент3D, line);
                }
                if (splitLine[0] == "Группа элементов 2D ")
                {
                    LoadElemGroup(newMesh, ObjType.Элемент2D, line);
                }
                if (splitLine[0] == "Группа элементов 1D ")
                {
                    LoadElemGroup(newMesh, ObjType.Элемент1D, line);
                }
                line = sr.ReadLine();
            }
        }

        private static void LoadElemGroup(ModelData newMesh, ObjType objType, string line)
        {
            var splitLine = line.Split(':');
            var name = splitLine[1].Split(' ')[1];
            var newGroup = new Group(name, objType);
            var indexLine = splitLine[1].Remove(0, name.Length + 2);
            if (indexLine != "")
            {
                var elements = newMesh.ObjectData.GetObjects(objType).ToList();
                var indexes = indexLine.Split(' ').Select(x => int.Parse(x)).ToList();
                indexes.Sort();

                foreach (var index in indexes)
                {
                    var obj = ByNumberFinder.Find(elements, index);
                    newGroup.Add(obj);
                }

                newMesh.GroupData.Add(newGroup);
            }
        }
    }
}
