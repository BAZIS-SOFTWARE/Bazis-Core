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
        float _x { get; set; }
        /// <summary>
        /// Y coord
        /// </summary>
        float _y { get; set; }
    }
}
