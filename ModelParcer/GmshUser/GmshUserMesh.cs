using Model;
using ModelParcer.GmshController;
using ModelParcer.GMSHController;
using PrFunctionLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer.GmshUser
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
        /// Получает данные о сетке текущей модели
        /// </summary>
        /// <returns></returns>
        public override GmshUserModelData GetElements() => new GmshUserMeshData(Id);

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
    }
}