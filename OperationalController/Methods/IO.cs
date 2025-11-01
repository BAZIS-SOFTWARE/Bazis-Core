using Model.Interfaces.ObjectsCollections;
using Model.Interfaces;
using System.Collections.Generic;
using Model.Interfaces.MeshObjects;
using Project.Interfaces.Tasks;
using Model.GeometryObjects;
using PropertiesCalculator.MaterialData;
using PropertiesCalculator.FunctionData;
using Model.ObjectsCollections;
using Model.MeshObjects;
using OperationalController.GmshController;
using System;
using System.Data;
using Model.IO;
using Project.IO;
using Project.Interfaces.IO;
using Model.IO.STL;
using GmshApi;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        public event Action<string> MessageEvent;
      
        public void UnsubMessasge()
        {
            MessageEvent = null;
        }

        /// <inheritdoc/>

        public void Load(string path)
        {
            var loader = new LoadProjectFromTextFormat();
            loader.LoadEvent += (ar1, ar2) => { MessageEvent?.Invoke(ar2.Message); };
            project = loader.Load(path);
        }
       
        /// <inheritdoc/>

        public void Save(string path)
        {
            if (project == null)
                throw new Exception("Проект не создан");

            var saver = new SaveProjectTextFormat();
            saver.Save(project, path);
        }
