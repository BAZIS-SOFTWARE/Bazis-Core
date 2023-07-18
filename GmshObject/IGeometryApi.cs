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
        int CurveSteps { get; set; }
        /// <summary>
        /// получить узлы
        /// </summary>
        /// <returns>узлы</returns>
        IEnumerable<Node> GetControlPoints();

        IEnumerable<Curve> GetСurves();
    }
}
