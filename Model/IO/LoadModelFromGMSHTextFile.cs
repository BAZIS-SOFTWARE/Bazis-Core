using System;
using System.IO;
using System.Linq;
using Geometry;
using ModelInterfaces;
using System.Globalization;
using System.Collections.Generic;
using Model.GroupsData;
using Model.Events;
using ModelInterfaces.ObjectsFinders;
using Model.MeshObjects;

namespace Model.IO
{
    public class LoadModelFromGMSHTextFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;

        private ModelData newMesh;
        private Dictionary<int, int> dict;
        private List<Node> nodes;
        private StreamReader reader;
        private string[] nextElementFlags;

        public LoadModelFromGMSHTextFile()
        {
            dict = new Dictionary<int, int>();
            newMesh = new ModelData();
            nodes = new List<Node>();
            nextElementFlags = new string[3] { "*ELEMENT", "*ELSET", "*NSET" };
        }

        public IModelData Load(string path)
        {
            using (reader = new StreamReader(path))
            {
                LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки модели"));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("*NODE"))
                    {
                        line = reader.ReadLine();
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки узлов"));
                        while (line != "******* E L E M E N T S *************")
                        {
                            var tokens = line.Replace(" ", "")
                                .Split(',');
                            var nodeNumber = int.Parse(tokens[0]) - 1;
                            var coordinates = tokens.Skip(1)
                                .Select(x => float.Parse(x, NumberStyles.Float, CultureInfo.InvariantCulture))
                                .ToArray();
                            var node = new Node(nodeNumber, new Point3D(coordinates[0], coordinates[1], coordinates[2]));
                            nodes.Add(node);
                            line = reader.ReadLine();
                        }
                        var orderedNodes = nodes.OrderBy(x => x.Number);
                        newMesh.ObjectData.Add(ObjType.Узел, orderedNodes);
                    }
                    if (line.Contains("type=T3D2"))
                    {
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки элементов"));
                        line = reader.ReadLine();
                        CreateElements("type=T3D2", line, "*ELSET");
                    }
                }
            }
            return newMesh;
        }

        private ModelObject BuildElement(string elementKind, int[] selectedNodesId, int currentNumber)
        {
            var nodeArr = new Node[selectedNodesId.Length];
            for (int i = 0; i < nodeArr.Length; i++)
            {
                nodeArr[i] = nodes.Find(selectedNodesId[i]);
            }
            switch(elementKind)
            {
                case "type=T3D2": return new Beam(currentNumber, nodeArr);
                case "type=CPS3": return new Triangle(currentNumber, nodeArr);
                case "type=CPS4": return new Quad(currentNumber, nodeArr);
                case "type=C3D4": return new Tetra(currentNumber, nodeArr);
                default: throw new Exception($"Unknown element type with {selectedNodesId.Length} nodes");
            };
        }

        private static ObjType GetObjType(string elementKind)
        {
            switch (elementKind)
            {
                case "type=T3D2": return ObjType.Элемент1D;
                case "type=CPS3": return ObjType.Элемент2D;
                case "type=CPS4": return ObjType.Элемент2D;
                case "type=C3D4": return ObjType.Элемент3D;
                default: throw new Exception($"Unknown obj type {elementKind}");
            };
        }

        private void CreateElements(string elementKind, string line, string recBase)
        {
            var tokens = line.Replace(" ", "").Split(',');
            var hasStopWord = nextElementFlags.Contains(tokens[0]);
            while (line != null && !hasStopWord)
            {
                var objType = GetObjType(elementKind);
                var lastNumber = newMesh.ObjectData.GetLastObjNumber(ObjType.Элемент);

                var currentNumber = lastNumber + 1;

                dict.Add(int.Parse(tokens[0]), currentNumber);
                var selectedNodesId = tokens.Skip(1)
                    .Select(m => int.Parse(m) - 1)
                    .ToArray();
                var element = BuildElement(elementKind, selectedNodesId, currentNumber);
                newMesh.ObjectData.Add(element.ObjType, element);
                line = reader.ReadLine();
                if (line != null)
                {
                    tokens = line.Replace(" ", "").Split(',');
                    hasStopWord = nextElementFlags.Contains(tokens[0]);
                }   
            }
            if (line != null && tokens[0] != recBase)
            {
                tokens = line.Replace(" ", "").Split(',');
                elementKind = tokens[1];
                line = reader.ReadLine();
                CreateElements(elementKind, line, recBase);
            }
            if (line != null && line.Contains("*ELSET"))
                CreateGroups(elementKind,line, dict);
        }

        private void CreateGroups(string elementKind,string line, Dictionary<int, int> dic)
        {
            var groupName = line.Split('=').Last();
            var strAr = line.Split(',');
            while (!reader.EndOfStream || line.Contains("*NSET"))
            {
                line = reader.ReadLine();
                strAr = line.Replace(" ", "").Split(',');
                if (strAr[0] == "")
                    break;
                var firstNumber = int.Parse(strAr.First());
                var objNumber = dic[firstNumber];
                //var objsType = newMesh.ObjectData.FindMany(elementType).ToArray();
                //var objsType = ObjectsFinder.Find(objsType,objNumber).ObjType;
                LoadEvent(this, new LoaderEventArgs($"Загрузка группы объектов {groupName}"));

                var objType = GetObjType(elementKind);
                var group = new Group(groupName, objType);

                var objs = newMesh.ObjectData.GetObjects(objType).ToList();

                while (line != null && !line.Contains("*NSET") && !line.Contains("*ELSET"))
                {
                    strAr = line.Replace(" ", "").Split(',');
                    var objsNumbers = strAr.Where(x => !string.IsNullOrEmpty(x)).Select(y =>
                    dic[int.Parse(y)]);
                    foreach (var number in objsNumbers)
                    {
                        var obj = objs.Find(number);
                        group.Add(obj);
                    }

                    line = reader.ReadLine();
                }
                newMesh.GroupData.Add(group);
                if (line != null)
                    groupName = line.Split('=').Last();
                if (line.Contains("*NSET"))
                    break;
            }
        }
    }
}