/// <inheritdoc/>

        public void LoadSMF(string path)
        {
            using (var sw = new StreamReader(path, Encoding.Unicode))
            {
                while (!sw.EndOfStream)
                {
                    var line = sw.ReadLine().Split(' ');
                    var gmsh = GmshController.Gmsh;
                    if (line[0] == "Мин.размер")
                        gmsh.Option.SetNumber("Mesh.MeshSizeMin", double.Parse(line[1]));
                    else if (line[0] == "Макс.размер")
                        gmsh.Option.SetNumber("Mesh.MeshSizeMax", 100);
                    else if (line[0] == "Алгоритм2D")
                        gmsh.Option.SetNumber("Mesh.Algorithm", double.Parse(line[1]));
                    else if (line[0] == "Алгоритм3D")
                        gmsh.Option.SetNumber("Mesh.Algorithm3D", double.Parse(line[1]));
                    else if (line[0] == "Масш.фактор")
                        gmsh.Option.SetNumber("Mesh.MeshSizeFactor", double.Parse(line[1]));

                    else if (line[0] == "Загрузить")
                    {
                        if (line[1] == "модель")
                        {
                            gmsh.Clear();
                            gmsh.Merge(line[2]);
                            //gmsh.Open(line[2]);
                        }
                    }

                    else if (line[0] == ObjType.Точка.ToString())
                    {
                        var dimTags = new int[] { 0, int.Parse(line[1]) };
                        //var meshSize = gmsh.Model.Mesh.GetSizes(dimTags);
                        gmsh.Model.Mesh.SetSize(dimTags, double.Parse(line[2]));
                    }
                    else if (line[0] == ObjType.Кривая.ToString())
                    {
                        var points = int.Parse(line[2]);
                        var meshType = ToEnum<MeshType>(line[3]);
                        var coeff = double.Parse(line[4]);
                        GmshController.SetTransfiniteCurve(int.Parse(line[1]), points, meshType, coeff);
                    }

                    else if (line[0] == ObjType.Поверхность.ToString())
                    {
                        var points = line[2].Split(',').
                            Select(x => int.Parse(x));
                        var arrange = ToEnum<Arrangement>(line[3]);
                        GmshController.SetTransfiniteSurface(int.Parse(line[1]), arrange, points.ToArray());
                    }
                    else if (line[0] == "Объем")
                    {
                        if (line[2] == "регулярная")
                            GmshController.SetTransfiniteVolume(int.Parse(line[1]));
                        else
                        {
                            var power = double.Parse(line[3]);
                            var distMax = double.Parse(line[4]);
                            var surfSize = double.Parse(line[5]);
                            var coreSize = double.Parse(line[6]);
                            GmshController.SetGradientVolume(int.Parse(line[1]), power, distMax, surfSize, coreSize);
                        }


                    }



                }
            }
        }
        /// <summary>
        /// ToEnum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public T ToEnum<T>(string value) where T : struct, Enum
        {
            if (Enum.TryParse(value, out T result))
                return result;
            else throw new ArgumentException($"Ошибка: значение '{value}' не соответствует ни одному значению.");
        }
        /// <inheritdoc/>

        public void SaveSMF(string path)
        {
            var ext = Path.GetExtension(path);

            if (ext == null | ext == string.Empty)
                throw new Exception("Расширение не задано");

            if (GmshController.Gmsh == null)
                throw new Exception("Контроллер сетки не загружен");
            //TODO реализовать сохранение настроек построения сетки языком geo
            var gmsh = GmshController.Gmsh;

            using (var sw = new StreamWriter(path, false, Encoding.Unicode))
            {
                var file = gmsh.Model.GetFileName();
                sw.WriteLine($"Загрузить модель {file}");

                var actMinSize = gmsh.Option.GetNumber("Mesh.MeshSizeMin");
                sw.WriteLine($"Мин.размер {actMinSize}");

                var actMaxSize = gmsh.Option.GetNumber("Mesh.MeshSizeMax");
                sw.WriteLine($"Макс.размер {actMaxSize}");

                var actAlgo2d = gmsh.Option.GetNumber("Mesh.Algorithm");
                sw.WriteLine($"Алгоритм2D {actAlgo2d}");

                var actAlgo3d = gmsh.Option.GetNumber("Mesh.Algorithm3D");
                sw.WriteLine($"Алгоритм3D {actAlgo3d}");

                var actSizeFactor = gmsh.Option.GetNumber("Mesh.MeshSizeFactor");
                sw.WriteLine($"Масш.фактор {actSizeFactor}");

                foreach (var item in ModelData.ObjectData.PointsSet)
                {
                    var meshSize = GmshController.GetSize(item.Value.Number);
                    sw.WriteLine($"{ObjType.Точка} {item.Value.Number} {meshSize}");
                }

                foreach (var item in ModelData.ObjectData.CurveSet)
                {
                    var atrs = GmshController.GetTransfiniteCurve(item.Value.Number);
                    if (atrs.Length != 0)
                    {
                        var joinStr = string.Join(" ", atrs);
                        sw.WriteLine($"{ObjType.Кривая} {item.Value.Number} {joinStr}");
                    }
                }

                foreach (var item in ModelData.ObjectData.SurfaceSet)
                {
                    var atrs = GmshController.GetTransfiniteSurface(item.Value.Number);
                    if (atrs.Length != 0)
                    {
                        var joinStr = string.Join(" ", atrs);
                        sw.WriteLine($"{ObjType.Поверхность} {item.Value.Number} {joinStr}");
                    }
                }

                foreach (var vol in ModelData.ObjectData.VolumeData)
                {
                    var atrs = GmshController.GetTransfiniteVolume(vol.Number);
                    if (atrs.Length != 0)
                    {
                        var joinStr = string.Join(" ", atrs);
                        sw.WriteLine($"Объем {vol.Number} {joinStr}");
                    }
                }
            }
        }
        /// <inheritdoc/>

        public void Append(string path)
        {
            if (project == null)
                throw new Exception("Проект не создан");

            var ext = System.IO.Path.GetExtension(path);

            IModelLoader loader;
            if (ext == ".inp")
                loader = new LoadModelFromINPTextFile();
            else if (ext == ".inp_v2")
                loader = new LoadModelFromINPTextFile_v2();
            else if (ext == ".ASC" | ext == ".asc")
                loader = new LoadModelFromASCIITextFile_v2();
            else if (ext == ".dat")
                loader = new LoadModelFromSalomeFile();
            else if (ext == ".STL" | ext == ".stl")
                loader = new LoadFromSTLFile();
            else if (ext == ".cdb")
                loader = new LoadModelFromCDBTextFile();

            else
                throw new Exception("неправильно задано расширение файла");

            loader.LoadEvent += (ar1, ar2) =>
            {
                MessageEvent?.Invoke(ar2.Message);
            };
            var modelData = loader.Load(path);

            project.ModelData.Append(modelData);
        }
    }
}
