using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    public interface ILineObject<T> : IVertexObject<T> where T : IGeometryPoint
    {
        /// <summary>
        /// CalcLength
        /// </summary>
        /// <returns></returns>
        float CalcLength();
    }
}
