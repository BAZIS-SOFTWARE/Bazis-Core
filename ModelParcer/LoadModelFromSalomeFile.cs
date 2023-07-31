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
        private List<Node> allNodes;

        public ModelData Load(string path)
        {
            var destination = "./temp.txt";
            GenerateTextFile(path, destination);
            return ReadTextFile(destination);
        }

        private void GenerateTextFile(string source, string destination)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = "./mesh2ascii.exe",
                UseShellExecute = false,
                Arguments = $"{source} {destination}",
            };
            process.Start();
            process.WaitForExit();
            var fileInfo = new FileInfo(destination);
            if (fileInfo.Length == 0)
                throw new FileLoadException($"An error occured while reading the file {source}. Refer to log.txt");
        }

        private ModelData ReadTextFile(string filePath)
        {
            var modelData = new ModelData();
            using (var reader = new StreamReader(filePath))
            {
                LoadEvent(this, new LoaderEventArgs("Начало загрузки модели"));
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var tokens = line.Split().ToArray();
                    var count = int.Parse(tokens[0]);
                    if (line.Contains("NODES"))
                    {
                        allNodes = GetNodes(reader, count).ToList();
                        modelData.ObjectData.AddRange(allNodes);
                    }
                    else if (line.Contains("ELEMENTS"))
                        modelData.ObjectData.AddRange(GetElements(reader, count));
                }
            }
            return modelData;
        }

        private IEnumerable<Node> GetNodes(StreamReader reader, int count)
        {
            for(var i = 0; i < count; i++)
            {
                var line = reader.ReadLine();
                var tokens = line.Split().Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();
                yield return new Node(i, new Point3D(tokens[0], tokens[1], tokens[2]));
            }
        }

        private IEnumerable<ModelObject> GetElements(StreamReader reader, int count)
        {
            var counter = 0;
            for (var i = 0; i < count; i++)
            {
                var line = reader.ReadLine();
                var tokens = line.Split().ToArray();
                var elementType = tokens[0];
                var elementsCount = int.Parse(tokens[1]);
                for(int j = 0; j < elementsCount; j++)
                {
                    var nodeIndexes = reader.ReadLine().Split().Select(int.Parse).ToArray();
                    var nodes = nodeIndexes.Select(index => allNodes[index]).ToArray();
                    yield return GetElement(elementType, nodes, counter++);
                }
            }
        }

        private ModelObject GetElement(string elementType, Node[] nodes, int counter)
        {
            switch(elementType)
            {
                case "line": return new Element1D(counter, nodes.ToArray(), ObjKind.Beam);
                case "triangle": return new Element2D(counter, nodes.ToArray(), ObjKind.Triangle);
                case "quad": return new Element2D(counter, nodes.ToArray(), ObjKind.Quad);
                case "tetra": return new Element3D(counter, nodes.ToArray(), ObjKind.Tetra);
                case "hexahedron": return new Element3D(counter, nodes.ToArray(), ObjKind.Hexa);
                default: throw new ArgumentException($"Unable to recognize element {elementType}");
            }
        }
    }
}