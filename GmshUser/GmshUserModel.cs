using Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.GmshController;
using Geometry;
using Model.Interfaces;

namespace Model.GmshUser
{
    /// <summary>
    /// Класс модель представляет сущность в gmsh api
    /// </summary>
    public abstract class GmshUserModel : IDisposable
    {
        /// <summary>
        /// Уникальный идентификатор следущей точки
        /// </summary>
        protected int nextPointId = 1;
        /// <summary>
        /// Уникальный идентифкатор следущей кривой
        /// </summary>
        protected int nextCurveId = 1;
        /// <summary>
        /// Уникальный идентификатор следущей поверхности
        /// </summary>
        protected int nextSurfaceId = 1;
        /// <summary>
        /// Список треугольников
        /// </summary>
        protected List<Node> nodes = new List<Node>();
        protected List<Element1D> lines = new List<Element1D>();
        protected List<Surface> triangles = new List<Surface>();
        /// <summary>
        /// Извлекает идентифкатор модели
        /// </summary>
        public long Id { get; protected set; }

        /// <summary>
        /// Создает модель заданную пользователем
        /// </summary>
        public GmshUserModel()
        {
            GmshUserConnection.Init();
        }
        /// <summary>
        /// Создает модель заданную пользователем из скрипт-файла
        /// </summary>
        public GmshUserModel(string path)
        {
            GmshUserConnection.Init();
            Load(path);
        }
        /// <summary>
        /// Создает уникальный идентификатор для модели и ассоциирует файл с данной моделью
        /// </summary>
        /// <param name="path">[In]Путь до скрипта</param>
        public void Load(string path)
        {
            var error = 0;
            Id = GmshUserConnection.GetId();
            GmshModel.gmshModelAdd(Id.ToString(), ref error);
            GmshIO.gmshMerge(path, ref error);
            GmshIO.gmshModelSetFileName(path, ref error);
        }

