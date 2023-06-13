using Geometry;
using Model;
using Model.Events;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ModelParcer
{
    public class LoadModelFromCDBTextFile : IModelLoader
    {
        public event Action<object, LoaderEventArgs> LoadEvent;
        private const string NODE_BLOCK = "NBLOCK";
        private const string ELEMENTS_BLOCK = "EBLOCK";
        private const int OFFSET_TO_READ_NODES = 11;
        private ModelData model = new ModelData();
        private string exponentialFloatPattern = "(-)?\\d+\\.\\d+E(\\+|-)\\d+";
        private List<Node> allNodes = new List<Node>();

        /// <summary>
        /// Загрузить модель из файла формата .cdb по указанному пути
        /// </summary>
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
                        allNodes = ReadNodes(reader, tokens).ToList();
                        model.ObjectData.AddRange(allNodes);
                    }
                    if (line.Contains(ELEMENTS_BLOCK))
                    {
                        LoadEvent(this, new LoaderEventArgs("Начало загрузки элементов"));
                        var elements = ReadElements(reader, tokens);
                        model.ObjectData.AddRange(elements);
                    }
                }
            }
            return model;
        }

        /// <summary>
        /// Читает и возвращает координаты в форме файла .cdb с помощью регулярного выражения
        /// </summary>
        private IEnumerable<float> GetNodeCoordinates(string line)
        {
            var matches = Regex.Matches(line, exponentialFloatPattern);
            foreach (Match match in matches)
            {
                yield return float.Parse(match.Value, NumberStyles.Any, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Возвращает первое число в строке - номер элемента
        /// </summary>
        private int GetNodeNumber(string line)
        {
            var fields = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            return int.Parse(fields[0]);
        }

        /// <summary>
        /// Читает список узлов после строки NBLOCK в файле
        /// </summary>
        private IEnumerable<Node> ReadNodes(StreamReader reader, string[] tokens)
        {
            var format = reader.ReadLine();
            var maxFieldsCount = int.Parse(tokens[1]);
            var nodeCount = int.Parse(tokens[3]);
            for (var i = 0; i < nodeCount; i++)
            {
                var line = reader.ReadLine();
                var nodeNumber = GetNodeNumber(line);
                var coordinates = GetNodeCoordinates(line).ToList();
                if (coordinates.Count < maxFieldsCount)
                {
                    var missedCoordinatesCount = maxFieldsCount - coordinates.Count;
                    coordinates.AddRange(Enumerable.Repeat(0f, missedCoordinatesCount));
                }
                var point = new Point3D(coordinates[0], coordinates[1], coordinates[2]);
                var node = new Node(nodeNumber, point);
                yield return node;
            }
        }

        /// <summary>
        /// Определяет форму элемента по числу узлов
        /// </summary>
        private ModelObject GetElement(int[] elementNodes)
        {
            var nodes = new Node[elementNodes.Length];
            var currentNumber = model.ObjectData.GetLastObjNumber() + 1;
            for (var i = 0; i < elementNodes.Length; i++)
            {
                nodes[i] = model.ObjectData.Find(allNodes, elementNodes[i]);
            }
            switch (elementNodes.Length)
            {
                case 4: return new Element3D(currentNumber, nodes, ObjKind.Tetra);
                case 6: return new Element3D(currentNumber, nodes, ObjKind.Penta);
                case 8: return new Element3D(currentNumber, nodes, ObjKind.Hexa);
                default: throw new Exception($"Unable to recognize element with {elementNodes.Length} nodes");
            }
        }

        /// <summary>
        /// Читает список элементов после строки EBLOCK в файле
        /// </summary>
        private IEnumerable<ModelObject> ReadElements(StreamReader reader, string[] tokens)
        {
            var format = reader.ReadLine();
            var maxFieldsInLine = int.Parse(tokens[1]);
            var elementsNumber = int.Parse(tokens[4]);
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
                var elementNodes = fields.Skip(OFFSET_TO_READ_NODES).Distinct().ToArray();
                var element = GetElement(elementNodes);
                yield return element;
            }
        }
    }
}