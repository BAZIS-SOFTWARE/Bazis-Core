using Geometry;
using Model.MeshObjects;
using Model.Events;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Model.IO
{
    public class LoadModelFromSalomeFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;

        public IModelData Load(string path)
        {
            var modelData = new ModelData();
            using (var reader = new StreamReader(path))
            {
                LoadEvent(this, new LoaderEventArgs("Начало загрузки модели"));
                while (!reader.EndOfStream)
                {
                    var tokens = reader.ReadLine()
                        .Split()
                        .ToArray();
                    var nodesNumber = int.Parse(tokens[0]);
                    var elementsNumber = int.Parse(tokens[1]);
                    var nodes = GetNodes(reader, nodesNumber).ToList();
                    modelData.ObjectData.AddRange(nodes);
                    var elements = GetElements(reader, elementsNumber,
                        nodes.ToDictionary(x => x.Number, x => x)).ToList();
                    modelData.ObjectData.AddRange(elements);
                }
            }
            return modelData;
        }

        private IEnumerable<Node> GetNodes(StreamReader reader, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var tokens = reader.ReadLine().Split();
                var nodeIndex = int.Parse(tokens[0]);
                var coordinates = tokens.Select(x => float.Parse(x, NumberStyles.Any, CultureInfo.InvariantCulture))
                    .ToArray();
                yield return new Node(nodeIndex, new Point3D(coordinates[1], coordinates[2], coordinates[3]));
            }
        }

        private IEnumerable<ModelObject> GetElements(StreamReader reader, int count,
            Dictionary<int, Node> indexToNodeMapping)
        {
            for (var i = 0; i < count; i++)
            {
                var tokens = reader.ReadLine()
                         .Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                         .ToArray();
                var elementIndex = int.Parse(tokens[0]);
                var elementType = tokens[1];
                var nodes = tokens.Skip(2)
                    .Select(int.Parse)
                    .Select(x => indexToNodeMapping[x])
                    .ToArray();
                yield return GetElement(elementType, nodes, elementIndex);
            }
        }

        private ModelObject GetElement(string elementType, Node[] nodes, int elementIndex)
        {
            switch (elementType)
            {
                case "102": return new Beam(elementIndex, nodes.ToArray());
                case "203": return new Triangle(elementIndex, nodes.ToArray());
                case "204": return new Quad(elementIndex, nodes.ToArray());
                case "304": return new Tetra(elementIndex, nodes.ToArray());
                case "308": return new Hexa(elementIndex, nodes.ToArray());
                default: throw new ArgumentException($"Unable to recognize element {elementType}");
            }
        }
    }
}