using Model.GeometryObjects;
using Model.IO;
using Model.IO.STL;
using Model.MeshObjects;
using Model.ObjectsCollections;
using OperationalController.GmshController;
using Project;
using Project.Tasks;
using System;
using System.IO;
using System.Linq;

namespace OperationalController
{
    public partial class ProjectController
    {
/// <inheritdoc/>

        public void HideMesh(int elemDim, IGmshController controller)
        {
            if (elemDim == 1)
            {
                foreach (var item in ModelData.ObjectData.E1DCollection.Objects)
                {
                    foreach (var element in item)
                    {
                        var curveTag = controller.Gmsh.Model.Mesh.GetElement(element.Value.Number).Item4;
                        var viewState = ModelData.ObjectData.CurveSet[curveTag].ViewState;
                        element.Value.ViewState = viewState;
                    }
                }

            }
            else if (elemDim == 2)
            {
                foreach (var item in ModelData.ObjectData.E2DCollection.Objects)
                {
                    foreach (var element in item)
                    {
                        var surfaceTag = controller.Gmsh.Model.Mesh.GetElement(element.Value.Number).Item4;
                        var viewState = ModelData.ObjectData.SurfaceSet[surfaceTag].ViewState;
                        element.Value.ViewState = viewState;
                    }
                }
            }

            else
            {
                var volumes = ModelData.VolumeData.ToDictionary(v => v.Number, k => k.ViewState);
                foreach (var item in ModelData.ObjectData.E3DCollection.Objects)
                {
                    foreach (var element in item)
                    {
                        var volumeTag = controller.Gmsh.Model.Mesh.GetElement(element.Value.Number).Item4;
                        element.Value.ViewState = volumes[volumeTag];
                    }
                }
            }
        }
    }
}
