using PrFunctionLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.GmshController;
using Model.ModelParcer;
using Model.Interfaces;
using Geometry;
using System.Collections;
using MathNet.Numerics.Integration;

namespace Model.GmshUser
{
    /// <summary>
    /// Класс для манипуляции с Gmsh сеткой
    /// </summary>
    public class GmshMeshObject : GmshGeometryObject, IMeshApi
    {
        /// <summary>
        /// Флаг указывает будет ли удаляться файл связанный с ModelParcer
        /// </summary>
        public bool DeleteDstFile { get; set; }
        /// <summary>
        /// Задает/Извлекает плотность распределения точек для модели
        /// </summary>
        public string Dencity
        {
            get
            {
                var error = 0;
                GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
                double min = 0, max = 0;
                GmshGeneral.gmshOptionGetNumber("Mesh.MeshSizeMin", ref min, ref error);
                GmshGeneral.gmshOptionGetNumber("Mesh.MeshSizeMax", ref max, ref error);
                return min.ToString(CultureInfo.InvariantCulture) + " "
                     + max.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                var error = 0;
                GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
                var values = value.Split(' ');
                var min = double.Parse(values[0], CultureInfo.InvariantCulture);
                var max = double.Parse(values[1], CultureInfo.InvariantCulture);
                GmshGeneral.gmshOptionSetNumber("Mesh.MeshSizeMin", min, ref error);
                GmshGeneral.gmshOptionSetNumber("Mesh.MeshSizeMax", max, ref error);
            }
        }

        /// <summary>
        /// Экземпляр для манипуляции с Gmsh сеткой
        /// </summary>
        /// <param name="path">[In]Путь до файла скрипта</param>
        public GmshMeshObject(string path) : base(path)
        {
        }
        /// <summary>
        /// Заменяет по возможности сетку из треугольников четурехугольниками
        /// </summary>
        public void QuadrangulateMesh()
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
            GmshMesh.gmshModelMeshRecombine(ref error);
        }

