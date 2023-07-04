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

namespace Model.GmshUser
{
    /// <summary>
    /// Класс для манипуляции с Gmsh сеткой
    /// </summary>
    public class GmshUserMesh : GmshUserModel
    {
        private List<Node> nodes = new List<Node>();
        private List<Element1D> elements1D = new List<Element1D>();
        private List<Element2D> elements2D = new List<Element2D>();
        private List<Element3D> elements3D = new List<Element3D>();
        /*private List<Surface> tetrahedra = new List<Surface>();
        private List<Surface> hexahedra = new List<Surface>();
        private List<Surface> penta = new List<Surface>();
        private List<Surface> pyramids = new List<Surface>();
        private List<Surface> trihedra = new List<Surface>();*/
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
                var min = Double.Parse(values[0], CultureInfo.InvariantCulture);
                var max = Double.Parse(values[1], CultureInfo.InvariantCulture);
                GmshGeneral.gmshOptionSetNumber("Mesh.MeshSizeMin", min, ref error);
                GmshGeneral.gmshOptionSetNumber("Mesh.MeshSizeMax", max, ref error);
            }
        }

        /// <summary>
        /// Экземпляр для манипуляции с Gmsh сеткой
        /// </summary>
        /// <param name="path">[In]Путь до файла скрипта</param>
        public GmshUserMesh(string path) : base(path)
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
        public override IEnumerable<Node> GetNodes()
        {
            GetElements();
            return nodes;
        }
        /// <summary>
        /// Возвращает элементы 1D
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Element1D> GetElement1D()
        {
            GetElements();
            return elements1D;
        }
        /// <summary>
        /// Возвращает элементы 2D
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Element2D> GetElement2D()
        {
            GetElements();
            return elements2D;
        }
        /// <summary>
        /// Возвращает элементы 3D
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Element3D> GetElement3D()
        {
            GetElements();
            return elements3D;
        }
        /// <summary>
        /// Определяет тип элемента для заполнения
        /// </summary>
        /// <param name="name">[In]Имя элемента</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы ассоциированные с элементом</param>
        protected override void ChooseElement(ref string name, long elTag , Node[] nodes)
        {
            if (name.Contains("Line"))
                CreateElement1D(elTag, nodes, ObjKind.Beam);
            else if (name.Contains("Triangle"))
                CreateElement2D(elTag, nodes, ObjKind.Triangle);
            else if (name.Contains("Quadrilateral"))
                CreateElement2D(elTag, nodes, ObjKind.Quad);
            else if (name.Contains("Tetrahedron"))
                CreateElement3D(elTag, nodes, ObjKind.Tetra);
            else if (name.Contains("Hexahedron"))
                CreateElement3D(elTag, nodes, ObjKind.Hexa);
            else if (name.Contains("Prism"))
                CreateElement3D(elTag, nodes, ObjKind.Penta);
            //Нет ObjKind.Pyramid
        }
        /// <summary>
        /// Создает Element1D
        /// </summary>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        /// <param name="kind">[In]Тип объекта</param>
        protected void CreateElement1D(long elTag, Node[] nodes, ObjKind kind)
        {
            elements1D.Add(new Element1D((int)elTag, nodes, kind));
        }
        /// <summary>
        /// Создает Element2D
        /// </summary>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        /// <param name="kind">[In]Тип объекта</param>
        protected void CreateElement2D(long elTag, Node[] nodes, ObjKind kind)
        {
            elements2D.Add(new Element2D((int)elTag, nodes, kind));
        }
        /// <summary>
        /// Создает Element3D
        /// </summary>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы</param>
        /// <param name="kind">[In]Тип объекта</param>
        protected void CreateElement3D(long elTag, Node[] nodes, ObjKind kind)
        {
            elements3D.Add(new Element3D((int)elTag, nodes, kind));
        }
        /// <summary>
        /// Заполняет узлы сетки
        /// </summary>
        /// <param name="coords">[In]Координаты</param>
        /// <param name="tagPoints">[In]Идентификаторы</param>
        protected override void CreateNodes(double[] coords, long[] tagPoints)
        {
            for (var i = 0; i < tagPoints.Length; ++i)
            {
                var point = new Point3D((float)coords[i * 3], (float)coords[i * 3 + 1], (float)coords[i * 3 + 2]);
                nodes.Add(new Node((int)tagPoints[i], point));
            }
        }
    }
}