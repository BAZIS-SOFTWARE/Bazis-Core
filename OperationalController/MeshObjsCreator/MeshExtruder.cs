using Geometry;
using MathNet.Numerics.Distributions;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Обобщенный класс экструзии сетки из 2д поверхности
    /// </summary>
    public abstract class MeshExtruder
    {
        /// <summary>
        /// Словарь для взятия противоположных индексов точек противоположных боковых граней относительно указанной точки грани
        /// Индекс списка = индекс боковой грани (0 - передняя, 1 - правая, 2 - задняя, 3 - левая)
        /// Ключ словаря - индекс точки 3д элемента принадлежащей указанному индексу грани
        /// Значение словаря - индекс противоположно лежащего индека другой грани
        /// </summary>
        private List<Dictionary<int, int>> hexIndexMap = new List<Dictionary<int, int>>()
        {
            new Dictionary<int, int>(){ { 0, 3 }, { 1, 2 }, { 4, 7 }, { 5, 6 } },//0 грань
            new Dictionary<int, int>(){ { 1, 0 }, { 2, 3 }, { 5, 4 }, { 6, 7 } },//1 грань
            new Dictionary<int, int>(){ { 2, 1 }, { 3, 0 }, { 6, 5 }, { 7, 4 } },//2 грань
            new Dictionary<int, int>(){ { 0, 1 }, { 3, 2 }, { 4, 5}, { 7, 6 } },//3 грань
        };
        private class Element2DSorter : IComparer<Element2D>
        {
            public static Point3D Center { get; set; }
            public int Compare(Element2D x, Element2D y)
            {
                var xL = x.CalcCentr().Sub(Center);
                var yL = y.CalcCentr().Sub(Center);

                var xSquared = xL._x * xL._x + xL._y * xL._y + xL._z * xL._z;
                var ySquared = yL._x * yL._x + yL._y * yL._y + yL._z * yL._z;

                if (Math.Abs(xSquared - ySquared) < 1e-4)
                    return 0;
                else if (xSquared > ySquared)
                    return 1;
                return -1;
            }
        }
        /// <summary>
        /// Словарь ребер основания
        /// </summary>
        protected HashSet<string> primEdges = new HashSet<string>();
        /// <summary>
        /// Словарь ребер боковых граней
        /// </summary>
        protected HashSet<string> lateralEdges = new HashSet<string>();
        /// <summary>
        /// Индексы для создания новых вершин
        /// </summary>
        protected int nodeTag;
        /// <summary>
        /// Индексы для создания новых 3д элементов
        /// </summary>
        protected int elementTag;
        /// <summary>
        /// Узлы образованных 3д элементов
        /// </summary>
        public Dictionary<int, Node> Nodes { get; private set; } = new Dictionary<int, Node>();
        /// <summary>
        /// Пропущенные 3д-элементы в результате плохой конфигурации ребер
        /// </summary>
        public int MissedElements { get; private set; }
        /// <summary>
        /// Сортирует 2д элементы слоя по возрастанию расстоянию от центра слоя
        /// </summary>
        /// <param name="layer">Слой 2д элементов</param>
        /// <returns>Слой 2д элементов</returns>
        protected List<Element2D> SortFirstLayer(IEnumerable<Element2D> layer)
        {
            var xSum = 0.0;
            var ySum = 0.0;
            var zSum = 0.0;

            var index = 0;
            foreach (var elem in layer)
            {
                foreach (var point in elem.GetVertexes())
                {
                    xSum += point.Position._x;
                    ySum += point.Position._y;
                    zSum += point.Position._z;
                    ++index;
                }
            }
            Element2DSorter.Center = new Point3D((float)xSum / index, (float)ySum / index, (float)zSum / index);
            var comparer = new Element2DSorter();
            return layer.OrderBy(v => v, comparer).ToList();
        }
        /// <summary>
        /// Создает копию слоя переданную для операции экструзии
        /// </summary>
        /// <param name="src">Слой источник 2д элементов</param>
        /// <returns>Копия 2д элементов</returns>
        protected virtual List<Element2D> CreateCopy(IEnumerable<Element2D> src)
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
                        var point = new Node(nodeTag++, new Point3D(node.Position._x, node.Position._y, node.Position._z));
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
        /// <summary>
        /// Возвращает список тетраэдров по смежным слоям 2д элементов
        /// </summary>
        /// <param name="curLayer">Нижний слой 2д элементов</param>
        /// <param name="nextLayer">Верхний слой 2д элементов</param>
        /// <returns>Список тетраэдров</returns>
        protected List<Element3D> CreateLayerTetrahedrons(IEnumerable<Element2D> curLayer, List<Element2D> nextLayer)
        {
            var index = 0;
            var layer = new List<Element3D>();

            foreach (var element in curLayer)
            {
                var nodes = element.GetVertexes();
                var upNodes = nextLayer[index++].GetVertexes();
                var e3dNodes = nodes.Concat(upNodes).ToArray();

                List<Tetra> tetras = null;
                if (e3dNodes.Length == 6)
                {
                    var penta = new Penta(0, e3dNodes);
                    tetras = SplitPenta(penta);
                }
                else
                {
                    var hexa = new Hexa(0, e3dNodes);
                    tetras = SplitHexa(hexa);
                }
                if (tetras.Count != 0)
                    layer.AddRange(tetras);
            }
            return layer;
        }
        /// <summary>
        /// Возвращает список 3D элементов по смежным слоям 2д элементов
        /// </summary>
        /// <param name="curLayer">Нижний слой 2д элементов</param>
        /// <param name="nextLayer">Верхний слой 2д элементов</param>
        /// <returns>Список 3д элементов (гекса, призмы)</returns>
        protected List<Element3D> CreateTransfiniteLayer(IEnumerable<Element2D> curLayer, List<Element2D> nextLayer)
        {
            var index = 0;
            var layer = new List<Element3D>();

            foreach (var element in curLayer)
            {
                var nodes = element.GetVertexes();
                var upNodes = nextLayer[index++].GetVertexes();
                var e3dNodes = nodes.Concat(upNodes).ToArray();

                Element3D newElement = null;
                if (e3dNodes.Length == 6)
                    newElement = new Penta(elementTag++, e3dNodes);
                else if (e3dNodes.Length == 8)
                    newElement = new Hexa(elementTag++, e3dNodes);
                else
                    continue;//Тетра не поддерживается
                layer.Add(newElement);
            }
            return layer;
        }
        /// <summary>
        /// Разбивает пента элемент на 3 тетраэдра
        /// </summary>
        /// <param name="penta">Пента элемент</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> SplitPenta(Penta penta)
        {
            var nodes = penta.GetVertexes().ToArray();

            var coincidents = new List<int>();
            var converter = GetNumberConverter(nodes, coincidents);
            if (coincidents.Count != 0)
                return DoDegeneratedPentaSplitting(nodes, converter, coincidents);

            var indices = GetExistingLateralEdges(nodes, converter, 3);
            indices = indices.Where(v => v != null).ToArray();
            if (indices.Length == 0)
                return DoPentaDefaultSplitting(nodes);
            if (indices.Length == 1)
                return DoPentaOneEdgeSplitting(nodes, indices[0]);
            if (indices.Length == 2)
                return DoPentaTwooEdgeSplitting(nodes, indices);
            return DoPentaThreeEdgeSplitting(nodes, indices);
        }
        /// <summary>
        /// Разбивает гекса элемент на 6 тетраэдров
        /// </summary>
        /// <param name="hexa">Гекса элемент</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> SplitHexa(Hexa hexa)
        {
            var nodes = hexa.GetVertexes().ToArray();

            var coincidents = new List<int>();
            var converter = GetNumberConverter(nodes, coincidents);
            if (coincidents.Count != 0)
                return DoDegeneratedHexaSplitting(nodes, converter, coincidents);

            var indices = GetPentaSplittingIndices(nodes, converter);
            var result = new List<Tetra>();
            if (indices.Length == 0)
            {
                MissedElements += 6;
                return result;
            }

            var a = indices[0];
            var b = indices[1];

            var first = new Penta(0, [nodes[a[0]], nodes[a[1]], nodes[a[2]], nodes[a[3]], nodes[a[4]], nodes[a[5]]]);
            var second = new Penta(0, [nodes[b[0]], nodes[b[1]], nodes[b[2]], nodes[b[3]], nodes[b[4]], nodes[b[5]]]);

            var firstTetras = SplitPenta(first);
            if (firstTetras.Count != 0)
                result.AddRange(firstTetras);

            var secondTetras = SplitPenta(second);
            if (secondTetras.Count != 0)
                result.AddRange(secondTetras);

            return result;
        }
        /// <summary>
        /// Ищет наилучшую конфигурацию ребер для разбиения гексы опираясь на существующие конфигурации ребер
        /// </summary>
        /// <param name="nodes">Узлы гекса</param>
        /// <param name="converter">Конвертер из номеров в индексы</param>
        /// <returns>2 массива ребер [мин индекс, макс индекс])</returns>
        private int[][] GetPentaSplittingIndices(Node[] nodes, Dictionary<int, int> converter)
        {
            var primaryConfigs = new List<int[][]>();
            var slaveConfigs = new List<int[][]>();
            var emptyConfigs = new List<int>();

            GetPentaConfigurationFromPrimaryEdges(nodes, converter, primaryConfigs, slaveConfigs, emptyConfigs);
            if (primaryConfigs.Count != 0)
            {
                SetEdgeConfig(primEdges, nodes, primaryConfigs[0][0]);
                return primaryConfigs[0];
            }
            var existPrimarySlave = slaveConfigs.Count != 0;
            var existPrimaryEmpty = emptyConfigs.Count != 0;

            GetPentaConfigurationFromLateralEdges(nodes, converter, primaryConfigs, slaveConfigs, emptyConfigs);
            if (primaryConfigs.Count != 0)
                return primaryConfigs[0];

            if (slaveConfigs.Count != 0)
            {
                var set = existPrimarySlave ? primEdges : lateralEdges;
                SetEdgeConfig(set, nodes, slaveConfigs[0][0]);
                return slaveConfigs[0];
            }
            if (emptyConfigs.Count != 0)
            {
                var config = CreateTetraConfigFromEmptyEdges(emptyConfigs[0]);
                var set = existPrimaryEmpty ? primEdges : lateralEdges;
                SetEdgeConfig(set, nodes, config[0]);
                return config;
            }

            return [[]];
        }
        /// <summary>
        /// Записывает существующую конфигурацию ребер боковых граней в приоритетные списки (primary, slave, empty)
        /// </summary>
        /// <param name="nodes">Узлы 3д элементов</param>
        /// <param name="converter">Конвертер узлов из номеров в индексы</param>
        /// <param name="primary">Список главного приоритета</param>
        /// <param name="slave">Список подчиненного приоритета</param>
        /// <param name="emptyConfigs">Список пустых конфигураций по граням</param>
        private void GetPentaConfigurationFromLateralEdges(Node[] nodes, Dictionary<int, int> converter,
                                                           List<int[][]> primary, List<int[][]> slave,
                                                           List<int> emptyConfigs)
        {
            var indices = GetExistingLateralEdges(nodes, converter, 4);
            for (var i = 0; i < 2; ++i)
            {
                if (indices[i] != null && indices[i + 2] != null)
                {
                    SortIndices(indices[i]);
                    SortIndices(indices[i + 2]);

                    var minA = indices[i][0];
                    var minB = indices[i + 2][0];

                    if (hexIndexMap[i][minA] == minB)
                    {
                        var config = CreateTetraConfigFromLateralEdges(indices, i);
                        primary.Add(config);
                    }
                }
                else if (indices[i] != null)
                {
                    SortIndices(indices[i]);
                    indices[i + 2] = GetOppositeLateralEdge(indices[i], i);
                    var config = CreateTetraConfigFromLateralEdges(indices, i);
                    slave.Add(config);
                }
                else if (indices[i + 2] != null)
                {
                    SortIndices(indices[i + 2]);
                    indices[i] = GetOppositeLateralEdge(indices[i + 2], i + 2);
                    var config = CreateTetraConfigFromLateralEdges(indices, i);
                    slave.Add(config);
                }
                else
                    emptyConfigs.Add(i);
            }
        }
        /// <summary>
        /// Записывает существующую конфигурацию ребер оснований в приоритетные списки (primary, slave, empty)
        /// </summary>
        /// <param name="nodes">Узлы 3д элементов</param>
        /// <param name="converter">Конвертер узлов из номеров в индексы</param>
        /// <param name="primary">Список главного приоритета</param>
        /// <param name="slave">Список подчиненного приоритета</param>
        /// <param name="emptyConfigs">Список пустых конфигураций по граням</param>
        private void GetPentaConfigurationFromPrimaryEdges(Node[] nodes, Dictionary<int, int> converter,
                                                           List<int[][]> primary, List<int[][]> slave,
                                                           List<int> emptyConfigs)
        {
            var indices = GetExistingPrimaryEdges(nodes, converter);

            if (indices[0] != null && indices[1] != null)
            {
                SortIndices(indices);
                if (indices[0][0] == indices[1][0] - 4)
                {
                    var config = CreateTetraConfigFromPrimaryEdges(indices);
                    primary.Add(config);
                }
            }
            else if (indices[0] != null)
            {
                SortIndices(indices[0]);
                indices[1] = GetOppositePrimaryEdge(indices[0]);
                var config = CreateTetraConfigFromPrimaryEdges(indices);
                primary.Add(config);
            }
            else if (indices[1] != null)
            {
                SortIndices(indices[1]);
                indices[0] = GetOppositePrimaryEdge(indices[1]);
                var config = CreateTetraConfigFromPrimaryEdges(indices);
                primary.Add(config);
            }
            else
                emptyConfigs.Add(2);
        }
        /// <summary>
        /// По индексу грани строит индексы разбиения тетраэдров на пустой конфигурации ребер
        /// </summary>
        /// <param name="faceIndex">Индекс грани (0 - передняя, 1 - правая, 2 - нижняя)</param>
        /// <returns>Индексы тетра элементов</returns>
        private int[][] CreateTetraConfigFromEmptyEdges(int faceIndex)
        {
            if (faceIndex == 2)
            {
                var config = new int[][]
                {
                    [0,2,1,4,6,5],
                    [0,2,3,4,6,7]
                };
                return config;
            }
            else if (faceIndex == 1)
            {
                var config = new int[][]
                {
                    [1,6,2,0,7,3],
                    [1,6,5,0,7,4]
                };
                return config;
            }
            var conf = new int[][]
            {
                [0,5,1,3,6,2],
                [0,5,4,3,6,7]
            };
            return conf;
        }
        /// <summary>
        /// По указанной конфигурации ребер(два ребра противолежащих боковых граней) получает индексы двух тетра элементов, 
        /// по которым будет производится разбиение 
        /// </summary>
        /// <param name="indices">Индексы двух ребер</param>
        /// <param name="index">Индекс грани (0 - передняя, 1 - правая)</param>
        /// <returns>Индексы тетра элементов</returns>
        private int[][] CreateTetraConfigFromLateralEdges(int[][] indices, int index)
        {
            var a = index;
            var b = index + 2;

            var rightSideFirst = indices[a][1] - 4;//Точка лежащая справа от ребра на первой грани
            var leftSideFirst = indices[a][0] + 4;//Точка лежащая слева от ребра на первой грани
            var rightSideSecond = indices[b][1] - 4;//Точка лежащая справа от ребра на второй грани
            var leftSideSecond = indices[b][0] + 4;//Точка лежащая слева от ребра на второй грани

            var config = new int[][] { [indices[a][0], indices[a][1], rightSideFirst,
                                        indices[b][0], indices[b][1], rightSideSecond],
                                       [indices[a][0], indices[a][1], leftSideFirst,
                                        indices[b][0], indices[b][1], leftSideSecond]};

            return config;
        }
        /// <summary>
        /// По указанной конфигурации ребер(два ребра оснований) получает индексы двух тетра элементов, по которым будет производится разбиение 
        /// </summary>
        /// <param name="indices">Индексы двух ребер</param>
        /// <returns>Индексы тетра элементов</returns>
        private int[][] CreateTetraConfigFromPrimaryEdges(int[][] indices)
        {
            var adjacent = (indices[0][0] & 1) == 1 ? 1 : 3;//Индекс смежной грани

            var rightSideDown = indices[0][0] + 1;//Точка лежащая справа от ребра на нижней грани
            var leftSideDown = hexIndexMap[adjacent][indices[0][0]];//Точка лежащая слева от ребра на нижней грани
            var rightSideUp = indices[1][0] + 1;//Точка лежащая справа от ребра на верхней грани
            var leftSideUp = hexIndexMap[adjacent][indices[1][0]];//Точка лежащая слева от ребра на верхней грани

            var config = new int[][] { [indices[0][0], indices[0][1], rightSideDown, indices[1][0], indices[1][1], rightSideUp],
                                       [indices[0][0], indices[0][1], leftSideDown, indices[1][0], indices[1][1], leftSideUp]};
            return config;
        }
        /// <summary>
        /// По текущему ребру edge(нижней или верхней грани) получает коллинеарное ребро на противоположной грани 
        /// </summary>
        /// <param name="edge">Ребро нижней или верхней грани</param>
        /// <returns>Индексы противоположного ребра</returns>
        private int[] GetOppositePrimaryEdge(int[] edge)
        {
            var opposite = new int[2];
            if (edge[0] < 4)
            {
                opposite[0] = edge[0] + 4;
                opposite[1] = edge[1] + 4;

                return opposite;
            }

            opposite[0] = edge[0] - 4;
            opposite[1] = edge[1] - 4;

            return opposite;
        }
        /// <summary>
        /// По текущему ребру edge(боковой грани) и индексу faceIndex грани в котором оно лежит получает коллинеарное ребро на противоположной грани
        /// </summary>
        /// <param name="edge">Ребро боковой грани</param>
        /// <param name="faceIndex">Индекс грани (0 - передняя грань, 1-правая грань, 2-задняя грань, 3-левая грань)</param>
        /// <returns>Индексы противоположного ребра</returns>
        private int[] GetOppositeLateralEdge(int[] edge, int faceIndex) => [hexIndexMap[faceIndex][edge[0]], hexIndexMap[faceIndex][edge[1]]];
        /// <summary>
        /// Производит разбиение вырожденного гекса элемента(по одной или двум точкам)
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="converter">Словарь преобразования номер-индекс</param>
        /// <param name="coincidents">Список коинцидентных индексов вершин</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> DoDegeneratedHexaSplitting(Node[] nodes, Dictionary<int, int> converter, List<int> coincidents)
        {
            var set = new HashSet<int>() { 0, 1, 2, 3 };

            if (coincidents.Count == 2)
            {
                var unicals = set.Except([coincidents[0] - 4, coincidents[1] - 4]).ToList();

                var penta = new Penta(0, [ nodes[unicals[0]], nodes[unicals[0] + 4], nodes[coincidents[0]],
                                           nodes[unicals[1]], nodes[unicals[1] + 4], nodes[coincidents[1]]]);
                return SplitPenta(penta);
            }

            var pyrBaseA = (coincidents[0] + 1) % 4;
            var pyrBaseB = (coincidents[0] - 1) % 4;
            var pyrBaseC = pyrBaseA + 4;
            var pyrBaseD = pyrBaseB + 4;
            var pentaVert = (coincidents[0] - 2) % 4;

            var tetras = DoPyramidSplitting([nodes[pyrBaseA], nodes[pyrBaseD], nodes[coincidents[0]]], [nodes[pyrBaseB], nodes[pyrBaseC]]);
            var element = new Penta(0, [nodes[pentaVert], nodes[pyrBaseA], nodes[pyrBaseC],
                                        nodes[pentaVert + 4], nodes[pyrBaseC], nodes[pyrBaseD]]);
            tetras.AddRange(SplitPenta(element));

            return tetras;
        }
        /// <summary>
        /// Производит разбиение вырожденного пента элемента(по одной или двум точкам)
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="converter">Словарь преобразования номер-индекс</param>
        /// <param name="coincidents">Список коинцидентных индексов вершин</param>
        /// <returns>Список тераэдров</returns>
        private List<Tetra> DoDegeneratedPentaSplitting(Node[] nodes, Dictionary<int, int> converter, List<int> coincidents)
        {
            var set = new HashSet<int>() { 0, 1, 2 };
            if (coincidents.Count == 2)
            {
                var unical = set.Except([coincidents[0] - 3, coincidents[1] - 3]).First();

                var result = new List<Tetra>();
                var tetra = GetSingleTetraFromSplitting([nodes[unical], nodes[unical + 3], nodes[coincidents[0]], nodes[coincidents[1]]], []);
                result.Add(tetra);
                return result;
            }
            var dif = set.Except([coincidents[0] - 3]).ToList();
            var tetras = DoPyramidSplitting([nodes[dif[0]], nodes[dif[1] + 3], nodes[coincidents[0]]], [nodes[dif[1]], nodes[dif[0] + 3]]);

            return tetras;
        }
        /// <summary>
        /// Производит разбиение пента элемента на три тетраэдра на пустой конфигурации ребер
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> DoPentaDefaultSplitting(Node[] nodes)
        {
            var result = DoPyramidSplitting([nodes[2], nodes[4], nodes[3]], [nodes[1], nodes[5]]);
            var tetra = GetSingleTetraFromSplitting([nodes[0], nodes[1], nodes[2], nodes[3]],
                                                    [nodes[1], nodes[3], nodes[2], nodes[3]]);

            result.Add(tetra);
            return result;
        }
        /// <summary>
        /// Производит разбиение пента элемента по одному ребру на три тетраэдра в зависимости от конфигурации ребер записанных в indices
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="indices">Индексы ребер</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> DoPentaOneEdgeSplitting(Node[] nodes, int[] indices)
        {
            var pyrEdgeA = indices[0];
            var pyrEdgeB = indices[1];

            var min = Math.Min(indices[0], indices[1]);
            var max = Math.Max(indices[0], indices[1]);

            var pyrBaseA = max - 3;
            var pyrBaseB = min + 3;

            var set = new HashSet<int>() { 0, 1, 2 };
            var tetraTop = set.Except([pyrBaseA, min]).First() + 3;

            var tetraEdgeA = pyrBaseA;
            var tetraEdgeB = min;

            var result = DoPyramidSplitting([nodes[pyrBaseA], nodes[pyrBaseB], nodes[tetraTop]], [nodes[pyrEdgeA], nodes[pyrEdgeB]]);
            var tetra = GetSingleTetraFromSplitting([nodes[0], nodes[1], nodes[2], nodes[tetraTop]],
                                                    [nodes[tetraEdgeA], nodes[tetraTop], nodes[tetraEdgeB], nodes[tetraTop]]);

            result.Add(tetra);
            return result;
        }
        /// <summary>
        /// Производит разбиение пента элемента по двум ребрам на три тетраэдра в зависимости от конфигурации ребер записанных в indices
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="indices">Индексы ребер</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> DoPentaTwooEdgeSplitting(Node[] nodes, int[][] indices)
        {
            SortIndices(indices);

            if (indices[0][0] == indices[1][0])
                return DoSplittingWithCommonDownPoint(nodes, indices);
            if (indices[0][1] == indices[1][1])
                return DoSplittingWithCommonUpPoint(nodes, indices);
            return DoSplittingWithCommonVerticalEdge(nodes, indices);
        }
        /// <summary>
        /// Производит разбиение пента элемента по трем ребрам на три тетраэдра в зависимости от конфигурации ребер записанных в indices
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="indices">Индексы ребер</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> DoPentaThreeEdgeSplitting(Node[] nodes, int[][] indices)
        {
            SortIndices(indices);

            if (indices[0][0] == indices[1][0])
                return DoSplittingWithCommonDownPoint(nodes, indices);
            if (indices[0][0] == indices[2][0])
            {
                Swap(indices, 1, 2);
                return DoSplittingWithCommonDownPoint(nodes, indices);
            }
            if (indices[1][0] == indices[2][0])
            {
                Swap(indices, 0, 2);
                return DoSplittingWithCommonDownPoint(nodes, indices);
            }
            if (indices[0][1] == indices[1][1])
                return DoSplittingWithCommonUpPoint(nodes, indices);
            if (indices[0][1] == indices[2][1])
            {
                Swap(indices, 1, 2);
                return DoSplittingWithCommonUpPoint(nodes, indices);
            }
            if (indices[1][1] == indices[2][1])
            {
                Swap(indices, 0, 2);
                return DoSplittingWithCommonUpPoint(nodes, indices);
            }
            MissedElements += 3;//Плохая конфигурация ребер
            return new List<Tetra>();
            //return DoSplittingWithCommonVerticalEdge(edgesMap, nodes, indices);
        }
        /// <summary>
        /// Разбивает пента элемент по двум ребрам, когда две точки двух разных ребер имеют общую точку вверху 3д элемента
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="indices">Индексы ребер</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> DoSplittingWithCommonUpPoint(Node[] nodes, int[][] indices)
        {
            var tetraTop = indices[0][1];
            var set = new HashSet<int>() { 0, 1, 2 };
            var tetraLast = set.Except([indices[0][0], indices[1][0]]).First();

            var pyrBaseA = indices[0][0] + 3;
            var pyrBaseB = indices[1][0];

            var pyrEdgeA = indices.Length == 3 ? indices[2][0] : indices[0][0];
            var pyrEdgeB = indices.Length == 3 ? indices[2][1] : indices[1][0] + 3;

            var result = DoPyramidSplitting([nodes[pyrBaseA], nodes[pyrBaseB], nodes[tetraTop]], [nodes[pyrEdgeA], nodes[pyrEdgeB]]);
            var tetra = GetSingleTetraFromSplitting([nodes[indices[0][1]], nodes[indices[1][1]], nodes[tetraLast], nodes[tetraTop]],
                                                    [nodes[indices[0][0]], nodes[indices[0][1]], nodes[indices[0][0]], nodes[indices[1][1]]]);

            result.Add(tetra);
            return result;
        }
        /// <summary>
        /// Разбивает пента элемент по двум ребрам, когда две точки двух разных ребер имеют общую точку внизу 3д элемента
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="indices">Индексы ребер</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> DoSplittingWithCommonDownPoint(Node[] nodes, int[][] indices)
        {
            var tetraTop = indices[0][0];
            var set = new HashSet<int>() { 3, 4, 5 };
            var tetraLast = set.Except([indices[0][1], indices[1][1]]).First();

            var pyrBaseA = indices[0][1];
            var pyrBaseB = indices[1][1] - 3;

            var pyrEdgeA = indices.Length == 3 ? indices[2][0] : indices[0][1] - 3;
            var pyrEdgeB = indices.Length == 3 ? indices[2][1] : indices[1][1];

            var result = DoPyramidSplitting([nodes[pyrBaseA], nodes[pyrBaseB], nodes[tetraTop]], [nodes[pyrEdgeA], nodes[pyrEdgeB]]);
            var tetra = GetSingleTetraFromSplitting([nodes[indices[0][1]], nodes[indices[1][1]], nodes[tetraLast], nodes[tetraTop]],
                                                    [nodes[indices[0][0]], nodes[indices[0][1]], nodes[indices[0][0]], nodes[indices[1][1]]]);

            result.Add(tetra);
            return result;
        }
        /// <summary>
        /// Разбивает пента элемент по двум ребрам, когда две точки двух разных ребер лежат на одном вертикальном ребре 3д элемента
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="indices">Индексы ребер</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> DoSplittingWithCommonVerticalEdge(Node[] nodes, int[][] indices)
        {
            if (indices[0][1] - 3 == indices[1][0])
            {
                var result = DoPyramidSplitting([nodes[indices[1][0]], nodes[indices[0][0] + 3], nodes[indices[1][1]]],
                                                [nodes[indices[0][0]], nodes[indices[0][1]]]);
                var tetra = GetSingleTetraFromSplitting([nodes[indices[1][0]], nodes[indices[1][1]], nodes[indices[0][0]], nodes[indices[1][1] - 3]],
                                                        [nodes[indices[1][0]], nodes[indices[1][1]], nodes[indices[0][0]], nodes[indices[1][1]]]);
                result.Add(tetra);
                return result;
            }
            var tetras = DoPyramidSplitting([nodes[indices[0][0]], nodes[indices[1][0] + 3], nodes[indices[0][1]]],
                                            [nodes[indices[1][0]], nodes[indices[1][1]]]);
            var single = GetSingleTetraFromSplitting([nodes[indices[0][0]], nodes[indices[0][1]], nodes[indices[1][0]], nodes[indices[0][1] - 3]],
                                                     [nodes[indices[0][0]], nodes[indices[0][1]], nodes[indices[1][0]], nodes[indices[0][1]]]);
            tetras.Add(single);
            return tetras;

        }
        /// <summary>
        /// Сортирует индексы от младшего к старшему
        /// </summary>
        /// <param name="indices">Двумерный массив индексов</param>
        private void SortIndices(int[][] indices)
        {
            for (var i = 0; i < indices.Length; ++i)
                SortIndices(indices[i]);
        }
        /// <summary>
        /// Сортирует индексы от младшего к старшему
        /// </summary>
        /// <param name="indices">Массив индексов</param>
        private void SortIndices(int[] indices)
        {
            var min = Math.Min(indices[0], indices[1]);
            var max = Math.Max(indices[0], indices[1]);

            indices[0] = min;
            indices[1] = max;
        }
        /// <summary>
        /// Обменивает ссылкы в двумерном массиве по указанным индексам
        /// </summary>
        /// <param name="source">Двумерный массив</param>
        /// <param name="aIndex">Индекс элемента A</param>
        /// <param name="bIndex">Индекс элемента B</param>
        private void Swap(int[][] source, int aIndex, int bIndex)
        {
            var temp = source[aIndex];
            source[aIndex] = source[bIndex];
            source[bIndex] = temp;
        }
        /// <summary>
        /// Возвращает тетраэдр по 4 указанным точкам nodes опционально указываются ребра для записи в словарь ребер
        /// </summary>
        /// <param name="nodes">Точки в формате [точка_основания_0, точка_основания_1, точка_основания_2, вершина]</param>
        /// <param name="edges"></param>
        /// <returns>Тетра элемент</returns>
        private Tetra GetSingleTetraFromSplitting(Node[] nodes, Node[] edges)
        {
            var tetra = new Tetra(elementTag++, nodes);

            for (var i = 1; i < edges.Length; i += 2)
            {
                var edge = GetEdgeConfig(edges[i - 1], edges[i]);
                SetEdgeConfig(lateralEdges, edge);
            }

            return tetra;
        }
        /// <summary>
        /// Разбивает пирамиду на два тетраэдра и записывает ребро в карту ребер
        /// </summary>
        /// <param name="nodes">Массив следующего порядка (точка 1 основания пирамиды не лежащая на edge, 
        /// точка 2 основания пирамиды не лежащая на edge, вершина пирамиды)</param>
        /// <param name="edge">Ребро основания пирамиды</param>
        /// <returns>Список тетраэдров</returns>
        private List<Tetra> DoPyramidSplitting(Node[] nodes, Node[] edge)
        {
            var result = new List<Tetra>();

            var tetra_0 = GetSingleTetraFromSplitting([edge[0], edge[1], nodes[0], nodes[2]], []);
            var tetra_1 = GetSingleTetraFromSplitting([edge[0], edge[1], nodes[1], nodes[2]], []);

            result.Add(tetra_0);
            result.Add(tetra_1);

            var key = GetEdgeConfig(edge[0], edge[1]);
            SetEdgeConfig(lateralEdges, key);

            return result;
        }
        /// <summary>
        /// Возвращает конвертер номеров узлов в индексы, дополнительно заполняя список индексов коинцидентных узлов
        /// </summary>
        /// <param name="nodes">Узлы элемента</param>
        /// <param name="coincidents">Список индексов коинцидентных узлов</param>
        /// <returns>Словарь номер-индекс</returns>
        private Dictionary<int, int> GetNumberConverter(Node[] nodes, List<int> coincidents)
        {
            var converter = new Dictionary<int, int>();
            for (var i = 0; i < nodes.Length; ++i)
            {
                if (!converter.ContainsKey(nodes[i].Number))
                    converter.Add(nodes[i].Number, i);
                else
                    coincidents.Add(i);
            }
            return converter;
        }
        /// <summary>
        /// Конвертирует ключ (содержащий сортированные номера вершин через пробел) в индексы относительно выбранного 3д элемента
        /// </summary>
        /// <param name="converter">Конвертер номеров в индексы</param>
        /// <param name="key">Ключ ребра в сортированном формате (номер_узла_0 номер_узла_1)</param>
        /// <returns>Ребро индексов относительно выбранного 3д элемента</returns>
        private int[] ConvertEdgeNumbersToIndices(Dictionary<int, int> converter, string key)
        {
            var result = new int[2];
            var tokens = key.Split(' ');

            result[0] = int.Parse(tokens[0]);
            result[1] = int.Parse(tokens[1]);

            result[0] = converter[result[0]];
            result[1] = converter[result[1]];

            return result;
        }
        /// <summary>
        /// Возвращает существующие ребра боковых граней в формате индексов относительно указанных nodes, 
        /// может вернуть null внутреннего массива, если ребро не было найдено
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="converter">Конвертер номеров в индексы</param>
        /// <param name="pointsPerFace">Число точек основания</param>
        /// <returns>[Передняя грань][индекс точки_0 ребра, индекс точки_1 ребра],
        ///          [Правая грань][индекс точки_0 ребра, индекс точки_1 ребра]
        ///          [Задняя грань][индекс точки_0 ребра, индекс точки_1 ребра]
        ///          [Левая грань][индекс точки_0 ребра, индекс точки_1 ребра]</returns>
        private int[][] GetExistingLateralEdges(Node[] nodes, Dictionary<int, int> converter, int pointsPerFace)
        {
            var edges = GetLateralFaceEdges(nodes, pointsPerFace);
            var faceEdges = new int[pointsPerFace][];

            for (var i = 0; i < pointsPerFace; ++i)
            {
                for (var j = 0; j < edges[i].Length; ++j)
                {
                    var key = edges[i][j];
                    if (lateralEdges.Contains(key))
                    {
                        faceEdges[i] = ConvertEdgeNumbersToIndices(converter, key);
                        break;
                    }
                }
            }

            return faceEdges;
        }
        /// <summary>
        /// Возвращает существующие ребра нижней и верхней грани в формате индексов относительно указанных nodes, 
        /// может вернуть null внутреннего массива, если ребро не было найдено
        /// </summary>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="converter">Конвертер номеров в индексы</param>
        /// <returns>[Нижняя грань][индекс точки_0 ребра, индекс точки_1 ребра],
        ///          [Верхняя грань][индекс точки_0 ребра, индекс точки_1 ребра]</returns>
        private int[][] GetExistingPrimaryEdges(Node[] nodes, Dictionary<int, int> converter)
        {
            var faceEdges = new int[2][];
            var edges = GetPrimaryFaceEdges(nodes);

            for (var i = 0; i < 2; ++i)
            {
                for (var j = 0; j < edges[i].Length; ++j)
                {
                    var key = edges[i][j];
                    if (primEdges.Contains(key))
                    {
                        faceEdges[i] = ConvertEdgeNumbersToIndices(converter, key);
                        break;
                    }
                }
            }

            return faceEdges;
        }
        /// <summary>
        /// Возвращает все возможные конфигурации ребер основных граней (нижняя, верхняя) для гекса элемента
        /// </summary>
        /// <param name="nodes">Узлы элемента</param>
        /// <returns>Конфигурация ребер (ключи)</returns>
        private string[][] GetPrimaryFaceEdges(Node[] nodes)
        {
            var edgesData = new string[2][]
            {
                GetEdgeConfigs(nodes[0], nodes[1], nodes[2], nodes[3]),
                GetEdgeConfigs(nodes[4], nodes[5], nodes[6], nodes[7]),
            };
            return edgesData;
        }
        /// <summary>
        /// Возвращает все возможные конфигурации ребер боковых граней
        /// </summary>
        /// <param name="nodes">Узлы элемента</param>
        /// <param name="pointsPerFace">Точек в основании</param>
        /// <returns>Конфигурация ребер (ключи)</returns>
        private string[][] GetLateralFaceEdges(Node[] nodes, int pointsPerFace)
        {
            var edgesData = new string[pointsPerFace][];
            var end = pointsPerFace - 1;

            for (var i = 0; i < end; ++i)
                edgesData[i] = GetEdgeConfigs(nodes[i], nodes[i + 1], nodes[i + pointsPerFace], nodes[i + pointsPerFace + 1]);

            edgesData[end] = GetEdgeConfigs(nodes[end], nodes[0], nodes[end + pointsPerFace], nodes[pointsPerFace]);
            return edgesData;
        }
        /// <summary>
        /// Возвращает комбинацию ребер у грани с перечисленными точками
        /// </summary>
        /// <param name="downA">Нижняя точка A ребра A</param>
        /// <param name="downB">Нижняя точка B ребра A</param>
        /// <param name="upA">Верхняя точка A ребра B</param>
        /// <param name="upB">Верхняя точка B ребра B</param>
        /// <returns>Конфигурация ребер грани</returns>
        private string[] GetEdgeConfigs(Node downA, Node downB, Node upA, Node upB)
        {
            var edgeConfigs = new string[]
            {
                GetEdgeConfig(downA,upB),
                GetEdgeConfig(downB,upA),
            };

            return edgeConfigs;
        }
        /// <summary>
        /// Возвращает ключ ребра по указанным точкам
        /// </summary>
        /// <param name="downA">Точка A</param>
        /// <param name="downB">Противоположная точка B, лежащая по диагонали</param>
        /// <returns>Ключ ребра</returns>
        private string GetEdgeConfig(Node downA, Node downB)
        {
            var min = Math.Min(downA.Number, downB.Number);
            var max = Math.Max(downA.Number, downB.Number);

            return $"{min} {max}";
        }
        /// <summary>
        /// Записывает в указанный словарь ключ ребра(сортированные номера вершин), ключ извлекается из предоставленной конфигурации индексов
        /// </summary>
        /// <param name="edgesMap">Словарь ребер</param>
        /// <param name="nodes">Узлы 3д элемента</param>
        /// <param name="config">Конфигурация индексов</param>
        private void SetEdgeConfig(HashSet<string> edgesMap, Node[] nodes, int[] config)
        {
            var downA = nodes[config[0]];
            var downB = nodes[config[1]];

            var key = GetEdgeConfig(downA, downB);
            SetEdgeConfig(edgesMap, key);

            downA = nodes[config[3]];
            downB = nodes[config[4]];

            key = GetEdgeConfig(downA, downB);
            SetEdgeConfig(edgesMap, key);
        }
        /// <summary>
        /// Записывает в указанный словарь ключ ребра(сортированные номера вершин)
        /// </summary>
        /// <param name="edgesMap">Словарь ребер</param>
        /// <param name="value">Ребро строка в сортированном формате (номер_0 номер_1)</param>
        private void SetEdgeConfig(HashSet<string> edgesMap, string value) => edgesMap.Add(value);
    }
}