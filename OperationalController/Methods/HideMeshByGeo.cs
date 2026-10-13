using Model.GeometryObjects;
using Model.Interfaces;
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

        public void HideMesh(int elemDim)
        {
            using (ModelView.BeginUpdate())
            {
                if (elemDim == 1)
                {
                    foreach (var item in ModelData.ObjectData.E1DCollection.Objects)
                    {
                        foreach (var element in item)
                        {
                            var curveTag = GmshController.Gmsh.Model.Mesh.GetElement(element.Value.Number).Item4;
                            var visible = ModelView.GetVisible(ObjType.Кривая, curveTag);
                            ModelView.SetVisible(ObjType.Элемент1D, [element.Value.Number], visible);

                            foreach (var node in element.Value.GetVertexes())
                                ModelView.SetVisible(ObjType.Узел, [node.Number], visible);

                        }
                    }

                }
                else if (elemDim == 2)
                {
                    foreach (var item in ModelData.ObjectData.E2DCollection.Objects)
                    {
                        foreach (var element in item)
                        {
                            var surfaceTag = GmshController.Gmsh.Model.Mesh.GetElement(element.Value.Number).Item4;
                            var visible = ModelView.GetVisible(ObjType.Поверхность, surfaceTag);
                            ModelView.SetVisible(ObjType.Элемент2D, [element.Value.Number], visible);

                            foreach (var node in element.Value.GetVertexes())
                                ModelView.SetVisible(ObjType.Узел, [node.Number], visible);
                        }
                    }
                }

                else
                {
                    ModelView.ShowAll();
                }
            }
        }
    }
}
