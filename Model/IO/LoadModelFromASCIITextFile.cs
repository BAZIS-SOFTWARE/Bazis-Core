using Geometry;
using Model.MeshObjects;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Model.Interfaces.MeshObjects;
using System.Xml.Linq;

namespace Model.IO
{
    public class LoadModelFromASCIITextFile : IModelLoader
    {
        private Dictionary<int, Node> allNodes = new Dictionary<int, Node>();
        private Dictionary<int, Element> allElements = new Dictionary<int, Element>();

        public IModelData Load(string path, IProgress<int> progress = null)
        {
            ModelData model = new ModelData();
            using (var reader = new ProgressTextReader(path, progress))
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
            var elLastNumber = 0;
            if (model.ObjectData.GetAllElements().Count() > 0)
                elLastNumber = model.ObjectData.GetMaxElementNumber();

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

                
                var element = GetElementByType(nodes, model,type, elLastNumber + i + 1);
                allElements.Add(elCurrentNumber, element);
            }
        }

        private Element GetElementByType(Node[] nodes, ModelData model,int type,int elNumber)
        {
            switch (type) 
            {
                case 3004:
                    var tetra = new Tetra(elNumber, nodes);
                    model.ObjectData.E3DCollection.Add(ObjType.Элемент3D.ToString(),tetra);
                    return tetra;
                case 2003:
                    var triangle = new Triangle(elNumber, nodes);
                    model.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(),triangle);
                    return triangle;
                case 1002:
                    var beam = new Beam(elNumber, nodes);
                    model.ObjectData.E1DCollection.Add(ObjType.Элемент1D.ToString(),beam);
                    return beam;
                default: throw new ArgumentException($"Unable to recognize element of type {type}");
            }
        }

        private void ReadGroup(StreamReader reader, int count, ModelData model)
        {
            string[] tokens;
            for (var i = 0; i < count; i++)
            {
                var line = reader.ReadLine();
                tokens = ReplaceQuotes(line)
                    .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                    .ToArray();
                var groupName = tokens[1];
                var numbers = tokens.Skip(7).Select(int.Parse);

                ObjType objsType;
                if (tokens[2] == "2")
                    objsType = allElements[numbers.First()].ObjType;
                else objsType = ObjType.Узел;

                IEnumerable<IModelObject> objs;

                if (objsType == ObjType.Узел)
                    objs = numbers.Select(x => allNodes[x]);
                else
                    objs = numbers.Select(x => allElements[x]);

                model.GroupData.Create(groupName, objs);
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
