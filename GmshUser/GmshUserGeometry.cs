using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Model.GmshUser
{
    /// <summary>
    /// Класс для манипуляции Gmsh геометрии 
    /// </summary>
    public class GmshUserGeometry : GmshUserModel
    {
        private int steps;

        /// <summary>
        /// Возвращает данные о геометрии
        /// </summary>
        /// <returns></returns>
        public override GmshData ModelData 
        { 
            get 
            {
                return new GmshUserGeometryData(Id); 
            } 
        }
        /// <summary>
        /// Экземпляр для манипуляции Gmsh геометрии 
        /// </summary>
        /// <param name="path"></param>
        public GmshUserGeometry(string path) : base(path)
        {
        }
    }
}
