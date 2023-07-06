using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshUser
{
    internal interface IModelApi
    {
        /// <summary>
        /// Удалить сущность по щелчку мыши на неё
        /// </summary>
        /// <param name="clickPoint">[In]Точка в видовых координатах</param>
        void DeleteEntityOnMouseClick(Point3D clickPoint);
    }
}
