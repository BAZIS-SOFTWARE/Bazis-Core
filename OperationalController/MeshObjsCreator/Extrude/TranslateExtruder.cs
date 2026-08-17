using Geometry;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController.MeshObjsCreator.Extrude
{
    /// <summary>
    /// Класс экструзии сетки из 2д поверхности методом переноса
    /// </summary>
    public class TranslateExtruder : MeshExtruder
    {
        /// <summary>
        /// Создает экструзию 3д элементов по указанному слою 2д элементов
        /// </summary>
        /// <param name="curLayer">Текущий слой 2д элементов</param>
        /// <param name="dir">Вектор направления экструзии</param>
        /// <param name="layers">Количество слоев</param>
        /// <param name="layerSize">Высота слоя</param>
        /// <returns>Список 3д элементов</returns>
        public List<List<Element3D>> Extrude(IEnumerable<Element2D> curLayer, Point3D dir, int layers = 1, float layerSize = 1)
        {
            dir = Vector.GetVectorNorm(dir);
            var layerDir = dir.Mult(layerSize);

            var tetLayers = new List<List<Element3D>>();

            var sortedLayer = SortFirstLayer(curLayer);
            curLayer = CreateCopy(sortedLayer);

            for (var i = 0; i < layers; ++i)
            {
                var nextLayer = CreateNextLayer(curLayer, layerDir);
                var layer = CreateLayerTetrahedrons(curLayer, nextLayer);
                tetLayers.Add(layer);
                curLayer = nextLayer;
            }
            return tetLayers;
        }
        /// <summary>
        /// Создает трансфинитную экструзию 3д элементов по указанному слою 2д элементов
        /// </summary>
        /// <param name="curLayer">Текущий слой 2д элементов</param>
        /// <param name="dir">Вектор направления экструзии</param>
        /// <param name="layers">Количество слоев</param>
        /// <param name="layerSize">Высота слоя</param>
        /// <returns>Список 3д элементов</returns>
        public List<List<Element3D>> ExtrudeTransfinite(IEnumerable<Element2D> curLayer, Point3D dir, int layers = 1, float layerSize = 1)
        {
            dir = Vector.GetVectorNorm(dir);
            var layerDir = dir.Mult(layerSize);

            var tetLayers = new List<List<Element3D>>();

            var sortedLayer = SortFirstLayer(curLayer);
            curLayer = CreateCopy(sortedLayer);

            for (var i = 0; i < layers; ++i)
            {
                var nextLayer = CreateNextLayer(curLayer, layerDir);
                var layer = CreateTransfiniteLayer(curLayer, nextLayer);
                tetLayers.Add(layer);
                curLayer = nextLayer;
            }
            return tetLayers;
        }
        /// <summary>
        /// Строит следующий слой 2д элементов на основе текущего используя вектор направления layerDir
        /// </summary>
        /// <param name="curLayer">Текущий слой 2д элементов</param>
        /// <param name="layerDir">Вектор направления выращивания</param>
        /// <returns>Список 2д элементов</returns>
        private List<Element2D> CreateNextLayer(IEnumerable<Element2D> curLayer, Point3D layerDir)
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
                        var coords = node.Position.Sum(layerDir);
                        var point = new Node(NodeTag, coords);
                        Nodes.Add(NodeTag++, point);
                        points.Add(point);
                        visited.Add(node.Number, point);
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
    }
}
