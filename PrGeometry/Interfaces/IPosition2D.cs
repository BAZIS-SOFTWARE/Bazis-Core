using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry.Interfaces
{
    public interface IPosition2D
    {
        /// <summary>
        /// X coord
        /// </summary>
        double _x { get; set; }
        /// <summary>
        /// Y coord
        /// </summary>
        double _y { get; set; }
    }
}
