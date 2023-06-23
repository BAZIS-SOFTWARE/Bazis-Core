using Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.GmshController;

namespace Model.GmshUser
{
    /// <summary>
    /// Класс модель представляет сущность в gmsh api
    /// </summary>
    public class GmshUserModel : IDisposable
    {
        private long id;
        /// <summary>
        /// Извлекает идентифкатор модели
        /// </summary>
        public long Id { get; protected set; }
        /// <summary>
        /// Возвращает данные модели
        /// </summary>
        /// <returns></returns>
        public virtual GmshData ModelData 
        { 
            get 
            { 
                return new GmshUserModelData(Id);
            } 
        }
        /// <summary>
        /// Экземпляр ассоциирует файл скрипта с созданной моделью
        /// </summary>
        /// <param name="path">[In]Путь до скрипта</param>
        public GmshUserModel(string path)
        {
            GmshUserConnection.Init();
            Id = GmshUserConnection.GetId();
            var error = 0;
            GmshModel.gmshModelAdd(Id.ToString(), ref error);
            GmshIO.gmshMerge(path, ref error);
            GmshIO.gmshModelSetFileName(path, ref error);
        }
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
