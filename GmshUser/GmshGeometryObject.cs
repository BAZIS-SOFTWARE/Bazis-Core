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
using System.Collections;
using System.Drawing.Imaging;

namespace Model.GmshUser
{
    public enum TransfiniteAlgorithm
    {
        Progressive,
        Bump,
        Beta
    }
    /// <summary>
    /// Класс для манипуляции Gmsh геометрии 
    /// </summary>
    public class GmshGeometryObject : GmshModelObject, IGeometryApi
    {
        private const int defaultCurveSteps = 10;
        private const double defaultMinDencity = 0;
        private const double defaultMaxDencity = 1e+22;
        /// <summary>
        /// Задает\получает шаг параметризации для кривой
        /// </summary>
        public int CurveSteps { get; set; }
        /// <summary>
        /// Задает\получает плотность распределения точек
        /// </summary>
        public double MinDencity 
        { 
            get{ return GmshGeometryObjectData.GetMinDencity(Id); }
            set{ GmshGeometryObjectData.SetMinDencity(Id,value); }
        }
        /// <summary>
        /// Задает\получает плотность распределения точек
        /// </summary>
        public double MaxDencity
        {
            get { return GmshGeometryObjectData.GetMaxDencity(Id); }
            set { GmshGeometryObjectData.SetMaxDencity(Id, value); }
        }
        /// <summary>
        /// Экземпляр для манипуляции Gmsh геометрии 
        /// </summary>
        /// <param name="path">[In]Путь до файла</param>
        public GmshGeometryObject(string path) : base(path)
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
        public GmshGeometryObject(string path, int curveSteps = defaultCurveSteps, double minDencity = defaultMinDencity,
                                double maxDencity = defaultMaxDencity) : base(path)
        {
            CurveSteps = curveSteps;
            MinDencity = minDencity;
            MaxDencity = maxDencity;
            GmshGeometryObjectData.SetMinDencity(Id, MinDencity);
            GmshGeometryObjectData.SetMaxDencity(Id, MaxDencity);
        }

