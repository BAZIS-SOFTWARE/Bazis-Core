using Geometry;
using GmshApi;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Model;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using Model.ObjectsCollections;
using OperationalController.GeomObjsCreator;
using OperationalController.GmshController;
using OperationalController.MeshObjsCreator;
using OperationalController.MeshObjsCreator.Extrude;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using static IronPython.Modules._ast;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace OperationalController
{
    public partial class ProjectController
    {
        private const float CurveMeshNodeMergeTolerance = 1e-4f;

        /// <inheritdoc/>
        public void CreateQuadMeshOnPoligon(List<Point2D> points, int numberOfElem)
        {
            using (BeginChanges())
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

                var addedNames = ModelData.ObjectData.E2DCollection.Append(elems);
                if (elems.Count != 0)
                    RegisterSetChange(ModelData.ObjectData.NodesSet, ChangeAction.Updated);
                foreach (var name in addedNames)
                {
                    var addedSet = ModelData.ObjectData.E2DCollection[name];
                    RegisterSetChange(addedSet, ChangeAction.Created);
                }
            }
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
            var condition = TaskData.Create(kind, info, ModelData.GroupData);
            return condition;
        }

        /// <inheritdoc/>
        public void Extrude3DForm2D(Point3D vector, int layers, IEnumerable<IElement2D> elements)
        {
            throw new NotImplementedException();
        }
        /// <inheritdoc/>

        public void CreateElementsSet(string setName, int dim)
        {
            using (BeginChanges())
            {
                if (dim < 1 | dim > 3)
                    throw new Exception("Неправильно указана размерность элементов");

                if (dim == 1)
                {
                    ModelData.ObjectData.E1DCollection.Add(setName);
                    var set = ModelData.ObjectData.E1DCollection[setName];
                    RegisterSetChange(set, ChangeAction.Created);
                }
            }
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
            using (BeginChanges())
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
            using (BeginChanges())
            {
                var sourcePoint = ModelData.ObjectData.PointsSet[basePointTag];

                var coord = new double[] { sourcePoint.Position._x, sourcePoint.Position._y, sourcePoint.Position._z };
                var (pointCoord, _)= GmshController.Gmsh.Model.GetClosestPoint(dim, geometryTag, coord);
                return CreatePoint(pointCoord[0], pointCoord[1], pointCoord[2]);
            }
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
            using (BeginChanges())
            {
                var pointNumber = GmshController.Gmsh.Model.Occ.AddPoint(x, y, z);
                if (pointNumber == 0)
                    throw new Exception("При создании точки произошла ошибка");
                GmshController.Gmsh.Model.Occ.Synchronize();
                var geoPoint = GmshController.CreateControlPoints().Where(x => x.Number == pointNumber).FirstOrDefault();
                ModelData.ObjectData.PointsSet.Add(geoPoint.Number, geoPoint);
                RegisterSetChange(ModelData.ObjectData.PointsSet, ChangeAction.Updated);
                RegisterGeometryChange(GeoChange.GeometryEntityKind.Point);
                return pointNumber;
            }
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
            using (BeginChanges())
            {
                var lineNumber = GmshController.Gmsh.Model.Occ.AddLine(startPointNumber, endPointNumber);
                if (lineNumber == 0)
                    throw new Exception("При создании линии произошла ошибка");
                GmshController.Gmsh.Model.Occ.Synchronize();
                var geoLine = GmshController.CreateLines().Where(x => x.Number == lineNumber).FirstOrDefault();
                ModelData.ObjectData.CurveSet.Add(geoLine.Number, geoLine);
                RegisterSetChange(ModelData.ObjectData.CurveSet, ChangeAction.Updated);
                RegisterGeometryChange(GeoChange.GeometryEntityKind.Curve);
                return lineNumber;
            }
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
            using (BeginChanges())
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
                RegisterSetChange(ModelData.ObjectData.SurfaceSet, ChangeAction.Updated);
                RegisterGeometryChange(GeoChange.GeometryEntityKind.Surface);
                return surf;
            }
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
            using (BeginChanges())
            {
                var elements2D = GetElements2DBelongingToSurface(surfaceTag);
                return ExtrudeAlongCurve(elements2D, curveTags, startPointTag, step, transfinite);
            }
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
            using (BeginChanges())
            {
                var set = project.ModelData.ObjectData.E2DCollection[setName].Values.ToList();
                return ExtrudeAlongCurve(set, curveTags, startPointTag, step, transfinite);
            }
        }

        /// <summary>
        /// Выполняет экструзию исходного узла вдоль кривой с созданием 1D-элементов Beam.
        /// </summary>
        /// <param name="curveTags">Теги кривых, вдоль которых выполняется экструзия.</param>
        /// <param name="startPointTag">Тег геометрической точки, от которой начинается экструзия.</param>
        /// <param name="startNodeTag">Тег исходного узла в модели.</param>
        /// <param name="step">Шаг экструзии вдоль кривой в у.е.</param>
        /// <returns>Имя набора созданных 1D-элементов.</returns>
        public string ExtrudeElement1DAlongCurve(int[] curveTags, int startPointTag, int startNodeTag, double step)
        {
            using (BeginChanges())
            {
                var startNode = ModelData.ObjectData.NodesSet[startNodeTag];
                var beams = ExtrudePointAlongCurve(startNode, curveTags, startPointTag, step);
                var name = UpdateBeamModelData(beams);
                return name;
            }
        }

        /// <summary>
        /// Выполняет экструзию 2D-элементов поверхности по узлам готовой сетки кривых.
        /// </summary>
        public string ExtrudeElement3DAlongCurveMesh(int surfaceTag, int[] curveTags, int startPointTag, bool transfinite)
        {
            using (BeginChanges())
            {
                var elements2D = GetElements2DBelongingToSurface(surfaceTag);
                return ExtrudeAlongCurveMesh(elements2D, curveTags, startPointTag, transfinite);
            }
        }

        /// <summary>
        /// Выполняет экструзию набора 2D-элементов по узлам готовой сетки кривых.
        /// </summary>
        public string ExtrudeElement3DAlongCurveMesh(string setName, int[] curveTags, int startPointTag, bool transfinite)
        {
            using (BeginChanges())
            {
                var set = project.ModelData.ObjectData.E2DCollection[setName].Values.ToList();
                return ExtrudeAlongCurveMesh(set, curveTags, startPointTag, transfinite);
            }
        }

        /// <summary>
        /// Выполняет экструзию по узлам сетки кривых и присоединяет к ней близкие узлы существующей сетки.
        /// </summary>
        public string ExtrudeElement3DAlongCurveMeshAdjusted(string setName, int[] curveTags, int startPointTag,
            bool transfinite, double mergeTolerance)
        {
            if (!double.IsFinite(mergeTolerance) || mergeTolerance <= 0 || mergeTolerance > float.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(mergeTolerance));

            using (BeginChanges())
            {
                var set = project.ModelData.ObjectData.E2DCollection[setName].Values.ToList();
                var extruder = new CurveExtruder(GmshController, curveTags, startPointTag, useMeshNodes: true);
                var resultSetName = ExtrudeAlongCurve(set, extruder, transfinite);
                MergeCoincidentExtrusionNodes(extruder);
                MergeExistingNodesWithExtrusion(extruder, (float)mergeTolerance);
                return resultSetName;
            }
        }

        /// <summary>
        /// Выполняет экструзию исходного узла по узлам готовой сетки кривых.
        /// </summary>
        public string ExtrudeElement1DAlongCurveMesh(int[] curveTags, int startPointTag, int startNodeTag)
        {
            using (BeginChanges())
            {
                var startNode = ModelData.ObjectData.NodesSet[startNodeTag];
                var beams = ExtrudePointAlongCurveMesh(startNode, curveTags, startPointTag);
                return UpdateBeamModelData(beams);
            }
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
            using (BeginChanges())
            {
                var radian = (float)(angle * Math.PI / 180);
                var elements2D = GetElements2DBelongingToSurface(surfaceTag);

                var extruder = new RotateExtruder();
                var e3d = transfinite ? extruder.ExtrudeTransfinite(elements2D, radian, originPoint, rotAxis)
                                      : extruder.Extrude(elements2D, radian, originPoint, rotAxis);

                var collection = e3d.SelectMany(v => v);
                return UpdateModelData(extruder, collection);
            }
        }

        /// <inheritdoc/>
        public int[] CreateChamfer(IEnumerable<int> curveTags, double firstLength, double secondLengthOrAngle, bool isAngle, bool reflected)
        {
            using (BeginChanges())
            {
                var builder = new ChamferGeometryBuilder(GmshController);
                var result = builder.Build(curveTags, firstLength, secondLengthOrAngle, isAngle, reflected);
                RefreshGeometryData();
                var entityKind = GeoChange.GeometryEntityKind.Point | GeoChange.GeometryEntityKind.Curve |
                    GeoChange.GeometryEntityKind.Surface | GeoChange.GeometryEntityKind.Volume;
                RegisterGeometryChange(entityKind);
                return result;
            }
        }

        private void RefreshGeometryData()
        {
            // OCC-операции изменяют топологию и делают прежнюю поверхностную сетку
            // непригодной для построения SurfaceFigure. Перестраиваем её до чтения
            // геометрии, иначе объём может ссылаться на отсутствующую поверхность.
            GmshController.Gmsh.Model.Mesh.Clear();
            //GmshController.Gmsh.Model.Mesh.Generate(2);
            GmshController.Gmsh.Model.Mesh.ImportStl();

            var points = GmshController.CreateControlPoints();
            var curves = GmshController.CreateLines();
            var surfaceData = CreateChamferSurfaceData();

            ClearModelCollection(ObjType.Точка);
            ModelData.VolumeData.Clear();
            var pointsChanged = AddSetObjects(ModelData.ObjectData.PointsSet, points);
            var curvesChanged = AddSetObjects(ModelData.ObjectData.CurveSet, curves);
            var surfaces = surfaceData.Item1.Values;
            var surfacesChanged = AddSetObjects(ModelData.ObjectData.SurfaceSet, surfaces);

            foreach (var volume in surfaceData.Item2)
                ModelData.VolumeData.Add(volume);
            if (pointsChanged)
                RegisterSetChange(ModelData.ObjectData.PointsSet, ChangeAction.Updated);
            if (curvesChanged)
                RegisterSetChange(ModelData.ObjectData.CurveSet, ChangeAction.Updated);
            if (surfacesChanged)
                RegisterSetChange(ModelData.ObjectData.SurfaceSet, ChangeAction.Updated);
        }

        private Tuple<ObjectsSet<SurfaceFigure>, VolumeData> CreateChamferSurfaceData()
        {
            var meshPoints = GmshController.GetNodes(2, -1, true)
                .ToDictionary(
                    node => node.Number,
                    node => new GeometryPoint(node.Number, node.Position));
            var surfaces = new ObjectsSet<SurfaceFigure>("FreeSurfaces");
            var surfaceEntities = GmshController.Gmsh.Model.GetEntities(2);

            for (var index = 1; index < surfaceEntities.Length; index += 2)
            {
                var surfaceTag = surfaceEntities[index];
                surfaces.Add(surfaceTag, CreateChamferSurfaceFigure(meshPoints, surfaceTag));
            }

            var volumes = new VolumeData();
            var volumeEntities = GmshController.Gmsh.Model.GetEntities(3);
            var random = new Random();

            for (var index = 1; index < volumeEntities.Length; index += 2)
            {
                var volumeTag = volumeEntities[index];
                var (_, surfaceTags) = GmshController.Gmsh.Model.GetAdjacencies(3, volumeTag);
                var volumeSurfaces = surfaceTags.Select(surfaceTag => surfaces[surfaceTag]).ToArray();
                var volume = new VolumeFigure(volumeTag, volumeSurfaces);
                var name = GmshController.Gmsh.Model.GetEntityName(3, volumeTag);
                volume.Name = string.IsNullOrEmpty(name)
                    ? volumeTag.ToString()
                    : name.Replace("Shapes/", "");
                volumes.Add(volume);
            }

            return System.Tuple.Create(surfaces, volumes);
        }

        private SurfaceFigure CreateChamferSurfaceFigure(
            IReadOnlyDictionary<int, GeometryPoint> points,
            int surfaceTag)
        {
            var elementData = GmshController.Gmsh.Model.Mesh.GetElements(2, surfaceTag);
            var geometrySurfaces = new List<GeometrySurface>();

            for (var typeIndex = 0; typeIndex < elementData.Item1.Length; typeIndex++)
            {
                var properties = GmshController.Gmsh.Model.Mesh.GetElementProperties(elementData.Item1[typeIndex]);
                var elements = elementData.Item2[typeIndex];
                var nodeTags = elementData.Item3[typeIndex];

                for (var elementIndex = 0; elementIndex < elements.Length; elementIndex++)
                {
                    var elementPoints = new GeometryPoint[properties.numNodes];
                    for (var nodeIndex = 0; nodeIndex < properties.numNodes; nodeIndex++)
                    {
                        var nodeTag = (int)nodeTags[elementIndex * properties.numNodes + nodeIndex];
                        if (!points.TryGetValue(nodeTag, out var point))
                            throw new InvalidOperationException($"Узел {nodeTag} поверхности {surfaceTag} не найден.");

                        elementPoints[nodeIndex] = point;
                    }

                    geometrySurfaces.Add(new GeometrySurface((int)elements[elementIndex], elementPoints));
                }
            }

            return new SurfaceFigure(surfaceTag, geometrySurfaces.ToArray());
        }

        /// <inheritdoc/>
        public void CreateElement(string setName, int dim, int[] nodeNumbers)
        {
            using (BeginChanges())
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
                    AddCollectionObject(ModelData.ObjectData.E1DCollection, setName, e1d);
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

                    AddCollectionObject(ModelData.ObjectData.E2DCollection, setName, e2d);
                }
            }
        }

        /// <inheritdoc/>

        public void CreateNode(int number, float x, float y, float z)
        {
            using (BeginChanges())
            {
                if (ModelData.ObjectData.NodesSet.ContainsKey(number))
                    throw new Exception("Обект с таким номером уже добавлен в набор");

                var position = new Point3D(x, y, z);
                var node = new Node(number, position);
                ModelData.ObjectData.NodesSet.Add(number, node);
                RegisterSetChange(ModelData.ObjectData.NodesSet, ChangeAction.Updated);
            }
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
        private string ExtrudeAlongCurve(List<Element2D> elements, int[] curveTags, int startPointTag, double step, bool transfinite)
        {
            var extruder = new CurveExtruder(GmshController, curveTags, GetNormalizedSteps(curveTags, step), startPointTag);
            return ExtrudeAlongCurve(elements, extruder, transfinite);
        }

        private List<Beam> ExtrudePointAlongCurve(Node startNode, int[] curveTags, int startPointTag, double step)
        {
            var extruder = new CurveExtruder(GmshController, curveTags, GetNormalizedSteps(curveTags, step), startPointTag);
            return ExtrudePointAlongCurve(startNode, extruder);
        }

        private string ExtrudeAlongCurveMesh(List<Element2D> elements, int[] curveTags, int startPointTag, bool transfinite)
        {
            var extruder = new CurveExtruder(GmshController, curveTags, startPointTag, useMeshNodes: true);
            var setName = ExtrudeAlongCurve(elements, extruder, transfinite);
            var coincidentNodes = FindCoincidentObjects(ObjType.Узел, CurveMeshNodeMergeTolerance);
            // тут в  дальнейшем следует использовать перегрузку метода слияния, которая бы обрабатывала только экструдированные узлы
            MergeNodes(coincidentNodes);
            return setName;
        }

        private void MergeExistingNodesWithExtrusion(MeshExtruder extruder, float tolerance)
        {
            var extrudedNodes = extruder.Nodes.Values
                .Where(node => ModelData.ObjectData.NodesSet.ContainsKey(node.Number) &&
                    ReferenceEquals(ModelData.ObjectData.NodesSet[node.Number], node))
                .ToDictionary(node => node.Number);
            var groups = FindCoincidentObjects(ObjType.Узел, tolerance);
            var nodesToMerge = new List<List<int>>();

            foreach (var group in groups)
            {
                var generated = group.Where(extrudedNodes.ContainsKey).ToArray();
                var existing = group.Where(number => !extrudedNodes.ContainsKey(number)).ToArray();
                if (generated.Length == 0 || existing.Length == 0)
                    continue;

                var assignments = new Dictionary<int, List<int>>();
                foreach (var existingNumber in existing)
                {
                    if (!ModelData.ObjectData.NodesSet.ContainsKey(existingNumber))
                        continue;

                    var existingNode = ModelData.ObjectData.NodesSet[existingNumber];
                    var generatedNumber = generated[0];
                    var distance = existingNode.Position.Sub(extrudedNodes[generatedNumber].Position)
                        .GetSquareSumCoords();
                    for (var index = 1; index < generated.Length; index++)
                    {
                        var candidateDistance = existingNode.Position.Sub(extrudedNodes[generated[index]].Position)
                            .GetSquareSumCoords();
                        if (candidateDistance >= distance)
                            continue;
                        generatedNumber = generated[index];
                        distance = candidateDistance;
                    }

                    if (!assignments.TryGetValue(generatedNumber, out var assignedNodes))
                    {
                        assignedNodes = new List<int>();
                        assignments.Add(generatedNumber, assignedNodes);
                    }
                    assignedNodes.Add(existingNumber);
                }

                foreach (var assignment in assignments)
                {
                    var numbers = new List<int> { assignment.Key };
                    numbers.AddRange(assignment.Value);
                    nodesToMerge.Add(numbers);
                }
            }

            MergeNodes(nodesToMerge);
        }

        private void MergeCoincidentExtrusionNodes(MeshExtruder extruder)
        {
            var extrudedNodeNumbers = extruder.Nodes.Values
                .Select(node => node.Number)
                .ToHashSet();
            var groups = FindCoincidentObjects(ObjType.Узел, CurveMeshNodeMergeTolerance);
            var nodesToMerge = new List<List<int>>();

            foreach (var group in groups)
            {
                var generated = group.Where(extrudedNodeNumbers.Contains).ToArray();
                if (generated.Length == 0)
                    continue;

                var masterNumber = generated[0];
                var numbers = new List<int> { masterNumber };
                numbers.AddRange(group.Where(number => number != masterNumber));
                nodesToMerge.Add(numbers);
            }

            MergeNodes(nodesToMerge);
        }

        private string ExtrudeAlongCurve(List<Element2D> elements, CurveExtruder extruder, bool transfinite)
        {
            var e3d = transfinite ? extruder.ExtrudeTransfinite(elements)
                      : extruder.Extrude(elements);
            var collection = e3d.SelectMany(v => v);
            return UpdateModelData(extruder, collection);
        }

        private List<Beam> ExtrudePointAlongCurveMesh(Node startNode, int[] curveTags, int startPointTag)
        {
            var extruder = new CurveExtruder(GmshController, curveTags, startPointTag, useMeshNodes: true);
            return ExtrudePointAlongCurve(startNode, extruder);
        }

        private List<Beam> ExtrudePointAlongCurve(Node startNode, CurveExtruder extruder)
        {
            var beams = extruder.ExtrudeFromPoint(startNode);
            AppendNodes(extruder);
            return beams;
        }

        private double[] GetNormalizedSteps(int[] curveTags, double step)
        {
            var normalizedSteps = new List<double>();
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
                normalizedSteps.Add(step / length);
            }
            return normalizedSteps.ToArray();
        }

        private string UpdateModelData(MeshExtruder extruder, IEnumerable<Element3D> collection)
        {
            var objSet = new ObjectsSet<Element3D>("extrude");
            foreach (var element in collection)
                objSet.Add(element.Number, element);

            var objCollection = new ObjectsCollection<Element3D> { objSet };

            var addedNames = ModelData.ObjectData.E3DCollection.Append(objCollection);
            var addedName = addedNames.Single();

            AppendNodes(extruder);
            var addedSet = ModelData.ObjectData.E3DCollection[addedName];
            RegisterSetChange(addedSet, ChangeAction.Created);
            return addedName;
        }

        private string UpdateBeamModelData(IEnumerable<Beam> beams)
        {
            var beamsSet = new ObjectsSet<Beam>("extrude");
            foreach (var beam in beams)
                beamsSet.Add(beam.Number, beam);

            var collection = new ObjectsCollection<Beam> { beamsSet };
            var addedNames = ModelData.ObjectData.E1DCollection.Append(collection);
            var addedName = addedNames.Single();
            var addedSet = ModelData.ObjectData.E1DCollection[addedName];
            RegisterSetChange(addedSet, ChangeAction.Created);

            return addedName;
        }

        private void AppendNodes(MeshExtruder extruder)
        {
            var nodesSet = new ObjectsSet<Model.MeshObjects.Node>("extrude");
            foreach (var node in extruder.Nodes.Values)
                nodesSet.Add(node.Number, node);
            ModelData.ObjectData.NodesSet.Append(nodesSet);
            if (nodesSet.Count != 0)
                RegisterSetChange(ModelData.ObjectData.NodesSet, ChangeAction.Updated);
        }
    }
}
