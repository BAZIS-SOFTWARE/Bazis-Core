using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    /// <summary>
    /// IGeometryPoint
    /// </summary>
    public interface IGeometryPoint : IPoint
    {
        /// <summary>
        /// AddLine
        /// </summary>
        /// <param name="line"></param>
        void AddLine(ILineObject line);
        /// <summary>
        /// GetLines
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILineObject> GetLines();
    }
}