        /// <summary>
        /// Освобождает неуправляемые ресурсы связанные с моделью 
        /// </summary>
        virtual public void Dispose()
        {
            if (Id != 0)
            {
                var error = 0;
                GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
                GmshModel.gmshModelRemove(ref error);
                GmshUserConnection.RemoveId(Id);
                Id = 0;
            }
        }
        /// <summary>
        /// Возращает набор точек связанный с данной моделью
        /// </summary>
        /// <returns>Набор точек</returns>
        public abstract IEnumerable<Node> GetNodes();
        /// <summary>
        /// Генерирует сетку для текущей модели
        /// </summary>
        /// <param name="objType">[In]Тип построения {Узлы,Элементы1D,Элементы2D,Элементы1D}</param>
        public void Generate(string objType)
        {
            var dim = new Dictionary<string, int>() { { "Узлы", 0 }, { "Линии", 1 }, { "Элементы1D", 1 },
                                                          { "Элементы2D", 2 }, { "Элементы3D", 3 } };
            var error = 0;
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
            GmshGeneral.gmshOptionSetNumber("Mesh.SaveAll", 1, ref error);
            GmshGeneral.gmshOptionSetNumber("Mesh.SaveGroupsOfNodes", -1111, ref error);
            GmshMesh.gmshModelMeshGenerate(dim[objType], ref error);
        }
        /// <summary>
        /// Группирует пары размерность : идентификаторы по размерности
        /// </summary>
        /// <param name="dimTagsPair">[In] Массив пар размерность : идентификаторы</param>
        /// <returns>Сгруппированные данные</returns>
        protected Tuple<int, int>[][] GroupByDimension(int[] dimTagsPair)
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
            var pair = new int[2];
            return dimTagsPair.Select((v, i) => pair[i & 1] = v)
                                .Where((v, i) => (i & 1) == 1)
                                .Select(v => Tuple.Create(pair[0], pair[1]))
                                .GroupBy(v => pair[0])
                                .Select(v => v.ToArray())
                                .ToArray();
        }
        /// <summary>
        /// Отфильтровывает пары размерность : идентификатор по размерности
        /// </summary>
        /// <param name="dimTagsPair">[In]Массив пар размерность : идентификатор</param>
        /// <param name="dims">[In]Фильтрация по конкретной размерности</param>
        /// <returns></returns>
        protected Tuple<int, int>[] FilterByDimension(int[] dimTagsPair, int[] dims)
        {
            var groups = GroupByDimension(dimTagsPair);
            return groups.SelectMany(v => v)
                         .Where(v => !dims.All(k => k != v.Item1))
                         .ToArray();
        }
        /// <summary>
        /// Определяет верхние и нижние границы у текущей сущности заданной парой dimTag
        /// </summary>
        /// <param name="down">[Out]Набор пар нижних границ  размерность : идентифкатор</param>
        /// <param name="up">[Out]Набор пар верхних границ  размерность : идентифкатор</param>
        /// <param name="dimTag">[In]Запрашиваемая пара размерность : идентифкатор</param>
        protected void GetEntityBounds(out int[] down, out int[] up, Tuple<int,int> dimTag)
        {
            var error = 0;
            IntPtr downward, downward_n, upward, upward_n;
            downward = downward_n = upward = upward_n = IntPtr.Zero;
            GmshModel.gmshModelGetAdjacencies(dimTag.Item1, dimTag.Item2, ref upward, ref upward_n,
                                              ref downward, ref downward_n, ref error);
            GmshUserMemory.GetTypesData(upward, out up, upward_n);
            GmshUserMemory.GetTypesData(downward, out down, downward_n);
        }
        /// <summary>
        /// Резервирует уникальные идентификаторы для точек\кривых\поверхностей смещает уникальный идентификатор
        /// </summary>
        /// <param name="count">[In]Количество идентификаторов</param>
        /// <param name="nextId">[In]Переменная идентификатор</param>
        /// <returns>Массив идентификаторов</returns>
        protected int[] CreateTags(int count, ref int nextId)
        {
            var tags = Enumerable.Range(nextId, count).ToArray();
            nextId += count;
            return tags;
        }
        /// <summary>
        /// Извлекает все элементы из модели
        /// </summary>
        protected void GetElements(string genObj)
        {
            long[] tagPoints;
            double[] coords;
            long[][] elTags, ptsTagsPerElems;
            ElementInfo[] info;
            Generate(genObj);
            GmshUserMeshData.GetElements(Id,out elTags, out ptsTagsPerElems, out info);
            GmshUserMeshData.GetPoints(Id, out coords, out tagPoints);
            var points3D = GmshUserGeometryData.ConvertToPoint3D(coords);
            for (var i = 0; i < info.Length; ++i)
            {
                var currentElement = elTags[i];
                var currentPoints = ptsTagsPerElems[i];
                for (var j = 0; j < currentElement.Length; ++j)
                {
                    var numNodes = info[i].numNodes;
                    var nodes = new Node[numNodes];
                    for (var k = 0; k < numNodes; ++k)
                    {
                        var tag = currentPoints[j * numNodes + k];
                        nodes[k] = new Node((int)tag, points3D[tag - 1]);
                    }
                    ChooseElement(ref info[i].elementName, currentElement[j], nodes);
                }
            }
        }
        /// <summary>
        /// Определяет тип элемента для заполнения
        /// </summary>
        /// <param name="name">[In]Имя элемента</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы ассоциированные с элементом</param>
        protected virtual void ChooseElement(ref string name, long elTag, Node[] nodes)
        {
            if (name.Contains("Triangle"))
                CreateElement(triangles, elTag, nodes, ObjKind.Triangle);
            else if (name.Contains("Line"))
                CreateLine(lines, elTag, nodes, ObjKind.Beam);
            else if (name.Contains("Point"))
                CreateNode(this.nodes, elTag, nodes);
        }
        /// <summary>
        /// Заполняет список элементов типа Surface
        /// </summary>
        /// <param name="list">[In]Список для заполнения</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы ассоциированные с элементом</param>
        /// <param name="kind">[In]Тип элемента</param>
        protected void CreateElement(List<Surface> list, long elTag, Node[] nodes, ObjKind kind)
        {
            list.Add(new Surface((int)elTag, nodes, kind));
        }
        /// <summary>
        /// Заполняет список элементов типа Node
        /// </summary>
        /// <param name="list">[In]Список для заполнения</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы ассоциированные с элементом</param>
        /// <param name="kind">[In]Тип элемента</param>
        protected void CreateNode(List<Node> list, long elTag, Node[] nodes)
        {
            list.Add(new Node((int)elTag, nodes[0].Position));
        }
        /// <summary>
        /// Заполняет список элементов типа Line
        /// </summary>
        /// <param name="list">[In]Список для заполнения</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы ассоциированные с элементом</param>
        /// <param name="kind">[In]Тип элемента</param>
        protected void CreateLine(List<Element1D> list, long elTag, Node[] nodes, ObjKind kind)
        {
            list.Add(new Element1D((int)elTag, nodes, kind));
        }


    }
}
