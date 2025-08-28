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
    public partial class Controller
    {
        /// <inheritdoc/>

        public void GenerateMesh(int elemDim, IGmshController controller)
        {
            var nodes = controller.GetNodes();

            foreach (var item in nodes)
                ModelData.ObjectData.NodesSet.Add(item.Number, item);

            if (elemDim == 1)
            {

                controller.Gmsh.Model.Mesh.Generate(1);
                foreach (var item in controller.GetMeshSets<Beam>(nodes))
                    ModelData.ObjectData.E1DCollection.Add(item);
            }
            else if (elemDim == 2)
            {
                controller.Gmsh.Model.Mesh.Generate(2);
                foreach (var item in controller.GetMeshSets<Beam>(nodes))
                    ModelData.ObjectData.E1DCollection.Add(item);
                foreach (var item in controller.GetMeshSets<Element2D>(nodes))
                    ModelData.ObjectData.E2DCollection.Add(item);

            }

            else
            {
                controller.Gmsh.Model.Mesh.Generate(3);
                foreach (var item in controller.GetMeshSets<Beam>(nodes))
                    ModelData.ObjectData.E1DCollection.Add(item);
                foreach (var item in controller.GetMeshSets<Element2D>(nodes))
                    ModelData.ObjectData.E2DCollection.Add(item);
                foreach (var item in controller.GetMeshSets<Element3D>(nodes))
                    ModelData.ObjectData.E3DCollection.Add(item);
            }
        }
    }
}
