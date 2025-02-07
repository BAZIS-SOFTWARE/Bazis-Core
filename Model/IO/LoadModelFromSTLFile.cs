using Geometry;
using Model.Events;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Model.GeometryObjects;
using System.Numerics;
using Model.GroupsData;
using Model.MeshObjects;
using Model.ObjectsCollections;
using Model.Interfaces.ObjectsCollections;

namespace Model.IO
{
    public class LoadModelFromSTLFile : IModelLoader
    {
        private Dictionary<Vector3, Node> map = new Dictionary<Vector3, Node>();

        //private Node IsPointExisted(Vector3 p)
        //{
        //    //var key = new Vector3(p._x, p._y, p._z);
        //    if (map.TryGetValue(p, out var mappedPoint))
        //        return mappedPoint;

        //    map[p] = new Geometry.Point3D(p.X,p.Y,p.Z);
        //    return p;
        //}

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

                var solidCounter = 0;
                while (!reader.EndOfStream)
                {
                    

                    while (str != null && !str.Contains("endsolid"))
                    {
                        if (str != null && str.Contains("outer loop"))
                        {
                            str = reader.ReadLine()?.Trim();
                            var nodes = GetPoints(reader, str, modelData.ObjectData.NodeCollection);

                            if (IsValidTriangle(nodes[0].Position, nodes[1].Position, nodes[2].Position))
                            {
                                try
                                {
                                    modelData.ObjectData.E2DCollection.Add(ObjType.Элемент2D.ToString(), new Triangle(counter++, nodes));
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


                    //var group = new Group($"newSolid_{solidCounter++}", ObjType.Фигура2D);
                    //group.AddRange(surfaces);
                    //modelData.GroupData.Add(group);
                }
                LoadEvent?.Invoke(this, new LoaderEventArgs("Модель загружена"));
            }
            return modelData;
        }
        private Node[] GetPoints(StreamReader stl, string str, IObjectsCollection<Node> nodes)
        {
            var points = new Node[3];
            for (int i = 0; i < 3; i++)
            {
                if (str != null && str != "endloop" && str.Contains("vertex"))
                {
                    str = str.Replace("vertex", "").Trim();
                    string[] lineData = str.Split(' ');
                    var x = float.Parse(lineData[0], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var y = float.Parse(lineData[1], NumberStyles.Float, CultureInfo.InvariantCulture);
                    var z = float.Parse(lineData[2], NumberStyles.Float, CultureInfo.InvariantCulture);

                    var node = new Node(counter, new Point3D(x, y, z));
                    counter++;

                    //points[i] = node;
                    if (map.TryGetValue(new Vector3(x, y, z), out var mappedPoint))
                        points[i] = mappedPoint;
                    else
                    {
                        map.Add(new Vector3(x, y, z), node);
                        points[i] = node;
                        nodes.Add(ObjType.Узел.ToString(), node);
                    }

                    str = stl.ReadLine()?.Trim();
                }
            }
            return points;
        }

        private IModelData LoadBinary(string path)
        {
            var modelData = new ModelData();

            LoadEvent?.Invoke(this, new LoaderEventArgs("Начало импорта STL модели"));

            using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
            {
                int prevAtrib = -1;

                reader.BaseStream.Position += 80;
                var count = reader.ReadUInt32();

                for (int i = 0; i < count; i++)
                {
                    reader.BaseStream.Position += 12;
                    var nodes = new Node[3];
                    nodes[0] = ReadNode(reader);
                    nodes[1] = ReadNode(reader);
                    nodes[2] = ReadNode(reader);

                    modelData.ObjectData.NodeCollection.AddRange(ObjType.Узел.ToString(), nodes);

                    if (IsValidTriangle(nodes[0].Position, nodes[1].Position, nodes[2].Position))
                    {
                        try
                        {
                            var attrib = reader.ReadUInt16();

                            if (prevAtrib != -1)
                                //if (attrib != prevAtrib && prevAtrib != -1)
                            {
                                modelData.ObjectData.E2DCollection.
                                    Add(ObjType.Элемент2D.ToString(), new Triangle(counter++, nodes));
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
                }
            }
            LoadEvent?.Invoke(this, new LoaderEventArgs("Модель загружена"));
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

            if (map.TryGetValue(new Vector3(x, y, z), out var mappedPoint))
                return mappedPoint;

            return new Node(counter++, new Point3D(x, y, z));
        }

    }


    public class StlFileLoadExeption : Exception
    {
        public StlFileLoadExeption(string message, Exception inner) : base(message, inner) { }
    }
}
