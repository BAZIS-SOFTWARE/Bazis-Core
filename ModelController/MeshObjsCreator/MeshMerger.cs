using GmshApi;
using Model.Interfaces;
using ModelController.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelController.MeshObjsCreator
{
    /// <summary>
    /// Производит слияние сеток в формате stl
    /// </summary>
    public class MeshMerger
    {
        private IEnumerable<string> MeshFiles { get; set; }
        private IGmshController Controller { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="meshFiles">Перечисление путей файлов до сеточных моделей в формате stl</param>
        /// <param name="controller">Контроллер gmsh</param>
        public MeshMerger(IEnumerable<string> meshFiles, IGmshController controller)
        {
            MeshFiles = meshFiles;
            Controller = controller;
        }
        /// <summary>
        /// Выполняет слияние двух сеток STL
        /// </summary>
        public void Merge()
        {
            var files = MeshFiles.ToArray();
            if (files.Length != 2)
                throw new Exception("Пока поддерживается только две модели для слияния");
            Controller.Gmsh.Merge(files[0]);
            Controller.Gmsh.Merge(files[1]);

            var cadSurfaces = GetCadSurfacesFromMesh();
            AddVolumes(cadSurfaces);

            var volumes = Controller.Gmsh.Model.GetEntities(3);
            var first = volumes.Take(2).ToArray();
            var second = volumes.Skip(2).ToArray();

            Controller.Gmsh.Model.Occ.Fuse(first, second);
            Controller.Gmsh.Model.Occ.Synchronize();

            Controller.Gmsh.Model.Mesh.Clear();
        }

        private int[][] GetCadSurfacesFromMesh()
        {
            var dimTags = Controller.Gmsh.Model.GetEntities(2);
            var cadSurfs = new int[dimTags.Length / 2][];
            for (int i = 0, surfCount = 0; i < dimTags.Length; i += 2, ++surfCount)
            {
                var points = CreateDictOfPoints(dimTags[i], dimTags[i + 1]);
                var data = Controller.Gmsh.Model.Mesh.GetElements(dimTags[i], dimTags[i + 1]);
                var surfaces = new List<int>();
                for (var j = 0; j < data.Item1.Length; ++j)
                {
                    var lines = new Dictionary<string, int>();
                    var properties = Controller.Gmsh.Model.Mesh.GetElementProperties(data.Item1[j]);
                    if (properties.numNodes != 3)
                        throw new Exception("Пока поддерживаются грани с 3 узлами");
                    for (var k = 0; k < data.Item2[j].Length; ++k)
                        surfaces.Add(CreateSurface(points, lines, data.Item3[j], k));
                }
                cadSurfs[surfCount] = surfaces.ToArray();
            }
            return cadSurfs;
        }

        private int CreateSurface(Dictionary<int, int> points, Dictionary<string, int> lines, IntPtr[] nodeTags, int elemIndex)
        {
            var idLines = new List<int>();
            for (int i = 1; i < 3; ++i)
                idLines.Add(GetLine(lines, points, (int)nodeTags[elemIndex * 3 + i - 1], (int)nodeTags[elemIndex * 3 + i]));
            idLines.Add(GetLine(lines, points, (int)nodeTags[elemIndex * 3 + 3 - 1], (int)nodeTags[elemIndex * 3]));

            var loop = Controller.Gmsh.Model.Occ.AddCurveLoop(idLines.ToArray());
            return Controller.Gmsh.Model.Occ.AddPlaneSurface(new int[] { loop });
        }

        private Dictionary<int, int> CreateDictOfPoints(int dim, int tag)
        {
            var dict = new Dictionary<int, int>();
            var data = Controller.Gmsh.Model.Mesh.GetNodes(dim, tag, false, false);
            for (var i = 0; i < data.Item1.Length; ++i)
            {
                var stride = i * 3;
                var number = (int)data.Item1[i];
                var idPoint = Controller.Gmsh.Model.Occ.AddPoint(data.Item2[stride], data.Item2[stride + 1], data.Item2[stride + 2], 0, number);
                dict.Add(number, idPoint);
            }
            return dict;
        }

        private int GetLine(Dictionary<string, int> lines, Dictionary<int, int> points, int point0Index, int point1Index)
        {
            var p0 = points[point0Index];
            var p1 = points[point1Index];
            var lineId = 0;
            var key = $"{p0} {p1}";
            var altKey = $"{p1} {p0}";
            if (lines.ContainsKey(key))
                lineId = lines[key];
            else if (lines.ContainsKey(altKey))
                lineId = -lines[altKey];
            else
            {
                lineId = Controller.Gmsh.Model.Occ.AddLine(p0, p1);
                lines.Add(key, lineId);
            }
            return lineId;
        }

        private void AddVolumes(int[][] cadSurfaces)
        {
            for (var i = 0; i < cadSurfaces.Length; ++i)
            {
                var shell = Controller.Gmsh.Model.Occ.AddSurfaceLoop(cadSurfaces[i]);
                Controller.Gmsh.Model.Occ.AddVolume(new int[] { shell });
            }
            Controller.Gmsh.Model.Occ.Synchronize();
        }
    }
}
