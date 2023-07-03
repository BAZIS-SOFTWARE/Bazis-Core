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
        private List<Surface> quads = new List<Surface>();
        private List<Surface> tetrahedra = new List<Surface>();
        private List<Surface> hexahedra = new List<Surface>();
        private List<Surface> penta = new List<Surface>();
        private List<Surface> pyramids = new List<Surface>();
        private List<Surface> trihedra = new List<Surface>();
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
            GetElements("Элементы1D");
            return nodes;
        }
        /// <summary>
        /// Возвращает элементы 2D
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Element2D> GetElement2D()
        {
            GetElements("Элементы2D");
            return (IEnumerable<Element2D>)triangles; ///Переделать
        }
        /// <summary>
        /// Определяет тип элемента для заполнения
        /// </summary>
        /// <param name="name">[In]Имя элемента</param>
        /// <param name="elTag">[In]Идентификатор элемента</param>
        /// <param name="nodes">[In]Узлы ассоциированные с элементом</param>
        protected override void ChooseElement(ref string name, long elTag , Node[] nodes)
        {
            base.ChooseElement(ref name, elTag, nodes);
            if (name.Contains("Quadrilateral"))
                CreateElement(quads, elTag, nodes, ObjKind.Quad);
            else if (name.Contains("Tetrahedron"))
                CreateElement(tetrahedra, elTag, nodes, ObjKind.Tetra);
            else if (name.Contains("Hexahedron"))
                CreateElement(hexahedra, elTag, nodes, ObjKind.Hexa);
            else if (name.Contains("Prism"))
                CreateElement(penta, elTag, nodes, ObjKind.Penta);
            /*else
                throw new NotImplementedException();*///Нет пирамиды в ObjKind
        }
    }
}