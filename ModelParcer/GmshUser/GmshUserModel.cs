using Model;
using ModelParcer.GmshController;
using ModelParcer.GMSHController;
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
    /// Класс модель представляет сущность в gmsh api
    /// </summary>
    public class GmshUserModel : IDisposable
    {
        private long id;

        public long Id { get { return id; } }
        /// <summary>
        /// Задает/Извлекает плотность распределения точек для модели
        /// </summary>
        public string Dencity
        {
            get
            {
                var error = 0;
                GmshModel.gmshModelSetCurrent(id.ToString(), ref error);
                double min = 0, max = 0;
                GmshGeneral.gmshOptionGetNumber("Mesh.MeshSizeMin", ref min, ref error);
                GmshGeneral.gmshOptionGetNumber("Mesh.MeshSizeMax", ref max, ref error);
                return min.ToString(CultureInfo.InvariantCulture) + " "
                     + max.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                var error = 0;
                GmshModel.gmshModelSetCurrent(id.ToString(), ref error);
                var values = value.Split(' ');
                var min = Double.Parse(values[0], CultureInfo.InvariantCulture);
                var max = Double.Parse(values[1], CultureInfo.InvariantCulture);
                GmshGeneral.gmshOptionSetNumber("Mesh.MeshSizeMin", min, ref error);
                GmshGeneral.gmshOptionSetNumber("Mesh.MeshSizeMax", max, ref error);
            }
        }

        /// <summary>
        /// Экземпляр ассоциирует файл скрипта с созданной моделью
        /// </summary>
        /// <param name="path">[In]Путь до скрипта</param>
        public GmshUserModel(string path)
        {
            GmshUserConnection.Init();
            id = GmshUserConnection.GetId();
            var error = 0;
            GmshModel.gmshModelAdd(id.ToString(), ref error);
            GmshIO.gmshMerge(path, ref error);
            GmshIO.gmshModelSetFileName(path, ref error);
        }

        /// <summary>
        /// Возвращает данные модели
        /// </summary>
        /// <returns></returns>
        public virtual GmshUserModelData GetElements() => new GmshUserModelData(Id);

        /// <summary>
        /// Освобождает неуправляемые ресурсы связанные с моделью 
        /// </summary>
        virtual public void Dispose()
        {
            var error = 0;
            GmshModel.gmshModelSetCurrent(id.ToString(), ref error);
            GmshModel.gmshModelRemove(ref error);
            GmshUserConnection.RemoveId(id);
        }
    }
}
