using Geometry;
using Model.MeshObjects;
using Model.Events;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Model.IO
{
    public class LoadModelFromSalomeFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;
        private ModelData modelData;

        public IModelData Load(string path)
        {
            modelData = new ModelData();
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
                    modelData.ObjectData.NodeCollection.AddRange(nodes);

                    GetElements(reader, elementsNumber,
                        nodes.ToDictionary(x => x.Number, x => x)); 
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

        private void GetElements(StreamReader reader, int count,
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
                GetElement(elementType, nodes, elementIndex);
            }
        }

        private void GetElement(string elementType, Node[] nodes, int elementIndex)
        {
            switch (elementType)
            {
                case "102": 
                    modelData.ObjectData.E1DCollection.Add(new Beam(elementIndex, nodes.ToArray()));
                    break;
                case "203": 
                    modelData.ObjectData.E2DCollection.Add(new Triangle(elementIndex, nodes.ToArray()));
                    break;
                case "204": 
                    modelData.ObjectData.E2DCollection.Add(new Quad(elementIndex, nodes.ToArray()));
                    break;
                case "304": 
                    modelData.ObjectData.E3DCollection.Add(new Tetra(elementIndex, nodes.ToArray()));
                    break;
                case "308": 
                    modelData.ObjectData.E3DCollection.Add(new Hexa(elementIndex, nodes.ToArray()));
                    break;
                default: throw new ArgumentException($"Unable to recognize element {elementType}");
            }
        }
    }
}