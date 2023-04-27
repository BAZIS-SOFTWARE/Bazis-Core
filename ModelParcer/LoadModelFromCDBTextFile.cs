using Geometry;
using Model;
using Model.Events;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer
{
    public class LoadModelFromCDBTextFile : IModelLoader
    {
        private const string NODE_BLOCK = "NBLOCK";
        private const string ELEMENTS_BLOCK = "EBLOCK";
        private const int OFFSET_TO_READ_NODES = 11;
        private ModelData model = new ModelData();

        public event Action<object, LoaderEventArgs> LoadEvent;

        public ModelData Load(string path)
        {
            using (var reader = new StreamReader(path))
            {
                LoadEvent(this, new LoaderEventArgs("Начало загрузки модели"));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var tokens = line.Replace(",", " ")
                            .Split(' ')
                            .Where(x => x.Length > 0)
                            .ToArray();
                    if (line.Contains(NODE_BLOCK))
                    {
                        LoadEvent(this, new LoaderEventArgs("Начало загрузки узлов"));
                        var nodes = ReadNodes(reader, tokens);
                        model.ObjectData.AddRange(nodes);
                    }
                    if (line.Contains(ELEMENTS_BLOCK))
                    {
                        LoadEvent(this, new LoaderEventArgs("Начало загрузки элементов"));
                        var elements = ReadElements(reader, tokens);
                        foreach (var element in elements)
                        {
                            model.ObjectData.Add(element);
                        }
                    }
                }
            }
            return model;
        }

        private IEnumerable<string> ParseAnsysFloatNotation(string ansysNotation)
        {
            var fields = ansysNotation.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            foreach (var field in fields)
            {
                if (field.Contains("-"))
                {
                    var splittedByMinus = field.Split('-');
                    yield return splittedByMinus[0];
                    yield return $"-{splittedByMinus[1]}";
                }
                else
                    yield return field;
            }
        }
        private float GetFloat(string exponentialNotation)
        {
            return float.Parse(exponentialNotation, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        private IEnumerable<Node> ReadNodes(StreamReader reader, string[] tokens)
        {
            var format = reader.ReadLine();
            var maxFieldsCount = int.Parse(tokens[1]);
            var nodeCount = int.Parse(tokens[3]);
            for (var i = 0; i < nodeCount; i++)
            {
                var line = reader.ReadLine();
                var fields = ParseAnsysFloatNotation(line).ToArray();
                var nodeNumber = int.Parse(fields[0]);
                var coordinates = new List<float>();
                for (var j = 3; j < fields.Length; j++)
                {
                    coordinates.Add(GetFloat(fields[j]));
                }
                if (fields.Length < maxFieldsCount)
                {
                    var missedCoordinatesCount = maxFieldsCount - fields.Length;
                    coordinates.AddRange(Enumerable.Repeat(0f, missedCoordinatesCount));
                }
                var point = new Point3D(coordinates[0], coordinates[1], coordinates[2]);
                var node = new Node(nodeNumber, point);
                yield return node;
            }
        }
        private ModelObject GetElement(int nodesCount, int[] elementNodes, List<Node> allNodes)
        {
            var nodes = new Node[nodesCount];
            var currentNumber = model.ObjectData.GetLastObjNumber() + 1;
            for (var i = 0; i < nodesCount; i++)
            {
                nodes[i] = model.ObjectData.Find(allNodes, elementNodes[i]);
            }
            switch (nodesCount)
            {
                case 2: return new Element1D(currentNumber, nodes, ObjKind.Beam);
                case 3: return new Element2D(currentNumber, nodes, ObjKind.Triangle);
                case 4: return new Element3D(currentNumber, nodes, ObjKind.Tetra);
                case 8: return new Element3D(currentNumber, nodes, ObjKind.Hexa);
                default: return new Surface(currentNumber, nodes, ObjKind.ElementSurface);
            }
        }

        private IEnumerable<ModelObject> ReadElements(StreamReader reader, string[] tokens)
        {
            var allNodes = model.ObjectData.FindMany<Node>().ToList();
            var format = reader.ReadLine();
            var maxFieldsInLine = int.Parse(tokens[1]);
            var elementsNumber = int.Parse(tokens[3]);
            for (int i = 0; i < elementsNumber; i++)
            {
                var line = reader.ReadLine();
                var fields = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
                var nodesCount = fields[8];
                if (OFFSET_TO_READ_NODES + nodesCount > maxFieldsInLine)
                {
                    var nextLineFields = reader.ReadLine().Split()
                        .Select(int.Parse)
                        .ToList();
                    fields.AddRange(nextLineFields);
                }
                var elementNodes = fields.Skip(OFFSET_TO_READ_NODES).ToArray();
                var element = GetElement(nodesCount, elementNodes, allNodes);
                yield return element;
            }
        }

    }
}
