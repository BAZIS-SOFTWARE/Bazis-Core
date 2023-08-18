using Geometry;
using Model.Events;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Model.ModelParcer
{
    public class LoadModelFromSalomeFile : IModelLoader
    {
        public event Action<object, LoaderEventArgs> LoadEvent;

        public ModelData Load(string path)
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
                case "102": return new Element1D(elementIndex, nodes.ToArray(), ObjKind.Beam);
                case "203": return new Element2D(elementIndex, nodes.ToArray(), ObjKind.Triangle);
                case "204": return new Element2D(elementIndex, nodes.ToArray(), ObjKind.Quad);
                case "304": return new Element3D(elementIndex, nodes.ToArray(), ObjKind.Tetra);
                case "308": return new Element3D(elementIndex, nodes.ToArray(), ObjKind.Hexa);
                default: throw new ArgumentException($"Unable to recognize element {elementType}");
            }
        }
    }
}