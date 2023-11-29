using Geometry;
using Model.MeshObjects;
using Model.Events;
using ModelInterfaces;
using System;
using System.Globalization;
using System.IO;

namespace Model.IO
{
    public class LoadModelFromSTLFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;
        public int counter = 0;

        public IModelData Load(string path)
        {
            var modelData = new ModelData();
            var reader = new StreamReader(path);
            var str = String.Empty;

            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало импорта STL модели"));

            while (str != "endsolid")
            {
                if (str != null && str.Contains("outer loop"))
                {
                    str = reader.ReadLine().Trim();
                    var nodes = GetNodes(reader, str);
                    modelData.ObjectData.AddRange(nodes);
                    var surface = new Surface(counter, nodes);
                    modelData.ObjectData.Add(surface);
                    counter++; 
                }
                str = reader.ReadLine();
            }
            LoadEvent?.Invoke(this, new LoaderEventArgs("Модель загружена"));
            return modelData;
        }
        public Node[] GetNodes(StreamReader stl, string str)
        {
            var nodes = new Node[3];
            for( int i = 0; i < 3; i++)
            {
                if (str != "endloop" && str.Contains("vertex"))
                {
                    str = str.Replace("vertex", "").Trim();
                    string[] lineData = str.Split(' ');
                    var x = float.Parse(lineData[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var y = float.Parse(lineData[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var z = float.Parse(lineData[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var node = new Node(counter, new Point3D(x, y, z));
                    counter++;
                    nodes[i] = node;
                    str = stl.ReadLine().Trim();
                }
            }
            return nodes;
        }
    }
}
