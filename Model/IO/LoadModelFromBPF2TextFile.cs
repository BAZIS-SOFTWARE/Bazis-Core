using Geometry;
using Model.Events;
using Model.MeshObjects;
using ModelInterfaces.ObjectsFinders;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Model.GroupsData;
using ModelInterfaces.MeshObjects;

namespace Model.IO
{
    public class LoadModelFromBPF2TextFile : IModelLoader
    {
        /// <inheritdoc/>
        public event Action<object, ILoaderEventArgs> LoadEvent;
/// <inheritdoc/>

        public IModelData Load(string path)
        {
            using (var sr = new StreamReader(path))
            {
                var newModel = new ModelData();
                var nodes = new List<Node>();
                var line = string.Empty;

                while (!sr.EndOfStream)
                {

                    if (line == "Узлы")
                    {
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка узлов..."));
                        Thread.Sleep(100);
                        LoadNodes(sr, newModel, nodes);
                    }
                    if (line == "Элемент1D" | line == "Элемент3D" | 
                        line == "Элемент2D" | line == "Элемент1D")
                    {
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка элементов..."));
                        Thread.Sleep(100);
                        LoadElement(sr, line, newModel);
                    }

                    if (line == "Группы узлов и элементов")
                    {
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка групп..."));
                        Thread.Sleep(100);
                        LoadGroup(sr, newModel);
                    }
                    line = sr.ReadLine();
                }

                LoadEvent?.Invoke(this, new LoaderEventArgs("Модель загружена"));
                Thread.Sleep(100);
                return newModel;
            }


        }

        private void LoadNodes(StreamReader sr, ModelData newMesh, List<Node> nodes)
        {
            var line = string.Empty;
            while (line != "#Узлы")
            {
                line = sr.ReadLine();
                float val = 0;
                var ar = line.Split(' ')
        .Where(m => float.TryParse(m, NumberStyles.Float, CultureInfo.InvariantCulture, out val))
        .Select(m => float.Parse(m, CultureInfo.InvariantCulture))
                                    .ToList();

                if (ar.Count != 0)
                {
                    var nInd = (int)ar[0];
                    ar.RemoveAt(0);
                    var coord = new Point3D(ar[0], ar[1], ar[2]);
                    var node = new Node(nInd, coord);
                    nodes.Add(node);
                }
            }
            newMesh.ObjectData.NodeCollection.AddRange(nodes);

        }

        private void LoadElement(StreamReader sr, string elType, ModelData newMesh)
        {
            var line = String.Empty;
            line = sr.ReadLine();

            //var number = newMesh.GetLastObjNumber() + 1;
            while (line != $"#{elType}")
            {
                int val = 0;
                var ar = line.Split(':');

        var ndNumbers = ar[1].TrimStart(' ').Split(' ').Where(m => int.TryParse(m, out val))
        .Select(m => int.Parse(m)).ToList();

                var elData = ar[0].TrimEnd(' ').Split(' ');

                int elNum;
                if(!int.TryParse(elData[0],out elNum))
                    throw new Exception("Номер элемента задан некорректно!");

                ObjType elType2;
                if(!Enum.TryParse(elData[1], out elType2))
                    throw new Exception("Найден элемент неизвестного типа!");

                ElementKind elKind;
                if (!Enum.TryParse(elData[2], out elKind))
                    throw new Exception("Найден элемент неизвестного вида!");
                
                int elintPnts;
                if (!int.TryParse(elData[3], out elintPnts))
                    throw new Exception("Кол-во точек интегрирования задано некорректно!");

                var nodeList = new List<INode>();
                foreach (var nodeNumber in ndNumbers)
                {
                    var node = newMesh.ObjectData.NodeCollection.Find(nodeNumber);
                    nodeList.Add(node);
                }

                switch (elKind)
                {
                    case ElementKind.Стержень:
                        newMesh.ObjectData.E1DCollection.Add(new Beam(elNum, nodeList.ToArray(), elintPnts));
                        break;
                    case ElementKind.Треугольник:
                        newMesh.ObjectData.E2DCollection.Add(new Triangle(elNum, nodeList.ToArray(), elintPnts));
                        break;
                    case ElementKind.Квадрат:
                        newMesh.ObjectData.E2DCollection.Add(new Quad(elNum, nodeList.ToArray(), elintPnts));
                        break;
                    case ElementKind.Тетра:
                        newMesh.ObjectData.E3DCollection.Add(new Tetra(elNum, nodeList.ToArray(), elintPnts));
                        break;
                    case ElementKind.Гекса:
                        newMesh.ObjectData.E3DCollection.Add(new Hexa(elNum, nodeList.ToArray(), elintPnts));
                        break;
                    default:
                        newMesh.ObjectData.E3DCollection.Add(new Penta(elNum, nodeList.ToArray(), elintPnts));
                        break;
                }
                line = sr.ReadLine();
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
                    var nodeGroup = new Group(name, ObjType.Точка);
                    var indexLine = splitLine[1].Remove(0, name.Length + 2);
                    var indexes = indexLine.Split(' ').Select(x => int.Parse(x)).ToList();

                    foreach (var index in indexes)
                    {
                        var obj = newMesh.ObjectData.Find(ObjType.Точка, index);
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
                var elements = newMesh.ObjectData.GetObjects(ObjType.Элемент).ToList();
                var indexes = indexLine.Split(' ').Select(x => int.Parse(x)).ToList();
                indexes.Sort();

                foreach (var index in indexes)
                {
                    var obj = elements.Find(index);
                    newGroup.Add(obj);
                }

                newMesh.GroupData.Add(newGroup);
            }
        }
    }
}
