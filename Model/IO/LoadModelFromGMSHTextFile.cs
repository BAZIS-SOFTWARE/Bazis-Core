using System;
using System.IO;
using System.Linq;
using Geometry;
using Model.Interfaces;
using System.Globalization;
using System.Collections.Generic;
using Model.GroupsData;
using Model.Events;
using Model.Utilities;

namespace Model.IO
{
    public class LoadModelFromGMSHTextFile : IModelLoader
    {
        public event Action<object, LoaderEventArgs> LoadEvent;

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

        public ModelData Load(string path)
        {
            using (reader = new StreamReader(path))
            {
                LoadEvent(this, new LoaderEventArgs("Начало загрузки модели"));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.Contains("*NODE"))
                    {
                        line = reader.ReadLine();
                        LoadEvent(this, new LoaderEventArgs("Начало загрузки узлов"));
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
                        newMesh.ObjectData.AddRange(orderedNodes);
                    }
                    if (line.Contains("type=T3D2"))
                    {
                        LoadEvent(this, new LoaderEventArgs("Начало загрузки элементов"));
                        line = reader.ReadLine();
                        CreateElements("type=T3D2", line, "*ELSET");
                    }
                }
            }
            return newMesh;
        }

        private ModelObject BuildElement(string elementType, int[] selectedNodesId, int currentNumber)
        {
            var nodeArr = new Node[selectedNodesId.Length];
            for (int i = 0; i < nodeArr.Length; i++)
            {
                nodeArr[i] = ObjectsFinder.Find(nodes, selectedNodesId[i]);
            }
            switch(elementType)
            {
                case "type=T3D2": return new Element1D(currentNumber, nodeArr, ObjKind.Beam);
                case "type=CPS3": return new Element2D(currentNumber, nodeArr, ObjKind.Triangle);
                case "type=CPS4": return new Element2D(currentNumber, nodeArr, ObjKind.Quad);
                case "type=C3D4": return new Element3D(currentNumber, nodeArr, ObjKind.Tetra);
                default: throw new Exception($"Unknown element type with {selectedNodesId.Length} nodes");
            };
        }

        private void CreateElements(string elementType, string line, string recBase)
        {
            var tokens = line.Replace(" ", "").Split(',');
            var hasStopWord = nextElementFlags.Contains(tokens[0]);
            while (line != null && !hasStopWord)
            {
                int currentNumber = newMesh.ObjectData.GetLastObjNumber() + 1;
                dict.Add(int.Parse(tokens[0]), currentNumber);
                var selectedNodesId = tokens.Skip(1)
                    .Select(m => int.Parse(m) - 1)
                    .ToArray();
                var element = BuildElement(elementType, selectedNodesId, currentNumber);
                newMesh.ObjectData.Add(element);
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
                elementType = tokens[1];
                line = reader.ReadLine();
                CreateElements(elementType, line, recBase);
            }
            if (line != null && line.Contains("*ELSET"))
                CreateGroups(elementType,line, dict);
        }

        private void CreateGroups(string elementType,string line, Dictionary<int, int> dic)
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
                var group = new Group(groupName, elementType);
                while (line != null && !line.Contains("*NSET") && !line.Contains("*ELSET"))
                {
                    strAr = line.Replace(" ", "").Split(',');
                    var objsNumbers = strAr.Where(x => !string.IsNullOrEmpty(x)).Select(y =>
                    dic[int.Parse(y)]);
                    foreach (var number in objsNumbers)
                    {
                        var obj = newMesh.ObjectData.Find(number);
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

