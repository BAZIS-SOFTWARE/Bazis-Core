using Geometry;
using Model.MeshObjects;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Security.Cryptography;
using Model.Interfaces.ObjectsCollections;

namespace Model.IO
{
    public class LoadModelFromSalomeFile : IModelLoader
    {
        private ModelData modelData;

        public IModelData Load(string path, IProgress<int> progress = null)
        {
            modelData = new ModelData();
            using (var reader = new ProgressTextReader(path, progress))
            {
                while (!reader.EndOfStream)
                {
                    var tokens = reader.ReadLine()
                        .Split()
                        .ToArray();
                    var nodesNumber = int.Parse(tokens[0]);
                    var elementsNumber = int.Parse(tokens[1]);
                    //var nodes = .ToList();

                    foreach (var item in GetNodes(reader, nodesNumber))
                        modelData.ObjectData.NodesSet.Add(item.Number, item);
                
                    GetElements(reader, elementsNumber,
                        modelData.ObjectData.NodesSet); 
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
            IObjectsSet<Node> indexToNodeMapping)
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
                    modelData.ObjectData.E1DCollection.Add(ObjType.Элемент1D.ToString(), new Beam(elementIndex, nodes.ToArray()));
                    break;
                case "203": 
                    modelData.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(), new Triangle(elementIndex, nodes.ToArray()));
                    break;
                case "204": 
                    modelData.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(), new Quad(elementIndex, nodes.ToArray()));
                    break;
                case "304": 
                    modelData.ObjectData.E3DCollection.Add(ObjType.Элемент3D.ToString(), new Tetra(elementIndex, nodes.ToArray()));
                    break;
                case "308": 
                    modelData.ObjectData.E3DCollection.Add(ObjType.Элемент3D.ToString(), new Hexa(elementIndex, nodes.ToArray()));
                    break;
                default: throw new ArgumentException($"Unable to recognize element {elementType}");
            }
        }
    }
}
