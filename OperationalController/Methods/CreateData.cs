using Geometry;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Model.ObjectsCollections;
using OperationalController.MeshObjsCreator;
using OperationalController.MeshObjsCreator.Extrude;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace OperationalController
{
    public partial class ProjectController
    {
        /// <inheritdoc/>
        public void CreateQuadMeshOnPoligon(List<Point2D> points, int numberOfElem)
        {
            if (points.Count != 4)
                throw new Exception("Колличество узлов не равно 4");

            var b_0 = GetBorderPoints(numberOfElem, points[0], points[1]);
            var b_2 = GetBorderPoints(numberOfElem, points[3], points[2]);

            var b_1 = GetBorderPoints(numberOfElem, points[1], points[2]);
            var b_3 = GetBorderPoints(numberOfElem, points[0], points[3]);

            var hor = new List<Segment2D>(); // набор линий гор
            var vert = new List<Segment2D>(); // набор линий гор

            for (int i = 0; i <= numberOfElem; i++)
                hor.Add(new Segment2D(b_0[i], b_2[i]));

            for (int i = 0; i <= numberOfElem; i++)
                vert.Add(new Segment2D(b_1[i], b_3[i]));

            var numbE = 0;
            var numbN = 0;

            var elems = new ObjectsCollection<Element2D>();

            for (int i = 1; i < hor.Count; i++)
            {
                for (int j = 1; j < vert.Count; j++)
                {

                    var p0 = hor[i - 1].GetIntersection(vert[j - 1]);
                    var p1 = hor[i].GetIntersection(vert[j - 1]);
                    var p2 = hor[i].GetIntersection(vert[j]);
                    var p3 = hor[i - 1].GetIntersection(vert[j]);
                    var elem = CreateElem(ref numbE, ref numbN, p0, p1, p2, p3);

                    elems.Add("create", elem);
                }
            }
            foreach (var item in elems.GetObjects())
                foreach (var node in item.GetVertexes())
                    ModelData.ObjectData.NodesSet.Add(node.Number, node);

            ModelData.ObjectData.E2DCollection.Append(elems);
        }

        private Quad CreateElem(ref int numbE, ref int numbN, Point2D p0, Point2D p1, Point2D p2, Point2D p3)
        {
            var nodes = new Node[]
            {
                    new Node(numbN++,new Point3D((float)p0._x,(float)p0._y,0)),
                    new Node(numbN++,new Point3D((float)p1._x,(float)p1._y,0)),
                    new Node(numbN++,new Point3D((float)p2._x,(float)p2._y,0)),
                    new Node(numbN++,new Point3D((float)p3._x,(float)p3._y,0)),
            };
            numbE++;
            return new Quad(numbE, nodes);
        }

        private List<Point2D> GetBorderPoints(int numberOfElem, Point2D fPoint, Point2D sPoint)
        {
            var border = new List<Point2D>();
            var temp = new Segment2D(fPoint, sPoint);

            var elmSize = temp.GetLength() / numberOfElem;
            var length = 0.0f;
            border.Add(fPoint);
            for (int i = 0; i < numberOfElem - 1; i++)
                border.Add(temp.GetPointOnLine(length += elmSize));

            border.Add(sPoint);

            return border;
        }

        /// <inheritdoc/>
        public ICondData Create(DataKind kind, string info)
        {
            return TaskData?.Create(kind, info, ModelData?.GroupData);
        }

        /// <inheritdoc/>
        public void Extrude3DForm2D(Point3D vector, int layers, IEnumerable<IElement2D> elements)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>

        public void CreateElementsSet(string setName, int dim)
        {
            if (dim < 1 | dim > 3)
                throw new Exception("Неправильно указана размерность элементов");

            if (dim == 1)
                ModelData.ObjectData.E1DCollection.Add(setName);
        }

        /// <summary>
        /// Создаёт новую точку, расположенную на указанном смещении от точки с тегом <paramref name="basePointTag"/>
        /// в направлении от этой точки к точке с тегом <paramref name="directionPointTag"/>.
        /// Метод берёт контрольные точки из контроллера Gmsh, нормализует вектор направления и вызывает <see cref="CreatePoint(double,double,double,double)"/>.
        /// </summary>
        /// <param name="basePointTag">Тег (идентификатор) базовой контрольной точки в Gmsh — начало вектора.</param>
        /// <param name="directionPointTag">Тег (идентификатор) контрольной точки в Gmsh, определяющей направление (вектор от базовой точки).</param>
        /// <param name="offset">Смещение вдоль направления в тех же единицах, что и координаты модели. Отрицательное значение допустимо (смещение в обратную сторону).</param>
        /// <returns>Тег (идентификатор) созданной в Gmsh точки.</returns>
        public int CreatePointByVector(int basePointTag, int directionPointTag, double offset)
        {
            var basePoint = ModelData.ObjectData.PointsSet[basePointTag];
            var directionPoint = ModelData.ObjectData.PointsSet[directionPointTag];

            double dx = directionPoint.Position._x - basePoint.Position._x;
            double dy = directionPoint.Position._y - basePoint.Position._y;
            double dz = directionPoint.Position._z - basePoint.Position._z;

            var length = Math.Sqrt(dx * dx + dy * dy + dz * dz);

            double nx = dx / length;
            double ny = dy / length;
            double nz = dz / length;

            return CreatePoint(basePoint.Position._x + nx * offset, basePoint.Position._y + ny * offset, basePoint.Position._z + nz * offset);
        }

        /// <summary>
        /// Проецирует контрольную точку с тегом <paramref name="basePointTag"/> на указанную геометрию в Gmsh
        /// и создаёт новую точку в модели в полученной позиции.
        /// </summary>
        /// <param name="basePointTag">Тег (идентификатор) существующей контрольной точки в Gmsh, которая будет проецироваться.</param>
        /// <param name="dim">Размерность целевой геометрии в Gmsh (0 — точка, 1 — кривая, 2 — поверхность и т.д.).</param>
        /// <param name="geometryTag">Тег (идентификатор) геометрического объекта в Gmsh, на который выполняется проекция.</param>
        public int CreatePointProjectionOntoGeometry(int basePointTag, int dim, int geometryTag)
        {
            var sourcePoint = ModelData.ObjectData.PointsSet[basePointTag];

            var coord = new double[] { sourcePoint.Position._x, sourcePoint.Position._y, sourcePoint.Position._z };
            var (pointCoord, _) = GmshController.Gmsh.Model.GetClosestPoint(dim, geometryTag, coord);
            return CreatePoint(pointCoord[0], pointCoord[1], pointCoord[2]);
        }

        /// <summary>
        /// Создаёт точку в модели Gmsh и регистрирует соответствующий геометрический объект в ModelData.
        /// </summary>
        /// <param name="x">Координата X создаваемой точки (в глобальной системе координат).</param>
        /// <param name="y">Координата Y создаваемой точки (в глобальной системе координат).</param>
        /// <param name="z">Координата Z создаваемой точки (в глобальной системе координат).</param>
        /// <param name="meshSize">Желаемый параметр размера конечного элемента (mesh size). Параметр необязательный и в текущей реализации не используется при создании точки.</param>
        /// <returns>Номер (идентификатор) созданной точки в Gmsh.</returns>
        /// <exception cref="Exception">Бросается, если Gmsh вернул 0 при попытке создать точку или при отсутствии созданного геометрического объекта в списке контрольных точек.</exception>
        public int CreatePoint(double x, double y, double z, double meshSize = 0)
        {
            var pointNumber = GmshController.Gmsh.Model.Occ.AddPoint(x, y, z);
            if (pointNumber == 0)
                throw new Exception("При создании точки произошла ошибка");
            GmshController.Gmsh.Model.Occ.Synchronize();
            var geoPoint = GmshController.CreateControlPoints().Where(x => x.Number == pointNumber).FirstOrDefault();
            ModelData.ObjectData.PointsSet.Add(geoPoint.Number, geoPoint);
            return pointNumber;
        }

        /// <summary>
        /// Создаёт отрезок (линию) в модели Gmsh между двумя контрольными точками и регистрирует соответствующий геометрический объект в ModelData.
        /// </summary>
        /// <param name="startPointNumber">Номер (идентификатор) начальной точки в модели Gmsh.</param>
        /// <param name="endPointNumber">Номер (идентификатор) конечной точки в модели Gmsh.</param>
        /// <returns>Номер (идентификатор) созданной линии в Gmsh.</returns>
        /// <exception cref="Exception">
        /// Бросается, если Gmsh вернул 0 при попытке создать линию (создание не удалось).
        /// Также возможна ошибка при отсутствии найденного геометрического объекта линии в списке возвращаемых объектов (тогда произойдёт <c>NullReferenceException</c> при попытке доступа к свойствам).
        /// </exception>
        public int CreateLine(int startPointNumber, int endPointNumber)
        {
            var lineNumber = GmshController.Gmsh.Model.Occ.AddLine(startPointNumber, endPointNumber);
            if (lineNumber == 0)
                throw new Exception("При создании линии произошла ошибка");
            GmshController.Gmsh.Model.Occ.Synchronize();
            var geoLine = GmshController.CreateLines().Where(x => x.Number == lineNumber).FirstOrDefault();
            ModelData.ObjectData.CurveSet.Add(geoLine.Number, geoLine);
            return lineNumber;
        }

        /// <summary>
        /// Создаёт плоскую поверхность в модели Gmsh по набору кривых (линий) и регистрирует
        /// соответствующий геометрический объект в ModelData.
        /// </summary>
        /// <param name="lineNumbers">
        /// Массив номеров (тегов) кривых в Gmsh, составляющих замкнутый контур поверхности.
        /// Порядок и ориентация кривых должны соответствовать требуемому контуру поверхности.
        /// </param>
        /// <returns>Тег (номер) созданной поверхности в Gmsh.</returns>
        /// <exception cref="Exception">
        /// Бросается, если Gmsh вернул 0 при попытке создать поверхность (создание не удалось),
        /// либо при других ошибках взаимодействия с Gmsh (например, если дальнейшие вызовы
        /// вернут некорректные данные).
        /// </exception>
        public int CreateSurface(int[] lineNumbers)
        {
            var contur = GmshController.Gmsh.Model.Occ.AddWire(lineNumbers);
            var surf = GmshController.Gmsh.Model.Occ.AddPlaneSurface([contur]);
            if (surf == 0)
                throw new Exception("При создании поверхности произошла ошибка");
            GmshController.Gmsh.Model.Occ.Synchronize();

            var points = GmshController.Gmsh.Model.GetBoundary([2, surf], true, true, true)
                .Where((value, index) => index % 2 == 1)
                .ToArray();
            var allPoints = GmshController.CreateControlPoints();
            var geoPoints = allPoints.Where(x => points.Contains(x.Number)).ToArray();

            ModelData.ObjectData.SurfaceSet.Add(surf, new SurfaceFigure(surf, [new GeometrySurface(surf, geoPoints)]));
            return surf;
        }

        /// <summary>
        /// Выполняет экструзию 2D-элементов, принадлежащих поверхности с указанным тегом в Gmsh,
        /// вдоль кривой с заданным тегом, начиная с указанной точки кривой.
        /// Полученные в результате 3D-элементы и узлы добавляются в <c>ModelData</c>.
        /// </summary>
        /// <param name="surfaceTag">
        /// Тег (идентификатор) плоской поверхности в Gmsh, элементы которой будут экструдированы.
        /// </param>
        /// <param name="curveTags">
        /// Тег (идентификатор) кривых (curve) в Gmsh, вдоль которых выполняется экструзия.
        /// </param>
        /// <param name="startPointTag">
        /// Тег (идентификатор) точки на кривой, от которой начинается экструзия.
        /// </param>
        /// <param name="step">
        /// Шаг экструзии вдоль кривой в у.е.
        /// </param>
        /// <param name="transfinite">
        /// Если <c>true</c>, используется транзитивный/«transfinite» режим экструзии
        /// (<c>ExtrudeTransfinite</c>), в противном случае — обычный режим (<c>Extrude</c>).
        /// </param>
        /// <returns>Имя набора созданных 3Д элементов.</returns>
        public string ExtrudeElement3DAlongCurve(int surfaceTag, int[] curveTags, int startPointTag, double step, bool transfinite)
        {
            var elements2D = GetElements2DBelongingToSurface(surfaceTag);
            return ExtrudeAlongCurve(elements2D, curveTags, startPointTag, step, transfinite);
        }

        /// <summary>
        /// Выполняет экструзию 2D-элементов, принадлежащих поверхности с указанным тегом в Gmsh,
        /// вдоль кривой с заданным тегом, начиная с указанной точки кривой.
        /// Полученные в результате 3D-элементы и узлы добавляются в <c>ModelData</c>.
        /// </summary>
        /// <param name="setName">
        /// Имя набора SetInfo элементов 2D
        /// </param>
        /// <param name="curveTags">
        /// Тег (идентификатор) кривых (curve) в Gmsh, вдоль которых выполняется экструзия.
        /// </param>
        /// <param name="startPointTag">
        /// Тег (идентификатор) точки на кривой, от которой начинается экструзия.
        /// </param>
        /// <param name="step">
        /// Шаг экструзии вдоль кривой в у.е.
        /// </param>
        /// <param name="transfinite">
        /// Если <c>true</c>, используется транзитивный/«transfinite» режим экструзии
        /// (<c>ExtrudeTransfinite</c>), в противном случае — обычный режим (<c>Extrude</c>).
        /// </param>
        /// <returns>Имя набора созданных 3Д элементов.</returns>
        public string ExtrudeElement3DAlongCurve(string setName, int[] curveTags, int startPointTag, double step, bool transfinite)
        {
            var set = project.ModelData.ObjectData.E2DCollection[setName].Values.ToList();
            return ExtrudeAlongCurve(set, curveTags, startPointTag, step, transfinite);
        }

        /// <summary>
        /// Выполняет вращательную (роторную) экструзию 2D-элементов, принадлежащих поверхности с указанным тегом,
        /// вокруг заданной оси и с центром в указанной точке.
        /// Результирующие 3D-элементы и узлы добавляются в <c>ModelData</c>.
        /// </summary>
        /// <param name="surfaceTag">Тег (идентификатор) плоской поверхности в Gmsh, элементы которой будут экструдированы.</param>
        /// <param name="angle">Угол поворота в градусах. Внутри метода выполняется преобразование в радианы.</param>
        /// <param name="originPoint">Точка центра вращения (в глобальной системе координат).</param>
        /// <param name="rotAxis">Вектор оси вращения. Ожидается, что направление определяет ось вращения.</param>
        /// <param name="transfinite">
        /// Если <c>true</c>, используется «transfinite» режим экструзии
        /// (<c>ExtrudeTransfinite</c>), в противном случае — обычный режим (<c>Extrude</c>).
        /// </param>
        /// <returns>Имя набора созданных 3Д элементов.</returns>
        public string ExtrudeElement3DRotate(int surfaceTag, float angle, Vector3 originPoint, Vector3 rotAxis, bool transfinite)
        {
            var radian = (float)(angle * Math.PI / 180);
            var elements2D = GetElements2DBelongingToSurface(surfaceTag);

            var extruder = new RotateExtruder();
            var e3d = transfinite ? extruder.ExtrudeTransfinite(elements2D, radian, originPoint, rotAxis)
                                  : extruder.Extrude(elements2D, radian, originPoint, rotAxis);

            var collection = e3d.SelectMany(v => v);
            return UpdateModelData(extruder, collection);
        }

        public string[] CreateMultiPassSection(List<List<Vector2>> cells, Vector3 origin, Vector3 axisX, Vector3 axisY, int transfinitePoints = 10)
        {
            if (cells == null)
                throw new ArgumentNullException(nameof(cells));
            if (transfinitePoints < 2)
                throw new ArgumentException("Количество точек трансфинитной кривой должно быть не меньше 2", nameof(transfinitePoints));

            var section = new MultiPassSection(GmshController)
            {
                Origin = origin,
                AxisX = axisX,
                AxisY = axisY,
                TransfinitePoints = transfinitePoints
            };

            var elementSets = section.Create(cells).ToList();
            var elementCollection = new ObjectsCollection<Element2D>();
            foreach (var set in elementSets)
                elementCollection.Add(set);

            var existedSetNames = new HashSet<string>(ModelData.ObjectData.E2DCollection.Names);
            ModelData.ObjectData.E2DCollection.Append(elementCollection);

            var createdSetNames = ModelData.ObjectData.E2DCollection.Names
                .Where(name => !existedSetNames.Contains(name))
                .ToList();

            for (var i = 0; i < createdSetNames.Count; ++i)
            {
                var counter = createdSetNames[i].Split('_').Last();
                var updateName = $"{elementSets[i].Name}_{counter}";
                ModelData.ObjectData.E2DCollection.ChangeName(createdSetNames[i], updateName);
                createdSetNames[i] = updateName;
            }

            var nodesSet = new ObjectsSet<Model.MeshObjects.Node>("MultiPassSection");
            foreach (var node in section.Nodes)
                nodesSet.Add(node.Number, node);

            ModelData.ObjectData.NodesSet.Append(nodesSet);

            return createdSetNames.ToArray();
        }

        public string[] CreateMultiPassSection2(List<List<Vector2>> cells, Vector3 origin, Vector3 axisX, Vector3 axisY,
                                                bool transfinite = true, double meshSize = 0, int transfinitePoints = 10)
        {
            if (cells == null)
                throw new ArgumentNullException(nameof(cells));
            if (transfinitePoints < 2)
                throw new ArgumentException("Количество точек трансфинитной кривой должно быть не меньше 2", nameof(transfinitePoints));

            var section = new MultiPassSection2(GmshController)
            {
                Origin = origin,
                AxisX = axisX,
                AxisY = axisY,
                TransfinitePoints = transfinitePoints,
                MeshSize = meshSize
            };

            var elementSets = transfinite ? section.CreateTransfinite(cells).ToList() : section.Create(cells).ToList();
            var elementCollection = new ObjectsCollection<Element2D>();
            foreach (var set in elementSets)
                elementCollection.Add(set);

            var existedSetNames = new HashSet<string>(ModelData.ObjectData.E2DCollection.Names);
            ModelData.ObjectData.E2DCollection.Append(elementCollection);

            var createdSetNames = ModelData.ObjectData.E2DCollection.Names
                .Where(name => !existedSetNames.Contains(name))
                .ToList();

            for (var i = 0; i < createdSetNames.Count; ++i)
            {
                var counter = createdSetNames[i].Split('_').Last();
                var updateName = $"{elementSets[i].Name}_{counter}";
                ModelData.ObjectData.E2DCollection.ChangeName(createdSetNames[i], updateName);
                createdSetNames[i] = updateName;
            }

            var nodesSet = new ObjectsSet<Model.MeshObjects.Node>("MultiPassSection");
            foreach (var node in section.Nodes)
                nodesSet.Add(node.Number, node);

            ModelData.ObjectData.NodesSet.Append(nodesSet);

            return createdSetNames.ToArray();
        }

        /// <inheritdoc/>
        public void CreateElement(string setName, int dim, int[] nodeNumbers)
        {
            if (dim < 1 | dim > 3)
                throw new Exception("Неправильно указана размерность элементов");

            var nodes = new Node[nodeNumbers.Length];

            for (int i = 0; i < nodes.Length; i++)
            {
                var number = nodeNumbers[i];
                nodes[i] = new Node(number,
                    ModelData.ObjectData.NodesSet[number].Position);
            }

            if (dim == 1)
            {
                var max = 0;
                if (ModelData.ObjectData.Count(ObjType.Элемент1D) != 0)
                    max = ModelData.ObjectData.GetMaxNumber(ObjType.Элемент1D) + 1;

                var e1d = new Beam(max, nodes);
                ModelData.ObjectData.E1DCollection.Add(setName, e1d);
            }
            else if (dim == 2)
            {
                var max = 0;
                if (ModelData.ObjectData.Count(ObjType.Элемент2D) != 0)
                    max = ModelData.ObjectData.GetMaxNumber(ObjType.Элемент2D) + 1;

                Element2D e2d;

                if (nodeNumbers.Length == 4)
                    e2d = new Quad(max, nodes);
                else
                    e2d = new Triangle_c(max, nodes);

                ModelData.ObjectData.E2DCollection.Add(setName, e2d);
            }
        }

        /// <inheritdoc/>

        public void CreateNode(int number, float x, float y, float z)
        {
            if (ModelData.ObjectData.NodesSet.ContainsKey(number))
                throw new Exception("Обект с таким номером уже добавлен в набор");

            ModelData.ObjectData.NodesSet.
                Add(number, new Node(number,
                new Geometry.Point3D(x, y, z)));
        }

        private string ExtrudeAlongCurve(List<Element2D> elements, int[] curveTags, int startPointTag, double step, bool transfinite)
        {
            var stepNormalizedArray = new List<double>();
            foreach (var curveTag in curveTags)
            {
                var type = GmshController.Gmsh.Model.GetType(1, curveTag);
                var length = 0d;
                if (type == "Circle")
                    length = GmshController.CalcCurveLength(curveTag);
                else
                {
                    var (min, max) = GmshController.Gmsh.Model.GetParametrizationBounds(1, curveTag);
                    length = Math.Abs(max[0] - min[0]);
                }
                var stepNormalized = step / length;
                stepNormalizedArray.Add(stepNormalized);
            }
            var extruder = new CurveExtruder(GmshController, curveTags, stepNormalizedArray.ToArray(), startPointTag);
            var e3d = transfinite ? extruder.ExtrudeTransfinite(elements)
                      : extruder.Extrude(elements);

            var collection = e3d.SelectMany(v => v);
            return UpdateModelData(extruder, collection);
        }

        private string UpdateModelData(MeshExtruder extruder, IEnumerable<Element3D> collection)
        {
            var objSet = new ObjectsSet<Element3D>("Extrude");
            foreach (var element in collection)
                objSet.Add(element.Number, element);

            var objCollection = new ObjectsCollection<Element3D> { objSet };
            ModelData.ObjectData.E3DCollection.Append(objCollection);

            var objectSet = ModelData.ObjectData.E3DCollection.Where(col => col.Key.StartsWith("append_")).Select(col => col.Value).Last();
            var counter = objectSet.Name.Split('_').Last();
            var updateName = $"extrude_{counter}";
            ModelData.ObjectData.E3DCollection.ChangeName(objectSet.Name, updateName);

            var nodesSet = new ObjectsSet<Model.MeshObjects.Node>("Extrude");
            foreach (var node in extruder.Nodes.Values)
                nodesSet.Add(node.Number, node);
            ModelData.ObjectData.NodesSet.Append(nodesSet);

            return updateName;
        }

        private List<Element2D> GetElements2DBelongingToSurface(int surfaceTag)
        {
            var result = GmshController.Gmsh.Model.Mesh.GetElements(2, surfaceTag);
            var elementNumbers = result.Item2.SelectMany(x => x).Select(x => x.ToInt32()).ToList();

            var elements2D = new List<Element2D>();
            foreach (var pair in ModelData.ObjectData.E2DCollection)
                foreach (var element in pair.Value)
                    if (elementNumbers.Contains(element.Key))
                        elements2D.Add(element.Value);

            return elements2D;
        }
    }
}
