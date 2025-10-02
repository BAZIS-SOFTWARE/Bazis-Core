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
using System.Xml.Linq;

namespace OperationalController
{
    public partial class Controller
    {
        /// <inheritdoc/>

        public void ImportCAD(string projectFile, IGmshController gmshController)
        {

            var path = System.IO.Path.GetDirectoryName(projectFile);
            var name = "новый_проект.bpf2";
            project = new ProjectData($"{path}//{name}");


            gmshController.Gmsh.Clear();
            gmshController.Gmsh.Open(projectFile);

            gmshController.Gmsh.Model.Mesh.ImportStl(); //обязательно для представления stl

            // TO DO спросить у Николая что думает по этой функции
            //var surfs = gmshController.Gmsh.Model.Occ.ImportShapes(projectFile);

            gmshController.CreateControlPoints().ForEach(
        x => project.ModelData.ObjectData.PointsSet.Add(x.Number, x));
            gmshController.CreateLines().ForEach(
                x => project.ModelData.ObjectData.CurveSet.Add(x.Number, x));
            gmshController.CreateSurfaceSets().ForEach(
x => project.ModelData.ObjectData.SurfaceCollection.Add(x));

            PresentVolumeInfo();
        }

        private void PresentVolumeInfo(IGmshController gmshController)
        {
            if (gmshController.Gmsh?.Model?.GetDimension() > 2)
            {
                var dimTags = gmshController.Gmsh.Model.GetEntities(3);

                for (var i = 0; i < dimTags.Length; i += 2)
                {
                    var dim = dimTags[i];
                    var tag = dimTags[i + 1];
                    var data = gmshController.Gmsh.Model.GetAdjacencies(dim, tag);
                    var downward = data.Item2;

                    var surfNumbers = string.Join(" ", downward);


                    var name = gmshController.Gmsh.Model.GetEntityName(dim, tag);
                    if (name == "" | name == null)
                        name = $"Объем_{tag}";
                    else
                        name = name.Replace("Shapes/", "");

                    var surfs = new SurfaceFigure[downward.Length];
                    for (int j = 0; j < downward.Length; j++)
                    {
                        var numb = downward[j];
                        var surf = project.ModelData.ObjectData.SurfaceCollection.Find(numb);
                        surfs[j] = surf;
                    }

                    var vol = new VolumeFigure(tag, surfs);
                    vol.Name = name;
                    project.ModelData.ObjectData.VolumeData.Add(vol);
                }
            }
        }

        private void PresentVolumeInfo()
        {
            foreach (var item in project.ModelData.ObjectData.SurfaceCollection)
            {
                // кодовое слово SurfaceSet
                if (item.Key.Contains("SurfaceSet"))
                {
                    var set = item.Value;
                    var vol = new VolumeFigure(set.Number, set.Values.ToArray());
                    vol.Name = $"Объем_{set.Number}";
                    project.ModelData.ObjectData.VolumeData.Add(vol);
                }

            }        
        }
    }
}
