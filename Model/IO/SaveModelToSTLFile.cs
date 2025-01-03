
﻿using Geometry;
using Model.Interfaces;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using Model.GeometryObjects;

 namespace Model.IO
{
    public class SaveModelToBinarySTLFile : IModelSaver
    {
        public event Action<object, ISaverEventArgs> SaveEvent;
        public void Save(IModelData model, string path)
        {
            ushort attrib = 0;

            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                var objects = model.ObjectData.Fig2DCollection;

                //сохранение координат узлов
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes("Версия " + $"{ver.Major}.{ver.Minor}.{ver.Build}");
                var captionBytes = new byte[Math.Max(buffer.Length, 80)];
                Array.Copy(buffer, captionBytes, Math.Min(buffer.Length, 80));
                writer.Write(captionBytes); // – Header
                var count = objects.Select(w => w.GetSurfaces().Count()).Sum();

                writer.Write((uint)count); // – Number of triangles

                foreach (var item in objects)
                {
                    foreach (var surface in item.GetSurfaces())
                    {
                        var points = surface.GetCoordinates().ToArray();
                        if (points.Length != 3)
                            throw new StlFileWriteExeption("В STL файл можно сохранять только треугольники");

                        Point3D norm = surface.CalcNormal();
                        writer.Write(norm._x);
                        writer.Write(norm._y);
                        writer.Write(norm._z);

                        foreach (var point in points) // Vertex 1,2,3
                        {
                            writer.Write(point._x);
                            writer.Write(point._y);
                            writer.Write(point._z);
                        }
                        writer.Write(attrib); // – Attribute byte count
                    }

                    unchecked
                    {
                        attrib++;
                    }

                }




            }
        }
    }

    public class SaveModelToTxtSTLFile : IModelSaver
    {
        public event Action<object, ISaverEventArgs> SaveEvent;
        public void Save(IModelData model, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                //сохранение координат узлов
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                writer.WriteLine("solid Версия " + $"{ver.Major}.{ver.Minor}.{ver.Build}"); // – Header

                var objects = model.ObjectData.E2DCollection;

                foreach (var item in objects)
                    foreach (var surface in item.GetSurfaces())
                    {
                        var points = surface.GetCoordinates().ToArray();
                        if (points.Length != 3)
                            throw new StlFileWriteExeption("В STL файл можно сохранять только треугольники");

                        Point3D norm = surface.CalcNormal();
                        writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "   facet normal {0:R} {1:R} {2:R}",
                            (float)norm._x, (float)norm._y, (float)norm._z));
                        writer.WriteLine($"      outer loop");

                        foreach (var point in points) // Vertex 1,2,3
                        {
                            writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "         vertex {0:R} {1:R} {2:R}",
                                point._x, point._y, point._z));
                        }
                        writer.WriteLine($"      endloop");
                        writer.WriteLine($"   endfacet");
                    }
                writer.WriteLine($"endsolid");
            }
        }
    }
    public class StlFileWriteExeption : Exception
    {
        public StlFileWriteExeption(string message, Exception inner) : base(message, inner) { }
        public StlFileWriteExeption(string message) : base(message) { }
    }
}

