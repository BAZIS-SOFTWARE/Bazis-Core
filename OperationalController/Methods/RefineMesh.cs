using System;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Уплотняет элементы сетки и синхронизирует данные модели.
        /// </summary>
        public void RefineMesh()
        {
            if (!IsGeometryInitialized)
                throw new InvalidOperationException("Geometry controller is not initialized.");

            GmshController.Gmsh.Model.Mesh.Refine();
            SynchronizeGeometryMesh();
        }
    }
}
