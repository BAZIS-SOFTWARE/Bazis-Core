using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ModelParcer.GmshUser
{
    /// <summary>
    /// Класс для манипуляции Gmsh геометрии 
    /// </summary>
    public class GmshUserGeometry : GmshUserModel
    {
        /// <summary>
        /// Экземпляр для манипуляции Gmsh геометрии 
        /// </summary>
        /// <param name="path"></param>
        public GmshUserGeometry(string path) : base(path)
        {
        }
        /// <summary>
        /// Получает данные о геометрии модели
        /// </summary>
        /// <returns></returns>
        public override GmshUserModelData GetElements() => new GmshUserGeometryData(Id);
    }
}
