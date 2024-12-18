using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces.IntegralObject
{
    /// <summary>
    /// IIntegralObject
    /// </summary>
    public interface IIntegralObject
    {
        /// <summary>
        /// GetIntegralPoints
        /// </summary>
        /// <returns></returns>
        IEnumerable<IIntegralPoint> GetIntegralPoints();
    }
}
