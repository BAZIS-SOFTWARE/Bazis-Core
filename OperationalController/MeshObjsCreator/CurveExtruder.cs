using Geometry;
using Model.MeshObjects;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Класс экструзии сетки из 2д поверхности методом протягивания вдоль кривой
    /// </summary>
    public class CurveExtruder : MeshExtruder
    {
        private IGmshController controller;
        private int curveTag;

        private Matrix4x4 init;

        /// <summary>
        /// Создает экземляр класса CurveExtruder
        /// </summary>
        /// <param name="controller">Контролле GMSH</param>
        /// <param name="curveTag">Идентификатор кривой вдоль которой будет производится протягивание слоя</param>
        public CurveExtruder(IGmshController controller, int curveTag)
        {
            this.controller = controller;
            this.curveTag = curveTag;
        }
        /// <summary>
        /// Создает экструзию 3д элементов методом протягивания вдоль кривой
        /// </summary>
        /// <param name="firstLayer"></param>
        /// <param name="layers">Число слоев на кривой</param>
        /// <returns>Список 3д элементов</returns>
        /// <exception cref="ArgumentException"></exception>
        public List<List<Element3D>> Extrude(IEnumerable<Element2D> firstLayer, int layers = 8)
        {
            if(layers < 1)
                throw new ArgumentException("Число слоев не может быть меньше 1");

            var tetLayers = new List<List<Element3D>>();
            var basisList = GetBasisOfLayers(layers);
            init = basisList[0];

            firstLayer = SortFirstLayer(firstLayer);
            var curLayer = CreateCopy(firstLayer);

            for (var i = 0; i < layers; ++i)
            {
                var nextLayer = CreateNextLayer(firstLayer, basisList[i + 1]);
                var layer = CreateLayerTetrahedrons(curLayer, nextLayer);
                tetLayers.Add(layer);
                curLayer = nextLayer;
            }
            return tetLayers;
        }
        /// <summary>
        /// Создает трансфинитную экструзию 3д элементов методом протягивания вдоль кривой
        /// </summary>
        /// <param name="firstLayer"></param>
        /// <param name="layers">Число слоев на кривой</param>
        /// <returns>Список 3д элементов</returns>
        /// <exception cref="ArgumentException"></exception>
        public List<List<Element3D>> ExtrudeTransfinite(IEnumerable<Element2D> firstLayer, int layers = 8)
        {
            if (layers < 1)
                throw new ArgumentException("Число слоев не может быть меньше 1");

            var tetLayers = new List<List<Element3D>>();
            var basisList = GetBasisOfLayers(layers);
            init = basisList[0];

            firstLayer = SortFirstLayer(firstLayer);
            var curLayer = CreateCopy(firstLayer);

            for (var i = 0; i < layers; ++i)
            {
                var nextLayer = CreateNextLayer(firstLayer, basisList[i + 1]);
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
        /// <param name="curBasis">Текущий базис слоя</param>
        /// <returns>Список 2д элементов</returns>
        private List<Element2D> CreateNextLayer(IEnumerable<Element2D> curLayer, Matrix4x4 curBasis)
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
                        var coords = new Vector4(node._x, node._y, node._z, 1);
                        coords = Vector4.Transform(coords, curBasis);
                        var point = new Node(nodeTag, new Point3D(coords.X, coords.Y, coords.Z));
                        Nodes.Add(nodeTag++, point);
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
        /// <summary>
        /// Создает копию слоя переданную для операции экструзии
        /// </summary>
        /// <param name="src">Слой источник 2д элементов</param>
        /// <returns>Копия 2д элементов</returns>
        protected override List<Element2D> CreateCopy(IEnumerable<Element2D> src)
        {
            var layer = new List<Element2D>();
            var visited = new Dictionary<int, Node>();

            foreach (var element in src)
            {
                var points = new List<Node>();
                foreach (var node in element.GetVertexes())
                {
                    if (!visited.ContainsKey(node.Number))
                    {
                        var coords = new Vector4(node._x, node._y, node._z, 1);
                        coords = Vector4.Transform(coords, init);
                        var point = new Node(nodeTag++, new Point3D(coords.X, coords.Y, coords.Z));
                        visited.Add(node.Number, point);
                        points.Add(point);
                        Nodes.Add(point.Number, point);
                    }
                    else
                        points.Add(visited[node.Number]);
                }
                var pArray = points.ToArray();
                Element2D elem = pArray.Length == 3 ? new Triangle_c(element.Number, pArray) : new Quad(element.Number, pArray);
                layer.Add(elem);
            }
            return layer;
        }

        private List<Matrix4x4> GetBasisOfLayers(int layers)
        {
            var p = controller.Gmsh.Model.GetParametrizationBounds(1, curveTag);
            var step = (p.Item2[0] - p.Item1[0]) / layers;
            var curT = p.Item1[0];

            var result = new List<Matrix4x4>();
            for (var i = 0; i <= layers; ++i)
            {
                curT = p.Item1[0] + step * i;
                curT = Math.Min(curT, p.Item2[0]);

                var o = controller.Gmsh.Model.GetValue(1, curveTag, [curT]);
                var t = controller.Gmsh.Model.GetDerivative(1, curveTag, [curT]);
                var a = controller.Gmsh.Model.GetSecondDerivative(1, curveTag, [curT]);

                var tangent = new Vector3((float)t[0], (float)t[1], (float)t[2]);
                var nVec = new Vector3((float)a[0], (float)a[1], (float)a[2]);
                //Формула получения нормали: dot(t,t) * a - dot(t,a) * t;
                var normal = Vector3.Dot(tangent, tangent) * nVec - Vector3.Dot(tangent, nVec) * tangent;

                var basis = new Vector3((float)o[0], (float)o[1], (float)o[2]);

                tangent = Vector3.Normalize(tangent);
                normal = Vector3.Normalize(normal);

                var bn = Vector3.Cross(tangent, normal);
                bn = Vector3.Normalize(bn);

                var matrix = CreateMatrix(basis, tangent, normal, bn);
                result.Add(matrix);
            }

            return result;
        }

        private Matrix4x4 CreateMatrix(Vector3 origin, Vector3 tangent, Vector3 normal, Vector3 binormal)
        {
            var matrix = Matrix4x4.Identity;

            matrix.Translation = origin;

            matrix.M11 = binormal.X;
            matrix.M12 = binormal.Y;
            matrix.M13 = binormal.Z;

            matrix.M21 = normal.X;
            matrix.M22 = normal.Y;
            matrix.M23 = normal.Z;

            matrix.M31 = tangent.X;
            matrix.M32 = tangent.Y;
            matrix.M33 = tangent.Z;

            return matrix;
        }
        /// <summary>
        /// Запретить создание экземпляра по умолчанию
        /// </summary>
        private CurveExtruder() 
        { 
        }
    }
}
