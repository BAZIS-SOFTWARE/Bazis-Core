using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    /// <summary>
    /// INormal
    /// </summary>
    public interface INormal
    {
        /// <summary>
        /// CalcNormal
        /// </summary>
        /// <returns></returns>
        Point3D CalcNormal();
    }
}
