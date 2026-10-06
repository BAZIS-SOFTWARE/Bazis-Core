using GmshApi;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.IO;
using Model.IO.STL;
using OperationalController.GmshController;
using Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        

        public void ImportCAD(string projectFile)
        {
            ImportGeometry(projectFile);
        }

        /// <summary>
        /// Импортирует CAD-геометрию и заменяет текущий проект после успешного чтения.
        /// </summary>
        public void ImportGeometry(string projectFile)
        {
            if (string.IsNullOrWhiteSpace(projectFile))
                throw new ArgumentException("Geometry file path is not specified.", nameof(projectFile));
            if (Regex.IsMatch(projectFile, "(([а-яА-Я])|([ ]))"))
                throw new ArgumentException("Geometry file path contains unsupported characters.", nameof(projectFile));
            if (modelView.HasOpenUpdate)
                throw new InvalidOperationException("Complete the view update before replacing the project.");

            var path = Path.GetDirectoryName(projectFile);
            var projectPath = Path.Combine(path ?? string.Empty, "новый_проект.bpf2");
            var data = new ProjectData(projectPath);

            GmshController.Gmsh.Clear();
            GmshController.Gmsh.Open(projectFile);
            GmshController.MergeGeometry();
            GmshController.Gmsh.Model.Mesh.ImportStl();

            GmshController.CreateControlPoints().ForEach(point => data.ModelData.ObjectData.PointsSet.Add(point.Number, point));
            GmshController.CreateLines().ForEach(curve => data.ModelData.ObjectData.CurveSet.Add(curve.Number, curve));
            var surfaceData = GmshController.CreateSurfaceData();
            foreach (var surface in surfaceData.Item1)
                data.ModelData.ObjectData.SurfaceSet.Add(surface.Key, surface.Value);
            foreach (var volume in surfaceData.Item2)
                data.ModelData.VolumeData.Add(volume);

            GmshController.Gmsh.Model.Mesh.Clear();
            Replace(data, projectPath, ProjectChange.ProjectChangeKind.Loaded);
        }

        //private void PresentVolumeInfo(IGmshController gmshController)
        //{
        //    if (gmshController.Gmsh?.Model?.GetDimension() > 2)
        //    {
        //        var dimTags = gmshController.Gmsh.Model.GetEntities(3);

        //        for (var i = 0; i < dimTags.Length; i += 2)
        //        {
        //            var dim = dimTags[i];
        //            var tag = dimTags[i + 1];
        //            var data = gmshController.Gmsh.Model.GetAdjacencies(dim, tag);
        //            var downward = data.Item2;

        //            var surfNumbers = string.Join(" ", downward);


        //            var name = gmshController.Gmsh.Model.GetEntityName(dim, tag);
        //            if (name == "" | name == null)
        //                name = $"Объем_{tag}";
        //            else
        //                name = name.Replace("Shapes/", "");

        //            var surfs = new SurfaceFigure[downward.Length];
        //            for (int j = 0; j < downward.Length; j++)
        //            {
        //                var numb = downward[j];
        //                var surf = project.ModelData.ObjectData.SurfaceCollection.Find(numb);
        //                surfs[j] = surf;
        //            }

        //            var vol = new VolumeFigure(tag, surfs);
        //            vol.Name = name;
        //            project.ModelData.ObjectData.VolumeData.Add(vol);
        //        }
        //    }
        //}

        //private void PresentVolumeInfo()
        //{
        //    foreach (var item in project.ModelData.ObjectData.SurfaceSet)
        //    {
        //        // кодовое слово SurfaceSet
        //        if (item.Key.Contains("SurfaceSet"))
        //        {
        //            var set = item.Value;
        //            var vol = new VolumeFigure(set.Number, set.Values.ToArray());
        //            vol.Name = $"Объем_{set.Number}";
        //            project.ModelData.ObjectData.VolumeData.Add(vol);
        //        }

        //    }        
        //}
    }
}
