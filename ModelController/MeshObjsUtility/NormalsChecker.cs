using ModelController.ModelScenePresentator.GlObjsPresenters;
using System;
using System.Collections.Generic;
using System.Text;
using Model.MeshObjects;
using Model.Interfaces.MeshObjects;
using Geometry;
using System.Collections;
using ModelController.ModelScenePresentator.GlObjsVertexCreators;
using System.IO;
using System.Reflection;

namespace ModelController.MeshObjsUtility
{
    /// <summary>
    /// Класс проверяет корректность нормалей 3D элементов в классе SurfaceObjsPresenter, можно использовать в тестах и сохранять лог
    /// </summary>
    public class NormalsChecker
    {
        private class ElementData
        {
            public List<Point3D> SurfaceNormals { get; set; }
            public List<Point3D> SurfacePoints { get; set; }
            public List<Point3D> SurfaceEdges { get; set; }
            public Point3D Center { get; set; }

            public ElementData()
            {
                SurfaceNormals = new List<Point3D>();
                SurfacePoints = new List<Point3D>();
                SurfaceEdges = new List<Point3D>();

                Center = new Point3D();
            }
        }

        private StreamWriter LogWriter { get; set; }

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
            Presenter = presenter;
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

            LogWriter = string.IsNullOrEmpty(Log) ? null : new StreamWriter(Log);
            for (var i = 1; i < separators.Length; ++i)
            {
                var elData = GetElementData(separators[i - 1], separators[i], coords, normals);
                CheckElement(elData, i - 1);
            }
            LogWriter?.Close();
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

            var points = elementData.SurfacePoints;
            var normals = elementData.SurfaceNormals;
            var edges = elementData.SurfaceEdges;

            for (var i = 0; i < triangleCount; ++i)
            {
                for (var j = 0; j < 9; j += 3)
                {
                    var stride = (start + i) * 9 + j;

                    var point = new Point3D(rawCoords[stride], rawCoords[stride + 1], rawCoords[stride + 2]);
                    elementData.Center = elementData.Center.Sum(point);
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
            elementData.Center = elementData.Center.Mult(scale);
            return elementData;
        }

        private void CheckElement(ElementData elData, int index)
        {
            var triangles = elData.SurfaceNormals.Count;
            WriteElementKind(triangles, index);

            for (var i = 0; i < triangles; ++i)
            {
                var edge = Vector.GetVectorNorm(elData.SurfaceEdges[i]);
                var normal = Vector.GetVectorNorm(elData.SurfaceNormals[i]);
                var length = Vector.GetVectorLenght(normal);

                var reference = elData.SurfacePoints[i].Sub(elData.Center);
                reference = Vector.GetVectorNorm(reference);

                var dpEdge = Vector.DotProd(edge, normal);
                var dpRef = Vector.DotProd(reference, normal);

                ZeroLengths = length < 1e-6f ? ZeroLengths + 1 : ZeroLengths;
                NotOthogonals = dpEdge > 1e-4f ? NotOthogonals + 1 : NotOthogonals;
                Inverted = dpRef < 0 ? Inverted + 1 : Inverted;

                WriteNormalInfo(i, length, dpEdge, dpRef);
            }
            LogWriter?.WriteLine();
        }

        private void WriteElementKind(int triangles, int index)
        {
            if (LogWriter != null)
            {
                var elemKind = IdentifyByTriangles(triangles);
                LogWriter.WriteLine($"{elemKind.ToString()} {index}");
            }
        }

        private void WriteNormalInfo(int index, float length, float dpEdge, float dpRef)
        {
            if (LogWriter != null)
            {
                var message = length < 1e-6f ? "(Имеет нулевую длину) " : "";
                message += dpEdge > 1e-4f ? "(Не перпендикулярна плоскости) " : "";
                message += dpRef < 0 ? "(Инвертирована) " : "";

                message = string.IsNullOrEmpty(message) ? "Корректна" : message;
                LogWriter.WriteLine($"Нормаль поверхности {index}: {message}");
            }
        }
    }
}
