using Geometry;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Model.MeshObjects;
using Model.ObjectsCollections;
using Model.Interfaces.ObjectsCollections;

namespace Model.IO.STL
{
    public class LoadFromSTLFile : IModelLoader
    {
        private Dictionary<(double X, double Y, double Z), Node> map = new Dictionary<(double X, double Y, double Z), Node>();

        private int nodes_counter = 0;
        private int elems_counter = 0;

        public IModelData Load(string path, IProgress<int> progress = null)
        {
            try
            {
                return IsBinary(path) ? LoadBinary(path, progress) : LoadTxt(path, progress);
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

        private IModelData LoadTxt(string path, IProgress<int> progress)
        {
            var modelData = new ModelData();
            
            using (var reader = new ProgressTextReader(path, progress))
            {
                var str = string.Empty;


                var solidCounter = 0;

                while (!reader.EndOfStream)
                {
                    var set = new ObjectsSet<Element2D>($"newSolid_{solidCounter++}");

                    while (str != null && !str.Contains("endsolid"))
                    {
                        if (str.Contains("outer loop"))
                        {
                            str = reader.ReadLine()?.Trim();
                            var nodes = GetPoints(reader, str, modelData.ObjectData.NodesSet);

                            if (IsValidTriangle(nodes[0].Position, nodes[1].Position, nodes[2].Position))
                            {
                                try
                                {
                                    set.Add(elems_counter, new Triangle(elems_counter, nodes));
                                    elems_counter++;
                                }
                                catch (Exception e)
                                {
                                    throw new StlFileLoadExeption("Невалидная поверхность в STL файле", e);
                                }
                            }
                            //counter++;
                        }
                        str = reader.ReadLine();
                    }
                    str = reader.ReadLine();

                    modelData.ObjectData.E2DCollection.Add(set);
                }
            }
            return modelData;
        }
        private Node[] GetPoints(StreamReader stl, string str, IObjectsSet<Node> nodes)
        {
            var points = new Node[3];
            for (int i = 0; i < 3; i++)
            {
                if (str != null && str != "endloop" && str.Contains("vertex"))
                {
                    str = str.Replace("vertex", "").Trim();
                    string[] lineData = str.Split(' ');
                    var x = double.Parse(lineData[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var y = double.Parse(lineData[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var z = double.Parse(lineData[2], NumberStyles.Float, CultureInfo.InvariantCulture);

                    var node = new Node(nodes_counter++, new Point3D(x, y, z));

                    //points[i] = node;
                    if (map.TryGetValue((x, y, z), out var mappedPoint))
                        points[i] = mappedPoint;
                    else
                    {
                        map.Add((x, y, z), node);
                        points[i] = node;
                        nodes.Add(node.Number, node);
                    }

                    str = stl.ReadLine()?.Trim();
                }
            }
            return points;
        }

        private IModelData LoadBinary(string path, IProgress<int> progress)
        {
            var modelData = new ModelData();


            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                int prevAtrib = -1;
                var lastPercent = 0;
                progress?.Report(0);

                reader.BaseStream.Position += 80;
                var count = reader.ReadUInt32();
                lastPercent = (int)(100 * reader.BaseStream.Position / reader.BaseStream.Length);
                if (lastPercent > 0)
                    progress?.Report(lastPercent);

                for (int i = 0; i < count; i++)
                {
                    reader.BaseStream.Position += 12;
                    var nodes = new Node[3];
                    nodes[0] = ReadNode(reader);
                    nodes[1] = ReadNode(reader);
                    nodes[2] = ReadNode(reader);

                    modelData.ObjectData.NodesSet.Add(nodes[0].Number, nodes[0]);
                    modelData.ObjectData.NodesSet.Add(nodes[1].Number, nodes[1]);
                    modelData.ObjectData.NodesSet.Add(nodes[2].Number, nodes[2]);

                    if (IsValidTriangle(nodes[0].Position, nodes[1].Position, nodes[2].Position))
                    {
                        try
                        {
                            var attrib = reader.ReadUInt16();

                            if (prevAtrib != -1)
                            //if (attrib != prevAtrib && prevAtrib != -1)
                            {
                                modelData.ObjectData.E2DCollection.
                                    Add(ObjType.Элемент2D.ToString(), new Triangle(elems_counter++, nodes));
                            }

                            prevAtrib = attrib;
                        }
                        catch (Exception e)
                        {
                            throw new StlFileLoadExeption("Невалидная поверхность в STL файле", e);
                        }
                    }
                    else
                    {
                        reader.BaseStream.Position += 2;
                    }
                    var percent = (int)(100 * reader.BaseStream.Position / reader.BaseStream.Length);
                    if (percent != lastPercent)
                    {
                        progress?.Report(percent);
                        lastPercent = percent;
                    }
                }
            }
            return modelData;
        }

        private bool IsValidTriangle(Point3D p1, Point3D p2, Point3D p3)
        {
            return Geometry.Vector.CrossProd(p1.Sub(p2), p1.Sub(p3)).GetSquareSumCoords() > 0;
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

        Node ReadNode(BinaryReader reader)
        {
            var x = reader.ReadSingle();
            var y = reader.ReadSingle();
            var z = reader.ReadSingle();

            if (map.TryGetValue((x, y, z), out var mappedPoint))
                return mappedPoint;

            return new Node(nodes_counter++, new Point3D(x, y, z));
        }

    }


    public class StlFileLoadExeption : Exception
    {
        public StlFileLoadExeption(string message, Exception inner) : base(message, inner) { }
    }
}
