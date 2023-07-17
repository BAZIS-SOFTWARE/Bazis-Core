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
        /// Удалить сущность по щелчку мыши на неё
        /// </summary>
        /// <param name="clickPoint">[In]Точка экранных координатах</param>
        void DeleteEntityOnMouseClick(Point2D clickPoint);
    }
}
