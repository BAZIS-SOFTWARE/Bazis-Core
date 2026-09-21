using GmshApi;
using System;

namespace OperationalController
{
    /// <summary>
    /// Хранит параметры построения сетки.
    /// </summary>
    public sealed class MeshSettings
    {
        /// <summary>
        /// Минимальный размер элемента сетки.
        /// </summary>
        public double MinimumSize { get; }

        /// <summary>
        /// Максимальный размер элемента сетки.
        /// </summary>
        public double MaximumSize { get; }

        /// <summary>
        /// Алгоритм построения двумерной сетки.
        /// </summary>
        public MeshAlgorithm2D Algorithm2D { get; }

        /// <summary>
        /// Алгоритм построения трёхмерной сетки.
        /// </summary>
        public MeshAlgorithm3D Algorithm3D { get; }

        /// <summary>
        /// Масштабный коэффициент размера элементов сетки.
        /// </summary>
        public double SizeFactor { get; }

        /// <summary>
        /// Инициализирует параметры построения сетки.
        /// </summary>
        public MeshSettings(double minimumSize, double maximumSize, MeshAlgorithm2D algorithm2D, MeshAlgorithm3D algorithm3D, double sizeFactor)
        {
            MinimumSize = minimumSize;
            MaximumSize = maximumSize;
            Algorithm2D = algorithm2D;
            Algorithm3D = algorithm3D;
            SizeFactor = sizeFactor;
        }
    }

    public partial class ProjectController
    {
        /// <summary>
        /// Возвращает параметры построения сетки.
        /// </summary>
        public MeshSettings GetMeshSettings()
        {
            if (!IsGeometryInitialized)
                throw new InvalidOperationException("Geometry controller is not initialized.");

            var option = GmshController.Gmsh.Option;
            var minimumSize = option.GetNumber("Mesh.MeshSizeMin");
            var maximumSize = option.GetNumber("Mesh.MeshSizeMax");
            var algorithm2D = (MeshAlgorithm2D)option.GetNumber("Mesh.Algorithm");
            var algorithm3D = (MeshAlgorithm3D)option.GetNumber("Mesh.Algorithm3D");
            var sizeFactor = option.GetNumber("Mesh.MeshSizeFactor");

            return new MeshSettings(minimumSize, maximumSize, algorithm2D, algorithm3D, sizeFactor);
        }
    }
}
