using Model.GroupsData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Geometry;
using Model.Events;
using ModelInterfaces;
using ModelInterfaces.ObjectsFinders;
using Model.MeshObjects;
using ModelInterfaces.MeshObjects;

namespace Model.IO
{
    /// <summary>
    /// LoadModelFromProjectTextFile
    /// </summary>
    public class LoadModelFromBPFTextFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;

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
                    if (line == "Элементы 3004" | line == "Элементы 2003" |
                        line == "Элементы3D" | line == "Элементы2D" |
                        line == "Элементы1D" | line == "Элемент3D" | line == "Элемент2D" |
                        line == "Элемент1D")
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

        private static void LoadNodes(StreamReader sr, ModelData newMesh, List<Node> nodes)
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
            while (line != "#" + elType)
            {                
                int val = 0;
                var ar = line.Split(' ')
        .Where(m => int.TryParse(m, out val))
        .Select(m => int.Parse(m)).ToList();

                var elNumber = ar[0];
                ar.RemoveAt(0);

                var nodeList = new List<INode>();
              
                foreach (var nodeNumber in ar)
                    nodeList.Add(newMesh.ObjectData.NodeCollection.Find(nodeNumber));

                if (elType == "Элементы3D" | elType == "Элемент3D" | elType == "Элементы 3004")
                {

                    if (ar.Count == 4)
                    {
                        var el3DObj = new Tetra(elNumber, nodeList.ToArray());
                        newMesh.ObjectData.E3DCollection.Add(el3DObj);
                    }
                    else
                    {
                        var el3DObj = new Hexa(elNumber, nodeList.ToArray());
                        newMesh.ObjectData.E3DCollection.Add(el3DObj);
                    }

  
                }

                if (elType == "Элементы2D" | elType == "Элемент2D" | elType == "Элементы 2003")
                {

                    if (ar.Count == 3)
                    {
                        var el2DObj = new Triangle(elNumber, nodeList.ToArray());
                        newMesh.ObjectData.E2DCollection.Add(el2DObj);
                    }
                    else
                    {
                        var el2DObj = new Quad(elNumber, nodeList.ToArray());
                        newMesh.ObjectData.E2DCollection.Add(el2DObj);
                    }


                }
                if(elType == "Элементы1D" | elType == "Элемент1D" | elType == "Элементы 1002")
                {
                        var el2DObj = new Beam(elNumber, nodeList.ToArray());
                        newMesh.ObjectData.E1DCollection.Add(el2DObj);                  
                }
                //number++;
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
                    var nodeGroup = new Group(name,ObjType.Узел);
                    var indexLine = splitLine[1].Remove(0, name.Length + 2);
                    var indexes = indexLine.Split(' ').Select(x => int.Parse(x)).ToList();

                    foreach (var index in indexes)
                    {
                        var obj = newMesh.ObjectData.Find(ObjType.Узел,index);
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
                    var obj = ByNumberFinder.Find(elements,index);
                    newGroup.Add(obj);
                }

                newMesh.GroupData.Add(newGroup);
            }
        }
    }
}
