using OperationalController.ModelScenePresentator.GlObjsPresenters;
using System;
using System.Collections.Generic;
using System.Text;
using Model.MeshObjects;
using Model.Interfaces.MeshObjects;
using Geometry;
using System.Collections;
using OperationalController.ModelScenePresentator.GlObjsVertexCreators;
using System.IO;
using System.Reflection;

namespace OperationalController.MeshObjsUtility
{
    /// <summary>
    /// Класс проверяет корректность нормалей 3D элементов в классе SurfaceObjsPresenter, можно использовать в тестах и сохранять лог
    /// </summary>
    public class NormalsChecker
    {
        private class ElementData
        {
            public readonly List<Point3D> surfaceNormals;
            public readonly List<Point3D> surfacePoints;
            public readonly List<Point3D> surfaceEdges;
            public Point3D center;

            public ElementData()
            {
                surfaceNormals = new List<Point3D>();
                surfacePoints = new List<Point3D>();
                surfaceEdges = new List<Point3D>();

                center = new Point3D();
            }
        }

        private StreamWriter logWriter;
        private StringBuilder logStorage;

        private SurfaceObjsPresenter<ElementSurface, Node> Presenter { get; set; }
        /// <summary>
        /// Количество инвертированных нормалей
        /// </summary>
        public int Inverted { get; private set; }
        /// <summary>
        /// Количество нормалей, которые не перпендикулярны поверхности
        /// </summary>
        public int NotOthogonals { get; private set; }
        /// <summary>
        /// Количество нормалей с нулевой длиной
        /// </summary>
        public int ZeroLengths { get; private set; }
        /// <summary>
        /// Путь до файла с отчетом
        /// </summary>
        public string Log { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="presenter">SurfaceObjsPresenter</param>
        public NormalsChecker(SurfaceObjsPresenter<ElementSurface, Node> presenter)
        {
            Presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        }
        /// <summary>
        /// Запустить проверку нормалей
        /// </summary>
        public void Check()
        {
            var inds = Presenter.CreateIndexes();
            var coords = Presenter.CreateVertexes(inds.Item2, "координаты");
            var normals = Presenter.CreateVertexes(inds.Item2, "нормаль");
            var separators = Presenter.CreateSeparators();

            if (!string.IsNullOrEmpty(Log))
            {
                logWriter = new StreamWriter(Log);
                logStorage = new StringBuilder(100);
            }
            for (var i = 1; i < separators.Length; ++i)
            {
                var elData = GetElementData(separators[i - 1], separators[i], coords, normals);
                CheckElement(elData, i - 1);
            }
            logWriter?.Close();
        }
        /// <summary>
        /// Определить тип элемента по количеству треугольников
        /// </summary>
        /// <param name="triangles">Количество треугольников</param>
        /// <returns>Тип элемента</returns>
        /// <exception cref="Exception">Неподдерживаемый элемент</exception>
        public static ElementKind IdentifyByTriangles(int triangles)
        {
            switch (triangles)
            {
                case 1:
                    return ElementKind.Треугольник;
                case 2:
                    return ElementKind.Квадрат;
                case 4:
                    return ElementKind.Тетра;
                case 8:
                    return ElementKind.Пента;
                case 12:
                    return ElementKind.Гекса;
                default:
                    throw new Exception("Неизвестный элемент");
            }
        }

        private ElementData GetElementData(int start, int end, float[] rawCoords, float[] rawNormals)
        {
            var elementData = new ElementData();

            var triangleCount = end - start;

            var points = elementData.surfacePoints;
            var normals = elementData.surfaceNormals;
            var edges = elementData.surfaceEdges;

            for (var i = 0; i < triangleCount; ++i)
            {
                for (var j = 0; j < 9; j += 3)
                {
                    var stride = (start + i) * 9 + j;

                    var point = new Point3D(rawCoords[stride], rawCoords[stride + 1], rawCoords[stride + 2]);
                    elementData.center = elementData.center.Sum(point);
                    if (j == 0)
                    {
                        points.Add(point);
                        normals.Add(new Point3D(rawNormals[stride], rawNormals[stride + 1], rawNormals[stride + 2]));
                    }
                    else if (j == 3)
                    {
                        var lastPoint = points[points.Count - 1];
                        edges.Add(point.Sub(lastPoint));
                    }
                }
            }
            var scale = 1f / (triangleCount * 3);
            elementData.center = elementData.center.Mult(scale);
            return elementData;
        }

        private void CheckElement(ElementData elData, int index)
        {
            var triangles = elData.surfaceNormals.Count;
            WriteElementKind(triangles, index);

            for (var i = 0; i < triangles; ++i)
            {
                var edge = Vector.GetVectorNorm(elData.surfaceEdges[i]);
                var normal = Vector.GetVectorNorm(elData.surfaceNormals[i]);
                var length = Vector.GetVectorLength(normal);

                var reference = elData.surfacePoints[i].Sub(elData.center);
                reference = Vector.GetVectorNorm(reference);

                var dpEdge = Vector.DotProd(edge, normal);
                var dpRef = Vector.DotProd(reference, normal);

                ZeroLengths = length < 1e-6f ? ZeroLengths + 1 : ZeroLengths;
                NotOthogonals = dpEdge > 1e-4f ? NotOthogonals + 1 : NotOthogonals;
                Inverted = dpRef < 0 ? Inverted + 1 : Inverted;

                WriteNormalInfo(i, length, dpEdge, dpRef);
            }
            logWriter?.WriteLine();
        }

        private void WriteElementKind(int triangles, int index)
        {
            if (logWriter != null)
            {
                var elemKind = IdentifyByTriangles(triangles);
                logWriter.WriteLine($"{elemKind.ToString()} {index}");
            }
        }

        private void WriteNormalInfo(int index, float length, float dpEdge, float dpRef)
        {
            if (logWriter != null)
            {
                if (length < 1e-6f)
                    logStorage.Append("(Имеет нулевую длину) ");
                if (dpEdge > 1e-4f)
                    logStorage.Append("(Не перпендикулярна плоскости) ");
                if (dpRef < 0)
                    logStorage.Append("(Инвертирована) ");
                if (logStorage.Length == 0)
                    logStorage.Append("Корректна");
                logWriter.WriteLine($"Нормаль поверхности {index}: { logStorage.ToString() }");
                logStorage.Clear();
            }
        }
    }
}
