using Geometry;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Класс экструзии сетки из 2д поверхности методом вращения
    /// </summary>
    public class RotateExtruder : MeshExtruder
    {
        /// <summary>
        /// Создает экструзию 3д элементов методом поворота вдоль указанного локального вектора и точке начала координат 
        /// </summary>
        /// <param name="curLayer">Текущий слой для экструзии</param>
        /// <param name="origin">Точка начала координат</param>
        /// <param name="rotAxis">Локальная ось поворота</param>
        /// <param name="segments">Число сегментов</param>
        /// <returns>Список 3д элементов</returns>
        /// <exception cref="ArgumentException">Число сегментов не должно быть меньше 3</exception>
        public virtual List<List<Element3D>> Extrude(IEnumerable<Element2D> curLayer, Vector3 origin, Vector3 rotAxis, int segments = 8)
        {
            if (segments < 3)
                throw new ArgumentException("Количество сегментов не должно быть меньше 3");

            rotAxis = Vector3.Normalize(rotAxis);
            var tetLayers = new List<List<Element3D>>();
            curLayer = SortFirstLayer(curLayer);
            var firstLayer = CreateFirstSurface(curLayer);
            curLayer = firstLayer;

            var offset = 2 * MathF.PI / segments;
            var rotMatrix = Matrix4x4.CreateTranslation(origin) 
                              * Matrix4x4.CreateFromAxisAngle(rotAxis, offset) 
                              * Matrix4x4.CreateTranslation(-origin);
            rotMatrix = Matrix4x4.Transpose(rotMatrix);

            List<Element2D> nextLayer = null;
            for (var i = 0; i < segments - 1; ++i)
            {
                nextLayer = CreateNextLayer(curLayer, rotMatrix);
                var layer = CreateLayerTetrahedrons(curLayer, nextLayer);
                tetLayers.Add(layer);
                curLayer = nextLayer;
            }
            var lastLayer = CreateLayerTetrahedrons(nextLayer, firstLayer);
            tetLayers.Add(lastLayer);
            return tetLayers;
        }
        /// <summary>
        /// Строит следующий слой 2д элементов на основе текущего используя матрицу прирощения угла
        /// </summary>
        /// <param name="curLayer">Текущий слой 2д элементов</param>
        /// <param name="transform">Матрица прирощения угла</param>
        /// <returns>Список 2д элементов</returns>
        protected List<Element2D> CreateNextLayer(IEnumerable<Element2D> curLayer, Matrix4x4 transform)
        {
            var nextLayer = new List<Element2D>();

            var visited = new Dictionary<int, Node>();
            foreach (var element in curLayer)
            {
                var points = new List<Node>();
                foreach (var node in element.GetVertexes())
                {
                    if (!visited.ContainsKey(node.Number))
                    {
                        var vec = new Vector4(node.Position._x, node.Position._y, node.Position._z, 1);
                        var coords = Vector4.Transform(vec, transform);
                        var point = new Node(nodeTag++, new Point3D(coords.X, coords.Y, coords.Z));
                        Nodes.Add(point.Number, point);
                        if (IsPointCoincident(node, coords))
                        {
                            visited.Add(node.Number, node);
                            points.Add(node);
                        }
                        else
                        {
                            visited.Add(node.Number, point);
                            points.Add(point);
                        }
                    }
                    else
                        points.Add(visited[node.Number]);
                }
                var pArray = points.ToArray();
                Element2D elem = pArray.Length == 3 ? new Triangle_c(element.Number, pArray) : new Quad(element.Number, pArray);
                nextLayer.Add(elem);
            }
            return nextLayer;
        }
        /// <summary>
        /// Проверяет коинцидентность узла вектору
        /// </summary>
        /// <param name="node">Узел</param>
        /// <param name="newPosition">Вектор</param>
        /// <returns>Флаг коинцидентности</returns>
        protected bool IsPointCoincident(Node node, Vector4 newPosition)
        {
            var xState = Math.Abs(node.Position._x - newPosition.X) < 1e-4;
            var yState = Math.Abs(node.Position._y - newPosition.Y) < 1e-4;
            var zState = Math.Abs(node.Position._z - newPosition.Z) < 1e-4;
            return xState && yState && zState;
        }
    }
}
