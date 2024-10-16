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
        float _x { get; }
        /// <summary>
        /// Y coord
        /// </summary>
        float _y { get; }
        /// <summary>
        /// Z coord
        /// </summary>
        float _z { get; }
    }
}
