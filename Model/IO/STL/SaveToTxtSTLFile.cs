
using Geometry;
using Model.Interfaces;
using System.IO;
using System.Linq;
using System.Reflection;
using System;
using Model.GeometryObjects;
using System.Collections.Generic;

namespace Model.IO.STL
{
    public class SaveModelToBinarySTLFile : IModelSaver
    {
        public event Action<object, ISaverEventArgs> SaveEvent;
        public void Save(IModelData model, string path)
        {
            ushort attrib = 0;

            using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate)))
            {
                var objects = model.ObjectData.SurfaceSet.Values;

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

    public class SaveToTxtSTLFile : IModelSaver
    {
        /// <inheritdoc/>
        public event Action<object, ISaverEventArgs> SaveEvent;
/// <inheritdoc/>

        public void Save(IModelData model, string path)
        {
            using (var writer = new StreamWriter(path))
            {
                //сохранение координат узлов
                var ver = Assembly.GetExecutingAssembly().GetName().Version;

                foreach (var set in model.ObjectData.E2DCollection)
                {
                    writer.WriteLine($"solid {set.Key}"); // – Header
                    foreach (var element in set.Value)
                    {
                        SaveSurfaces(writer, element.Value.GetSurfaces());
                    }
                    writer.WriteLine($"endsolid");
                }
            }
        }
        /// <summary>
        /// SaveSurfaces
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="surfaces"></param>
        /// <exception cref="StlFileWriteExeption"></exception>
        public void SaveSurfaces<T>(StreamWriter writer, IEnumerable<ISurface<T>> surfaces) where T : IPoint
        {
            foreach (var surface in surfaces)
            {
                var points = surface.GetVertexes();
                if (points.Count() != 3)
                    throw new StlFileWriteExeption("В STL файл можно сохранять только треугольники");

                Point3D norm = surface.CalcNormal();
                writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "   facet normal {0:R} {1:R} {2:R}",
                    norm._x, norm._y, norm._z));
                writer.WriteLine($"      outer loop");

                foreach (var point in points) // Vertex 1,2,3
                {
                    writer.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "         vertex {0:R} {1:R} {2:R}",
                        point._x, point._y, point._z));
                }
                writer.WriteLine($"      endloop");
                writer.WriteLine($"   endfacet");
            }
        }
    }
    public class StlFileWriteExeption : Exception
    {
        public StlFileWriteExeption(string message, Exception inner) : base(message, inner) { }
        public StlFileWriteExeption(string message) : base(message) { }
    }
}

