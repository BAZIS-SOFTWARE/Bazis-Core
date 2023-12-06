using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    /// <summary>
    /// IIntegralPoint
    /// </summary>
    public interface IIntegralPoint : IPoint
    {
        /// <summary>
        /// Weigt
        /// </summary>
        float Weigt { get; set; }
    }
}