        public override IEnumerator<Node> GetEnumerator()
        {
            var list = GetElements()[ObjKind.Point];
            foreach (Node node in list) 
            { 
                yield return node;
            }
        }
        /// <summary>
        /// Возвращает узлы привязанные к геометрии (контрольные точки)
        /// </summary>
        /// <returns>Точки</returns>
        public IEnumerable<ModelObject> GetNodes() => GetElements()[ObjKind.Point];
        /// <summary>
        /// Возвращает Curve
        /// </summary>
        /// <returns>Кривые или линии</returns>
        public IEnumerable<Curve> GetCurves() => GetLineObject<Curve>(CreateCurve);
        /// <summary>
        /// Возвращает Element1D
        /// </summary>
        /// <returns>Кривые или линии</returns>
        public IEnumerable<Element1D> GetElement1D() => GetLineObject<Element1D>(CreateElement1D);
        /// <summary>
        /// Возвращает поверхности привязанные к геометрии
        /// </summary>
        /// <returns>Поверхности</returns>
        public IEnumerable<ModelObject> GetSurfaces() => GetElements()[ObjKind.GeometrySurface];
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
        /// <param name="dict">[In]Словарь для выбора списка элементов</param>
        /// <param name="name">[In]Имя элемента</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы ассоциированные с элементом</param>
        protected override void ChooseElement(Dictionary<ObjKind, List<ModelObject>> dict,
                                            ref string name, long elTag, Node[] nodes)
        {
            if (name.Contains("Point"))
                CreateNode(dict[ObjKind.Point], elTag, nodes);
            if (name.Contains("Triangle"))
                CreateSurface(dict[ObjKind.GeometrySurface], elTag, nodes, ObjKind.Triangle);
            else if (name.Contains("Quadrilateral"))
                CreateSurface(dict[ObjKind.GeometrySurface], elTag, nodes, ObjKind.Quad);
        }
        /// <summary>
        /// Создает Node control point
        /// </summary>
        /// <param name="list">[In]Список Node для заполнения</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        protected void CreateNode(List<ModelObject> list, long elTag, Node[] nodes)
        {
            list.Add(new Node((int)elTag, nodes[0].Position));
        }
        /// <summary>
        /// Создает Surface
        /// </summary>
        /// <param name="list">[In]Список Surface для заполнения</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        /// <param name="kind">[In]Тип объекта</param>
        protected void CreateSurface(List<ModelObject> list, long elTag, Node[] nodes, ObjKind kind)
        {
            list.Add(new Surface((int)elTag, nodes, kind));
        }
        /// <summary>
        /// Добавляет в список Curve
        /// </summary>
        /// <param name="dimTag">[In]Пара размерность : идентификатор</param>
        /// <param name="nodes">[In]Список узлов</param>
        /// <param name="curves">[In]Список кривых для заполнения</param>
        private void CreateCurve(Tuple<int,int> dimTag, List<Node> nodes, List<Curve> curves)
        {
            curves.Add(new Curve(dimTag.Item2, nodes.ToArray()));
        }
        /// <summary>
        /// Добавляет в список Element1D
        /// </summary>
        /// <param name="dimTag">[In]Пара размерность : идентификатор</param>
        /// <param name="nodes">[In]Список узлов</param>
        /// <param name="list1D">[In]Список Element1D для заполнения</param>
        private void CreateElement1D(Tuple<int, int> dimTag, List<Node> nodes, List<Element1D> list1D)
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
            var pairs = GmshGeometryObjectData.GetEntities(Id);
            var lines = new List<T>();
            var steps = CurveSteps;
            foreach (var dimTag in FilterByDimension(pairs, new int[] { 1 }))
            {
                var paramsCoord = GmshGeometryObjectData.GetParametrizationBounds(Id, 1, dimTag);
                if (GmshGeometryObjectData.IsLine(Id, paramsCoord[0], dimTag))
                    steps = 2;
                else
                    paramsCoord[0] = SetCustomStepParametrization(paramsCoord[0], steps);
                var points = GmshGeometryObjectData.GetPoints(Id, paramsCoord[0], dimTag);
                var points3D = GmshGeometryObjectData.ConvertToPoint3D(points);
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
        /// <summary>
        /// Выбрать кривую по щелчку мыши на неё
        /// </summary>
        /// <param name="clickPoint">[In]Точка в видовых координатах</param>
        /// <returns>Возвращает id выбранной прямой</returns>
        public int SelectCurveOnMouseClick(Point2D clickPoint)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Задает расположение узлов на кривой 
        /// <param name="number">[In]Количество точек</param>
        /// <param name="ta">[In]Тип расположения точек (Progressive и т.д), числовой параметр</param>
        /// <param name="parameter">[In]Выбранная кривая (может состоять из нескольких последовательно соединённых кривых)</param>
        /// <param name="curve">[In]Кривая или набор кривых</param>
        public void SetMeshPointsOnCurve(int number, TransfiniteAlgorithm ta, double parameter, Curve[] curve)
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
            for(var  i = 0; i < curve.Length; ++i)
            {
                var points3D = new Point3D[0];
                switch (ta)
                {
                    case TransfiniteAlgorithm.Progressive:
                        points3D = GenerateCurveProgressivePoints(curve[i].Number, number, parameter);
                        break;
                    case TransfiniteAlgorithm.Bump:
                        points3D = GenerateCurveBumpPoints(curve[i].Number, number, parameter);
                        break;
                }
                //points3D
            }
        }

        private Point3D[] GenerateCurveProgressivePoints(int tag, int number, double parameter)
        {
            var pair = Tuple.Create(1, tag);
            var bound = GmshGeometryObjectData.GetParametrizationBounds(Id, 1, pair);
            var length = bound[0][1] - bound[0][0];
            //TODO: исключить деление на ноль
            var layer = new double[number - 1];
            var a = length * (parameter - 1) / (Math.Pow(parameter, number - 1));
            layer[0] = a;
            for (var j = 1; j < number - 1; ++j)
                layer[j] = parameter * layer[j - 1];
            var coords = GmshGeometryObjectData.GetPoints(Id, layer, pair);
            return GmshGeometryObjectData.ConvertToPoint3D(coords);
        }

        private Point3D[] GenerateCurveBumpPoints(int tag, int number, double parameter)
        {
            /*
            case 2: // Bump
            {
                double a;
                if (coef > 1.0)
                {
                    a = -4. * sqrt(coef - 1.) * atan2(1., sqrt(coef - 1.)) /
                      ((double)nbpt * length);
                }
                else
                {
                    a = 2. * sqrt(1. - coef) * log(fabs((1. + 1. / sqrt(1. - coef)) /
                                                        (1. - 1. / sqrt(1. - coef))))
                      / ((double)nbpt * length);
                }
                double b = -a * length * length / (4. * (coef - 1.));
                val = d / (-a * SQU(t * length - (length) * 0.5) + b);
            }*/

            /*t is the curvilinear abscissa, and d is the norm of the derivative of 
                the parametrization (Взять производную в точке). val - результат вычисления видимо как layer[i]*/
            return null;
        }
    }
}
