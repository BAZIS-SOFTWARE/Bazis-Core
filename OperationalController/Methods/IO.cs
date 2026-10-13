using GmshApi;
using Model.Interfaces;
using Model.IO;
using Model.IO.STL;
using Project.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>

        public void Load(string path)
        {
            using (BeginChanges())
            {
                Open(path);
            }
        }
       
        /// <inheritdoc/>

        public void Save(string path)
        {
            using (BeginChanges())
            {
                if (!hasProject)
                    throw new InvalidOperationException("Project is not open.");

                var saver = new SaveProjectTextFormat();
                saver.Save(project, path);
                var pathChanged = filePath != path;
                filePath = path;
                isModified = false;
                if (pathChanged)
                    RegisterChange(new ProjectChange(ProjectChange.ProjectChangeKind.PropertyChanged, ProjectChange.ProjectProperty.FilePath));
            }
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
                    {
                        var min = double.Parse(line[1]);
                        gmsh.Option.SetNumber("Mesh.MeshSizeMin", min);
                    }
                        
                    else if (line[0] == "Макс.размер")
                    {
                        var max = double.Parse(line[1]);
                        gmsh.Option.SetNumber("Mesh.MeshSizeMax", max);
                    }
                        
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
                        if(line[2] == "embeded")
                        {
                            var numbers = GetArray(line[3]);
                            var numberSurface = int.Parse(line[1]);
                            GmshController.Gmsh.Model.Mesh.Embed(1, numbers, 2, numberSurface);
                        }
                        else
                        {
                            var points = line[2].Split(',').Select(x => int.Parse(x));
                            var arrange = ToEnum<Arrangement>(line[3]);
                            GmshController.SetTransfiniteSurface(int.Parse(line[1]), arrange, points.ToArray());

                            // квадратизируем
                            if (line[4] == "quad")
                                GmshController.SetRecombineSurface(int.Parse(line[1]));
                        }
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

                        // проверка квадратизации

                        if (GmshController.GetRecombineSurface(item.Value.Number))
                            joinStr += " quad";
                        else
                            joinStr += " tria";

                        sw.WriteLine($"{ObjType.Поверхность} {item.Value.Number} {joinStr}");
                    }

                    var embedded = GmshController.Gmsh.Model.Mesh.GetEmbedded(2, item.Value.Number).Where((v, i) => (i & 1) == 1).ToArray();
                    if(embedded.Length != 0)
                    {
                        var joinStr = string.Join(",", embedded);
                        sw.WriteLine($"{ObjType.Поверхность} {item.Value.Number} embeded {joinStr}");
                    }
                }

                foreach (var vol in ModelData.VolumeData)
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
        private int[] GetArray(string data)
        {
            var arrayStr = data.Split(',');
            int[] tags = arrayStr.Where(s => int.TryParse(s, out _)).Select(int.Parse).ToArray();
            return tags;
        }
    }
}
