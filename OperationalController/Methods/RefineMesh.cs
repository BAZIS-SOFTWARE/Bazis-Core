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
            using (BeginChanges())
            {
                if (!IsGeometryInitialized)
                    throw new InvalidOperationException("Geometry controller is not initialized.");

                GmshController.Gmsh.Model.Mesh.Refine();
                SynchronizeGeometryMesh();
            }
        }
    }
}
