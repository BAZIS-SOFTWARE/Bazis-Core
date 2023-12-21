using Geometry;
using Model.MeshObjects;
using Model.Events;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Model.IO
{
    public class LoadModelFromASCIITextFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;
        private Dictionary<int, Node> allNodes = new Dictionary<int, Node>();
        private Dictionary<int, Element> allElements = new Dictionary<int, Element>();

        public IModelData Load(string path)
        {
            ModelData model = new ModelData();
            using (var reader = new StreamReader(path))
            {
                while(!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var tokens = line.Split().ToArray();
                    if(line.Contains("BEGIN_NODES"))
                    {
                        foreach (var item in ReadNodes(reader, int.Parse(tokens[1])))
                            model.ObjectData.Add(ObjType.Узел, item);                   
                    }
                    if(line.Contains("BEGIN_ELEMENTS"))
                    {
                        foreach (var item in ReadElements(reader, int.Parse(tokens[1]),model))
                            model.ObjectData.Add(item.ObjType, item);
                    }
                    if(line.Contains("BEGIN_GROUPS"))
                    {
                        ReadGroup(reader, int.Parse(tokens[1]), model);
                    }
                }
            }
            allNodes.Clear();
            allElements.Clear();
            return model;
        }

        private IEnumerable<INode> ReadNodes(StreamReader reader, int count)
        {
            LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка узлов..."));
            for (var i = 0; i < count; i++)
            {
                var line = reader.ReadLine();
                var tokens = line.Split().ToArray();
                var index = int.Parse(tokens[0]);
                var coordinates = tokens.Skip(6)
                    .Select(x => float.Parse(x, CultureInfo.InvariantCulture))
                    .ToArray();
                var node = new Node(i, new Point3D(coordinates[0], coordinates[1], coordinates[2]));
                allNodes.Add(index, node); // all nodes must have unic numbers started from zero
                yield return node;
            }
        }

        private IEnumerable<ModelObject> ReadElements(StreamReader reader, int count, ModelData model)
        {
            LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка элементов..."));
            for (var i = 0; i < count; i++)
            {
                var line = reader.ReadLine();
                var tokens = line.Split().ToArray();
                var elCurrentNumber = int.Parse(tokens[0]);
                var type = int.Parse(tokens[1]);
                var nodes = tokens.Skip(5)
                    .Select(int.Parse)
                    .Select(j => allNodes[j])
                    .ToArray();

                var elLastNumber = model.ObjectData.GetLastObjNumber(ObjType.Элемент) + 1;
                var element = GetElementByType(nodes, type, elLastNumber);
                allElements.Add(elCurrentNumber, element);
                yield return element;
            }
        }

        private Element GetElementByType(Node[] nodes, int type,int elNumber)
        {
            switch (type) 
            {
                case 3004: return new Tetra(elNumber, nodes);
                case 2003: return new Triangle(elNumber, nodes);
                case 1002: return new Beam(elNumber, nodes);
                default: throw new ArgumentException($"Unable to recognize element of type {type}");
            }
        }

        private void ReadGroup(StreamReader reader, int count, ModelData model)
        {
            LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка группы объектов "));
            string[] tokens;
            for (var i = 0; i < count; i++)
            {
                var line = reader.ReadLine();
                tokens = ReplaceQuotes(line)
                    .Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();
                var groupName = tokens[1];
                var numbers = tokens.Skip(7).Select(int.Parse);

                ObjType objsType;
                if (tokens[2] == "2")
                    objsType = allElements[numbers.First()].ObjType;
                else objsType = ObjType.Узел;

                var group = new GroupsData.Group(groupName, objsType);
                foreach (var number in numbers)
                {
                    var obj = allElements[number];
                    group.Add(obj);
                }
                model.GroupData.Add(group);
            }

        }

        private string ReplaceQuotes(string source)
        {
            var result = string.Empty;
            var regex = new Regex("\".*?\"");
            var matches = regex.Matches(source);
            foreach (var match in matches)
            {
                result = source.Replace(match.ToString(), "Quotes");
                source = result;
            }
            return result;
        }
    }
}
