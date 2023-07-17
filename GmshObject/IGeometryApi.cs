using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshObject
{
    public interface IGeometryApi
    {
        /// <summary>
        /// Выбрать кривую по щелчку мыши на неё
        /// </summary>
        /// <param name="clickPoint">[In]Точка в видовых координатах</param>
        /// <returns>Возвращает id выбранной прямой</returns>
        int SelectCurveOnMouseClick(Point2D clickPoint);
    }
}
