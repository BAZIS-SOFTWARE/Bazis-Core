using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.GmshUser
{
    internal interface IGeometryApi
    {
        /// <summary>
        /// Выбрать кривую по щелчку мыши на неё
        /// </summary>
        /// <param name="clickPoint">[In]Точка в видовых координатах</param>
        /// <returns>Возвращает id выбранной прямой</returns>
        int SelectCurveOnMouseClick(Point3D clickPoint);
        /// <summary>
        /// Задает расположение узлов на кривой 
        /// <param name="number">[In]Количество точек</param>
        /// <param name="typeId">[In]Тип расположения точек (Progressive и т.д), числовой параметр</param>
        /// <param name="parameter">[In]Выбранная кривая (может состоять из нескольких последовательно соединённых кривых)</param>
        /// <param name="curve">[In]Кривая или набор кривых</param>
        void SetMeshPointsOnCurve(int number, int typeId, int parameter, Curve[] curve);
    }
}
