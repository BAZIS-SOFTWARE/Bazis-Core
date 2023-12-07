using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces.IntegralObject
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
        /// <summary>
        /// AddIntegralPoint
        /// </summary>
        /// <param name="integralPoint"></param>
        void AddIntegralPoint(IIntegralPoint integralPoint);
    }
}
