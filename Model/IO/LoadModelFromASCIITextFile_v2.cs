using Geometry;
using Model.MeshObjects;
using Model.Events;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Model.Interfaces.MeshObjects;
using System.Xml.Linq;
using System.Drawing;
using Model.ObjectsCollections;
using Model.Interfaces.ObjectsCollections;
using System.Reflection;

namespace Model.IO
{
    public class LoadModelFromASCIITextFile_v2 : IModelLoader
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
                            model.ObjectData.NodesSet.Add(item.Number, item);                   
                    }
                    if(line.Contains("BEGIN_ELEMENTS"))
                    {
                        ReadElements(reader, int.Parse(tokens[1]), model);
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

        private IEnumerable<Node> ReadNodes(StreamReader reader, int count)
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

        private void ReadElements(StreamReader reader, int count, ModelData model)
        {
            LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка элементов..."));
            var elLastNumber = 0;
            if (model.ObjectData.GetAllElements().Count() > 0)
                elLastNumber = model.ObjectData.GetMaxNumber(ObjType.Элемент);

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

                
                var element = GetElementByType(nodes, model,type, elCurrentNumber);
                allElements.Add(elCurrentNumber, element);
            }
        }

        private Element GetElementByType(Node[] nodes, ModelData model,int type,int elNumber)
        {
            switch (type) 
            {
                case 3004:
                    var tetra = new Tetra(elNumber, nodes);
                    //model.ObjectData.E3DCollection.Add(ObjType.Элемент3D.ToString(),tetra);
                    return tetra;
                case 2003:
                    var triangle = new Triangle(elNumber, nodes);
                    //model.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(),triangle);
                    return triangle;
                case 1002:
                    var beam = new Beam(elNumber, nodes);
                    //model.ObjectData.E1DCollection.Add(ObjType.Элемент1D.ToString(),beam);
                    return beam;
                default: throw new ArgumentException($"Unable to recognize element of type {type}");
            }
        }

        private void ReadGroup(StreamReader reader, int count, ModelData model)
        {
            LoadEvent?.Invoke(this, new LoaderEventArgs("Загрузка группы объектов "));
            string[] tokens;
            var rnd = new Random();

            for (var i = 0; i < count; i++)
            {
                var line = reader.ReadLine();

                var colType = GetCollectionType(line);

                tokens = ReplaceQuotes(line)
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();
                var name = tokens[1];
                var numbers = tokens.Skip(7).Select(int.Parse);

                ObjType objsType;
                if (tokens[2] == "2")
                    objsType = allElements[numbers.First()].ObjType;
                else objsType = ObjType.Узел;

                if (colType == "COLLECTOR_ID")
                    CreateGroup(model, name, numbers, objsType);
                else
                {
                    var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                    CreateSet(model, name, objsType,color, numbers);
                    //set.SetColor(color);
                    //FillSet(set, numbers);
                }

            }
        }

        private void CreateSet(ModelData model, string setName,  ObjType objsType, Color color, IEnumerable<int> numbers)
        {
            if (objsType == ObjType.Элемент1D)
            {
                model.ObjectData.E1DCollection.Add(setName, objsType);
                var set1D = model.ObjectData.E1DCollection[setName];
                set1D.SetViewMode(ViewMode.Line);
                set1D.SetColor(color);
                FillSet_v2(model.ObjectData.E1DCollection[setName], numbers);
            }

            else if (objsType == ObjType.Элемент2D)
            {
                model.ObjectData.E2DCollection.Add(setName, objsType);
                var set2D = model.ObjectData.E2DCollection[setName];
                set2D.SetViewMode(ViewMode.LineSurface);
                set2D.SetColor(color);
                FillSet_v2(model.ObjectData.E2DCollection[setName], numbers);
            }
    
            else
            {
                model.ObjectData.E3DCollection.Add(setName, objsType);
                var set3D = model.ObjectData.E3DCollection[setName];
                set3D.SetViewMode(ViewMode.LineSurface);
                set3D.SetColor(color);
                FillSet_v2(set3D, numbers);
            }
        }

        private void FillSet_v2<T>(ObjectsSet<T> set, IEnumerable<int> numbers)
    where T : Element
        {
            foreach (var number in numbers)
            {
                var obj = allElements[number];
                set.Add(obj.Number, (T)obj);
            }
        }

        //private void FillSet(ModelData model, ObjType objsType, IEnumerable<int> numbers)
        //{
        //    if (objsType == ObjType.Элемент1D)
        //    {
        //        model.ObjectData.E1DCollection.Add(setName, objsType);
        //    }

        //    else if (objsType == ObjType.Элемент2D)
        //    {
        //        model.ObjectData.E2DCollection.Add(setName, objsType);
        //    }

        //    else
        //    {
        //        model.ObjectData.E3DCollection.Add(setName, objsType);
        //    }

        //    foreach (var number in numbers)
        //    {
        //        var obj = allElements[number];
        //        set.Add(obj.Number, obj);
        //    }
        //}

        private void CreateGroup(ModelData model, string setName, IEnumerable<int> numbers, ObjType objsType)
        {
            var group = new GroupsData.Group(setName, objsType);

            foreach (var number in numbers)
            {
                if (objsType == ObjType.Узел)
                {
                    var obj = allNodes[number];
                    group.Add(obj);
                }
                else
                {
                    var obj = allElements[number];
                    group.Add(obj);
                }
            }

            model.GroupData.Add(group);
        }

        private string GetCollectionType(string source)
        {
            var result = string.Empty;
            var regex = new Regex("\".*?\"");
            var matches = regex.Matches(source);

            if (matches[0].Value.Contains("COLLECTOR_ID"))
                return "COLLECTOR_ID";
            else
                return "Part_ID";
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
