using Model.IO;
using Model.IO.STL;
using Model.MeshObjects;
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

        public void GenerateMesh(int elemDim)
        {
            if (GmshController == null)
                throw new Exception("Генератор сетки не загружен");

            if (elemDim == 1)
            {

                GmshController.Gmsh.Model.Mesh.Generate(1);

                var nodes = GmshController.GetNodes();

                foreach (var item in nodes)
                    ModelData.ObjectData.NodesSet.Add(item.Number, item);

                foreach (var item in GmshController.GetMeshSets<Beam>(nodes))
                    ModelData.ObjectData.E1DCollection.Add(item);
            }
            else if (elemDim == 2)
            {
                GmshController.Gmsh.Model.Mesh.Generate(2);

                var nodes = GmshController.GetNodes();

                foreach (var item in nodes)
                    ModelData.ObjectData.NodesSet.Add(item.Number, item);

                foreach (var item in GmshController.GetMeshSets<Beam>(nodes))
                    ModelData.ObjectData.E1DCollection.Add(item);
                foreach (var item in GmshController.GetMeshSets<Element2D>(nodes))
                    ModelData.ObjectData.E2DCollection.Add(item);

            }

            else
            {
                GmshController.Gmsh.Model.Mesh.Generate(3);

                var nodes = GmshController.GetNodes();

                foreach (var item in nodes)
                    ModelData.ObjectData.NodesSet.Add(item.Number, item);
                foreach (var item in GmshController.GetMeshSets<Beam>(nodes))
                    ModelData.ObjectData.E1DCollection.Add(item);
                foreach (var item in GmshController.GetMeshSets<Element2D>(nodes))
                    ModelData.ObjectData.E2DCollection.Add(item);
                foreach (var item in GmshController.GetMeshSets<Element3D>(nodes))
                    ModelData.ObjectData.E3DCollection.Add(item);
            }
        }
    }
}
