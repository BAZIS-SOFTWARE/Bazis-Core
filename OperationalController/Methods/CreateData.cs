using Geometry;
using Model;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.IO;
using Model.IO.STL;
using Model.MeshObjects;
using Project;
using Project.Tasks;
using System;
using System.Collections.Generic;
using System.IO;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void Extrude3DForm2D(Point3D vector, int layers, IEnumerable<IElement2D> elements)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>

        public void CreateElementsSet(string setName, int dim)
        {
            if (dim < 1 | dim > 3)
                throw new Exception("Неправильно указана размерность элементов");

            if (dim == 1)
                ModelData.ObjectData.E1DCollection.Add(setName);
        }
        /// <inheritdoc/>
        public void CreateElement(string setName, int dim, int[] nodeNumbers)
        {
            if (dim < 1 | dim > 3)
                throw new Exception("Неправильно указана размерность элементов");

            var nodes = new Node[nodeNumbers.Length];

            for (int i = 0; i < nodes.Length; i++)
            {
                var number = nodeNumbers[i];
                nodes[i] = new Node(number,
                    ModelData.ObjectData.NodesSet[number].Position);
            }

            if (dim == 1)
            {
                var max = 0;
                if(ModelData.ObjectData.Count(ObjType.Элемент1D) != 0)
                    max = ModelData.ObjectData.GetMaxNumber(ObjType.Элемент1D) + 1;

                var e1d = new Beam(max, nodes);
                ModelData.ObjectData.E1DCollection.Add(setName, e1d);
            }

        }

        /// <inheritdoc/>

        public void CreateNode(int number, float x, float y, float z)
        {
            if (ModelData.ObjectData.NodesSet.ContainsKey(number))
                throw new Exception("Обект с таким номером уже добавлен в набор");
            
            ModelData.ObjectData.NodesSet.
                Add(number, new Node(number, 
                new Geometry.Point3D(x,y,z)));
        }
    }
}
