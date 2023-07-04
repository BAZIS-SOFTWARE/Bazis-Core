using System;
using System.Collections.Generic;
using Geometry;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Model.GmshController;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Reflection;
using Model.Interfaces;
using System.Runtime.Remoting.Messaging;

namespace Model.GmshUser
{
    /// <summary>
    /// Класс для манипуляции Gmsh геометрии 
    /// </summary>
    public class GmshUserGeometry : GmshUserModel
    {
        private const int defaultCurveSteps = 10;
        private const double defaultMinDencity = 0;
        private const double defaultMaxDencity = 1e+22;

        private List<Node> nodes = new List<Node>();
        private List<Curve> curves = new List<Curve>();
        private List<Surface> surfaces = new List<Surface>();
        
        /// <summary>
        /// Задает\получает шаг параметризации для кривой
        /// </summary>
        public int CurveSteps { get; set; }
        /// <summary>
        /// Задает\получает плотность распределения точек
        /// </summary>
        public double MinDencity 
        { 
            get{ return GmshUserGeometryData.GetMinDencity(Id); }
            set{ GmshUserGeometryData.SetMinDencity(Id,value); }
        }
        /// <summary>
        /// Задает\получает плотность распределения точек
        /// </summary>
        public double MaxDencity
        {
            get { return GmshUserGeometryData.GetMaxDencity(Id); }
            set { GmshUserGeometryData.SetMaxDencity(Id, value); }
        }
        /// <summary>
        /// Экземпляр для манипуляции Gmsh геометрии 
        /// </summary>
        /// <param name="path">[In]Путь до файла</param>
        public GmshUserGeometry(string path) : base(path)
        {
            CurveSteps = defaultCurveSteps;
            MinDencity = defaultMinDencity;
            MaxDencity = defaultMaxDencity;
        }
        /// <summary>
        /// Экземпляр для манипуляции Gmsh геометрии
        /// </summary>
        /// <param name="path">[In]Путь до файла</param>
        /// <param name="curveSteps">[In]Шаг параметризации для кривой</param>
        /// <param name="minDencity">[In]Минимальная плотность распределения точек</param>
        /// <param name="maxDencity">[In]Максимальная плотность распределения точек</param>
        public GmshUserGeometry(string path, int curveSteps = defaultCurveSteps, double minDencity = defaultMinDencity,
                                double maxDencity = defaultMaxDencity) : base(path)
        {
            CurveSteps = curveSteps;
            MinDencity = minDencity;
            MaxDencity = maxDencity;
        }
        /// <summary>
        /// Возвращает узлы привязанные к геометрии (контрольные точки)
        /// </summary>
        /// <returns>Точки</returns>
        public override IEnumerable<Node> GetNodes()
        {
            GetElements();
            return nodes;
        }
        /// <summary>
        /// Возвращает Curve
        /// </summary>
        /// <returns>Кривые или линии</returns>
        public IEnumerable<Curve> GetCurves()
        {
            curves = GetLineObject<Curve>(AddCurveObject);
            return curves;
        }
        /// <summary>
        /// Возвращает Element1D
        /// </summary>
        /// <returns>Кривые или линии</returns>
        public IEnumerable<Element1D> GetElement1D() => GetLineObject<Element1D>(AddElement1DObject);
        /// <summary>
        /// Возвращает поверхности привязанные к геометрии
        /// </summary>
        /// <returns>Поверхности</returns>
        public IEnumerable<Surface> GetSurfaces()
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
            GmshUserGeometryData.SetMinDencity(Id, MinDencity);
            GmshUserGeometryData.SetMaxDencity(Id, MaxDencity);
            GetElements();
            return surfaces;
        }
        /// <summary>
        /// Установить произвольный шаг параметризации
        /// </summary>
        /// <param name="paramsCoord">[In]Параметрические координаты</param>
        /// <param name="steps">[In]Число шагов параметризации</param>
        /// <returns>Преобразованный массив с заполненными шагами</returns>
        protected double[] SetCustomStepParametrization(double[] paramsCoord, int steps)
        {
            var step = (paramsCoord[1] - paramsCoord[0]) / (steps - 1);
            Array.Resize(ref paramsCoord, steps);
            for (var i = 0; i < steps; ++i)
                paramsCoord[i] = paramsCoord[0] + step * i;
            return paramsCoord;
        }
        /// <summary>
        /// Определяет тип элемента для заполнения
        /// </summary>
        /// <param name="name">[In]Имя элемента</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы ассоциированные с элементом</param>
        protected override void ChooseElement(ref string name, long elTag, Node[] nodes)
        {
            if (name.Contains("Point"))
                CreateNode(elTag, nodes);
            if (name.Contains("Triangle"))
                CreateSurface(elTag, nodes, ObjKind.Triangle);
            else if (name.Contains("Quadrilateral"))
                CreateSurface(elTag, nodes, ObjKind.Quad);
        }
        /// <summary>
        /// Создает Node control point
        /// </summary>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        protected void CreateNode(long elTag, Node[] nodes)
        {
            this.nodes.Add(new Node((int)elTag, nodes[0].Position));
        }
        /// <summary>
        /// Создает Surface
        /// </summary>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        /// <param name="kind">[In]Тип объекта</param>
        protected void CreateSurface(long elTag, Node[] nodes, ObjKind kind)
        {
            surfaces.Add(new Surface((int)elTag, nodes, kind));
        }
        /// <summary>
        /// Добавляет в список Curve
        /// </summary>
        /// <param name="dimTag">[In]Пара размерность : идентификатор</param>
        /// <param name="nodes">[In]Список узлов</param>
        /// <param name="curves">[In]Список кривых для заполнения</param>
        private void AddCurveObject(Tuple<int,int> dimTag, List<Node> nodes, List<Curve> curves)
        {
            curves.Add(new Curve(dimTag.Item2, nodes.ToArray()));
        }
        /// <summary>
        /// Добавляет в список Element1D
        /// </summary>
        /// <param name="dimTag">[In]Пара размерность : идентификатор</param>
        /// <param name="nodes">[In]Список узлов</param>
        /// <param name="list1D">[In]Список Element1D для заполнения</param>
        private void AddElement1DObject(Tuple<int, int> dimTag, List<Node> nodes, List<Element1D> list1D)
        {
            list1D.Add(new Element1D(dimTag.Item2, nodes.ToArray(), ObjKind.Beam));
        }
        /// <summary>
        /// Возвращает линейные объекты [Element1D,Curve] представленные в моделе
        /// </summary>
        /// <typeparam name="T">[In]ModelObject</typeparam>
        /// <param name="addLineObject">[In]Метод для заполнения списка</param>
        /// <returns>Список линейных объектов</returns>
        private List<T> GetLineObject<T>(Action<Tuple<int,int>, List<Node>, List<T>> addLineObject) where T : ModelObject
        {
            var pairs = GmshUserGeometryData.GetEntities(Id);
            var lines = new List<T>();
            var steps = CurveSteps;
            foreach (var dimTag in FilterByDimension(pairs, new int[] { 1 }))
            {
                var paramsCoord = GmshUserGeometryData.GetParametrizationBounds(Id, 1, dimTag);
                if (GmshUserGeometryData.IsLine(Id, paramsCoord[0], dimTag))
                    steps = 2;
                else
                    paramsCoord[0] = SetCustomStepParametrization(paramsCoord[0], steps);
                var points = GmshUserGeometryData.GetPoints(Id, paramsCoord[0], dimTag);
                var points3D = GmshUserGeometryData.ConvertToPoint3D(points);
                var nodes = new List<Node>();
                var indices = 0;
                var pointTags = CreateTags(steps * 2 - 2, ref nextPointId);
                foreach (var pair in points3D.Bigrams())
                {
                    var arrayNodes = new Node[] { new Node(pointTags[indices++], pair.Item1),
                                                  new Node(pointTags[indices++], pair.Item2)};
                    nodes.AddRange(arrayNodes);
                }
                addLineObject(dimTag, nodes, lines);
                steps = CurveSteps;
            }
            nextCurveId += lines.Count;
            return lines;
        }
    }
}
