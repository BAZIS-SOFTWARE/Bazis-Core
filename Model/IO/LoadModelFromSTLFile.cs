using Geometry;
using Model.MeshObjects;
using Model.Events;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Model.GeometryObjects;
using System.Collections.Generic;

namespace Model.IO
{
    public class LoadModelFromSTLFile : IModelLoader
    {
        public event Action<object, ILoaderEventArgs> LoadEvent;
        private int counter = 0;

        public IModelData Load(string path)
        {
            try
            {
                return IsBinary(path) ? LoadBinary(path) : LoadTxt(path);
            }
            catch (Exception e)
            {
                if (e is IndexOutOfRangeException
                    || e is FormatException
                    || e is EndOfStreamException)
                    throw new StlFileLoadExeption("Формат файла не соответствует формату STL", e);

                throw;
            }
        }

        private IModelData LoadTxt(string path)
        {
            var modelData = new ModelData();

            using (var reader = new StreamReader(path))
            {
                var str = String.Empty;

                LoadEvent?.Invoke(this, new LoaderEventArgs("Начало импорта STL модели"));


                //var surfaces = new List<GeometrySurface>();
                while (str != null)
                {
                    var surfaces = new List<GeometrySurface>();

                    while (str != "endsolid")
                    {
                        if (str != null && str.Contains("outer loop"))
                        {
                            str = reader.ReadLine()?.Trim();
                            var points = GetPoints(reader, str);
                            modelData.ObjectData.PointCollection.AddRange(points);
                            var surface = new GeometrySurface(counter, points);
                            surfaces.Add(surface);                          
                        }
                        var surfaceFig = new SurfaceFigure(counter++, surfaces.ToArray());
                        modelData.ObjectData.Fig2DCollection.Add(surfaceFig);
                        str = reader.ReadLine()?.Trim();
                    }
                    str = String.Empty;
                }

                LoadEvent?.Invoke(this, new LoaderEventArgs("Модель загружена"));
            }

            return modelData;
        }
        private GeometryPoint[] GetPoints(StreamReader stl, string str)
        {
            var points = new GeometryPoint[3];
            for (int i = 0; i < 3; i++)
            {
                if (str != null && str != "endloop" && str.Contains("vertex"))
                {
                    str = str.Replace("vertex", "").Trim();
                    string[] lineData = str.Split(' ');
                    var x = float.Parse(lineData[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var y = float.Parse(lineData[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var z = float.Parse(lineData[2], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var node = new GeometryPoint(counter, new Point3D(x, y, z));
                    counter++;
                    points[i] = node;
                    str = stl.ReadLine()?.Trim();
                }
            }
            return points;
        }

        private IModelData LoadBinary(string path)
        {
            var modelData = new ModelData();

            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало импорта STL модели"));

            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    var surfaces = new List<GeometrySurface>();
                    reader.BaseStream.Position += 80;
                    var count = reader.ReadUInt32();
                    reader.BaseStream.Position += 12;
                    for (int i = 0; i < count; i++)
                    {
                        var points = new GeometryPoint[3];
                        points[0] = ReadNode(reader);
                        points[1] = ReadNode(reader);
                        points[2] = ReadNode(reader);
                        var surface = new GeometrySurface(counter, points);
                        surfaces.Add(surface);
                        reader.BaseStream.Position += 14;
                    }
                    var surfaceFig = new SurfaceFigure(counter++, surfaces.ToArray());
                    modelData.ObjectData.Fig2DCollection.Add(surfaceFig);
                }
            }
            LoadEvent?.Invoke(this, new LoaderEventArgs("Модель загружена"));
            return modelData;
        }

        private bool IsBinary(string path)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                byte[] header = reader.ReadBytes(80);
                var name = Encoding.ASCII.GetString(header);
                return !name.StartsWith("solid ");
            }
        }

        GeometryPoint ReadNode(BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();
            var node = new GeometryPoint(counter++, new Point3D(x, y, z));
            return node;
        }

    }


    public class StlFileLoadExeption : Exception
    {
        public StlFileLoadExeption(string message, Exception inner) : base(message, inner) { }
    }
}
