using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelControllerInterfaces.Delone2D
{
    /// <summary>
    /// IDeloneNode2D
    /// </summary>
    public interface IDeloneNode2D
    {
        /// <summary>
        /// Number. Use unic number
        /// </summary>
        int Number { get; }
        /// <summary>
        /// Position. Use 3D coods with z zero
        /// </summary>
        Point2D Position { get; }
    }
}
