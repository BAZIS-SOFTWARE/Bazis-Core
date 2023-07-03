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


namespace Model.GmshUser
{
    /// <summary>
    /// Класс для манипуляции с Gmsh сеткой
    /// </summary>
    public class GmshUserMesh : GmshUserModel
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
        public GmshUserMesh(string path) : base(path)
        { 
        }

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

        public override IEnumerable<Node> GetNodes()
        {
            long[] tagPoints;
            double[] points;
            GmshUserMeshData.GetPoints(Id, out points, out tagPoints);
            var points3D = ConvertToPoint3D(points);
            var nodes = new List<Node>();
            for (var i = 0; i < points3D.Length; ++i)
                nodes.Add(new Node((int)tagPoints[i], points3D[i]));
            return nodes;
        }

        public IEnumerable<Element1D> GetCurves()
        {
            throw new NotImplementedException();
        }
    }
}