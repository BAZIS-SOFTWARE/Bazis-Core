using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshObject
{
    public class Model : IModelApi
    {
        /// <summary>
        /// Удалить сущность по щелчку мыши на неё
        /// </summary>
        /// <param name="clickPoint">[In]Точка экранных координатах</param>
        public void DeleteEntityOnMouseClick(Point2D clickPoint)
        {
            throw new NotImplementedException();
        }
    }
}
