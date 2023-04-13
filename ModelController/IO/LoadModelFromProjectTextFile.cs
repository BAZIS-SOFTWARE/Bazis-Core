using Model;
using Model.GroupsData;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Geometry;

namespace ModelController.IO
{
    public class LoadModelFromProjectTextFile
    {
        public event Action<object, LoaderEventArgs> LoadEvent;

        public ModelData Load(string path)
        {
            var sr = new StreamReader(path);

            var newModel = new ModelData();
            var nodes = new List<Node>();
            var line = string.Empty;

            while (!sr.EndOfStream)
            {               

                if (line == "Узлы")
                {
                    LoadEvent(this, new LoaderEventArgs("Загрузка узлов..."));
                    Thread.Sleep(100);
                    LoadNodes(sr, newModel, nodes);
                }
                if (line == "Элементы 3004" | line == "Элементы 2003" |
                    line == "Элементы3D" | line == "Элементы2D" | 
                    line == "Элементы1D")
                {
                    LoadEvent(this, new LoaderEventArgs("Загрузка элементов..."));
                    Thread.Sleep(100);
                    LoadElement(sr, line, newModel);
                }

                if (line == "Группы узлов и элементов")
                {
                    LoadEvent(this, new LoaderEventArgs("Загрузка групп..."));
                    Thread.Sleep(100);
                    LoadGroup(sr, newModel);
                }
                line = sr.ReadLine();
            }

            LoadEvent(this, new LoaderEventArgs("Модель загружена"));
            Thread.Sleep(100);
            sr.Dispose();
            return newModel;
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
            newMesh.ObjectData.AddRange(nodes);

        }

        private void LoadElement(StreamReader sr, string elType, ModelData newMesh)
        {
            var line = String.Empty;
            line = sr.ReadLine();

            var nodes = newMesh.ObjectData.FindMany<Node>().ToArray();
            //var number = newMesh.GetLastObjNumber() + 1;
            while (line != "#" + elType)
            {                
                int val = 0;
                var ar = line.Split(' ')
        .Where(m => int.TryParse(m, out val))
        .Select(m => int.Parse(m)).ToList();

                var elNumber = ar[0];
                ar.RemoveAt(0);

                var nodeList = new List<Node>();
                foreach (var nodeNumber in ar)
                    nodeList.Add((Node)newMesh.ObjectData.Find(nodeNumber));

                if (elType == "Элементы3D" | elType == "Элементы 3004")
                {
                    ObjKind objKind;
                    if (ar.Count == 4)
                        objKind = ObjKind.Tetra;
                    else objKind = ObjKind.Hexa;

                    var el3DObj = new Element3D(elNumber, nodeList.ToArray(), objKind);
                    newMesh.ObjectData.Add(el3DObj);
                }

                if (elType == "Элементы2D" | elType == "Элементы 2003")
                {
                    ObjKind objKind;
                    if (ar.Count == 3)
                        objKind = ObjKind.Triangle;
                    else objKind = ObjKind.Quad;

                    var el2DObj = new Element2D(elNumber, nodeList.ToArray(), objKind);
                    newMesh.ObjectData.Add(el2DObj);
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
                    var nodeGroup = new Group(name, "Узлы");
                    var indexLine = splitLine[1].Remove(0, name.Length + 2);
                    var index = indexLine.Split(' ').Select(x => int.Parse(x)).ToList();

                    nodeGroup.AddRange(index);
                    newMesh.GroupData.Add(nodeGroup);
                }
                if (splitLine[0] == "Группа элементов 3D ")
                {
                    LoadElemGroup(newMesh, ObjKind.Tetra, line);
                }
                if (splitLine[0] == "Группа элементов 2D ")
                {
                    LoadElemGroup(newMesh, ObjKind.Triangle, line);
                }
                line = sr.ReadLine();
            }
        }

        private static void LoadElemGroup(ModelData newMesh, ObjKind objType, string line)
        {
            var splitLine = line.Split(':');
            var name = splitLine[1].Split(' ')[1];
            var newGroup = new Group(name, objType);
            var indexLine = splitLine[1].Remove(0, name.Length + 2);
            if (indexLine != "")
            {
                //var startInd = GetElStartInd(newMesh, objType, version);
                var indexes = indexLine.Split(' ').Select(x => int.Parse(x)).ToList();
                indexes.Sort();
                newGroup.AddRange(indexes);
                newMesh.GroupData.Add(newGroup);
            }
        }
    }
}
