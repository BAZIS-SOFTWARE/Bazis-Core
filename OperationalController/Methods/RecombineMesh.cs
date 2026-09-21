using System;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Рекомбинирует элементы сетки и синхронизирует данные модели.
        /// </summary>
        public void RecombineMesh()
        {
            if (!IsGeometryInitialized)
                throw new InvalidOperationException("Geometry controller is not initialized.");

            GmshController.Gmsh.Model.Mesh.Recombine();
            SynchronizeGeometryMesh();
        }
    }
}
