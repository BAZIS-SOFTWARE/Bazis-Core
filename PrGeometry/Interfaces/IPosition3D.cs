using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry.Interfaces
{
    /// <summary>
    /// IPoint3D
    /// </summary>
    public interface IPosition3D
    {
        /// <summary>
        /// X coord
        /// </summary>
        double _x { get; }
        /// <summary>
        /// Y coord
        /// </summary>
        double _y { get; }
        /// <summary>
        /// Z coord
        /// </summary>
        double _z { get; }
    }
}
