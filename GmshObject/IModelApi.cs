using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshObject
{
    public interface IModelApi
    {
        /// <summary>
        /// Удалить сущность по Id
        /// </summary>
        /// <param name="entityId">[In]Id</param>
        void DeleteEntity(int entityId);
        /// <summary>
        /// // загрузить геометрию
        /// </summary>
        /// <param name="filePath">[In]Путь до файла</param>
        void Load(string filePath);
    }
}
