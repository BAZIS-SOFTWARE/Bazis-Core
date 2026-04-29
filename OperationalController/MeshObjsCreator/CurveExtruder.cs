using Geometry;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using Model.MeshObjects;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using static IronPython.Modules._ast;


namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Класс экструзии сетки из 2д поверхности методом протягивания вдоль кривой
    /// </summary>
    public class CurveExtruder : MeshExtruder
    {
        private IGmshController controller;
        private int[] curveTags;
        private double[] steps;
        private float maxAngle;
        /// <summary>
        /// Число сегментов в поворотной экструзии внешнего шва
        /// </summary>
        public int JointSegments { get; private set; } = 1;

        /// <summary>
        /// Создает экземляр класса CurveExtruder
        /// </summary>
        /// <param name="controller">Контроллер GMSH</param>
        /// <param name="curveTag">Идентификатор кривой вдоль которой будет производится протягивание слоя</param>
        /// <param name="step">Шаг (доля) от длины кривой</param>
        /// <param name="pointTag">Выбранный идентифкатор геометрической точки начала сечения</param>
        public CurveExtruder(IGmshController controller, int curveTag, double step, int pointTag)
        {
            this.controller = controller;
            curveTags = [curveTag];

            steps = [step];
            DetectDirection(pointTag);
        }

        /// <summary>
        /// Создает экземляр класса CurveExtruder с множеством кривых
        /// </summary>
        /// <param name="controller">Контроллер GMSH</param>
        /// <param name="curveTags">Кривые соединенные в контур, первой кривой должен принадлежать pointTag</param>
        /// <param name="steps">Шаг для каждой кривой (доля) от длины кривой</param>
        /// <param name="pointTag">Выбранный идентифкатор геометрической точки начала сечения</param>
        /// <param name="maxAngle">Максимальный угол при котором будет рассматриваться поворотная экструзия между кривыми</param>
        public CurveExtruder(IGmshController controller, int[] curveTags, double[] steps, int pointTag, float maxAngle = MathF.PI / 2)
        {
            this.controller = controller;
            this.maxAngle = maxAngle;

            if (steps.Length != curveTags.Length)
                throw new ArgumentException("Число кривых и число заданных шагов должно быть одинаковым");

            this.steps = steps;
            this.curveTags = DetectCurveLoop(curveTags, pointTag);
        }

        /// <summary>
        /// Создает экструзию 3д элементов методом протягивания вдоль кривой (Версия Merge по углу)
        /// </summary>
        /// <param name="firstLayer">Элементы 2D сечения</param>
        /// <returns>Список 3д элементов</returns>
        public List<List<Element3D>> Extrude(IEnumerable<Element2D> firstLayer)
        {
            var tetLayers = new List<List<Element3D>>();
            firstLayer = SortFirstLayer(firstLayer);

            List<Element2D> curLayer = null;
            var lastCorrect = Matrix4x4.Identity;

            for (var i = 0; i < curveTags.Length; ++i)
            {
                var compList = GetCurveComponents(steps[i], i);
                var matrixList = CreateLocalSystemSet(compList, compList[0]);

                curLayer = CreateNextLayer(firstLayer, matrixList[0]);
                var rotateStatus = false;

                for (var j = 1; j < matrixList.Count; ++j)
                {
                    var currentCorrect = Matrix4x4.Identity;
                    if (j == matrixList.Count - 1 && IsExistsCommonPoint(i))
                    {
                        var next = GetNextTangent(i);
                        var data = GetRotationDataBetweenComponents(compList[j].Item5, next);
                        lastCorrect = GetCorrectionMatrix(data.Item1, data.Item2, compList[j].Item4);
                        if (data.Item2 <= maxAngle)
                        {
                            rotateStatus = true;
                            currentCorrect = lastCorrect;
                        }
                    }

                    var nextLayer = CreateNextLayer(firstLayer, matrixList[j] * currentCorrect);
                    var layer = CreateLayerTetrahedrons(curLayer, nextLayer);
                    tetLayers.Add(layer);
                    curLayer = nextLayer;
                }
                firstLayer = rotateStatus || i == curveTags.Length - 1 ? curLayer 
                                                                       : CreateNextLayer(firstLayer, matrixList[matrixList.Count - 1] * lastCorrect);
            }
            return tetLayers;
        }

        /// <summary>
        /// Создает трансфинитную экструзию 3д элементов методом протягивания вдоль кривой (Версия Merge по углу )
        /// </summary>
        /// <param name="firstLayer"></param>
        /// <returns>Список 3д элементов</returns>
        public List<List<Element3D>> ExtrudeTransfinite(IEnumerable<Element2D> firstLayer)
        {
            var tetLayers = new List<List<Element3D>>();
            firstLayer = SortFirstLayer(firstLayer);

            List<Element2D> curLayer = null;
            var lastCorrect = Matrix4x4.Identity;

            for (var i = 0; i < curveTags.Length; ++i)
            {
                var compList = GetCurveComponents(steps[i], i);
                var matrixList = CreateLocalSystemSet(compList, compList[0]);

                curLayer = CreateNextLayer(firstLayer, matrixList[0]);
                var rotateStatus = false;

                for (var j = 1; j < matrixList.Count; ++j)
                {
                    var currentCorrect = Matrix4x4.Identity;
                    if (j == matrixList.Count - 1 && IsExistsCommonPoint(i))
                    {
                        var next = GetNextTangent(i);
                        var data = GetRotationDataBetweenComponents(compList[j].Item5, next);
                        lastCorrect = GetCorrectionMatrix(data.Item1, data.Item2, compList[j].Item4);
                        if (data.Item2 <= maxAngle)
                        {
                            rotateStatus = true;
                            currentCorrect = lastCorrect;
                        }
                    }

                    var nextLayer = CreateNextLayer(firstLayer, matrixList[j] * currentCorrect);
                    var layer = CreateTransfiniteLayer(curLayer, nextLayer);
                    tetLayers.Add(layer);
                    curLayer = nextLayer;
                }
                firstLayer = rotateStatus || i == curveTags.Length  - 1 ? curLayer 
                                                                        : CreateNextLayer(firstLayer, matrixList[matrixList.Count - 1] * lastCorrect);
            }
            return tetLayers;
        }
        /*
        /// <summary>
        /// Создает экструзию 3д элементов методом протягивания вдоль кривой (Версия с поворотной экструзий)
        /// </summary>
        /// <param name="firstLayer">Элементы 2D сечения</param>
        /// <returns>Список 3д элементов</returns>
        public List<List<Element3D>> Extrude2(IEnumerable<Element2D> firstLayer)
        {
            var tetLayers = new List<List<Element3D>>();
            firstLayer = SortFirstLayer(firstLayer);

            (Vector3, Vector3, Vector3, Vector3, Vector3) lastComponents = (Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);
            (Vector3, Vector3, Vector3, Vector3, Vector3) firstComponents = (Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);

            List<Element2D> curLayer = null;
            for (var i = 0; i < curveTags.Length; ++i)
            {
                var compList = GetCurveComponents(steps[i], i);
                var matrixList = CreateLocalSystemSet(compList, compList[0]);

                if (i != 0)
                {
                    AddRotarySection(tetLayers, curLayer, lastComponents, compList[0], false);
                    var corrMat = GetCorrectionMatrix(lastComponents.Item5, compList[0].Item5, compList[0].Item4);
                    curLayer = CreateNextLayer(curLayer, corrMat);
                }
                else
                {
                    curLayer = CreateNextLayer(firstLayer, matrixList[0]);
                    firstComponents = compList[0];
                }
                firstLayer = curLayer;

                for (var j = 1; j < matrixList.Count; ++j)
                {
                    var nextLayer = CreateNextLayer(firstLayer, matrixList[j]);
                    var layer = CreateLayerTetrahedrons(curLayer, nextLayer);
                    tetLayers.Add(layer);
                    curLayer = nextLayer;
                }
                lastComponents = compList[compList.Count - 1];
            }

            AddRotarySection(tetLayers, curLayer, lastComponents, firstComponents, false);
            return tetLayers;
        }*/
        /*/// <summary>
        /// Создает трансфинитную экструзию 3д элементов методом протягивания вдоль кривой (Версия с поворотной экструзий)
        /// </summary>
        /// <param name="firstLayer"></param>
        /// <param name="step">Минимальный шаг от длины кривой</param>
        /// <returns>Список 3д элементов</returns>
        public List<List<Element3D>> ExtrudeTransfinite2(IEnumerable<Element2D> firstLayer)
        {
            var tetLayers = new List<List<Element3D>>();
            firstLayer = SortFirstLayer(firstLayer);

            (Vector3, Vector3, Vector3, Vector3, Vector3) lastComponents = (Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);
            (Vector3, Vector3, Vector3, Vector3, Vector3) firstComponents = (Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero);

            List<Element2D> curLayer = null;
            for (var i = 0; i < curveTags.Length; ++i)
            {
                var compList = GetCurveComponents(steps[i], i);
                var matrixList = CreateLocalSystemSet(compList, compList[0]);

                if (i != 0)
                {
                    AddRotarySection(tetLayers, curLayer, lastComponents, compList[0], true);
                    var corrMat = GetCorrectionMatrix(lastComponents.Item5, compList[0].Item5, compList[0].Item4);
                    curLayer = CreateNextLayer(curLayer, corrMat);
                }
                else
                {
                    curLayer = CreateNextLayer(firstLayer, matrixList[0]);
                    firstComponents = compList[0];
                }
                firstLayer = curLayer;

                for (var j = 1; j < matrixList.Count; ++j)
                {
                    var nextLayer = CreateNextLayer(firstLayer, matrixList[j]);
                    var layer = CreateTransfiniteLayer(curLayer, nextLayer);
                    tetLayers.Add(layer);
                    curLayer = nextLayer;
                }
                lastComponents = compList[compList.Count - 1];
            }

            AddRotarySection(tetLayers, curLayer, lastComponents, firstComponents, true);
            return tetLayers;
        }*/


        private bool IsExistsCommonPoint(int index)
        {
            var nextIndex = curveTags.Length - 1 == index ? 0 : index + 1;
            var bounds0 = controller.Gmsh.Model.GetBoundary([1, curveTags[index]], false);
            var bounds1 = controller.Gmsh.Model.GetBoundary([1, curveTags[nextIndex]], false);

            return bounds0[3] == bounds1[1];
        }

        private Vector3 GetNextTangent(int index)
        {
            var nextIndex = curveTags.Length - 1 == index ? 0 : index + 1;
            var tag = curveTags[nextIndex];
            var absTag = Math.Abs(tag);

            var p = controller.Gmsh.Model.GetParametrizationBounds(1, absTag);

            var l0 = p.Item1[0];
            var l1 = p.Item1[0] + 0.2;
            if (tag < 0)
            {
                Swap(ref p.Item1[0], ref p.Item2[0]);
                l0 = p.Item1[0];
                l1 = p.Item1[0] - 0.2;
            }

            var coords = controller.Gmsh.Model.GetValue(1, absTag, [l0, l1]);

            var p0 = new Vector3((float)coords[0], (float)coords[1], (float)coords[2]);
            var p1 = new Vector3((float)coords[3], (float)coords[4], (float)coords[5]);

            var t0 = p1 - p0;

            return t0;
        }

        /*
        private void AddRotarySection(List<List<Element3D>> e3dData, List<Element2D> elements, (Vector3, Vector3, Vector3, Vector3, Vector3) lastComponents,
                                      (Vector3, Vector3, Vector3, Vector3, Vector3) curComponents, bool transfinite)
        {
            var rotData = InsertRotarySection(elements, lastComponents, curComponents, transfinite);
            if (rotData != null)
            {
                foreach (var layer in rotData.Item1)
                    e3dData.Add(layer);
                foreach (var node in rotData.Item2)
                    Nodes.Add(node.Number, node);
            }
        }

        
        private Tuple<List<List<Element3D>>, List<Node>> InsertRotarySection(List<Element2D> elements, (Vector3, Vector3, Vector3, Vector3, Vector3) lastComponents,
                                                          (Vector3, Vector3, Vector3, Vector3, Vector3) curComponents, bool transfinite)
        {
            var data = GetRotationDataBetweenComponents(lastComponents.Item5, curComponents.Item5);
            if (data.Item3 > 1e-4 && data.Item2 <= maxAngle && data.Item2 > 1e-4)
            {
                var lastOrigin = lastComponents.Item4;
                var curOrigin = curComponents.Item4;
                var len = (curOrigin - lastOrigin).Length();
                if (len < 1e-4)
                {
                    var extruder = new RotateExtruder();
                    extruder.NodeTag = nodeTag;
                    extruder.ElementTag = elementTag;

                    var newElements = transfinite ? extruder.ExtrudeTransfinite(elements, data.Item2, curOrigin, data.Item1, JointSegments)
                                                  : extruder.Extrude(elements, data.Item2, curOrigin, data.Item1, JointSegments);

                    nodeTag = extruder.NodeTag;
                    elementTag = extruder.ElementTag;

                    return Tuple.Create(newElements, extruder.Nodes.Values.ToList());
                }
            }
            return null;
        }*/

        private (Vector3, float, float) GetRotationDataBetweenComponents(Vector3 lastTangent, Vector3 curTangent)
        {
            lastTangent = Vector3.Normalize(lastTangent);
            curTangent = Vector3.Normalize(curTangent);

            var cos = Math.Min(Vector3.Dot(lastTangent, curTangent), 1.0f);
            cos = Math.Max(cos, -1.0f);
            var angle = MathF.Acos(cos);

            var axis = Vector3.Cross(lastTangent, curTangent);
            axis = Vector3.Normalize(axis);

            var sign = Vector3.Dot(axis, Vector3.UnitZ);

            return (axis, angle, sign);
        }

        private Matrix4x4 GetCorrectionMatrix(Vector3 axis, float angle, Vector3 curOrigin)
        {
            var negOrigin = Matrix4x4.CreateTranslation(-curOrigin);
            var origin = Matrix4x4.CreateTranslation(curOrigin);
            var rotate = Matrix4x4.CreateFromAxisAngle(axis, angle);

            var result = negOrigin * rotate * origin;

            return result;
        }

        /*private Matrix4x4 GetCorrectionMatrix(Vector3 lastTangent, Vector3 curTangent, Vector3 curOrigin)
        {
            var data = GetRotationDataBetweenComponents(lastTangent, curTangent);

            var negOrigin = Matrix4x4.CreateTranslation(-curOrigin);
            var origin = Matrix4x4.CreateTranslation(curOrigin);
            var rotate = Matrix4x4.CreateFromAxisAngle(data.Item1, data.Item2);

            var result = negOrigin * rotate * origin;

            return result;
        }*/

        /// <summary>
        /// По неупорядоченному набору кривых, образующих контур строит ориентированный контур
        /// </summary>
        /// <param name="curves">Набор кривых, первая кривая должна быть инцидентна точке pointTag</param>
        /// <param name="pointTag">Точка начала контура кривых, должна принадлежать первой кривой curves[]</param>
        /// <returns>Сортированный список кривых образующий контур (замкнутый или полузамкнутый)</returns>
        /// <exception cref="Exception">Выбрасывается в случае если набор кривых не является контуром, у 1 точки более двух кривых или есть обособленные группы кривых.
        /// Выбрасывается в случае, если точка pointTag не принадлежит первой кривой</exception>
        private int[] DetectCurveLoop(int[] curves, int pointTag)
        {
            var current = curves[0];
            var set = curves.ToHashSet();
            var contour = new List<int>();

            while (set.Count != 0)
            {
                var points = controller.Gmsh.Model.GetBoundary([1, current], false, false)
                                                  .Where((v, i) => (i & 1) == 1)
                                                  .ToArray();
                if (pointTag != points[0] && pointTag != points[1])
                    throw new Exception($"Точка {pointTag} не принадлежит кривой {current}");

                set.Remove(current);
                var index = 1;
                if (pointTag == points[1])
                {
                    current = -current;
                    index = 0;
                }

                contour.Add(current);
                pointTag = points[index];

                var adj = controller.Gmsh.Model.GetAdjacencies(0, pointTag).Item1;
                var candidates = adj.Where(v => set.Contains(v));

                if (candidates != null)
                {
                    var cList = candidates.ToList();
                    if (cList.Count > 0)
                    {
                        if (cList.Count > 1)
                            throw new Exception($"У точки {pointTag} найдено более 2 общих кривых, контур не валидный");
                        current = cList[0];
                    }
                }
            }
            if (set.Count > 0)
                throw new Exception($"Найдена обособленная группа кривых, контур не валидный");

            return contour.ToArray();
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

        private void DetectDirection(int pointTag)
        {
            var point = controller.Gmsh.Model.GetAdjacencies(1, curveTags[0]).Item2
                                             .Select((v, i) => new { V = v, I = i })
                                             .Where((v, i) => v.V == pointTag)
                                             .FirstOrDefault();
            if (point == null)
                throw new Exception($"Точка с идентификатором {pointTag} не принадлежит кривой {curveTags[0]}");
            curveTags[0] = point.I != 0 ? -curveTags[0] : curveTags[0];
        }

        //Взятие шага в зависимости от кривизны
        /*
        private double GetCurrentStep(double curvature, double minT, double maxT)
        {
            var value = curvature / (maxCurvature + 1e-4);
            return (1 - value) * maxT + value * minT;
        }
        //Определение максимальной кривизнвы кривой
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
        }*/
        //Проверки шагов в случае использования адаптивного шага
        /*
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
        }*/

        private List<(Vector3, Vector3, Vector3, Vector3, Vector3)> GetCurveComponents(double step, int index)
        {
            if (Math.Sign(curveTags[index]) > 0)
                return ProcessDirectParameterization(step, index);
            return ProcessInvertParameterization(step, index);
        }

        private List<(Vector3, Vector3, Vector3, Vector3, Vector3)> ProcessDirectParameterization(double step, int index)
        {
            var p = controller.Gmsh.Model.GetParametrizationBounds(1, curveTags[index]);
            var dir = p.Item2[0] - p.Item1[0];

            var l0 = p.Item1[0];
            var offset = 0.0;

            var components = new List<(Vector3, Vector3, Vector3, Vector3, Vector3)>();
            while (p.Item2[0] - l0 > 1e-4)
            {
                l0 = Math.Min(l0 + offset, p.Item2[0]);
                WriteComponentsInPoint(components, index, l0, l0 + 0.2);
                offset = step * dir;
            }
            return components;
        }

        private List<(Vector3, Vector3, Vector3, Vector3, Vector3)> ProcessInvertParameterization(double step, int index)
        {
            var p = controller.Gmsh.Model.GetParametrizationBounds(1, Math.Abs(curveTags[index]));
            Swap(ref p.Item1[0], ref p.Item2[0]);
            var dir = p.Item2[0] - p.Item1[0];

            var l0 = p.Item1[0];
            var offset = 0.0;

            var components = new List<(Vector3, Vector3, Vector3, Vector3, Vector3)>();
            while (l0 - p.Item2[0] > 1e-4)
            {
                l0 = Math.Max(l0 + offset, p.Item2[0]);
                WriteComponentsInPoint(components, index, l0, l0 - 0.2);
                offset = step * dir;
            }
            return components;
        }

        private List<Matrix4x4> CreateLocalSystemSet(List<(Vector3, Vector3, Vector3, Vector3, Vector3)> components,
                                                    (Vector3, Vector3, Vector3, Vector3, Vector3) firstComponent)
        {
            var init = CreateLocalMatrix(firstComponent.Item1, firstComponent.Item2, firstComponent.Item3);
            init = Matrix4x4.Transpose(init);
            var initOrigin = firstComponent.Item4;

            var ls = new List<Matrix4x4>();
            for (var i = 0; i < components.Count; ++i)
            {
                var local = CreateLocalMatrix(components[i].Item1, components[i].Item2, components[i].Item3);
                var globalTransform = GetConvertMatrix(local, components[i].Item4, init, initOrigin);
                ls.Add(globalTransform);
            }

            return ls;
        }


        private void WriteComponentsInPoint(List<(Vector3, Vector3, Vector3, Vector3, Vector3)> result, int index, double l0, double l2)
        {
            var l1 = l0 + 0.5 * (l2 - l0);

            var coords = controller.Gmsh.Model.GetValue(1, Math.Abs(curveTags[index]), [l0, l1, l2]);

            var p0 = new Vector3((float)coords[0], (float)coords[1], (float)coords[2]);
            var p1 = new Vector3((float)coords[3], (float)coords[4], (float)coords[5]);
            var p2 = new Vector3((float)coords[6], (float)coords[7], (float)coords[8]);

            var t0 = p2 - p0;
            var a0 = (p2 - p1) - (p1 - p0);
            var n0 = Vector3.Dot(t0, t0) * a0 - Vector3.Dot(t0, a0) * t0;

            var last = result.Count - 1;
            (Vector3, Vector3, Vector3, Vector3, Vector3) components;

            if (Vector3.Dot(n0, n0) < 1e-8)
                components = result.Count == 0 ? (Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ, p0, t0)
                                               : (GetComponent(result[last], 1), GetComponent(result[last], 2), GetComponent(result[last], 3), p0, t0);
            else
            {
                t0 = Vector3.Normalize(t0);
                n0 = Vector3.Normalize(n0);

                var b0 = Vector3.Cross(t0, n0);
                if (result.Count > 0 && Vector3.Dot(b0, GetComponent(result[last], 2)) < 1e-4)
                {
                    n0 = -n0;
                    b0 = Vector3.Cross(t0, n0);
                }
                components = (n0, b0, t0, p0, t0);
            }
            result.Add(components);
        }

        /*
        /// <summary>
        /// Определяет является ли смещение шага последним
        /// </summary>
        /// <param name="l">Текущий шаг</param>
        /// <param name="lMax">Максимальная величина шага</param>
        /// <returns>true - если шаг последний</returns>
        private bool IsLastStep(double l, double lMax)
        {
            var a = Math.Sign(l);
            var b = Math.Sign(lMax - l);
            return a != b;
        }
        
        /// <summary>
        /// Усредняет нормаль, полезен при грубом шаге
        /// </summary>
        /// <param name="last">Предыдущие компоненты</param>
        /// <param name="current">Текущие компоненты</param>
        /// <returns>Возвращает усредненные компоненты</returns>
        private (Vector3, Vector3, Vector3, Vector3) AverageComponents((Vector3, Vector3, Vector3, Vector3) last, (Vector3, Vector3, Vector3, Vector3) current)
        {
            var n0 = Vector3.Normalize(current.Item1 + last.Item1);
            var b0 = Vector3.Cross(current.Item3, n0);

            return (n0, b0, current.Item3, current.Item4);
        }*/

        private Vector3 GetComponent((Vector3, Vector3, Vector3, Vector3, Vector3) components, int item)
        {
            if (item == 1)
                return components.Item1;
            if (item == 2)
                return components.Item2;
            if (item == 3)
                return components.Item3;
            if (item == 4)
                return components.Item4;
            return components.Item5;
        }

        //Вариант построения через gmsh
        /*
        private List<Matrix4x4> ProcessDirectParameterization(double min, double max)
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
        //Вариант построения через gmsh
        private List<Matrix4x4> ProcessInvertParameterization(double min, double max)
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
        }*/

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
