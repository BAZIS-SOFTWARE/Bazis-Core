using Geometry;
using Geometry.Exteisions;
using GmshApi;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.Scripting.Utils;
using Model;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsCollections;
using Model.IO;
using Model.IO.STL;
using Model.MeshObjects;
using Model.ObjectsCollections;
using Project;
using Project.Interfaces.Tasks;
using Project.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Geometry.Exteisions;
using Model.Interfaces.ObjectsCollections;
using Model.ObjectsCollections;
using OperationalController.MeshObjsCreator;
using OperationalController.GmshController;
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
        public void CreateGroup(IEnumerable<IModelObject> objs)
        {
            ModelData.GroupData.Create(objs);
        }
        /// <inheritdoc/>
        public void CreateGroup(string grName, IEnumerable<IModelObject> objs)
        {
            ModelData.GroupData.Create(grName, objs);
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
            if(pointNumber == 0)
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
        

        public void ExtrudeElement3DAlongCurve(int sTag,int cTag,int pTag, bool tranf)
        {
            // находим номера 2д эл-ов по номеру sTag
            // формируем список этих элементов из ModelData
            // создаешь экструдер по кривой
            // передаешь параметры и запускаешь его
            // создаешь новый набор элементов и через метод append добавляешь его в коллекцию 3d ModelData
            // сформировать новый набор узлов экстр.элементов и через метод append тоже добавить в набор узлов ModelData          
        }

        public void ExtrudeElement3DRotate(int sTag, float angle, Vector3 sP, Vector3 eP)
        {
            // находим номера 2д эл-ов по номеру sTag
            // формируем список этих элементов из ModelData
            // создаешь экструдер поворота
            // перевести угол из градусов в радианы
            // передаешь параметры и запускаешь его
            // создаешь новый набор элементов и через метод append добавляешь его в коллекцию 3d ModelData
            // сформировать новый набор узлов экстр.элементов и через метод append тоже добавить в набор узлов ModelData          
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
                if(ModelData.ObjectData.Count(ObjType.Элемент1D) != 0)
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

                if(nodeNumbers.Length == 4)
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
                new Geometry.Point3D(x,y,z)));
        }
    }
}
