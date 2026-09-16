using System;

namespace OperationalController
{
    public partial class ProjectController
    {
        /// <summary>
        /// Возвращает путь к загруженной геометрической модели.
        /// </summary>
        /// <returns>Путь к файлу геометрической модели.</returns>
        public string GetGeometryFilePath()
        {
            if (!IsGeometryInitialized)
                throw new InvalidOperationException("Geometry controller is not initialized.");

            return GmshController.Gmsh.Model.GetFileName();
        }
    }
}
