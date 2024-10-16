using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry
{
    /// <summary>
    /// Point3DEqualityComparer
    /// </summary>
    public class Point3DEqualityComparer : IEqualityComparer<Point3D>
    {

        /// <inheritdoc/>

        public bool Equals(Point3D a, Point3D b)
        {
            return a == b;
        }
        /// <inheritdoc/>

        public int GetHashCode(Point3D obj)
        {
            return obj.GetHashCode();
        }
    }
}
