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
    public class LoadModelFromCDBTextFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;
        private const string NODE_BLOCK = "NBLOCK";
        private const string ELEMENTS_BLOCK = "EBLOCK";
        private const int OFFSET_TO_READ_NODES = 11;
        private const string exponentialFloatPattern = "(-)?\\d+\\.\\d+E(\\+|-)\\d+";
        private Dictionary<int, Node> allNodes = new Dictionary<int, Node>();

        /// <summary>
        /// Загрузить модель из файла формата .cdb по указанному пути
        /// </summary>
        public IModelData Load(string path)
        {
            var model = new ModelData();
            using (var reader = new StreamReader(path))
            {
                LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки модели"));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var tokens = line.Replace(",", " ")
                            .Split(' ')
                            .Where(x => x.Length > 0)
                            .ToArray();
                    if (line.Contains(NODE_BLOCK))
                    {
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки узлов"));
                        var nodes = ReadNodes(reader, tokens).ToList();
                        model.ObjectData.Add(ObjType.Узел, nodes);
                    }
                    if (line.Contains(ELEMENTS_BLOCK))
                    {
                        LoadEvent?.Invoke(this, new LoaderEventArgs("Начало загрузки элементов"));
                        var elements = ReadElements(reader, tokens);
                        model.ObjectData.Add(ObjType.Элемент, elements);
                    }
                }
            }
            allNodes.Clear();
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
                allNodes.Add(nodeNumber, node);
                yield return node;
            }
        }

        /// <summary>
        /// Определяет форму элемента по числу узлов
        /// </summary>
        private ModelObject GetElement(int[] elementNodes, int elementNumber)
        {
            var nodes = elementNodes.Select(i => allNodes[i]).ToArray();
            switch (elementNodes.Length)
            {
                case 4: return new Tetra(elementNumber, nodes);
                case 6: return new Penta(elementNumber, nodes);
                case 8: return new Hexa(elementNumber, nodes);
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
                var elementNumber = fields[10];
                if (OFFSET_TO_READ_NODES + nodesCount > maxFieldsInLine)
                {
                    var nextLineFields = reader.ReadLine().Split()
                        .Select(int.Parse)
                        .ToList();
                    fields.AddRange(nextLineFields);
                }
                var elementNodes = fields.Skip(OFFSET_TO_READ_NODES).Distinct().ToArray();
                var element = GetElement(elementNodes, elementNumber);
                yield return element;
            }
        }
    }
}