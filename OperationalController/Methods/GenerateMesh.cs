using Model.IO;
using Model.IO.STL;
using GmshApi;
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

        /// <summary>
        /// Устанавливает минимальный размер элемента сетки.
        /// </summary>
        public void SetMeshMinimumSize(double size)
        {
            GmshController.Gmsh.Option.SetNumber("Mesh.MeshSizeMin", size);
        }

        /// <summary>
        /// Устанавливает максимальный размер элемента сетки.
        /// </summary>
        public void SetMeshMaximumSize(double size)
        {
            GmshController.Gmsh.Option.SetNumber("Mesh.MeshSizeMax", size);
        }

        /// <summary>
        /// Устанавливает алгоритм построения двумерной сетки.
        /// </summary>
        public void SetMeshAlgorithm2D(MeshAlgorithm2D algorithm)
        {
            GmshController.Gmsh.Option.SetNumber("Mesh.Algorithm", (double)algorithm);
        }

        /// <summary>
        /// Устанавливает алгоритм построения трёхмерной сетки.
        /// </summary>
        public void SetMeshAlgorithm3D(MeshAlgorithm3D algorithm)
        {
            GmshController.Gmsh.Option.SetNumber("Mesh.Algorithm3D", (double)algorithm);
        }

        /// <summary>
        /// Устанавливает масштабный коэффициент размера элементов сетки.
        /// </summary>
        public void SetMeshSizeFactor(double factor)
        {
            GmshController.Gmsh.Option.SetNumber("Mesh.MeshSizeFactor", factor);
        }

        /// <summary>
        /// Экспортирует геометрическую модель.
        /// </summary>
        /// <param name="filePath">Путь к экспортируемому файлу.</param>
        public void ExportGeometry(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Geometry file path is not specified.", nameof(filePath));

            GmshController.Gmsh.Write(filePath);
        }

        /// <summary>
        /// Очищает сетку геометрического контроллера.
        /// </summary>
        public void ClearGeometryMesh()
        {
            GmshController.Gmsh.Model.Mesh.Clear();
        }

        /// <summary>
        /// Очищает сетку геометрических сущностей заданной размерности.
        /// </summary>
        /// <param name="dimension">Размерность геометрических сущностей.</param>
        public void ClearGeometryMesh(int dimension)
        {
            if (dimension < 1 || dimension > 3)
                throw new ArgumentOutOfRangeException(nameof(dimension), "Geometry dimension must be from 1 to 3.");

            var dimTags = GmshController.Gmsh.Model.GetEntities(dimension);
            GmshController.Gmsh.Model.Mesh.Clear(dimTags);
        }

        /// <summary>
        /// Возвращает последнюю ошибку геометрического контроллера.
        /// </summary>
        public string GetGeometryLastError()
        {
            return GmshController.Gmsh.Logger.GetLastError();
        }

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

                MarkModelSurfaces();
            }
        }
    }
}
