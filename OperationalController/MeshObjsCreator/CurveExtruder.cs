using Geometry;
using Model.MeshObjects;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Класс экструзии сетки из 2д поверхности методом протягивания вдоль кривой
    /// </summary>
    public class CurveExtruder : MeshExtruder
    {
        private IGmshController controller;
        private int curveTag;
        private double maxCurvature;
        private bool isDirect;

        private Matrix4x4 init;

        /// <summary>
        /// Создает экземляр класса CurveExtruder
        /// </summary>
        /// <param name="controller">Контролле GMSH</param>
        /// <param name="curveTag">Идентификатор кривой вдоль которой будет производится протягивание слоя</param>
        /// <param name="pointTag">Выбранный идентифкатор геометрической точки начала сечения</param>
        public CurveExtruder(IGmshController controller, int curveTag, int pointTag)
        {
            this.controller = controller;
            this.curveTag = curveTag;

            maxCurvature = GetMaxCurvature();
            DetectDirection(pointTag);
        }
        /// <summary>
        /// Создает экструзию 3д элементов методом протягивания вдоль кривой
        /// </summary>
        /// <param name="firstLayer"></param>
        /// <param name="min">Минимальный шаг для резкой кривизны</param>
        /// <param name="max">Максимальный шаг для пологой кривизны</param>
        /// <returns>Список 3д элементов</returns>
        /// <exception cref="ArgumentException"></exception>
        public List<List<Element3D>> Extrude(IEnumerable<Element2D> firstLayer, double min = 0.05, double max = 0.3)
        {
            CheckSteps(min, max);

            var tetLayers = new List<List<Element3D>>();
            var basisList = GetBasisOfLayers(firstLayer, min, max);
            init = basisList[0];

            firstLayer = SortFirstLayer(firstLayer);
            var curLayer = CreateCopy(firstLayer);

            for (var i = 0; i < basisList.Count - 1; ++i)
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
        /// <param name="min">Минимальный шаг для резкой кривизны</param>
        /// <param name="max">Максимальный шаг для пологой кривизны</param>
        /// <returns>Список 3д элементов</returns>
        /// <exception cref="ArgumentException"></exception>
        public List<List<Element3D>> ExtrudeTransfinite(IEnumerable<Element2D> firstLayer, double min = 0.05, double max = 0.3)
        {
            CheckSteps(min, max);

            var tetLayers = new List<List<Element3D>>();
            var basisList = GetBasisOfLayers(firstLayer, min, max);
            init = basisList[0];

            firstLayer = SortFirstLayer(firstLayer);
            var curLayer = CreateCopy(firstLayer);

            for (var i = 0; i < basisList.Count - 1; ++i)
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

        private void DetectDirection(int pointTag)
        {
            var point = controller.Gmsh.Model.GetAdjacencies(1, curveTag).Item2
                                             .Select((v, i) => new { V = v, I = i })
                                             .Where((v, i) => v.V == pointTag)
                                             .FirstOrDefault();
            if (point == null)
                throw new Exception($"Точка с идентификатором {pointTag} не принадлежит кривой {curveTag}");
            isDirect = point.I == 0;
        }


        private double GetCurrentStep(double curvature, double minT, double maxT)
        {
            var value = curvature / (maxCurvature + 1e-4);
            return (1 - value) * maxT + value * minT;
        }

        private double GetMaxCurvature(int points = 100)
        {
            var p = controller.Gmsh.Model.GetParametrizationBounds(1, curveTag);
            var step = (p.Item2[0] - p.Item1[0]) / (points - 1);

            var cList = new List<double>();
            for (var i = 0; i < points; ++i)
            {
                var t = p.Item1[0] + step * i;
                var curvature = controller.Gmsh.Model.GetCurvature(1, curveTag, [t])[0];
                cList.Add(curvature);
            }

            var max = cList.Max();
            return max;
        }

        private void CheckSteps(double min, double max)
        {
            if (min > max)
                throw new ArgumentException("Минимальный шаг не должен быть больше максимального");
            if (min <= 0)
                throw new ArgumentException("Минимальный шаг должен быть положителен");
        }

        private double GetNextStep(double t, double min, double max)
        {
            var curCurv = controller.Gmsh.Model.GetCurvature(1, curveTag, [t]);
            if (curCurv.Length > 0)
            {
                var charCurv = curCurv[0] / (maxCurvature + 1e-4);
                var modT = (1 - charCurv) * max + charCurv * min;
                return modT;
            }
            return 0;
        }

        private List<Matrix4x4> GetBasisOfLayers(IEnumerable<Element2D> elements, double min, double max)
        {
            if (isDirect)
                return ProcessDirectParameterization(elements, min, max);
            return ProcessInvertParameterization(elements, min, max);
        }

        private List<Matrix4x4> ProcessDirectParameterization(IEnumerable<Element2D> elements, double min, double max)
        {
            var result = new List<Matrix4x4>();
            var p = controller.Gmsh.Model.GetParametrizationBounds(1, curveTag);
            var t = p.Item1[0];
            var dir = Math.Abs(p.Item2[0] - p.Item1[0]);

            var initComps = GetBaseComponents(t, 1);
            var initM = CreateLocalMatrix(initComps.Item2, initComps.Item3, initComps.Item4);
            initM = Matrix4x4.Transpose(initM);

            var lastNormal = initComps.Item2;
            var step = 0.0;

            while (p.Item2[0] - t > 1e-4)
            {
                t = t + step >= p.Item2[0] ? t = p.Item2[0] : t + step;

                var components = GetCorrectComponents(t, 1, ref lastNormal);
                var current = CreateLocalMatrix(components.Item2, components.Item3, components.Item4);
                var convert = GetConvertMatrix(current, components.Item1, initM, initComps.Item1);
                result.Add(convert);

                step = GetNextStep(t, min, max) * dir;
            }

            return result;
        }

        private List<Matrix4x4> ProcessInvertParameterization(IEnumerable<Element2D> elements, double min, double max)
        {
            var result = new List<Matrix4x4>();
            var p = controller.Gmsh.Model.GetParametrizationBounds(1, curveTag);
            Swap(ref p.Item1[0], ref p.Item2[0]);
            var t = p.Item1[0];
            var dir = Math.Abs(p.Item2[0] - p.Item1[0]);

            var initComps = GetBaseComponents(t, -1);
            var initM = CreateLocalMatrix(initComps.Item2, initComps.Item3, initComps.Item4);
            initM = Matrix4x4.Transpose(initM);

            var lastNormal = initComps.Item2;
            var step = 0.0;

            while (t - p.Item2[0] > 1e-4)
            {
                t = t - step >= p.Item2[0] ? t - step : p.Item2[0];

                var components = GetCorrectComponents(t, -1, ref lastNormal);
                var current = CreateLocalMatrix(components.Item2, components.Item3, components.Item4);
                var convert = GetConvertMatrix(current, components.Item1, initM, initComps.Item1);
                result.Add(convert);

                step = GetNextStep(t, min, max) * dir;
            }

            return result;
        }

        private (Vector3, Vector3, Vector3, Vector3) GetCorrectComponents(double t, int mul, ref Vector3 lastNormal)
        {
            var components = GetBaseComponents(t, mul);
            var dp = Vector3.Dot(components.Item2, lastNormal);
            if (dp < 0)
            {
                components.Item2 = -components.Item2;
                components.Item3 = -components.Item3;
            }
            lastNormal = components.Item2;

            return components;
        }

        private (Vector3, Vector3, Vector3, Vector3) GetBaseComponents(double t, int mul)
        {
            var rawOrigin = controller.Gmsh.Model.GetValue(1, curveTag, [t]);
            var rawTangent = controller.Gmsh.Model.GetDerivative(1, curveTag, [t]);
            var sDer = controller.Gmsh.Model.GetSecondDerivative(1, curveTag, [t]);

            var origin = new Vector3((float)rawOrigin[0], (float)rawOrigin[1], (float)rawOrigin[2]);
            var normal = new Vector3(mul * (float)sDer[0], mul * (float)sDer[1], mul * (float)sDer[2]);
            if (Vector3.Dot(normal, normal) < 1e-4)
                return (origin, Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ);

            var tangent = new Vector3(mul * (float)rawTangent[0], mul * (float)rawTangent[1], mul * (float)rawTangent[2]);
            tangent = Vector3.Normalize(tangent);

            normal = Vector3.Normalize(normal);
            normal = Vector3.Dot(tangent, tangent) * normal - Vector3.Dot(tangent, normal) * tangent;
            normal = Vector3.Normalize(normal);

            var binormal = Vector3.Cross(tangent, normal);
            binormal = Vector3.Normalize(binormal);

            return (origin, normal, binormal, tangent);
        }

        private Matrix4x4 GetConvertMatrix(Matrix4x4 local, Vector3 localOrigin, Matrix4x4 init, Vector3 initOrigin)
        {
            var convert = init * local;

            var origin = Vector3.Transform(initOrigin, convert);
            convert.Translation = localOrigin - origin;

            return convert;
        }

        private Matrix4x4 CreateLocalMatrix(Vector3 normal, Vector3 binormal, Vector3 tangent)
        {
            var matrix = Matrix4x4.Identity;

            matrix.M11 = normal.X;
            matrix.M12 = normal.Y;
            matrix.M13 = normal.Z;

            matrix.M21 = binormal.X;
            matrix.M22 = binormal.Y;
            matrix.M23 = binormal.Z;

            matrix.M31 = tangent.X;
            matrix.M32 = tangent.Y;
            matrix.M33 = tangent.Z;

            return matrix;
        }

        private void Swap<T>(ref T a, ref T b)
        {
            var temp = a;
            a = b;
            b = temp;
        }


        /// <summary>
        /// Запретить создание экземпляра по умолчанию
        /// </summary>
        private CurveExtruder() 
        { 
        }
    }
}