        /// <summary>
        /// Уплотняет сетку текущей модели
        /// </summary>
        public void RefineMesh()
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
            GmshMesh.gmshModelMeshRefine(ref error);
        }

        /// <summary>
        /// Создает ModelData по текущему набору элементов через ModelParcer
        /// </summary>
        /// <returns></returns>
        public ModelData GetModelDataFromParcer()
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
            var dstFile = GetDstFileName();
            GmshIO.gmshWrite(dstFile, ref error);
            var data = new LoadModelFromGMSHTextFile();
            data.LoadEvent += (ar1, ar2) => { };
            var modelData = data.Load(dstFile);
            return modelData;
        }
        /// <summary>
        /// Освобождает неуправляемые ресурсы связанные с сеткой
        /// </summary>
        public override void Dispose()
        {
            if (DeleteDstFile)
                File.Delete(GetDstFileName());
            base.Dispose();
        }

        /// <summary>
        /// Получает текущий файл связанные с сеткой
        /// </summary>
        /// <returns></returns>
        private string GetDstFileName()
        {
            var error = 0;
            var idStr = Id.ToString();
            GmshModel.gmshModelSetCurrent(idStr, ref error);
            var fileName = GmshUserIO.GetFileName();
            var file = Path.GetFileNameWithoutExtension(fileName);
            var folder = Path.GetDirectoryName(fileName);
            return folder + @"\" + file + idStr + ".inp";
        }
        /// <summary>
        /// Возвращает узлы
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelObject> GetNodes() => GetElements()[ObjKind.Node];
        /// <summary>
        /// Возвращает элементы 1D
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelObject> GetElement1D() => GetElements()[ObjKind.Beam];
        /// <summary>
        /// Возвращает элементы 2D
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelObject> GetElement2D() => GetElements()[ObjKind.ElementSurface];
        /// <summary>
        /// Возвращает элементы 3D
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ModelObject> GetElement3D() => GetElements()[ObjKind.Tetra];
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
            if (name.Contains("Line"))
                CreateElement1D(dict[ObjKind.Beam], elTag, nodes, ObjKind.Beam);
            else if (name.Contains("Triangle"))
                CreateElement2D(dict[ObjKind.Triangle], elTag, nodes, ObjKind.Triangle);
            else if (name.Contains("Quadrilateral"))
                CreateElement2D(dict[ObjKind.Quad], elTag, nodes, ObjKind.Quad);
            else if (name.Contains("Tetrahedron"))
                CreateElement3D(dict[ObjKind.Tetra], elTag, nodes, ObjKind.Tetra);
            else if (name.Contains("Hexahedron"))
                CreateElement3D(dict[ObjKind.Hexa], elTag, nodes, ObjKind.Hexa);
            else if (name.Contains("Prism"))
                CreateElement3D(dict[ObjKind.Penta], elTag, nodes, ObjKind.Penta);
            //Нет ObjKind.Pyramid
        }
        /// <summary>
        /// Создает Element1D
        /// </summary>
        /// <param name="list">[In]Список Element1D для заполнения</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        /// <param name="kind">[In]Тип объекта</param>
        protected void CreateElement1D(List<ModelObject> list, long elTag, Node[] nodes, ObjKind kind)
        {
            list.Add(new Element1D((int)elTag, nodes, kind));
        }
        /// <summary>
        /// Создает Element2D
        /// </summary>
        /// <param name="list">[In]Список Element2D для заполнения</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        /// <param name="kind">[In]Тип объекта</param>
        protected void CreateElement2D(List<ModelObject> list, long elTag, Node[] nodes, ObjKind kind)
        {
            list.Add(new Element2D((int)elTag, nodes, kind));
        }
        /// <summary>
        /// Создает Element3D
        /// </summary>
        /// <param name="list">[In]Список Element3D для заполнения</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        /// <param name="kind">[In]Тип объекта</param>
        protected void CreateElement3D(List<ModelObject> list, long elTag, Node[] nodes, ObjKind kind)
        {
            list.Add(new Element3D((int)elTag, nodes, kind));
        }
        /// <summary>
        /// Заполняет узлы сетки
        /// </summary>
        /// <param name="nodes">[In]Узлы для заполнения</param>
        /// <param name="coords">[In]Координаты</param>
        /// <param name="tagPoints">[In]Идентификаторы</param>
        protected override void CreateNodes(List<ModelObject> nodes, double[] coords, long[] tagPoints)
        {
            for (var i = 0; i < tagPoints.Length; ++i)
            {
                var point = new Point3D((float)coords[i * 3], (float)coords[i * 3 + 1], (float)coords[i * 3 + 2]);
                nodes.Add(new Node((int)tagPoints[i], point));
            }
        }
        /// <summary>
        /// установить новый алгоритм триангуляции по его enum
        /// (Delaunay, Frontal Delaunay) и т.д для 2D
        /// </summary>
        /// <param name="algorithm">[In]Enum алгоритма</param>
        public void ChangeTriangulationAlgorithm(Triangulation2DAlgorithm algorithm = Triangulation2DAlgorithm.FrontalDelaunay)
        {
            var error = 0;
            GmshGeneral.gmshOptionSetNumber("Mesh.Algorithm", (double)algorithm, ref error);
        }
        /// <summary>
        /// установить новый алгоритм триангуляции по его enum
        /// (Delaunay, Frontal Delaunay) и т.д для 3D
        /// </summary>
        /// <param name="algorithm">[In]Enum алгоритма</param>
        public void ChangeTriangulationAlgorithm(Triangulation3DAlgorithm algorithm = Triangulation3DAlgorithm.Delaunay)
        {
            var error = 0;
            GmshGeneral.gmshOptionSetNumber("Mesh.Algorithm3D", (double)algorithm, ref error);
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
            for (var i = 0; i < curve.Length; ++i)
            {
                switch (ta)
                {
                    case TransfiniteAlgorithm.Progressive:
                        GmshMesh.gmshModelMeshSetTransfiniteCurve(curve[i].Number, number, "Progression", parameter, ref error);
                        break;
                    case TransfiniteAlgorithm.Bump:
                        GmshMesh.gmshModelMeshSetTransfiniteCurve(curve[i].Number, number, "Bump", parameter, ref error);
                        break;
                    case TransfiniteAlgorithm.Beta:
                        GmshMesh.gmshModelMeshSetTransfiniteCurve(curve[i].Number, number, "Beta", parameter, ref error);
                        break;
                }
            }
        }
    }
}