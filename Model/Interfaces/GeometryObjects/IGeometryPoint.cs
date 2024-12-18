using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.GeometryObjects
{
    public interface IGeometryPoint : IPoint
    {
        /// <summary>
        /// AddLine
        /// </summary>
        /// <param name="line"></param>
        void AddLine(ILineObject<IPoint> line);
        /// <summary>
        /// GetLines
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILineObject<IPoint>> GetLines();
    }
}
