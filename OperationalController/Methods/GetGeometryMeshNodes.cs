using Model.MeshObjects;
using System;
using System.Collections.Generic;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Строит сетку заданной размерности и возвращает узлы геометрических сущностей.
        /// </summary>
        /// <param name="dimension">Размерность геометрических сущностей.</param>
        /// <param name="entityNumbers">Номера геометрических сущностей.</param>
        /// <param name="includeBoundary">Включать граничные узлы.</param>
        /// <returns>Узлы сетки геометрических сущностей.</returns>
        public IEnumerable<Node> GetGeometryMeshNodes(int dimension, IEnumerable<int> entityNumbers, bool includeBoundary = false)
        {
            if (!IsGeometryInitialized)
                throw new InvalidOperationException("Geometry controller is not initialized.");

            if (dimension < 1 || dimension > 3)
                throw new ArgumentOutOfRangeException(nameof(dimension), "Geometry dimension must be from 1 to 3.");

            if (entityNumbers == null)
                throw new ArgumentNullException(nameof(entityNumbers));

            GmshController.Gmsh.Model.Mesh.Generate(dimension);

            var nodeNumbers = new HashSet<int>();
            foreach (var entityNumber in entityNumbers)
            {
                var nodes = GmshController.GetNodes(dimension, entityNumber, includeBoundary);
                foreach (var node in nodes)
                    if (nodeNumbers.Add(node.Number))
                        yield return node;
            }
        }
    }
}
